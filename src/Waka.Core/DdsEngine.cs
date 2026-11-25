using System;
using System.Collections.Generic;
using System.Linq;

namespace Waka.Core
{
    internal static class DdsEngine
    {
        private const int DDS_MAGIC = 0x20534444;
        private const int FCC_DX10  = 0x30315844; // "DX10"

        public enum DdsFmt { Unknown, Bc1, Bc2, Bc3, Bc4, Bc5, Bc7, Rgba8 }

        public struct DdsHeader
        {
            public int Width, Height, MipCount;
            public DdsFmt Format;
            public string FormatName;    // human-readable name for skip logging
            public int DataOffset;       // 128 for classic, 148 for DX10
        }

        public static DdsHeader ParseHeader(byte[] dds)
        {
            var h = new DdsHeader { FormatName = "Unknown", DataOffset = 128 };
            if (dds == null || dds.Length < 128) return h;
            if (BitConverter.ToInt32(dds, 0) != DDS_MAGIC) return h;
            h.Height   = BitConverter.ToInt32(dds, 12);
            h.Width    = BitConverter.ToInt32(dds, 16);
            h.MipCount = BitConverter.ToInt32(dds, 28);
            if (h.MipCount == 0) h.MipCount = 1;
            int pfFlags = BitConverter.ToInt32(dds, 80);
            int fourcc  = BitConverter.ToInt32(dds, 84);
            int rgbBits = BitConverter.ToInt32(dds, 88);

            const int FCC_DXT1 = 0x31545844;
            const int FCC_DXT2 = 0x32545844; // DXT2 (BC2 premultiplied alpha)
            const int FCC_DXT3 = 0x33545844; // DXT3 (BC2)
            const int FCC_DXT5 = 0x35545844;
            const int FCC_ATI1 = 0x31495441;
            const int FCC_ATI2 = 0x32495441;
            const int FCC_BC5U = 0x55354342;
            const int FCC_BC5S = 0x53354342;

            if ((pfFlags & 4) != 0)
            {
                if (fourcc == FCC_DX10 && dds.Length >= 148)
                {
                    // DX10 extended header at offset 128
                    int dxgiFmt = BitConverter.ToInt32(dds, 128);
                    h.DataOffset = 148;
                    h.Format     = DxgiToFmt(dxgiFmt, out h.FormatName);
                }
                else if (fourcc == FCC_DXT1) { h.Format = DdsFmt.Bc1;  h.FormatName = "BC1/DXT1"; }
                else if (fourcc == FCC_DXT2 || fourcc == FCC_DXT3) { h.Format = DdsFmt.Bc2;  h.FormatName = "BC2/DXT2-3"; }
                else if (fourcc == FCC_DXT5) { h.Format = DdsFmt.Bc3;  h.FormatName = "BC3/DXT5"; }
                else if (fourcc == FCC_ATI1) { h.Format = DdsFmt.Bc4;  h.FormatName = "BC4/ATI1"; }
                else if (fourcc == FCC_ATI2 || fourcc == FCC_BC5U || fourcc == FCC_BC5S)
                                             { h.Format = DdsFmt.Bc5;  h.FormatName = "BC5/ATI2"; }
                else
                {
                    h.Format = DdsFmt.Unknown;
                    h.FormatName = "FCC_" + ((char)(fourcc & 0xFF)).ToString()
                                          + ((char)((fourcc >> 8) & 0xFF)).ToString()
                                          + ((char)((fourcc >> 16) & 0xFF)).ToString()
                                          + ((char)((fourcc >> 24) & 0xFF)).ToString();
                }
            }
            else if ((pfFlags & 0x40) != 0 && rgbBits == 32) { h.Format = DdsFmt.Rgba8; h.FormatName = "RGBA8"; }
            else if ((pfFlags & 0x40) != 0 && rgbBits == 24) { h.Format = DdsFmt.Unknown; h.FormatName = "RGB24"; }
            else if ((pfFlags & 0x40) != 0 && rgbBits == 16) { h.Format = DdsFmt.Unknown; h.FormatName = "RGB16"; }
            else if ((pfFlags & 0x20000) != 0 && rgbBits == 8) { h.Format = DdsFmt.Unknown; h.FormatName = "L8"; }
            else if ((pfFlags & 1) != 0 && rgbBits == 8) { h.Format = DdsFmt.Unknown; h.FormatName = "A8"; }
            else if (rgbBits > 0)
            {
                h.Format = DdsFmt.Unknown;
                h.FormatName = "Unknown" + rgbBits + "bit";
            }
            return h;
        }

        private static DdsFmt DxgiToFmt(int dxgi, out string name)
        {
            switch (dxgi)
            {
                case 71: case 72: name = "BC1/DX10";  return DdsFmt.Bc1;
                case 74: case 75: name = "BC2/DX10";  return DdsFmt.Bc2; // BC2→treat as BC3
                case 77: case 78: name = "BC3/DX10";  return DdsFmt.Bc3;
                case 80: case 81: name = "BC4/DX10";  return DdsFmt.Bc4;
                case 83: case 84: name = "BC5/DX10";  return DdsFmt.Bc5;
                case 95: case 96: name = "BC6H/DX10"; return DdsFmt.Unknown; // HDR — skip
                case 98: case 99: name = "BC7/DX10";  return DdsFmt.Bc7;
                case 28: name = "RGBA8/DX10";          return DdsFmt.Rgba8;
                case 87: name = "BGRA8/DX10";          return DdsFmt.Rgba8; // will swap channels
                default: name = "DXGI_" + dxgi;        return DdsFmt.Unknown;
            }
        }

        // ── DECOMPRESS ────────────────────────────────────────────────────────────
        public static int BytesPerBlock(DdsFmt f)
        { return (f == DdsFmt.Bc1 || f == DdsFmt.Bc4) ? 8 : 16; }

        // Размер одного mip-уровня в байтах.
        private static int MipByteSize(int w, int h, DdsFmt fmt)
        {
            if (fmt == DdsFmt.Rgba8) return w * h * 4;
            return Math.Max(1, (w + 3) / 4) * Math.Max(1, (h + 3) / 4) * BytesPerBlock(fmt);
        }

        // ── ВЫБОР ИСХОДНОГО MIP ───────────────────────────────────────────────────
        // Ключевая оптимизация: нет смысла декодить mip 0 (2048x2048) и потом
        // ресайзить до 512x512, если в DDS уже лежит готовый mip 2 = 512x512.
        // Находим наименьший mip, который >= целевого размера, и декодим ЕГО.
        // Экономия до 16x работы декодера на крупных текстурах.
        public struct MipPick { public int Offset, Width, Height; }

        public static MipPick PickSourceMip(byte[] dds, DdsHeader hdr, int targetW, int targetH)
        {
            var pick = new MipPick
            {
                Offset = hdr.DataOffset > 0 ? hdr.DataOffset : 128,
                Width  = hdr.Width,
                Height = hdr.Height
            };
            int levels = hdr.MipCount > 0 ? hdr.MipCount : 1;
            if (levels <= 1) return pick;

            int off = pick.Offset, w = hdr.Width, h = hdr.Height;
            for (int lvl = 0; lvl < levels; lvl++)
            {
                int sz = MipByteSize(w, h, hdr.Format);
                // Данных на этот уровень не хватает — дальше идти нельзя
                if (off + sz > dds.Length) break;

                // Этот уровень ещё покрывает цель — запоминаем как лучший
                if (w >= targetW && h >= targetH)
                { pick.Offset = off; pick.Width = w; pick.Height = h; }
                else break; // уже мельче цели — предыдущий был оптимальным

                if (w <= 4 || h <= 4) break;
                off += sz;
                w = Math.Max(1, w / 2);
                h = Math.Max(1, h / 2);
            }
            return pick;
        }

        // Декод произвольного mip-уровня. Пишет напрямую в byte[] — без Color[].
        public static byte[] DecompressRegion(byte[] dds, DdsFmt fmt, int dataOff, int w, int h)
        {
            byte[] rgba = new byte[w * h * 4];
            if (fmt == DdsFmt.Rgba8)
            {
                int len = Math.Min(w * h * 4, dds.Length - dataOff);
                if (len > 0) Buffer.BlockCopy(dds, dataOff, rgba, 0, len);
                return rgba;
            }

            int blocksX = Math.Max(1, (w + 3) / 4);
            int blocksY = Math.Max(1, (h + 3) / 4);
            int bpb     = BytesPerBlock(fmt);

            // Крупные текстуры декодим построчно параллельно.
            // Каждая строка блоков пишет в свою полосу rgba — гонок нет.
            if (blocksY >= 8 && blocksX * blocksY >= 1024)
            {
                System.Threading.Tasks.Parallel.For(0, blocksY, by =>
                { DecodeBlockRow(dds, fmt, dataOff, w, h, blocksX, bpb, by, rgba); });
            }
            else
            {
                for (int by = 0; by < blocksY; by++)
                    DecodeBlockRow(dds, fmt, dataOff, w, h, blocksX, bpb, by, rgba);
            }
            return rgba;
        }

        // Декодит одну строку блоков. blk — локальный буфер, переиспользуется
        // на всю строку вместо аллокации Color[16] на каждый блок.
        private static void DecodeBlockRow(byte[] dds, DdsFmt fmt, int dataOff,
            int w, int h, int blocksX, int bpb, int by, byte[] rgba)
        {
            byte[] blk = new byte[64];
            for (int bx = 0; bx < blocksX; bx++)
            {
                int off = dataOff + (by * blocksX + bx) * bpb;
                if (off + bpb > dds.Length) break;

                switch (fmt)
                {
                    case DdsFmt.Bc1: DecBC1(dds, off, blk); break;
                    case DdsFmt.Bc2: DecBC2(dds, off, blk); break;
                    case DdsFmt.Bc3: DecBC3(dds, off, blk); break;
                    case DdsFmt.Bc4: DecBC4Gray(dds, off, blk); break;
                    case DdsFmt.Bc5: DecBC5(dds, off, blk); break;
                    case DdsFmt.Bc7: throw new NotSupportedException("BC7");
                    default: continue;
                }

                int baseX = bx * 4, baseY = by * 4;
                int maxPy = Math.Min(4, h - baseY);
                int maxPx = Math.Min(4, w - baseX);
                for (int py = 0; py < maxPy; py++)
                {
                    int di = ((baseY + py) * w + baseX) * 4;
                    int si = py * 16;
                    Buffer.BlockCopy(blk, si, rgba, di, maxPx * 4);
                }
            }
        }

        // ── ДЕКОДЕРЫ БЕЗ АЛЛОКАЦИЙ ────────────────────────────────────────────────
        // Пишут 4x4 RGBA прямо в blk[64]. Никаких Color[]/byte[] на блок.

        private static void DecBC1(byte[] d, int o, byte[] blk)
        {
            int c0 = d[o] | (d[o + 1] << 8);
            int c1 = d[o + 2] | (d[o + 3] << 8);

            // распаковка RGB565 → 8 бит, без Color
            int r0 = (c0 >> 11) & 0x1F, g0 = (c0 >> 5) & 0x3F, b0 = c0 & 0x1F;
            int r1 = (c1 >> 11) & 0x1F, g1 = (c1 >> 5) & 0x3F, b1 = c1 & 0x1F;
            r0 = (r0 << 3) | (r0 >> 2); g0 = (g0 << 2) | (g0 >> 4); b0 = (b0 << 3) | (b0 >> 2);
            r1 = (r1 << 3) | (r1 >> 2); g1 = (g1 << 2) | (g1 >> 4); b1 = (b1 << 3) | (b1 >> 2);

            // палитра 4 цвета в плоских массивах на стеке
            int pr0 = r0, pg0 = g0, pb0 = b0, pa0 = 255;
            int pr1 = r1, pg1 = g1, pb1 = b1, pa1 = 255;
            int pr2, pg2, pb2, pa2, pr3, pg3, pb3, pa3;
            if (c0 > c1)
            {
                pr2 = (r0 * 2 + r1) / 3; pg2 = (g0 * 2 + g1) / 3; pb2 = (b0 * 2 + b1) / 3; pa2 = 255;
                pr3 = (r0 + r1 * 2) / 3; pg3 = (g0 + g1 * 2) / 3; pb3 = (b0 + b1 * 2) / 3; pa3 = 255;
            }
            else
            {
                pr2 = (r0 + r1) / 2; pg2 = (g0 + g1) / 2; pb2 = (b0 + b1) / 2; pa2 = 255;
                pr3 = 0; pg3 = 0; pb3 = 0; pa3 = 0; // 1-bit alpha: индекс 3 прозрачный
            }

            uint bits = (uint)(d[o + 4] | (d[o + 5] << 8) | (d[o + 6] << 16) | (d[o + 7] << 24));
            for (int i = 0; i < 16; i++)
            {
                int idx = (int)((bits >> (i * 2)) & 3);
                int di  = i * 4;
                if (idx == 0)      { blk[di]=(byte)pr0; blk[di+1]=(byte)pg0; blk[di+2]=(byte)pb0; blk[di+3]=(byte)pa0; }
                else if (idx == 1) { blk[di]=(byte)pr1; blk[di+1]=(byte)pg1; blk[di+2]=(byte)pb1; blk[di+3]=(byte)pa1; }
                else if (idx == 2) { blk[di]=(byte)pr2; blk[di+1]=(byte)pg2; blk[di+2]=(byte)pb2; blk[di+3]=(byte)pa2; }
                else               { blk[di]=(byte)pr3; blk[di+1]=(byte)pg3; blk[di+2]=(byte)pb3; blk[di+3]=(byte)pa3; }
            }
        }

        // Восстанавливает 8 уровней BC4/alpha-канала в переданный vals[8]
        private static void BuildRamp8(byte v0, byte v1, byte[] vals)
        {
            vals[0] = v0; vals[1] = v1;
            if (v0 > v1)
                for (int i = 2; i < 8; i++) vals[i] = (byte)(((8 - i) * v0 + (i - 1) * v1) / 7);
            else
            {
                for (int i = 2; i < 6; i++) vals[i] = (byte)(((6 - i) * v0 + (i - 1) * v1) / 5);
                vals[6] = 0; vals[7] = 255;
            }
        }

        private static ulong Read48(byte[] d, int o)
        {
            return (ulong)d[o] | ((ulong)d[o+1] << 8) | ((ulong)d[o+2] << 16)
                 | ((ulong)d[o+3] << 24) | ((ulong)d[o+4] << 32) | ((ulong)d[o+5] << 40);
        }

        [ThreadStatic] private static byte[] _ramp;
        private static byte[] Ramp { get { if (_ramp == null) _ramp = new byte[8]; return _ramp; } }

        private static void DecBC2(byte[] d, int o, byte[] blk)
        {
            DecBC1(d, o + 8, blk);                 // RGB из BC1-части (байты 8-15)
            // Explicit alpha: 8 байт = 16 пикселей × 4 бита
            for (int i = 0; i < 16; i++)
            {
                int byteIdx = i / 2;               // каждый байт = 2 пикселя
                int shift = (i & 1) == 0 ? 0 : 4;  // low/high nibble
                int a4 = (d[o + byteIdx] >> shift) & 0xF;
                blk[i * 4 + 3] = (byte)((a4 << 4) | a4); // 4-bit → 8-bit: repeat nibble
            }
        }

        private static void DecBC3(byte[] d, int o, byte[] blk)
        {
            DecBC1(d, o + 8, blk);                 // RGB из BC1-части
            byte[] a = Ramp;
            BuildRamp8(d[o], d[o + 1], a);
            ulong abits = Read48(d, o + 2);
            for (int i = 0; i < 16; i++)
                blk[i * 4 + 3] = a[(int)((abits >> (i * 3)) & 7)];
        }

        private static void DecBC4Gray(byte[] d, int o, byte[] blk)
        {
            byte[] v = Ramp;
            BuildRamp8(d[o], d[o + 1], v);
            ulong bits = Read48(d, o + 2);
            for (int i = 0; i < 16; i++)
            {
                byte g = v[(int)((bits >> (i * 3)) & 7)];
                int di = i * 4;
                blk[di] = g; blk[di + 1] = g; blk[di + 2] = g; blk[di + 3] = 255;
            }
        }

        private static void DecBC5(byte[] d, int o, byte[] blk)
        {
            byte[] v = Ramp;
            // R-канал
            BuildRamp8(d[o], d[o + 1], v);
            ulong rb = Read48(d, o + 2);
            for (int i = 0; i < 16; i++) blk[i * 4] = v[(int)((rb >> (i * 3)) & 7)];
            // G-канал
            BuildRamp8(d[o + 8], d[o + 9], v);
            ulong gb = Read48(d, o + 10);
            for (int i = 0; i < 16; i++)
            {
                int di = i * 4;
                blk[di + 1] = v[(int)((gb >> (i * 3)) & 7)];
                blk[di + 2] = 128; blk[di + 3] = 255;
            }
        }

        private static ushort ToRgb565(byte r,byte g,byte b)
        { return (ushort)(((r>>3)<<11)|((g>>2)<<5)|(b>>3)); }

        // ── BILINEAR RESIZE ───────────────────────────────────────────────────────
        public static byte[] BilinearResize(byte[] rgba, int sw, int sh, int dw, int dh)
        {
            if (sw<=0||sh<=0||dw<=0||dh<=0) return rgba;
            byte[] out2 = new byte[dw*dh*4];
            double xr=(double)sw/dw, yr=(double)sh/dh;
            // Параллельно по строкам на крупных изображениях; каждая строка пишет
            // в свой диапазон out2 — блокировок не нужно.
            System.Action<int> doRow = delegate(int y)
            {
                double sy=(y+0.5)*yr-0.5; if(sy<0)sy=0;
                int y0=(int)sy, y1=System.Math.Min(y0+1,sh-1); double fy=sy-y0;
                for(int x=0;x<dw;x++)
                {
                    double sx=(x+0.5)*xr-0.5; if(sx<0)sx=0;
                    int x0=(int)sx, x1=System.Math.Min(x0+1,sw-1); double fx=sx-x0;
                    int i00=(y0*sw+x0)*4,i10=(y0*sw+x1)*4,i01=(y1*sw+x0)*4,i11=(y1*sw+x1)*4;
                    int di=(y*dw+x)*4;
                    for(int c=0;c<4;c++)
                    {
                        double top=rgba[i00+c]*(1-fx)+rgba[i10+c]*fx;
                        double bot=rgba[i01+c]*(1-fx)+rgba[i11+c]*fx;
                        out2[di+c]=(byte)System.Math.Max(0,System.Math.Min(255,(int)(top*(1-fy)+bot*fy+0.5)));
                    }
                }
            };
            if (dh >= 16 && (long)dw * dh >= 4096)
                System.Threading.Tasks.Parallel.For(0, dh, doRow);
            else
                for (int y = 0; y < dh; y++) doRow(y);
            return out2;
        }

        public static void ApplyGreyscale(byte[] rgba, int w, int h)
        {
            for(int i=0;i<w*h*4;i+=4)
            {
                byte lum=(byte)(rgba[i]*0.2126+rgba[i+1]*0.7152+rgba[i+2]*0.0722);
                rgba[i]=rgba[i+1]=rgba[i+2]=lum;
            }
        }

        // ── BLOCK FILL (без аллокаций) ────────────────────────────────────────────
        // Заполняет существующий blk[64] пикселями тайла (bx,by) из rgba.
        // Используется всеми компрессорами вместо Extract4x4, который аллоцирует.
        private static void Fill4x4(byte[] rgba, int w, int h, int bx, int by, byte[] blk)
        {
            for(int py=0;py<4;py++) for(int px=0;px<4;px++)
            {
                int sx=bx*4+px, sy=by*4+py;
                if(sx>=w)sx=w-1; if(sx<0)sx=0;
                if(sy>=h)sy=h-1; if(sy<0)sy=0;
                int si=(sy*w+sx)*4, di=(py*4+px)*4;
                if(si+3<rgba.Length){blk[di]=rgba[si];blk[di+1]=rgba[si+1];blk[di+2]=rgba[si+2];blk[di+3]=rgba[si+3];}
                else{blk[di]=0;blk[di+1]=0;blk[di+2]=0;blk[di+3]=255;}
            }
        }

        // ── BC1 COMPRESS ──────────────────────────────────────────────────────────
        // Параллель по строкам блоков. Каждая строка пишет в свой участок out2
        // (offset детерминирован от by) — блокировок не нужно.
        public static byte[] CompressBC1(byte[] rgba, int w, int h)
        {
            int bx=Math.Max(1,(w+3)/4), by2=Math.Max(1,(h+3)/4);
            byte[] out2=new byte[bx*by2*8];
            RunBlockRows(by2, bx, delegate(int by, byte[] blk)
            {
                int off = by * bx * 8;
                for (int bxx = 0; bxx < bx; bxx++)
                { Fill4x4(rgba, w, h, bxx, by, blk); EncBC1(blk, out2, off); off += 8; }
            });
            return out2;
        }

        public static byte[] CompressBC2(byte[] rgba, int w, int h)
        {
            int bx=Math.Max(1,(w+3)/4), by2=Math.Max(1,(h+3)/4);
            byte[] out2=new byte[bx*by2*16];
            RunBlockRows(by2, bx, delegate(int by, byte[] blk)
            {
                int off = by * bx * 16;
                for (int bxx = 0; bxx < bx; bxx++)
                {
                    Fill4x4(rgba, w, h, bxx, by, blk);
                    // BC2: 8 байт explicit alpha + 8 байт BC1 для RGB
                    for (int i = 0; i < 16; i++)
                    {
                        int byteIdx = i / 2;
                        int shift = (i & 1) == 0 ? 0 : 4;
                        int a8 = blk[i * 4 + 3];
                        int a4 = a8 >> 4; // 8-bit → 4-bit (теряем младшие биты)
                        out2[off + byteIdx] |= (byte)(a4 << shift);
                    }
                    EncBC1(blk, out2, off + 8); // RGB в BC1
                    off += 16;
                }
            });
            return out2;
        }

        // Диспетчер строк блоков: параллельно на крупных текстурах, иначе линейно.
        // blk[64] аллоцируется один раз на поток, не на блок.
        private static void RunBlockRows(int rows, int cols, Action<int, byte[]> body)
        {
            if (rows >= 8 && rows * cols >= 1024)
            {
                System.Threading.Tasks.Parallel.For(0, rows,
                    delegate() { return new byte[64]; },
                    delegate(int by, System.Threading.Tasks.ParallelLoopState st, byte[] blk)
                    { body(by, blk); return blk; },
                    delegate(byte[] blk) { });
            }
            else
            {
                byte[] blk = new byte[64];
                for (int by = 0; by < rows; by++) body(by, blk);
            }
        }

        private static void EncBC1(byte[] blk, byte[] out2, int off)
        {
            byte rMin=255,gMin=255,bMin=255,rMax=0,gMax=0,bMax2=0;
            for(int i=0;i<16;i++){byte r=blk[i*4],g=blk[i*4+1],b=blk[i*4+2];
                if(r<rMin)rMin=r;if(r>rMax)rMax=r;if(g<gMin)gMin=g;if(g>gMax)gMax=g;if(b<bMin)bMin=b;if(b>bMax2)bMax2=b;}
            ushort c0=ToRgb565(rMax,gMax,bMax2), c1=ToRgb565(rMin,gMin,bMin);
            if(c0<c1){ushort t=c0;c0=c1;c1=t;byte tr=rMax,tg=gMax,tb=bMax2;rMax=rMin;gMax=gMin;bMax2=bMin;rMin=tr;gMin=tg;bMin=tb;}
            int[] cols={rMax,gMax,bMax2,rMin,gMin,bMin,
                (rMax*2+rMin)/3,(gMax*2+gMin)/3,(bMax2*2+bMin)/3,
                (rMax+rMin*2)/3,(gMax+gMin*2)/3,(bMax2+bMin*2)/3};
            uint bits=0;
            for(int i=0;i<16;i++){byte r=blk[i*4],g=blk[i*4+1],b=blk[i*4+2];
                int best=0,bestD=int.MaxValue;
                for(int k=0;k<4;k++){int dr=r-cols[k*3],dg=g-cols[k*3+1],db=b-cols[k*3+2];int d=dr*dr+dg*dg+db*db;if(d<bestD){bestD=d;best=k;}}
                bits|=(uint)(best<<(i*2));}
            BitConverter.GetBytes(c0).CopyTo(out2,off);
            BitConverter.GetBytes(c1).CopyTo(out2,off+2);
            BitConverter.GetBytes(bits).CopyTo(out2,off+4);
        }

        // ── BC3 COMPRESS ──────────────────────────────────────────────────────────
        public static byte[] CompressBC3(byte[] rgba, int w, int h)
        {
            int bx=Math.Max(1,(w+3)/4), by2=Math.Max(1,(h+3)/4);
            byte[] out2=new byte[bx*by2*16];
            RunBlockRows(by2, bx, delegate(int by, byte[] blk)
            {
                int off = by * bx * 16;
                for (int bxx = 0; bxx < bx; bxx++)
                { Fill4x4(rgba, w, h, bxx, by, blk); EncAlpha(blk, out2, off); EncBC1(blk, out2, off+8); off += 16; }
            });
            return out2;
        }

        private static void EncAlpha(byte[] blk, byte[] out2, int off)
        {
            byte aMin=255,aMax=0;
            for(int i=0;i<16;i++){byte a=blk[i*4+3];if(a<aMin)aMin=a;if(a>aMax)aMax=a;}
            out2[off]=aMax; out2[off+1]=aMin;
            byte[] alphas=new byte[8]; alphas[0]=aMax; alphas[1]=aMin;
            if(aMax>aMin) for(int i=2;i<8;i++) alphas[i]=(byte)(((8-i)*aMax+(i-1)*aMin)/7);
            else { for(int i=2;i<6;i++) alphas[i]=(byte)(((6-i)*aMax+(i-1)*aMin)/5); alphas[6]=0; alphas[7]=255; }
            ulong bits=0;
            for(int i=0;i<16;i++){byte a=blk[i*4+3];int best=0,bestD=int.MaxValue;
                for(int k=0;k<8;k++){int d=Math.Abs(a-alphas[k]);if(d<bestD){bestD=d;best=k;}}
                bits|=((ulong)best<<(i*3));}
            for(int i=0;i<6;i++) out2[off+2+i]=(byte)((bits>>(i*8))&0xFF);
        }

        // ── BC5 COMPRESS ──────────────────────────────────────────────────────────
        public static byte[] CompressBC5(byte[] rgba, int w, int h)
        {
            int bx=Math.Max(1,(w+3)/4), by2=Math.Max(1,(h+3)/4);
            byte[] out2=new byte[bx*by2*16];
            RunBlockRows(by2, bx, delegate(int by, byte[] blk)
            {
                int off = by * bx * 16;
                for (int bxx = 0; bxx < bx; bxx++)
                { Fill4x4(rgba, w, h, bxx, by, blk); EncBC4Chan(blk, 0, out2, off); EncBC4Chan(blk, 1, out2, off+8); off += 16; }
            });
            return out2;
        }

        private static void EncBC4Chan(byte[] blk, int ch, byte[] out2, int off)
        {
            byte vMin=255,vMax=0;
            for(int i=0;i<16;i++){byte v=blk[i*4+ch];if(v<vMin)vMin=v;if(v>vMax)vMax=v;}
            out2[off]=vMax; out2[off+1]=vMin;
            byte[] vals=new byte[8]; vals[0]=vMax; vals[1]=vMin;
            if(vMax>vMin) for(int i=2;i<8;i++) vals[i]=(byte)(((8-i)*vMax+(i-1)*vMin)/7);
            else { for(int i=2;i<6;i++) vals[i]=(byte)(((6-i)*vMax+(i-1)*vMin)/5); vals[6]=0; vals[7]=255; }
            ulong bits=0;
            for(int i=0;i<16;i++){byte v=blk[i*4+ch];int best=0,bestD=int.MaxValue;
                for(int k=0;k<8;k++){int d=Math.Abs(v-vals[k]);if(d<bestD){bestD=d;best=k;}}
                bits|=((ulong)best<<(i*3));}
            for(int i=0;i<6;i++) out2[off+2+i]=(byte)((bits>>(i*8))&0xFF);
        }

        public static byte[] CompressRgba(byte[] rgba, int w, int h, DdsFmt fmt)
        {
            if (fmt==DdsFmt.Bc1) return CompressBC1(rgba,w,h);
            if (fmt==DdsFmt.Bc2) return CompressBC2(rgba,w,h);
            if (fmt==DdsFmt.Bc3) return CompressBC3(rgba,w,h);
            if (fmt==DdsFmt.Bc4) return CompressBC4Single(rgba,w,h);
            if (fmt==DdsFmt.Bc5) return CompressBC5(rgba,w,h);
            if (fmt==DdsFmt.Bc7) throw new NotSupportedException("BC7 encoding is unavailable; preserve the original texture."); // re-encode BC7 as BC3 (16 bytes/block)
            byte[] copy=new byte[rgba.Length]; Buffer.BlockCopy(rgba,0,copy,0,rgba.Length); return copy;
        }

        // BC4 compress: single-channel (R→red channel from RGBA), 8 bytes per block
        public static byte[] CompressBC4Single(byte[] rgba, int w, int h)
        {
            int bx=Math.Max(1,(w+3)/4), by2=Math.Max(1,(h+3)/4);
            byte[] out2=new byte[bx*by2*8];
            RunBlockRows(by2, bx, delegate(int by, byte[] blk)
            {
                int off = by * bx * 8;
                for (int bxx = 0; bxx < bx; bxx++)
                { Fill4x4(rgba, w, h, bxx, by, blk); EncBC4Chan(blk, 0, out2, off); off += 8; }
            });
            return out2;
        }

        // ── MIP CHAIN ─────────────────────────────────────────────────────────────
        public static byte[] BuildMipData(byte[] rgba, int w, int h, DdsFmt fmt, out int mipCount)
        {
            var chunks=new List<byte[]>();
            byte[] cur=rgba; int cw=w,ch=h; mipCount=0;
            while(true)
            {
                chunks.Add(CompressRgba(cur,cw,ch,fmt)); mipCount++;
                if(cw<=4||ch<=4) break;
                int nw=Math.Max(4,cw/2), nh=Math.Max(4,ch/2);
                cur=BilinearResize(cur,cw,ch,nw,nh); cw=nw; ch=nh;
            }
            int total=0; foreach(var c in chunks) total+=c.Length;
            byte[] result=new byte[total]; int off=0;
            foreach(var c in chunks){Buffer.BlockCopy(c,0,result,off,c.Length);off+=c.Length;}
            return result;
        }
    }
}
