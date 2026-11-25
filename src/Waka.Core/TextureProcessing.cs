using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using CodeWalker.GameFiles;
using CodeWalker.Utils;

namespace Waka.Core
{
    internal sealed partial class GtaProcessor
    {
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        [System.Security.SecurityCritical]
        private bool ProcessDict(TextureDictionary dict, string fileName)
        {
            if (dict == null || dict.Textures == null || dict.Textures.data_items == null) return false;
            var items = dict.Textures.data_items;
            int changedCount = 0;
            // Параллельная обработка текстур: каждая tex независима, ProcessTex
            // пишет только в поля tex (уже лежит в массиве) — гонок нет.
            System.Threading.Tasks.Parallel.For(0, items.Length,
                new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = _opts.MaxThreads },
                delegate(int i)
                {
                    var tex = items[i];
                    if (tex == null) return;
                    try { if (ProcessTex(tex, fileName)) Interlocked.Increment(ref changedCount); }
                    catch (Exception ex)
                    {
                        string tn2 = tex.Name != null ? tex.Name : "?";
                        L("    [tex] error <" + tn2 + "> in " + fileName + ": " + ex.Message, LogLevel.Warn);
                        Interlocked.Increment(ref _stats.TexturesFailed);
                    }
                });
            return changedCount > 0;
        }

        private bool ProcessTex(Texture tex, string fileName)
        {
            // ── ФИЛЬТР ПО ИМЕНИ ТЕКСТУРЫ ──────────────────────────────────────────
            // ShouldSkipEntry работает по пути RPF/файла. Но текстуры воды и тела
            // персонажа часто лежат внутри общих YTD, чей путь не содержит нужных
            // паттернов — они проходили фильтр и портились. Проверяем имя самой
            // текстуры И имя контейнера.
            string nmChk = tex.Name != null ? tex.Name : "";
            if (ShouldProtectPlayerTexture(fileName, nmChk) || (_opts.SkipPeds && IsNpcResource(fileName)))
            { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }
            if (IsWaterFog(nmChk) || IsWaterFog(fileName))
            { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }

            // ── ВЫБОР ПАРАМЕТРОВ СЖАТИЯ ПО КАТЕГОРИИ ───────────────────────────────
            int factor;
            bool gray;
            string tn = tex.Name != null ? tex.Name : "";
            bool isNorm = IsNormalMap(tn), isSpec = IsSpecMap(tn);
            bool isPalette = IsPalette(tn) || IsPalette(fileName);

            // ПАЛИТРЫ: не сжимать (factor=1), ВСЕГДА красить в ч/б (без сохранения розового)
            if (isPalette)
            {
                factor = 1;
                bool paletteGray = IsClothing(fileName) ? _opts.ClothingUseGrayscale : IsPed(fileName) ? _opts.PedsUseGrayscale : _opts.UseGrayscale;
                gray = paletteGray && !isNorm && !isSpec;
                if (!gray) { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }
                if (_opts.VerboseLog) L("    [PALETTE] " + tn + " in " + fileName + " gray=" + gray, LogLevel.Info);
            }
            else if (IsClothing(fileName))
            {
                factor = _opts.ClothingCompressionFactor;
                gray = _opts.ClothingUseGrayscale && !isNorm && !isSpec;
            }
            else if (IsPed(fileName))
            {
                factor = _opts.PedsCompressionFactor;
                gray = _opts.PedsUseGrayscale && !isNorm && !isSpec;
            }
            else
            {
                factor = _opts.CompressionFactor;
                gray = _opts.UseGrayscale && !isNorm && !isSpec;
            }

            // ── РАННИЙ ВЫХОД без DDSIO ─────────────────────────────────────────────
            // tex.Width/Height доступны прямо из ресурса. Если текстура заведомо
            // слишком мала — выходим ДО GetDDSFile, который аллоцирует и копирует
            // весь блок пикселей. На мелких текстурах это основная экономия.
            // НО! Палитры (_pal) всегда узкие (256x4), их не пропускаем!
            int srcW0 = 0, srcH0 = 0;
            try { srcW0 = tex.Width; srcH0 = tex.Height; } catch { }
            if (srcW0 > 0 && srcH0 > 0)
            {
                // ПАЛИТРЫ: не пропускаем по размеру - они обрабатываются для grayscale!
                if (!gray && !isPalette && (srcW0 < _opts.MinTextureSize * 2 || srcH0 < _opts.MinTextureSize * 2))
                { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }

                // Проверяем, даст ли уменьшение вообще что-то — тоже до GetDDSFile
                // ПАЛИТРЫ: не применяем minSize - они всегда обрабатываются для grayscale!
                bool   ns0    = IsNormalMap(tn) || IsSpecMap(tn);
                int    minSz0 = isPalette ? 1 : (ns0 ? 32 : _opts.MinTextureSize);
                int    pw     = Math.Max(minSz0, srcW0 / factor);
                int    ph     = Math.Max(minSz0, srcH0 / factor);
                if (!gray && !isPalette && pw >= srcW0 && ph >= srcH0)
                { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }
            }

            byte[] dds = null;
            try { dds = DDSIO.GetDDSFile(tex); }
            catch (Exception ex)
            {
                string tn3 = tex.Name != null ? tex.Name : "?";
                L("    [tex] DDSIO fail <" + tn3 + ">: " + ex.Message, LogLevel.Warn);
                Interlocked.Increment(ref _stats.TexturesSkipped);
                return false;
            }
            if (dds == null || dds.Length < 128)
            { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }

            var hdr = DdsEngine.ParseHeader(dds);
            if (hdr.Format == DdsEngine.DdsFmt.Unknown || hdr.Format == DdsEngine.DdsFmt.Bc7)
            {
                string tn4 = tex.Name != null ? tex.Name : "?";
                L("    [tex] skip <" + tn4 + "> [" + hdr.FormatName + "] in " + fileName);
                Interlocked.Increment(ref _stats.TexturesSkipped);
                return false;
            }

            // ПАЛИТРЫ: не пропускаем по размеру DDS - они обрабатываются для grayscale!
            if (hdr.Width < _opts.MinTextureSize * 2 || hdr.Height < _opts.MinTextureSize * 2)
            {
                if (!gray && !isPalette)
                { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }
            }

            // нормали/спек: не капим factor, но минимальный размер 32x32
            // ПАЛИТРЫ: не применяем minSize - они всегда обрабатываются для grayscale!
            int minSize = isPalette ? 1 : ((isNorm || isSpec) ? 32 : _opts.MinTextureSize);

            int tw = Math.Min(hdr.Width, Math.Max(minSize, hdr.Width / factor));
            int th = Math.Min(hdr.Height, Math.Max(minSize, hdr.Height / factor));

            // ПАЛИТРЫ: если factor=1, всё равно обрабатываем (для grayscale)
            if (!gray && !isPalette && tw >= hdr.Width && th >= hdr.Height)
            { Interlocked.Increment(ref _stats.TexturesSkipped); return false; }

            // ── BYPASS DDSIO.GetTexture ────────────────────────────────────────────
            // DDSIO.GetTexture(dds) использует SharpDX/D3D11 и крашит процесс через
            // AccessViolationException на некоторых текстурах — некатчуемо даже с
            // [HandleProcessCorruptedStateExceptions] т.к. после первого AVE D3D-девайс
            // остаётся в сломанном состоянии и следующий вызов убивает CLR насмерть.
            // Решение: вызываем DecompressToRgba/BilinearResize/BuildMipData напрямую
            // и записываем результат прямо в поля tex — без D3D11 вообще.
            // ──────────────────────────────────────────────────────────────────────

            // 1. Распаковать в RGBA — но НЕ mip 0, а наименьший mip >= цели.
            //    Если исходник 2048x2048 и цель 512x512, в DDS уже лежит готовый
            //    mip 2 = 512x512. Декодим его вместо 2048x2048 — экономия до 16x.
            var pick = DdsEngine.PickSourceMip(dds, hdr, tw, th);

            byte[] rgba = null;
            try { rgba = DdsEngine.DecompressRegion(dds, hdr.Format, pick.Offset, pick.Width, pick.Height); }
            catch (Exception ex)
            {
                L("    [tex] Decompress fail <" + tn + ">: " + ex.Message, LogLevel.Warn);
                Interlocked.Increment(ref _stats.TexturesFailed);
                return false;
            }
            if (rgba == null || rgba.Length == 0)
            { Interlocked.Increment(ref _stats.TexturesFailed); return false; }

            // 2. Grayscale (опционально)
            if (gray)
            {
                try
                {
                    // ДЛЯ ВСЕХ: обычное ч/б без сохранения розового
                    DdsEngine.ApplyGreyscale(rgba, pick.Width, pick.Height);
                }
                catch { }
            }

            // 3. Ресайз до целевого размера (часто уже 1:1 после mip-выбора)
            byte[] resized = null;
            try
            {
                resized = (pick.Width == tw && pick.Height == th)
                        ? rgba
                        : DdsEngine.BilinearResize(rgba, pick.Width, pick.Height, tw, th);
            }
            catch (Exception ex)
            {
                L("    [tex] Resize fail <" + tn + ">: " + ex.Message, LogLevel.Warn);
                Interlocked.Increment(ref _stats.TexturesFailed);
                return false;
            }
            if (resized == null || resized.Length == 0)
            { Interlocked.Increment(ref _stats.TexturesFailed); return false; }

            // 4. Сжать в BC + сгенерировать мип-цепочку
            int mipCount = 1;
            byte[] mipData = null;
            try { mipData = DdsEngine.BuildMipData(resized, tw, th, hdr.Format, out mipCount); }
            catch (Exception ex)
            {
                L("    [tex] BuildMips fail <" + tn + ">: " + ex.Message, LogLevel.Warn);
                Interlocked.Increment(ref _stats.TexturesFailed);
                return false;
            }
            if (mipData == null || mipData.Length == 0)
            { Interlocked.Increment(ref _stats.TexturesFailed); return false; }

            // 5. Записать результат прямо в tex — никакого DDSIO/SharpDX
            // Stride = (ceil(w/4)) * bytesPerBlock;  BC1/BC4=8, BC2/3/5=16
            int blocksW = Math.Max(1, (tw + 3) / 4);
            ushort newStride = (ushort)(blocksW * ((hdr.Format == DdsEngine.DdsFmt.Bc1 || hdr.Format == DdsEngine.DdsFmt.Bc4) ? 8 : 16));

            string display = tn.Length > 28 ? tn.Substring(0, 28) : tn;
            string pad = new string(' ', Math.Max(0, 28 - display.Length));
            if (_opts.VerboseLog) L(string.Format("    [tex] {0}{1} {2}x{3} -> {4}x{5}  {6}{7}",
                display, pad, hdr.Width, hdr.Height, tw, th,
                hdr.Format, gray ? " +gray" : ""));

            tex.Width  = (ushort)tw;
            tex.Height = (ushort)th;
            tex.Levels = (byte)mipCount;
            tex.Stride = newStride;
            // tex.Format не меняем — формат остаётся тем же (BC1→BC1, BC3→BC3)
            if (tex.Data == null) tex.Data = new TextureData();
            tex.Data.FullData = mipData;
            Interlocked.Increment(ref _stats.TexturesShrunken);
            return true;
        }

        private static bool IsNormalMap(string n)
        {
            string s = n.ToLowerInvariant();
            return s.EndsWith("_n") || s.Contains("_n_") || s.Contains("normal")
                || s.Contains("_nrm") || s.Contains("_bump");
        }
        private static bool IsSpecMap(string n)
        {
            string s = n.ToLowerInvariant();
            return s.EndsWith("_s") || s.Contains("_s_") || s.Contains("spec")
                || s.Contains("_gloss") || s.Contains("_rough");
        }

        // ── ПАЛИТРЫ (_pal) ────────────────────────────────────────────────────────
        // Палитры не сжимать (factor=1), но красить в ч/б с сохранением розового
        private static bool IsPalette(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            string p = s.ToLowerInvariant();
            string name = Path.GetFileNameWithoutExtension(p);
            return name.EndsWith("_pal") || name.Contains("_pal_") || name.EndsWith("_palette") || name == "palette";
        }

    }
}
