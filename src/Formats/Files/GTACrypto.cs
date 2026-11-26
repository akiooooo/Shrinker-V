using System;
using System.Security.Cryptography;

namespace GameFormats.Files
{
	public class GTACrypto
	{
		public static byte[] DecryptAES(byte[] data)
		{
			return DecryptAESData(data, GTA5Keys.PC_AES_KEY);
		}

		public static byte[] EncryptAES(byte[] data)
		{
			return EncryptAESData(data, GTA5Keys.PC_AES_KEY);
		}

		public static byte[] DecryptAESData(byte[] data, byte[] key, int rounds = 1)
		{
			Rijndael rijndael = Rijndael.Create();
			rijndael.KeySize = 256;
			rijndael.Key = key;
			rijndael.BlockSize = 128;
			rijndael.Mode = CipherMode.ECB;
			rijndael.Padding = PaddingMode.None;
			byte[] array = (byte[])data.Clone();
			int num = data.Length - data.Length % 16;
			if (num > 0)
			{
				ICryptoTransform cryptoTransform = rijndael.CreateDecryptor();
				for (int i = 0; i < rounds; i++)
				{
					cryptoTransform.TransformBlock(array, 0, num, array, 0);
				}
			}
			return array;
		}

		public static byte[] EncryptAESData(byte[] data, byte[] key, int rounds = 1)
		{
			Rijndael rijndael = Rijndael.Create();
			rijndael.KeySize = 256;
			rijndael.Key = key;
			rijndael.BlockSize = 128;
			rijndael.Mode = CipherMode.ECB;
			rijndael.Padding = PaddingMode.None;
			byte[] array = (byte[])data.Clone();
			int num = data.Length - data.Length % 16;
			if (num > 0)
			{
				ICryptoTransform cryptoTransform = rijndael.CreateEncryptor();
				for (int i = 0; i < rounds; i++)
				{
					cryptoTransform.TransformBlock(array, 0, num, array, 0);
				}
			}
			return array;
		}

		public static byte[] GetNGKey(string name, uint length)
		{
			uint num = (GTA5Hash.CalculateHash(name) + length + 61) % 101;
			return GTA5Keys.PC_NG_KEYS[num];
		}

		public static byte[] DecryptNG(byte[] data, string name, uint length)
		{
			byte[] nGKey = GetNGKey(name, length);
			return DecryptNG(data, nGKey);
		}

		public static byte[] DecryptNG(byte[] data, byte[] key)
		{
			byte[] array = new byte[data.Length];
			uint[] array2 = new uint[key.Length / 4];
			Buffer.BlockCopy(key, 0, array2, 0, key.Length);
			for (int i = 0; i < data.Length / 16; i++)
			{
				byte[] array3 = new byte[16];
				Array.Copy(data, 16 * i, array3, 0, 16);
				Array.Copy(DecryptNGBlock(array3, array2), 0, array, 16 * i, 16);
			}
			if (data.Length % 16 != 0)
			{
				int num = data.Length % 16;
				Buffer.BlockCopy(data, data.Length - num, array, data.Length - num, num);
			}
			return array;
		}

		public static byte[] DecryptNGBlock(byte[] data, uint[] key)
		{
			byte[] data2 = data;
			uint[][] array = new uint[17][];
			for (int i = 0; i < 17; i++)
			{
				array[i] = new uint[4];
				array[i][0] = key[4 * i];
				array[i][1] = key[4 * i + 1];
				array[i][2] = key[4 * i + 2];
				array[i][3] = key[4 * i + 3];
			}
			data2 = DecryptNGRoundA(data2, array[0], GTA5Keys.PC_NG_DECRYPT_TABLES[0]);
			data2 = DecryptNGRoundA(data2, array[1], GTA5Keys.PC_NG_DECRYPT_TABLES[1]);
			for (int j = 2; j <= 15; j++)
			{
				data2 = DecryptNGRoundB(data2, array[j], GTA5Keys.PC_NG_DECRYPT_TABLES[j]);
			}
			return DecryptNGRoundA(data2, array[16], GTA5Keys.PC_NG_DECRYPT_TABLES[16]);
		}

		public static byte[] DecryptNGRoundA(byte[] data, uint[] key, uint[][] table)
		{
			uint value = table[0][data[0]] ^ table[1][data[1]] ^ table[2][data[2]] ^ table[3][data[3]] ^ key[0];
			uint value2 = table[4][data[4]] ^ table[5][data[5]] ^ table[6][data[6]] ^ table[7][data[7]] ^ key[1];
			uint value3 = table[8][data[8]] ^ table[9][data[9]] ^ table[10][data[10]] ^ table[11][data[11]] ^ key[2];
			uint value4 = table[12][data[12]] ^ table[13][data[13]] ^ table[14][data[14]] ^ table[15][data[15]] ^ key[3];
			byte[] array = new byte[16];
			Array.Copy(BitConverter.GetBytes(value), 0, array, 0, 4);
			Array.Copy(BitConverter.GetBytes(value2), 0, array, 4, 4);
			Array.Copy(BitConverter.GetBytes(value3), 0, array, 8, 4);
			Array.Copy(BitConverter.GetBytes(value4), 0, array, 12, 4);
			return array;
		}

		public static byte[] DecryptNGRoundB(byte[] data, uint[] key, uint[][] table)
		{
			uint num = table[0][data[0]] ^ table[7][data[7]] ^ table[10][data[10]] ^ table[13][data[13]] ^ key[0];
			uint num2 = table[1][data[1]] ^ table[4][data[4]] ^ table[11][data[11]] ^ table[14][data[14]] ^ key[1];
			uint num3 = table[2][data[2]] ^ table[5][data[5]] ^ table[8][data[8]] ^ table[15][data[15]] ^ key[2];
			uint num4 = table[3][data[3]] ^ table[6][data[6]] ^ table[9][data[9]] ^ table[12][data[12]] ^ key[3];
			return new byte[16]
			{
				(byte)(num & 0xFF),
				(byte)((num >> 8) & 0xFF),
				(byte)((num >> 16) & 0xFF),
				(byte)((num >> 24) & 0xFF),
				(byte)(num2 & 0xFF),
				(byte)((num2 >> 8) & 0xFF),
				(byte)((num2 >> 16) & 0xFF),
				(byte)((num2 >> 24) & 0xFF),
				(byte)(num3 & 0xFF),
				(byte)((num3 >> 8) & 0xFF),
				(byte)((num3 >> 16) & 0xFF),
				(byte)((num3 >> 24) & 0xFF),
				(byte)(num4 & 0xFF),
				(byte)((num4 >> 8) & 0xFF),
				(byte)((num4 >> 16) & 0xFF),
				(byte)((num4 >> 24) & 0xFF)
			};
		}

		public static byte[] EncryptNG(byte[] data, string name, uint length)
		{
			byte[] nGKey = GetNGKey(name, length);
			return EncryptNG(data, nGKey);
		}

		public static byte[] EncryptNG(byte[] data, byte[] key)
		{
			if (GTA5Keys.PC_NG_ENCRYPT_TABLES == null || GTA5Keys.PC_NG_ENCRYPT_LUTs == null)
			{
				throw new Exception("Unable to encrypt - tables not loaded.");
			}
			byte[] array = new byte[data.Length];
			uint[] array2 = new uint[key.Length / 4];
			Buffer.BlockCopy(key, 0, array2, 0, key.Length);
			for (int i = 0; i < data.Length / 16; i++)
			{
				byte[] array3 = new byte[16];
				Array.Copy(data, 16 * i, array3, 0, 16);
				Array.Copy(EncryptBlock(array3, array2), 0, array, 16 * i, 16);
			}
			if (data.Length % 16 != 0)
			{
				int num = data.Length % 16;
				Buffer.BlockCopy(data, data.Length - num, array, data.Length - num, num);
			}
			return array;
		}

		public static byte[] EncryptBlock(byte[] data, uint[] key)
		{
			byte[] data2 = data;
			uint[][] array = new uint[17][];
			for (int i = 0; i < 17; i++)
			{
				array[i] = new uint[4];
				array[i][0] = key[4 * i];
				array[i][1] = key[4 * i + 1];
				array[i][2] = key[4 * i + 2];
				array[i][3] = key[4 * i + 3];
			}
			data2 = EncryptRoundA(data2, array[16], GTA5Keys.PC_NG_ENCRYPT_TABLES[16]);
			for (int num = 15; num >= 2; num--)
			{
				data2 = EncryptRoundB_LUT(data2, array[num], GTA5Keys.PC_NG_ENCRYPT_LUTs[num]);
			}
			data2 = EncryptRoundA(data2, array[1], GTA5Keys.PC_NG_ENCRYPT_TABLES[1]);
			return EncryptRoundA(data2, array[0], GTA5Keys.PC_NG_ENCRYPT_TABLES[0]);
		}

		public static byte[] EncryptRoundA(byte[] data, uint[] key, uint[][] table)
		{
			byte[] array = new byte[16];
			Buffer.BlockCopy(key, 0, array, 0, 16);
			uint value = table[0][data[0] ^ array[0]] ^ table[1][data[1] ^ array[1]] ^ table[2][data[2] ^ array[2]] ^ table[3][data[3] ^ array[3]];
			uint value2 = table[4][data[4] ^ array[4]] ^ table[5][data[5] ^ array[5]] ^ table[6][data[6] ^ array[6]] ^ table[7][data[7] ^ array[7]];
			uint value3 = table[8][data[8] ^ array[8]] ^ table[9][data[9] ^ array[9]] ^ table[10][data[10] ^ array[10]] ^ table[11][data[11] ^ array[11]];
			uint value4 = table[12][data[12] ^ array[12]] ^ table[13][data[13] ^ array[13]] ^ table[14][data[14] ^ array[14]] ^ table[15][data[15] ^ array[15]];
			byte[] array2 = new byte[16];
			Array.Copy(BitConverter.GetBytes(value), 0, array2, 0, 4);
			Array.Copy(BitConverter.GetBytes(value2), 0, array2, 4, 4);
			Array.Copy(BitConverter.GetBytes(value3), 0, array2, 8, 4);
			Array.Copy(BitConverter.GetBytes(value4), 0, array2, 12, 4);
			return array2;
		}

		public static byte[] EncryptRoundB_LUT(byte[] dataOld, uint[] key, GTA5NGLUT[] lut)
		{
			byte[] array = (byte[])dataOld.Clone();
			byte[] array2 = new byte[16];
			Buffer.BlockCopy(key, 0, array2, 0, 16);
			for (int i = 0; i < 16; i++)
			{
				array[i] ^= array2[i];
			}
			return new byte[16]
			{
				lut[0].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[0],
					array[1],
					array[2],
					array[3]
				}, 0)),
				lut[1].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[4],
					array[5],
					array[6],
					array[7]
				}, 0)),
				lut[2].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[8],
					array[9],
					array[10],
					array[11]
				}, 0)),
				lut[3].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[12],
					array[13],
					array[14],
					array[15]
				}, 0)),
				lut[4].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[4],
					array[5],
					array[6],
					array[7]
				}, 0)),
				lut[5].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[8],
					array[9],
					array[10],
					array[11]
				}, 0)),
				lut[6].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[12],
					array[13],
					array[14],
					array[15]
				}, 0)),
				lut[7].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[0],
					array[1],
					array[2],
					array[3]
				}, 0)),
				lut[8].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[8],
					array[9],
					array[10],
					array[11]
				}, 0)),
				lut[9].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[12],
					array[13],
					array[14],
					array[15]
				}, 0)),
				lut[10].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[0],
					array[1],
					array[2],
					array[3]
				}, 0)),
				lut[11].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[4],
					array[5],
					array[6],
					array[7]
				}, 0)),
				lut[12].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[12],
					array[13],
					array[14],
					array[15]
				}, 0)),
				lut[13].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[0],
					array[1],
					array[2],
					array[3]
				}, 0)),
				lut[14].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[4],
					array[5],
					array[6],
					array[7]
				}, 0)),
				lut[15].LookUp(BitConverter.ToUInt32(new byte[4]
				{
					array[8],
					array[9],
					array[10],
					array[11]
				}, 0))
			};
		}
	}
}
