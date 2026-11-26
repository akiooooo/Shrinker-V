using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using GameFormats.Files;
using GameFormats.Graphics;

namespace ShrinkerV.Core
{
    internal sealed partial class GtaProcessor
    {
        private readonly ProcessingOptions        _opts;
        private readonly Action<string,LogLevel>  _log;
        private readonly ProcessingStats          _stats = new ProcessingStats();
        // GameFormats имеет статический стейт в RpfFile.CreateFile (таблица ключей,
        // пул памяти). Все вызовы CreateFile идут через этот lock — остальная работа
        // (декодинг/кодирование текстур) остаётся параллельной.
        private RpfBatch _batch;

        // ── Batch pipeline structs ────────────────────────────────────────────────
        // Extract (sequential I/O) → Compute (Parallel.For, no file I/O) → Write (sequential I/O)
        private struct RpfWorkItem
        {
            public RpfFileEntry Entry;
            public string       ExtLower;   // entry.Name.ToLowerInvariant()
            public string       EntryPath;
            public byte[]       RawData;    // bytes from ExtractFile
        }
        private struct RpfWorkResult
        {
            public RpfFileEntry Entry;
            public byte[]       ResultBytes; // null → entry unchanged / skip
        }

        private int _progressValue;
        private int _progressMax;

        public ProcessingStats Stats         { get { return _stats; } }
        public int             ProgressValue { get { return _progressValue; } }
        public int             ProgressMax   { get { return _progressMax; } }

        public GtaProcessor(ProcessingOptions opts, Action<string,LogLevel> log)
        {
            _opts = opts;
            _log  = log != null ? log : (Action<string,LogLevel>)((m, l) => {});
        }

        private void L(string msg, LogLevel lvl)
        {
            _log(msg, lvl);
        }
        private void L(string msg)
        {
            _log(msg, LogLevel.Info);
        }

        // -- MAIN RUN -------------------------------------------------------------
        public ProcessingStats Run()
        {
            _opts.Cancel.ThrowIfCancellationRequested();

            L("ProtectPlayer=" + _opts.ProtectCharacters + "; SkipNPC=" + _opts.SkipPeds + "; Gray=" + _opts.UseGrayscale);
            L("Загружаю криптоключи GTA V...");
            bool keysOk = LoadCryptoKeys();
            L(keysOk ? "Криптоключи OK." : "Криптоключи не найдены -- зашифрованные RPF будут пропущены.",
              keysOk ? LogLevel.Ok : LogLevel.Warn);

            L("Сканирую архивы в: " + _opts.InputFolder);
            var rpfPaths = ScanSourceArchives();
            L("Найдено RPF: " + rpfPaths.Count,
              rpfPaths.Count > 0 ? LogLevel.Ok : LogLevel.Warn);
            _progressMax = rpfPaths.Count;

            // Необработанные архивы переносим ПЕРВЫМ делом и до любого return:
            // выход обязан быть полным, иначе игра ругается на отсутствующие паки.
            CopySkippedArchives();

            if (rpfPaths.Count == 0)
            {
                L("Нет RPF для обработки. Укажите папку с dlcpacks или GTA V.",
                  LogLevel.Warn);
                return _stats;
            }

            L(string.Format("Запускаю обработку {0} архивов (последовательно)...",
                rpfPaths.Count));

            // SemaphoreSlim(1) = последовательная обработка.
            // GameFormats (DDSIO, RpfFile.CreateFile) использует статический стейт
            // и не является thread-safe — параллельный запуск давал рандомные крашы.
            foreach (var path in rpfPaths) {
                _opts.Cancel.ThrowIfCancellationRequested();
                ProcessTopLevel(path);
                Interlocked.Increment(ref _progressValue);
            }
            _opts.Cancel.ThrowIfCancellationRequested();

            L(string.Format("=== ГОТОВО  RPF:{0}  Текстур:{1}  AWC:{2}  Ошибок:{3} ===",
                _stats.ArchivesProcessed, _stats.TexturesShrunken,
                _stats.AwcReplaced, _stats.ErrorsEncountered), LogLevel.Ok);
            return _stats;
        }

        // -- CRYPTO KEYS ----------------------------------------------------------
        private bool LoadCryptoKeys()
        {
            try
            {
                byte[] aes = RuntimeFiles.Read("gtav_aes_key.dat");
                GTA5Keys.LoadFromPath(key: Convert.ToBase64String(aes));
                GTA5Keys.PC_NG_ENCRYPT_TABLES = null;
                GTA5Keys.PC_NG_ENCRYPT_LUTs = null;
                try { GTA5Keys.PC_NG_ENCRYPT_TABLES = CryptoIO.ReadNgTables(RuntimeFiles.Read("gtav_ng_encrypt_tables.dat")); }
                catch (IOException ex) { L(ex.Message, LogLevel.Warn); }
                try { GTA5Keys.PC_NG_ENCRYPT_LUTs = CryptoIO.ReadNgLuts(RuntimeFiles.Read("gtav_ng_encrypt_luts.dat")); }
                catch (IOException ex) { L(ex.Message, LogLevel.Warn); }
                return true;
            }
            catch (Exception ex)
            {
                L("Ошибка загрузки криптоключей: " + ex.Message, LogLevel.Error);
                Interlocked.Increment(ref _stats.ErrorsEncountered);
                return false;
            }
        }

        // -- SCAN -----------------------------------------------------------------
        // Архивы, которые мы не обрабатываем. КРИТИЧНО: они обязаны попасть в выходную
        // папку как есть. Если файла нет, а content.xml на него ссылается — игра падает
        // с ошибкой файлов. Именно это ломало сборки: 45 отфильтрованных архивов
        // (все patchday*, _g9ec и др.) просто отсутствовали в выходе.
        private readonly List<string> _skippedArchives = new List<string>();

        private List<string> ScanSourceArchives()
        {
            var result = new List<string>();
            _skippedArchives.Clear();
            try
            {
                foreach (var f in Directory.GetFiles(
                    _opts.InputFolder, "*.rpf", SearchOption.AllDirectories))
                {
                    if (PreserveWholeArchive(f) || IsProtectedByPath(f) || (_opts.ProtectCharacters && IsPlayerResource(f)) || (_opts.SkipPeds && IsNpcResource(f)))
                    {
                        L("  Без обработки (будет копия 1:1): " + f.Substring(_opts.InputFolder.Length));
                        _skippedArchives.Add(f);
                        continue;
                    }
                    result.Add(f);
                }
            }
            catch (Exception ex)
            {
                L("Ошибка сканирования: " + ex.Message, LogLevel.Error);
                Interlocked.Increment(ref _stats.ErrorsEncountered);
            }
            return result;
        }

        // Копирует необработанные архивы в выход без изменений.
        private void CopySkippedArchives()
        {
            if (_opts.ProcessInPlace) return;          // файлы уже на месте
            if (_skippedArchives.Count == 0) return;
            L(string.Format("Копирую без обработки: {0} архивов...", _skippedArchives.Count));
            int ok = 0, fail = 0;
            foreach (var src in _skippedArchives)
            {
                _opts.Cancel.ThrowIfCancellationRequested();
                if (ProcessArchive(src, true)) ok++;
                else fail++;
            }
            L(string.Format("  Скопировано 1:1: {0}, не удалось: {1}", ok, fail),
              fail > 0 ? LogLevel.Error : LogLevel.Ok);
        }

        private void ProcessTopLevel(string source)
        {
            ProcessArchive(source, false);
        }

        // Keep the original name inside staging: GTA's archive keys depend on it.
        private bool ProcessArchive(string source, bool copyOnly)
        {
            string relative = source.Substring(_opts.InputFolder.Length);
            string destination = Path.Combine(_opts.OutputFolder, relative);
            string staging = Path.Combine(Path.GetDirectoryName(destination), ".shrinker-v-" + Guid.NewGuid().ToString("N"));
            string name = Path.GetFileName(source);
            string staged = Path.Combine(staging, name);
            try
            {
                _opts.Cancel.ThrowIfCancellationRequested();
                Directory.CreateDirectory(staging);
                if (copyOnly) File.Copy(source, staged);
                
                int changes = 0;
                long before = new FileInfo(source).Length;
                if (!copyOnly)
                {
                    L("АРХИВ: " + relative);
                    var archive = new RpfFile(source, name);
                    archive.ScanStructure(null, null);
                    if (archive.LastException != null) throw new InvalidDataException(name + ": " + archive.LastException.Message, archive.LastException);
                    if (archive.AllEntries == null) throw new InvalidDataException(name);
                    int errorsBefore = _stats.ErrorsEncountered;
                    using (_batch = new RpfBatch(source, staging, archive))
                    {
                        changes = ProcessRpfRecursive(archive, name, 0);
                        _opts.Cancel.ThrowIfCancellationRequested();
                        if (_stats.ErrorsEncountered != errorsBefore)
                            throw new InvalidDataException("Ошибка обработки записей " + name);
                        if (changes > 0)
                        {
                            _batch.Write(archive, staged);
                            if (!_batch.Verify(archive, staged))
                                throw new InvalidDataException("Архив не прошёл проверку: " + name);
                        }
                        else if (!_opts.ProcessInPlace) File.Copy(source, staged);
                    }
                }
                long after = File.Exists(staged) ? new FileInfo(staged).Length : before;
                _opts.Cancel.ThrowIfCancellationRequested();
                if (!_opts.ProcessInPlace || changes > 0)
                {
                    if (File.Exists(destination)) File.Replace(staged, destination, null);
                    else File.Move(staged, destination);
                }
                if (!copyOnly)
                {
                    _stats.BytesBefore += before;
                    _stats.BytesAfter += after;
                    _stats.ArchivesProcessed++;
                    if (changes > 0) _stats.ArchivesModified++;
                }
                return true;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _stats.ErrorsEncountered++;
                _stats.ArchivesFailed++;
                L(name + ": " + ex.Message, LogLevel.Error);
                return false;
            }
            finally
            {
                // Only the single staged archive belongs to this call.
                if (File.Exists(staged)) File.Delete(staged);
                if (Directory.Exists(staging)) Directory.Delete(staging);
            }
        }

        private bool TryReadResource(Action read, string name)
        {
            try { read(); return true; }
            catch (Exception ex) when (!(ex is OutOfMemoryException) && !(ex is OperationCanceledException))
            {
                L("    Сохранён оригинал <" + name + ">: " + ex.Message, LogLevel.Warn);
                return false;
            }
        }
        private RpfWorkResult ComputeEntry(RpfWorkItem w, string topName)
        {
            string lo = w.ExtLower;
            byte[] outBytes = null;
            try
            {
                if (lo.EndsWith(".ytd"))
                {
                    var f = new YtdFile(w.Entry);
                    if (!TryReadResource(() => f.Load(w.RawData, w.Entry), w.Entry.Name)) return default(RpfWorkResult);
                    if (f.TextureDict == null) return default(RpfWorkResult);
                    if (!ProcessDict(f.TextureDict, w.EntryPath)) return default(RpfWorkResult);
                    outBytes = f.Save();
                }
                else if (lo.EndsWith(".ydr"))
                {
                    var f = new YdrFile(w.Entry);
                    if (!TryReadResource(() => f.Load(w.RawData, w.Entry), w.Entry.Name)) return default(RpfWorkResult);
                    if (f.Drawable == null || f.Drawable.ShaderGroup == null) return default(RpfWorkResult);
                    if (f.Drawable.ShaderGroup.TextureDictionary == null) return default(RpfWorkResult);
                    if (!ProcessDict(f.Drawable.ShaderGroup.TextureDictionary, w.EntryPath)) return default(RpfWorkResult);
                    outBytes = f.Save();
                }
                else if (lo.EndsWith(".ydd"))
                {
                    var f = new YddFile(w.Entry);
                    if (!TryReadResource(() => f.Load(w.RawData, w.Entry), w.Entry.Name)) return default(RpfWorkResult);
                    bool changed = false;
                    if (f.DrawableDict != null && f.DrawableDict.Drawables != null
                        && f.DrawableDict.Drawables.data_items != null)
                        foreach (var d in f.DrawableDict.Drawables.data_items)
                        {
                            if (d == null || d.ShaderGroup == null
                                || d.ShaderGroup.TextureDictionary == null) continue;
                            if (ProcessDict(d.ShaderGroup.TextureDictionary, w.EntryPath)) changed = true;
                        }
                    if (!changed) return default(RpfWorkResult);
                    outBytes = f.Save();
                }
                else if (lo.EndsWith(".yft"))
                {
                    var f = new YftFile(w.Entry);
                    if (!TryReadResource(() => f.Load(w.RawData, w.Entry), w.Entry.Name)) return default(RpfWorkResult);
                    if (f.Fragment == null || f.Fragment.Drawable == null) return default(RpfWorkResult);
                    if (f.Fragment.Drawable.ShaderGroup == null) return default(RpfWorkResult);
                    if (f.Fragment.Drawable.ShaderGroup.TextureDictionary == null) return default(RpfWorkResult);
                    if (!ProcessDict(f.Fragment.Drawable.ShaderGroup.TextureDictionary, w.EntryPath)) return default(RpfWorkResult);
                    outBytes = f.Save();
                }
            }
            catch (Exception ex)
            {
                L("    ! compute <" + w.Entry.Name + ">: " + (_opts.VerboseLog ? ex.ToString() : ex.Message), LogLevel.Warn);
                Interlocked.Increment(ref _stats.ErrorsEncountered);
                return default(RpfWorkResult);
            }
            if (outBytes == null || outBytes.Length == 0) return default(RpfWorkResult);
            return new RpfWorkResult { Entry = w.Entry, ResultBytes = outBytes };
        }

        private int ProcessRpfRecursive(RpfFile rpf, string topName, int depth, string ancestry = "")
        {
            if (rpf == null || depth > 8) return 0;
            List<RpfFileEntry> entries = null;
            try { entries = rpf.AllEntries.OfType<RpfFileEntry>().ToList(); } catch { return 0; }
            int total = 0;
            // ── Конвейер: Extract(seq) → Compute(parallel) → Write(seq) ──────────
            // Phase 1: ExtractFile по одному (GameFormats FileShare.None — только seq)
            // Phase 2: Load+ProcessDict+Save для всего батча одновременно (нет I/O)
            // Phase 3: CreateFile по одному (seq)
            int batchSz = Math.Max(8, Math.Min(32, _opts.MaxThreads * 4));
            var batch   = new List<RpfWorkItem>(batchSz);
            Action flushBatch = delegate()
            {
                if (batch.Count == 0) return;
                var r = new RpfWorkResult[batch.Count];
                System.Threading.Tasks.Parallel.For(0, batch.Count,
                    new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = _opts.MaxThreads },
                    i => r[i] = ComputeEntry(batch[i], topName));
                for (int k = 0; k < batch.Count; k++)
                    if (r[k].ResultBytes != null)
                        try { _batch.Add(r[k].Entry, r[k].ResultBytes); Interlocked.Increment(ref total); }
                        catch (Exception ex2) { Interlocked.Increment(ref _stats.ErrorsEncountered); L("    ! write <" + r[k].Entry.Name + ">: " + ex2.Message, LogLevel.Warn); }
                batch.Clear();
            };
            foreach (var entry in entries)
            {
                if (_opts.Cancel.IsCancellationRequested) break;
                if (entry == null || string.IsNullOrEmpty(entry.Name)) continue;
                string lo = entry.Name.ToLowerInvariant();
                string ep = ancestry + "\\" + rpf.Name + "\\" + (entry.Path ?? lo);
                if (ShouldSkipEntry(ep)) continue;
                try
                {
                    if (IsLightweightAwc(entry, topName))
                    { _batch.Add(entry, _awcStub); Interlocked.Increment(ref _stats.AwcReplaced); Interlocked.Increment(ref total); continue; }
                    if (IsLightweightYpt(entry))
                    { _batch.Add(entry, _yptStub); Interlocked.Increment(ref total); continue; }
                    if (lo.EndsWith(".bik"))
                    { _batch.Add(entry, GetBikStub()); Interlocked.Increment(ref total); continue; }

                    // Map removal - replace YDR/YFT/YBN/YTD with 1KB stubs
                    if (IsMapFile(entry))
                    {
                        byte[] mapStub = null;
                        if (lo.EndsWith(".ydr")) mapStub = _ydrStub;
                        else if (lo.EndsWith(".yft")) mapStub = _yftStub;
                        else if (lo.EndsWith(".ybn")) mapStub = _ybnStub;
                        else if (lo.EndsWith(".ytd")) mapStub = BuildEmptyYtd();

                        if (mapStub != null)
                        {
                            _batch.Add(entry, mapStub);
                            Interlocked.Increment(ref total);
                            continue;
                        }
                    }
                }
                catch (Exception ex) { L("    ! stub <" + lo + ">: " + ex.Message, LogLevel.Warn); continue; }
                if (lo.EndsWith(".ytd") || lo.EndsWith(".ydr") || lo.EndsWith(".ydd") || lo.EndsWith(".yft"))
                {
                    byte[] data = null;
                    try { data = rpf.ExtractFile(entry); }
                    catch (Exception ex) { L("    ! extract <" + lo + ">: " + ex.Message, LogLevel.Warn); Interlocked.Increment(ref _stats.ErrorsEncountered); continue; }
                    if (data != null && data.Length > 0) batch.Add(new RpfWorkItem { Entry = entry, ExtLower = lo, EntryPath = ep, RawData = data });
                    if (batch.Count >= batchSz) flushBatch();
                }
            }
            flushBatch();

            // ── Дочерние RPF ──────────────────────────────────────────────────
            List<RpfFile> children = null;
            try { if (rpf.Children != null) children = rpf.Children.ToList(); }
            catch { }

            if (children != null)
            {
                int childIdx = 0;
                foreach (var child in children)
                {
                    if (_opts.Cancel.IsCancellationRequested) break;
                    if (child == null) continue;
                    childIdx++;
                    string cname = child.Name != null ? child.Name : "";
                    if (IsProtectedByPath(cname) || ShouldSkipEntry(child.Path ?? cname)) continue;
                    try
                    {
                        if (children.Count >= 50 && childIdx % 10 == 0)
                            L(string.Format("  [{0}] вложенных обработано: {1}/{2}",
                                topName, childIdx, children.Count));

                        int childMods = ProcessRpfRecursive(child, topName, depth + 1, ancestry + "\\" + rpf.Name);
                        if (childMods > 0)
                        {
                            total += childMods;
                            L(string.Format("    [{0}] {1} изм.: {2}", depth + 1, cname, childMods));
                        }
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (OutOfMemoryException oomEx)
                    {
                        L("    ! OOM [" + cname + "]: " + oomEx.Message, LogLevel.Warn);
                        Interlocked.Increment(ref _stats.ErrorsEncountered);
                    }
                    catch (Exception ex)
                    {
                        L(string.Format("    ! [{0}]: {1}", cname, ex.Message), LogLevel.Warn);
                        Interlocked.Increment(ref _stats.ErrorsEncountered);
                    }
                }
            }
            return total;
        }
        private byte[] _awcStub, _yptStub, _ydrStub, _yftStub, _ybnStub;
        private static byte[] ReadResourceStub(string name, uint system, uint graphics)
        {
            byte[] payload = RuntimeFiles.Read(name);
            if (payload.Length != new RpfResourcePageFlags(system).Size + new RpfResourcePageFlags(graphics).Size)
                throw new InvalidDataException(name);
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(0x37435352);
                writer.Write(((system >> 28) << 4) | (graphics >> 28));
                writer.Write(system);
                writer.Write(graphics);
                writer.Write(RpfFile.CompressBytes(payload));
                return stream.ToArray();
            }
        }

        internal void LoadReplacementAssets()
        {
            if (!_opts.KeepAudio) _awcStub = RuntimeFiles.Read("embedded_timer.awc");
            if (!_opts.KeepEffects) _yptStub = ReadResourceStub("embedded_cut_iaaj.ypt", 0x40240800, 0x40000000);
            if (_opts.RemoveMap)
            {
                _ydrStub = ReadResourceStub("minimal_ydr.stub", 0xA0020000, 0x50000000);
                _yftStub = ReadResourceStub("minimal_yft.stub", 0xA0040000, 0x20000000);
                _ybnStub = ReadResourceStub("minimal_ybn.stub", 0x20020000, 0xB0000000);
            }
        }
        private static volatile byte[] _ytdStub;
        private static readonly object _ytdLock = new object();

        private static byte[] BuildEmptyYtd()
        {
            if (_ytdStub != null) return _ytdStub;
            lock (_ytdLock)
            {
                if (_ytdStub != null) return _ytdStub;
                try
                {
                    var ytd  = new YtdFile();
                    var dict = new TextureDictionary();
                    dict.Textures = new ResourcePointerList64<Texture>();
                    dict.Textures.data_items = new Texture[0];
                    dict.TextureNameHashes = new ResourceSimpleList64_uint();
                    dict.TextureNameHashes.data_items = new uint[0];
                    ytd.TextureDict = dict;
                    _ytdStub = ytd.Save();
                }
                catch { _ytdStub = null; }
                return _ytdStub;
            }
        }
        private static byte[] GetBikStub()
        {
            using (var stream = typeof(GtaProcessor).Assembly.GetManifestResourceStream("black.bik"))
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
        // Check if entry is a map file (buildings, roads, landscape)
        // EXCLUDES: peds (a_c_, a_f_, a_m_, cs_, ig_, mp_f_, mp_m_, p_), props (prop_), vehicles (veh_)
        private bool IsMapFile(RpfFileEntry entry)
        {
            if (!_opts.RemoveMap) return false;
            if (entry.Name == null) return false;
            string lo = entry.Name.ToLowerInvariant();

            // Only process map-related extensions
            if (!lo.EndsWith(".ydr") && !lo.EndsWith(".yft") && !lo.EndsWith(".ytd") && !lo.EndsWith(".ybn"))
                return false;

            // Exclude peds (characters)
            if (lo.StartsWith("a_c_") || lo.StartsWith("a_f_") || lo.StartsWith("a_m_") ||
                lo.StartsWith("cs_") || lo.StartsWith("ig_") ||
                lo.StartsWith("mp_f_") || lo.StartsWith("mp_m_") || lo.StartsWith("p_"))
                return false;

            // Exclude props (movable objects)
            if (lo.StartsWith("prop_")) return false;

            // Exclude vehicles
            if (lo.StartsWith("veh_")) return false;

            // Exclude weapons
            if (lo.Contains("weapon") || lo.StartsWith("w_")) return false;

            // Everything else is map geometry
            return true;
        }
    }
}
