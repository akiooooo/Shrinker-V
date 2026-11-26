using System;

namespace GameFormats.Files
{
    public sealed class GTA5NGLUT
    {
        internal const int Size = 3 * 65536;
        private readonly byte[] bytes;
        private readonly int start;

        internal GTA5NGLUT(byte[] data, int offset)
        {
            bytes = data;
            start = offset;
        }

        public byte LookUp(uint value)
        {
            int index = bytes[start + 131072 + (int)(value >> 16)];
            int row = bytes[start + 65536 + (index << 8) + (int)((value >> 8) & 255)];
            return bytes[start + (row << 8) + (int)(value & 255)];
        }
    }
}
