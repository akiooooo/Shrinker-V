using System;
using System.IO;
using System.IO.Compression;
using GameFormats.Properties;

namespace GameFormats.Files
{
	public static class GTA5Keys
	{
		public static string ToS = "(c)2017";

		public static byte[] PC_AES_KEY;

		public static byte[][] PC_NG_KEYS;

		public static uint[][][] PC_NG_DECRYPT_TABLES;

		public static uint[][][] PC_NG_ENCRYPT_TABLES;

		public static GTA5NGLUT[][] PC_NG_ENCRYPT_LUTs;

		public static byte[] PC_LUT;

		public static uint[] PC_AWC_KEY;

		public static void GenerateV2(byte[] exeData, Action<string> updateStatus)
		{
			MemoryStream stream = new MemoryStream(exeData);
			updateStatus?.Invoke("Searching for AES key...");
			PC_AES_KEY = HashSearch.SearchHash(stream, GTA5KeyHashes.PC_AES_KEY_HASH);
			updateStatus?.Invoke("Complete.");
		}

		public static void LoadFromPath(string path = ".\\Keys", bool gen9 = false, string key = null)
		{
			UseMagicData(path, gen9, key);
		}

		private static void UseMagicData(string path, bool gen9, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				string text = (gen9 ? "\\gta5_enhanced.exe" : "\\gta5.exe");
				GenerateV2(File.ReadAllBytes(path + text), null);
			}
			else
			{
				PC_AES_KEY = Convert.FromBase64String(key);
			}
			Random random = new Random((int)JenkHash.GenHash(PC_AES_KEY));
			byte[] magic = Resources.magic;
			int num = magic.Length;
			byte[] array = new byte[num];
			byte[] array2 = new byte[num];
			byte[] array3 = new byte[num];
			byte[] array4 = new byte[num];
			random.NextBytes(array);
			random.NextBytes(array2);
			random.NextBytes(array3);
			random.NextBytes(array4);
			byte[] array5 = new byte[num];
			for (int i = 0; i < num; i++)
			{
				array5[i] = (byte)((magic[i] - array[i] - array2[i] - array3[i] - array4[i]) & 0xFF);
			}
			array5 = GTACrypto.DecryptAESData(array5, PC_AES_KEY);
			byte[] array6 = null;
			using (MemoryStream stream = new MemoryStream(array5))
			{
				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						deflateStream.CopyTo(memoryStream);
						array6 = memoryStream.GetBuffer();
					}
				}
			}
			if (array6 == null)
			{
				throw new Exception("Error inflating magic data.");
			}
			byte[] array7 = new byte[27472];
			byte[] array8 = new byte[278528];
			byte[] array9 = new byte[256];
			uint[] array10 = new uint[4];
			int num2 = 0;
			Buffer.BlockCopy(array6, num2, array7, 0, array7.Length);
			num2 += array7.Length;
			Buffer.BlockCopy(array6, num2, array8, 0, array8.Length);
			num2 += array8.Length;
			Buffer.BlockCopy(array6, num2, array9, 0, array9.Length);
			num2 += array9.Length;
			Buffer.BlockCopy(array6, num2, array10, 0, array10.Length * 4);
			num2 += array10.Length;
			PC_NG_KEYS = CryptoIO.ReadNgKeys(array7);
			PC_NG_DECRYPT_TABLES = CryptoIO.ReadNgTables(array8);
			PC_LUT = array9;
			PC_AWC_KEY = array10;
		}
	}
}
