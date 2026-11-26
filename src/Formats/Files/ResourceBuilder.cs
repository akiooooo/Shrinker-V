using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace GameFormats.Files
{
	public class ResourceBuilder
	{
		public static void GetBlocks(IResourceBlock rootBlock, out IList<IResourceBlock> sys, out IList<IResourceBlock> gfx)
		{
			HashSet<IResourceBlock> systemBlocks = new HashSet<IResourceBlock>();
			HashSet<IResourceBlock> graphicBlocks = new HashSet<IResourceBlock>();
			HashSet<IResourceBlock> processed = new HashSet<IResourceBlock>();
			addBlock(rootBlock);
			addChildren(rootBlock);
			sys = new List<IResourceBlock>();
			foreach (IResourceBlock item in systemBlocks)
			{
				sys.Add(item);
			}
			gfx = new List<IResourceBlock>();
			foreach (IResourceBlock item2 in graphicBlocks)
			{
				gfx.Add(item2);
			}
			void addBlock(IResourceBlock block)
			{
				if (block is IResourceSystemBlock)
				{
					if (!systemBlocks.Contains(block))
					{
						systemBlocks.Add(block);
					}
				}
				else if (block is IResourceGraphicsBlock && !graphicBlocks.Contains(block))
				{
					graphicBlocks.Add(block);
				}
			}
			void addChildren(IResourceBlock block)
			{
				if (block is IResourceSystemBlock resourceSystemBlock)
				{
					IResourceBlock[] references = resourceSystemBlock.GetReferences();
					foreach (IResourceBlock resourceBlock in references)
					{
						if (!processed.Contains(resourceBlock))
						{
							processed.Add(resourceBlock);
							addBlock(resourceBlock);
							addChildren(resourceBlock);
						}
					}
					Tuple<long, IResourceBlock>[] parts = resourceSystemBlock.GetParts();
					for (int i = 0; i < parts.Length; i++)
					{
						addChildren(parts[i].Item2);
					}
				}
			}
		}

		public static void AssignPositionsForMeta(IList<IResourceBlock> blocks, uint basePosition, out RpfResourcePageFlags pageFlags)
		{
			long num = 0L;
			foreach (IResourceBlock block in blocks)
			{
				if (num < block.BlockLength)
				{
					num = block.BlockLength;
				}
			}
			long num2;
			for (num2 = 8192L; num2 < num; num2 *= 2)
			{
			}
			long num3;
			while (true)
			{
				num3 = 0L;
				long num4 = 0L;
				foreach (IResourceBlock block2 in blocks)
				{
					block2.FilePosition = -1L;
				}
				foreach (IResourceBlock block3 in blocks)
				{
					if (block3.FilePosition != -1)
					{
						throw new Exception("Block was already assigned a position!");
					}
					if (num3 * num2 - num4 < block3.BlockLength + 16)
					{
						num3++;
						num4 = num2 * (num3 - 1);
					}
					block3.FilePosition = basePosition + num4;
					num4 += block3.BlockLength;
					if (num4 % 16 != 0L)
					{
						num4 += 16 - num4 % 16;
					}
				}
				if (num3 < 128)
				{
					break;
				}
				num2 *= 2;
			}
			pageFlags = new RpfResourcePageFlags(RpfResourceFileEntry.GetFlagsFromBlocks((uint)num3, (uint)num2, 0u));
		}

		public static void AssignPositions2(IList<IResourceBlock> blocks, uint basePosition, out RpfResourcePageFlags pageFlags, uint maxPageCount, bool gen9)
		{
			if (blocks.Count > 0 && blocks[0] is Meta)
			{
				AssignPositionsForMeta(blocks, basePosition, out pageFlags);
				return;
			}
			bool flag = basePosition == 1342177280;
			long num = 16L;
			long num2 = 268435456 * num;
			long num3 = 0L;
			long num4 = ((blocks.Count == 0) ? 0 : num2);
			if (gen9)
			{
				foreach (IResourceBlock block in blocks)
				{
					if (block.BlockLength_Gen9 > num3)
					{
						num3 = block.BlockLength_Gen9;
					}
					if (block.BlockLength_Gen9 < num4)
					{
						num4 = block.BlockLength_Gen9;
					}
				}
			}
			else
			{
				foreach (IResourceBlock block2 in blocks)
				{
					if (block2.BlockLength > num3)
					{
						num3 = block2.BlockLength;
					}
					if (block2.BlockLength < num4)
					{
						num4 = block2.BlockLength;
					}
				}
			}
			int num5 = 0;
			long num6 = 8192L;
			while ((num6 < num4 || num6 * num < num3) && num5 < 15)
			{
				num5++;
				num6 = 8192L << num5;
			}
			if (num6 * num < num3)
			{
				throw new Exception("Unable to fit largest block!");
			}
			List<IResourceBlock> list = new List<IResourceBlock>();
			IResourceBlock resourceBlock = ((flag && blocks.Count > 0) ? blocks[0] : null);
			foreach (IResourceBlock block3 in blocks)
			{
				if (block3 != null && block3 != resourceBlock)
				{
					list.Add(block3);
				}
			}
			if (gen9)
			{
				list.Sort((IResourceBlock a, IResourceBlock b) => b.BlockLength_Gen9.CompareTo(a.BlockLength_Gen9));
			}
			else
			{
				list.Sort((IResourceBlock a, IResourceBlock b) => b.BlockLength.CompareTo(a.BlockLength));
			}
			if (resourceBlock != null)
			{
				list.Insert(0, resourceBlock);
			}
			uint[] array = new uint[5];
			List<long>[] array2 = new List<long>[5];
			Dictionary<IResourceBlock, (int, int, long)> dictionary = new Dictionary<IResourceBlock, (int, int, long)>();
			while (true)
			{
				for (int num7 = 0; num7 < 5; num7++)
				{
					array[num7] = 0u;
					array2[num7] = null;
				}
				int num8 = 0;
				for (long num9 = num6; num9 < num3; num9 *= 2)
				{
					num8++;
				}
				for (int num10 = 0; num10 < list.Count; num10++)
				{
					IResourceBlock resourceBlock2 = list[num10];
					long num11 = (gen9 ? resourceBlock2.BlockLength_Gen9 : resourceBlock2.BlockLength);
					if (num10 == 0)
					{
						array2[num8] = new List<long> { num11 };
						dictionary[resourceBlock2] = (num8, 0, 0L);
						continue;
					}
					int num12 = 0;
					long num13 = num6;
					while (num11 > num13 && num12 < num8)
					{
						num12++;
						num13 *= 2;
					}
					bool flag2 = false;
					int num14 = num12;
					long num15 = num13;
					while (!flag2 && num14 <= num8)
					{
						List<long> list2 = array2[num14];
						if (list2 != null)
						{
							for (int num16 = 0; num16 < list2.Count; num16++)
							{
								long num17 = list2[num16];
								num17 += (16 - num17 % 16) % 16;
								long item = num17;
								num17 += num11;
								if (num17 <= num15)
								{
									list2[num16] = num17;
									flag2 = true;
									dictionary[resourceBlock2] = (num14, num16, item);
									break;
								}
							}
						}
						num14++;
						num15 *= 2;
					}
					if (!flag2)
					{
						List<long> list3 = array2[num12];
						if (list3 == null)
						{
							list3 = (array2[num12] = new List<long>());
						}
						int count = list3.Count;
						list3.Add(num11);
						dictionary[resourceBlock2] = (num12, count, 0L);
					}
				}
				bool flag3 = true;
				uint num18 = 0u;
				for (int num19 = 0; num19 < 5; num19++)
				{
					num18 += (array[num19] = (uint)(array2[num19]?.Count ?? 0));
				}
				if (num18 > maxPageCount)
				{
					flag3 = false;
				}
				if (array[0] > 127)
				{
					flag3 = false;
				}
				if (array[1] > 63)
				{
					flag3 = false;
				}
				if (array[2] > 15)
				{
					flag3 = false;
				}
				if (array[3] > 3)
				{
					flag3 = false;
				}
				if (array[4] > 1)
				{
					flag3 = false;
				}
				if (flag3)
				{
					break;
				}
				if (num5 >= 15)
				{
					throw new Exception("Unable to pack blocks with largest possible base!");
				}
				num5++;
				num6 = 8192 << num5;
			}
			long num20 = 0L;
			long[] array3 = new long[5];
			for (int num21 = 4; num21 >= 0; num21--)
			{
				array3[num21] = num20;
				long num22 = num6 * (1 << num21);
				uint num23 = array[num21];
				num20 += num22 * num23;
			}
			foreach (KeyValuePair<IResourceBlock, (int, int, long)> item5 in dictionary)
			{
				IResourceBlock key = item5.Key;
				int item2 = item5.Value.Item1;
				int item3 = item5.Value.Item2;
				long item4 = item5.Value.Item3;
				long num24 = num6 * (1 << item2);
				long num25 = array3[item2] + num24 * item3 + item4;
				key.FilePosition = basePosition + num25;
			}
			uint num26 = (uint)(num5 & 0xF);
			num26 += (array[4] & 1) << 4;
			num26 += (array[3] & 3) << 5;
			num26 += (array[2] & 0xF) << 7;
			num26 += (array[1] & 0x3F) << 11;
			num26 += (array[0] & 0x7F) << 17;
			pageFlags = new RpfResourcePageFlags(num26);
		}

		public static byte[] Build(ResourceFileBase fileBase, int version, bool compress = true, bool gen9 = false)
		{
			fileBase.FilePagesInfo = new ResourcePagesInfo();
			GetBlocks(fileBase, out var sys, out var gfx);
			AssignPositions2(sys, 1342177280u, out var pageFlags, 128u, gen9);
			AssignPositions2(gfx, 1610612736u, out var pageFlags2, 128 - pageFlags.Count, gen9);
			fileBase.FilePagesInfo.SystemPagesCount = (byte)pageFlags.Count;
			fileBase.FilePagesInfo.GraphicsPagesCount = (byte)pageFlags2.Count;
			MemoryStream memoryStream = new MemoryStream();
			MemoryStream memoryStream2 = new MemoryStream();
			ResourceDataWriter resourceDataWriter = new ResourceDataWriter(memoryStream, memoryStream2);
			resourceDataWriter.IsGen9 = gen9;
			resourceDataWriter.Position = 1342177280L;
			foreach (IResourceBlock item in sys)
			{
				resourceDataWriter.Position = item.FilePosition;
				long position = resourceDataWriter.Position;
				item.Write(resourceDataWriter);
				long position2 = resourceDataWriter.Position;
				long num = (resourceDataWriter.IsGen9 ? item.BlockLength_Gen9 : item.BlockLength);
				if (position2 - position != num)
				{
					throw new Exception("error in system length");
				}
			}
			resourceDataWriter.Position = 1610612736L;
			foreach (IResourceBlock item2 in gfx)
			{
				resourceDataWriter.Position = item2.FilePosition;
				long position3 = resourceDataWriter.Position;
				item2.Write(resourceDataWriter);
				long position4 = resourceDataWriter.Position;
				long num2 = (resourceDataWriter.IsGen9 ? item2.BlockLength_Gen9 : item2.BlockLength);
				if (position4 - position3 != num2)
				{
					throw new Exception("error in graphics length");
				}
			}
			int size = (int)pageFlags.Size;
			byte[] array = new byte[size];
			memoryStream.Flush();
			memoryStream.Position = 0L;
			memoryStream.Read(array, 0, (int)memoryStream.Length);
			int size2 = (int)pageFlags2.Size;
			byte[] array2 = new byte[size2];
			memoryStream2.Flush();
			memoryStream2.Position = 0L;
			memoryStream2.Read(array2, 0, (int)memoryStream2.Length);
			uint num3 = ((uint)version >> 4) & 0xF;
			uint num4 = (uint)(version & 0xF);
			uint value = pageFlags.Value + (num3 << 28);
			uint value2 = pageFlags2.Value + (num4 << 28);
			byte[] array3 = new byte[size + size2];
			Buffer.BlockCopy(array, 0, array3, 0, size);
			Buffer.BlockCopy(array2, 0, array3, size, size2);
			byte[] array4 = (compress ? Compress(array3) : array3);
			byte[] array5 = new byte[16 + array4.Length];
			byte[] bytes = BitConverter.GetBytes(927159122u);
			byte[] bytes2 = BitConverter.GetBytes(version);
			byte[] bytes3 = BitConverter.GetBytes(value);
			byte[] bytes4 = BitConverter.GetBytes(value2);
			Buffer.BlockCopy(bytes, 0, array5, 0, 4);
			Buffer.BlockCopy(bytes2, 0, array5, 4, 4);
			Buffer.BlockCopy(bytes3, 0, array5, 8, 4);
			Buffer.BlockCopy(bytes4, 0, array5, 12, 4);
			Buffer.BlockCopy(array4, 0, array5, 16, array4.Length);
			return array5;
		}

		public static byte[] Compress(byte[] data)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, leaveOpen: true);
				deflateStream.Write(data, 0, data.Length);
				deflateStream.Close();
				byte[] buffer = memoryStream.GetBuffer();
				byte[] array = new byte[memoryStream.Length];
				Array.Copy(buffer, array, array.Length);
				return array;
			}
		}
	}
}
