using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace GameFormats.Files
{
	public static class HashSearch
	{
		public static byte[] SearchHash(Stream stream, byte[] hash, int length = 32)
		{
			return SearchHashes(stream, new List<byte[]> { hash }, length)[0];
		}

		public static byte[][] SearchHashes(Stream stream, IList<byte[]> hashes, int length = 32)
		{
			byte[][] result = new byte[hashes.Count][];
			Parallel.For(0L, stream.Length / 1048576, delegate(long k)
			{
				SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider();
				byte[] array = new byte[length];
				for (long num = 0L; num < 131072; num++)
				{
					long num2 = k * 1048576 + num * 8;
					if (num2 < stream.Length)
					{
						lock (stream)
						{
							stream.Position = num2;
							stream.Read(array, 0, length);
						}
						byte[] first = sHA1CryptoServiceProvider.ComputeHash(array);
						for (int i = 0; i < hashes.Count; i++)
						{
							if (first.SequenceEqual(hashes[i]))
							{
								result[i] = (byte[])array.Clone();
							}
						}
					}
				}
			});
			return result;
		}
	}
}
