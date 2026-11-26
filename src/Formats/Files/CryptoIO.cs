using System.IO;

namespace GameFormats.Files
{
	public class CryptoIO
	{
		public static byte[][] ReadNgKeys(byte[] data)
		{
			DataReader dataReader = new DataReader(new MemoryStream(data));
			byte[][] array = new byte[101][];
			for (int i = 0; i < 101; i++)
			{
				array[i] = dataReader.ReadBytes(272);
			}
			return array;
		}

        public static uint[][][] ReadNgTables(byte[] data)
        {
            if (data.Length != 17 * 16 * 1024) throw new InvalidDataException("Invalid NG table size.");
            var tables = new uint[17][][];
            for (int i = 0; i < tables.Length; i++)
            {
                tables[i] = new uint[16][];
                for (int j = 0; j < 16; j++)
                {
                    tables[i][j] = new uint[256];
                    System.Buffer.BlockCopy(data, (i * 16 + j) * 1024, tables[i][j], 0, 1024);
                }
            }
            return tables;
        }

        public static GTA5NGLUT[][] ReadNgLuts(byte[] data)
        {
            if (data.Length != 17 * 16 * GTA5NGLUT.Size) throw new InvalidDataException("Invalid NG lookup size.");
            var tables = new GTA5NGLUT[17][];
            for (int i = 0; i < tables.Length; i++)
            {
                tables[i] = new GTA5NGLUT[16];
                for (int j = 0; j < 16; j++)
                    tables[i][j] = new GTA5NGLUT(data, (i * 16 + j) * GTA5NGLUT.Size);
            }
            return tables;
        }
    }
}