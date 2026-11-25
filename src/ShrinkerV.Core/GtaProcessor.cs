using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using CodeWalker.GameFiles;
using CodeWalker.Utils;

namespace ShrinkerV.Core
{
    internal sealed partial class GtaProcessor
    {
        private readonly ProcessingOptions        _opts;
        private readonly Action<string,LogLevel>  _log;
        private readonly ProcessingStats          _stats = new ProcessingStats();
        // CodeWalker имеет статический стейт в RpfFile.CreateFile (таблица ключей,
        // пул памяти). Все вызовы CreateFile идут через этот lock — остальная работа
        // (декодинг/кодирование текстур) остаётся параллельной.
        private static readonly object _createFileLock = new object();
        private static void SafeCreateFile(RpfDirectoryEntry parent, string name, byte[] data, bool compress)
        { lock (_createFileLock) { RpfFile.CreateFile(parent, name, data, compress); } }

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
            // CodeWalker (DDSIO, RpfFile.CreateFile) использует статический стейт
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
                string root = RuntimeFiles.DirectoryPath;
                string aesPath = Path.Combine(root, "gtav_aes_key.dat");
                if (!File.Exists(aesPath)) return false;
                byte[] aes = File.ReadAllBytes(aesPath);
                // LoadFromPath загружает только decrypt-таблицы (и ключи).
                // Encrypt-таблицы и LUT нужны для сохранения NG-RPF — грузим отдельно.
                GTA5Keys.LoadFromPath(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, false, Convert.ToBase64String(aes));

                string encTabFile = Path.Combine(root, "gtav_ng_encrypt_tables.dat");
                string encLutFile = Path.Combine(root, "gtav_ng_encrypt_luts.dat");
                if (File.Exists(encTabFile))
                {
                    try
                    {
                        GTA5Keys.PC_NG_ENCRYPT_TABLES = CryptoIO.ReadNgTables(encTabFile);
                        L("  Encrypt-таблицы загружены");
                    }
                    catch (Exception ex) { L("  Encrypt-таблицы: " + ex.Message, LogLevel.Warn); }
                }
                if (File.Exists(encLutFile))
                {
                    try
                    {
                        L("  Загружаю LUT (53 MB)...");
                        GTA5Keys.PC_NG_ENCRYPT_LUTs = CryptoIO.ReadNgLuts(encLutFile);
                        L("  LUT загружены");
                    }
                    catch (Exception ex) { L("  LUT: " + ex.Message, LogLevel.Warn); }
                }
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
                    if (IsProtectedByPath(f) || (_opts.ProtectCharacters && IsPlayerResource(f)) || (_opts.SkipPeds && IsNpcResource(f)))
                    {
                        L("  Без обработки (будет копия 1:1): " + Path.GetFileName(f));
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
                File.Copy(source, staged);
                File.SetAttributes(staged, File.GetAttributes(staged) & ~FileAttributes.ReadOnly);
                int changes = 0;
                long before = new FileInfo(staged).Length;
                if (!copyOnly)
                {
                    L("АРХИВ: " + name);
                    var archive = new RpfFile(staged, name);
                    archive.ScanStructure(null, null);
                    if (archive.LastException != null) throw new InvalidDataException(name, archive.LastException);
                    if (archive.AllEntries == null) throw new InvalidDataException(name);
                    int errorsBefore = _stats.ErrorsEncountered;
                    changes = ProcessRpfRecursive(archive, name, 0);
                    _opts.Cancel.ThrowIfCancellationRequested();
                    if (_stats.ErrorsEncountered != errorsBefore)
                        throw new InvalidDataException("Ошибка обработки записей " + name);
                    if (changes > 0)
                    {
                        RpfFile.Defragment(archive, null, true);
                        if (!VerifyRpfVsSource(staged, source))
                            throw new InvalidDataException("Архив не прошёл проверку: " + name);
                    }
                }
                long after = new FileInfo(staged).Length;
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

        // ── #3: ВЕРИФИКАЦИЯ АРХИВА ────────────────────────────────────────────────
        // Сравнивает output RPF с оригиналом (srcPath):
        //   1. Все entries источника должны присутствовать в output
        //   2. Ни одна запись не должна иметь нулевой размер (если в src > 0)
        //   3. Все вложенные RPF output должны читаться
        // Если srcPath == outPath (in-place), падаем на базовую структурную проверку.
        private bool VerifyRpfVsSource(string outPath, string srcPath)
        {
            try
            {
                var fi = new FileInfo(outPath);
                if (!fi.Exists || fi.Length < 2048)
                { L("    verify: output пуст/слишком мал", LogLevel.Warn); return false; }

                // Читаем output RPF
                var outRpf = new RpfFile(outPath, Path.GetFileName(outPath));
                outRpf.ScanStructure(null, null);

                int outCount = 0;
                try { outCount = outRpf.AllEntries != null ? outRpf.AllEntries.Count : 0; }
                catch { return false; }
                if (outCount == 0) { L("    verify: output не содержит записей", LogLevel.Warn); return false; }

                // Все вложенные RPF должны читаться
                int badChildren = 0;
                try
                {
                    if (outRpf.Children != null)
                        foreach (var c in outRpf.Children)
                        {
                            if (c == null) continue;
                            try { if (c.AllEntries == null || c.AllEntries.Count == 0) badChildren++; }
                            catch { badChildren++; }
                        }
                }
                catch { return false; }

                if (badChildren > 0)
                {
                    L(string.Format("    verify: {0} вложенных RPF нечитаемы", badChildren), LogLevel.Warn);
                    return false;
                }

                // Читаем src RPF — собираем { path → FileSize }
                // Используем имя entry как ключ (Path если есть, иначе Name)
                var srcSizes = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var srcRpf = new RpfFile(srcPath, Path.GetFileName(srcPath));
                    srcRpf.ScanStructure(null, null);
                    if (srcRpf.AllEntries != null)
                    {
                        foreach (var e in srcRpf.AllEntries.OfType<RpfFileEntry>())
                        {
                            if (e == null || string.IsNullOrEmpty(e.Name)) continue;
                            string key = !string.IsNullOrEmpty(e.Path) ? e.Path : e.Name;
                            long sz = 0;
                            try { sz = e.FileSize; } catch { }
                            srcSizes[key] = sz;
                        }
                    }
                }
                catch (Exception srcEx)
                {
                    L("verify: " + srcEx.Message, LogLevel.Error);
                    return false;
                }

                if (srcSizes.Count == 0)
                {
                    L("    verify OK (structural, src пустой): " + outCount + " записей");
                    return true;
                }

                // Собираем output entries
                var outPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var outSizes = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
                foreach (var e in outRpf.AllEntries.OfType<RpfFileEntry>())
                {
                    if (e == null || string.IsNullOrEmpty(e.Name)) continue;
                    string key = !string.IsNullOrEmpty(e.Path) ? e.Path : e.Name;
                    outPaths.Add(key);
                    long sz = 0;
                    try { sz = e.FileSize; } catch { }
                    outSizes[key] = sz;
                }

                // Сравнение: каждая src-запись должна быть в output и не быть пустой
                int missing = 0, zeroed = 0;
                foreach (var kvp in srcSizes)
                {
                    if (!outPaths.Contains(kvp.Key))
                    {
                        missing++;
                        if (missing <= 5)
                            L("    verify: ОТСУТСТВУЕТ: " + kvp.Key, LogLevel.Error);
                    }
                    else if (kvp.Value > 256)  // src запись непустая
                    {
                        long outSz;
                        if (outSizes.TryGetValue(kvp.Key, out outSz) && outSz == 0)
                        {
                            zeroed++;
                            if (zeroed <= 3)
                                L("    verify: ОБНУЛЕНА: " + kvp.Key, LogLevel.Warn);
                        }
                    }
                }

                if (missing > 0)
                    L(string.Format("    verify FAIL: {0} записей отсутствует (из {1})", missing, srcSizes.Count),
                        LogLevel.Error);
                if (zeroed > 0)
                    L(string.Format("    verify WARN: {0} записей обнулено", zeroed), LogLevel.Warn);

                if (missing > 0 || zeroed > 0) return false;

                L(string.Format("    verify OK: src {0} = out {1} записей{2}",
                    srcSizes.Count, outCount,
                    zeroed > 0 ? string.Format("  (warn: {0} zeroed)", zeroed) : ""));
                return true;
            }
            catch (Exception ex)
            {
                L("    verify: " + ex.Message, LogLevel.Warn);
                return false;
            }
        }

        // -- RECURSIVE RPF WALK ---------------------------------------------------
        // НЕ вызываем Defragment на дочерних RPF здесь.
        // Только ProcessTopLevel вызывает Defragment(rpf, null, TRUE).

        // Вычислительная фаза батча (без I/O): Load + ProcessDict + Save.
        // Вызывается из Parallel.For — не трогает файл на диске.
        private RpfWorkResult ComputeEntry(RpfWorkItem w, string topName)
        {
            string lo = w.ExtLower;
            byte[] outBytes = null;
            try
            {
                if (lo.EndsWith(".ytd"))
                {
                    var f = new YtdFile(w.Entry);
                    try { f.Load(w.RawData, w.Entry); } catch { return default(RpfWorkResult); }
                    if (f.TextureDict == null) return default(RpfWorkResult);
                    if (!ProcessDict(f.TextureDict, w.EntryPath)) return default(RpfWorkResult);
                    try { outBytes = f.Save(); } catch { return default(RpfWorkResult); }
                }
                else if (lo.EndsWith(".ydr"))
                {
                    var f = new YdrFile(w.Entry);
                    try { f.Load(w.RawData, w.Entry); } catch { return default(RpfWorkResult); }
                    if (f.Drawable == null || f.Drawable.ShaderGroup == null) return default(RpfWorkResult);
                    if (f.Drawable.ShaderGroup.TextureDictionary == null) return default(RpfWorkResult);
                    if (!ProcessDict(f.Drawable.ShaderGroup.TextureDictionary, w.EntryPath)) return default(RpfWorkResult);
                    try { outBytes = f.Save(); } catch { return default(RpfWorkResult); }
                }
                else if (lo.EndsWith(".ydd"))
                {
                    var f = new YddFile(w.Entry);
                    try { f.Load(w.RawData, w.Entry); } catch { return default(RpfWorkResult); }
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
                    try { outBytes = f.Save(); } catch { return default(RpfWorkResult); }
                }
                else if (lo.EndsWith(".yft"))
                {
                    var f = new YftFile(w.Entry);
                    try { f.Load(w.RawData, w.Entry); } catch { return default(RpfWorkResult); }
                    if (f.Fragment == null || f.Fragment.Drawable == null) return default(RpfWorkResult);
                    if (f.Fragment.Drawable.ShaderGroup == null) return default(RpfWorkResult);
                    if (f.Fragment.Drawable.ShaderGroup.TextureDictionary == null) return default(RpfWorkResult);
                    if (!ProcessDict(f.Fragment.Drawable.ShaderGroup.TextureDictionary, w.EntryPath)) return default(RpfWorkResult);
                    try { outBytes = f.Save(); } catch { return default(RpfWorkResult); }
                }
            }
            catch (Exception ex)
            {
                L("    ! compute <" + w.Entry.Name + ">: " + ex.Message, LogLevel.Warn);
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
            // Phase 1: ExtractFile по одному (CodeWalker FileShare.None — только seq)
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
                        try { SafeCreateFile(r[k].Entry.Parent, r[k].Entry.Name, r[k].ResultBytes, true); Interlocked.Increment(ref total); }
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
                    { SafeCreateFile(entry.Parent, entry.Name, _awcStub, true); Interlocked.Increment(ref _stats.AwcReplaced); Interlocked.Increment(ref total); continue; }
                    if (IsLightweightYpt(entry))
                    { SafeCreateFile(entry.Parent, entry.Name, _yptStub, true); Interlocked.Increment(ref total); continue; }
                    if (lo.EndsWith(".bik"))
                    { SafeCreateFile(entry.Parent, entry.Name, GetBikStub(), true); Interlocked.Increment(ref total); continue; }

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
                            SafeCreateFile(entry.Parent, entry.Name, mapStub, true);
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
        internal void LoadReplacementAssets()
        {
            if (!_opts.KeepAudio) _awcStub = RuntimeFiles.Read("embedded_timer.awc");
            if (!_opts.KeepEffects) _yptStub = RuntimeFiles.Read("embedded_cut_iaaj.ypt");
            if (_opts.RemoveMap)
            {
                _ydrStub = RuntimeFiles.Read("minimal_ydr.stub");
                _yftStub = RuntimeFiles.Read("minimal_yft.stub");
                _ybnStub = RuntimeFiles.Read("minimal_ybn.stub");
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
            byte[] stub = new byte[1024];
            // BIKf signature
            stub[0] = (byte)'B'; stub[1] = (byte)'I'; stub[2] = (byte)'K'; stub[3] = (byte)'f';
            // File length - 8
            uint sz = 1024 - 8;
            stub[4] = (byte)(sz & 0xFF);
            stub[5] = (byte)((sz >>  8) & 0xFF);
            stub[6] = (byte)((sz >> 16) & 0xFF);
            stub[7] = (byte)((sz >> 24) & 0xFF);
            // 1 frame (bytes 8-11), largest frame size = 4 (bytes 12-15)
            stub[8]  = 1;
            stub[12] = 4;
            return stub;
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
