using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	public class RpfFile
	{
		[CompilerGenerated]
		private string _003CNameLower_003Ek__BackingField;

		[CompilerGenerated]
		private bool _003CIsNGEncrypted_003Ek__BackingField;

		public string Name { get; set; }

		public string NameLower
		{
			[CompilerGenerated]
			set
			{
				_003CNameLower_003Ek__BackingField = value;
			}
		}

		public string Path { get; set; }

		public string FilePath { get; set; }

		public long FileSize { get; set; }

		public string LastError { get; set; }

		public Exception LastException { get; set; }

		public RpfDirectoryEntry Root { get; set; }

		public bool IsAESEncrypted { get; set; }

		public bool IsNGEncrypted
		{
			[CompilerGenerated]
			set
			{
				_003CIsNGEncrypted_003Ek__BackingField = value;
			}
		}

		public long StartPos { get; set; }

		public uint Version { get; set; }

		public uint EntryCount { get; set; }

		public uint NamesLength { get; set; }

		public RpfEncryption Encryption { get; set; }

		public List<RpfEntry> AllEntries { get; set; }

		public List<RpfFile> Children { get; set; }

		public RpfFile Parent { get; set; }

		public RpfBinaryFileEntry ParentFileEntry { get; set; }

		public BinaryReader CurrentFileReader { get; set; }

		public uint TotalFileCount { get; set; }

		public uint TotalFolderCount { get; set; }

		public uint TotalResourceCount { get; set; }

		public uint TotalBinaryFileCount { get; set; }

		public uint GrandTotalRpfCount { get; set; }

		public uint GrandTotalFileCount { get; set; }

		public uint GrandTotalFolderCount { get; set; }

		public uint GrandTotalResourceCount { get; set; }

		public uint GrandTotalBinaryFileCount { get; set; }

		public RpfFile(string fpath, string relpath)
		{
			FileInfo fileInfo = new FileInfo(fpath);
			Name = fileInfo.Name;
			NameLower = Name.ToLowerInvariant();
			Path = relpath.ToLowerInvariant();
			FilePath = fpath;
			FileSize = fileInfo.Length;
		}

		public RpfFile(string name, string path, long filesize)
		{
			Name = name;
			NameLower = Name.ToLowerInvariant();
			Path = path.ToLowerInvariant();
			FilePath = path;
			FileSize = filesize;
		}

		public RpfFile GetTopParent()
		{
			RpfFile rpfFile = this;
			while (rpfFile.Parent != null)
			{
				rpfFile = rpfFile.Parent;
			}
			return rpfFile;
		}

		public string GetPhysicalFilePath()
		{
			return GetTopParent().FilePath;
		}

		private void ReadHeader(BinaryReader br)
		{
			CurrentFileReader = br;
			StartPos = br.BaseStream.Position;
			Version = br.ReadUInt32();
			EntryCount = br.ReadUInt32();
			NamesLength = br.ReadUInt32();
			Encryption = (RpfEncryption)br.ReadUInt32();
			if (Version != 1380992567)
			{
				throw new Exception("Invalid Resource - not GTAV!");
			}
			byte[] array = br.ReadBytes((int)(EntryCount * 16));
			byte[] array2 = br.ReadBytes((int)NamesLength);
			switch (Encryption)
			{
			case RpfEncryption.AES:
				array = GTACrypto.DecryptAES(array);
				array2 = GTACrypto.DecryptAES(array2);
				IsAESEncrypted = true;
				break;
			case RpfEncryption.NG:
				array = GTACrypto.DecryptNG(array, Name, (uint)FileSize);
				array2 = GTACrypto.DecryptNG(array2, Name, (uint)FileSize);
				IsNGEncrypted = true;
				break;
			default:
				array = GTACrypto.DecryptNG(array, Name, (uint)FileSize);
				array2 = GTACrypto.DecryptNG(array2, Name, (uint)FileSize);
				IsNGEncrypted = true;
				break;
			case RpfEncryption.NONE:
			case RpfEncryption.OPEN:
				break;
			}
			DataReader dataReader = new DataReader(new MemoryStream(array));
			DataReader dataReader2 = new DataReader(new MemoryStream(array2));
			AllEntries = new List<RpfEntry>();
			TotalFileCount = 0u;
			TotalFolderCount = 0u;
			TotalResourceCount = 0u;
			TotalBinaryFileCount = 0u;
			for (uint num = 0u; num < EntryCount; num++)
			{
				uint h = dataReader.ReadUInt32();
				uint num2 = dataReader.ReadUInt32();
				dataReader.Position -= 8L;
				RpfEntry rpfEntry;
				if (num2 == 2147483392)
				{
					rpfEntry = new RpfDirectoryEntry();
					TotalFolderCount++;
				}
				else if ((num2 & 0x80000000u) == 0)
				{
					rpfEntry = new RpfBinaryFileEntry();
					TotalBinaryFileCount++;
					TotalFileCount++;
				}
				else
				{
					rpfEntry = new RpfResourceFileEntry();
					TotalResourceCount++;
					TotalFileCount++;
				}
				rpfEntry.File = this;
				rpfEntry.H1 = h;
				rpfEntry.H2 = num2;
				rpfEntry.Read(dataReader);
				dataReader2.Position = rpfEntry.NameOffset;
				rpfEntry.Name = dataReader2.ReadString();
				if (rpfEntry.Name.Length > 256)
				{
					rpfEntry.Name = rpfEntry.Name.Substring(0, 256);
				}
				rpfEntry.NameLower = rpfEntry.Name.ToLowerInvariant();
				if (rpfEntry is RpfFileEntry)
				{
					string.IsNullOrEmpty(rpfEntry.Name);
				}
				if (rpfEntry is RpfResourceFileEntry)
				{
					RpfResourceFileEntry obj = rpfEntry as RpfResourceFileEntry;
					obj.IsEncrypted = obj.NameLower.EndsWith(".ysc");
				}
				AllEntries.Add(rpfEntry);
			}
			Root = (RpfDirectoryEntry)AllEntries[0];
			Root.Path = Path.ToLowerInvariant();
			Stack<RpfDirectoryEntry> stack = new Stack<RpfDirectoryEntry>();
			stack.Push(Root);
			while (stack.Count > 0)
			{
				RpfDirectoryEntry rpfDirectoryEntry = stack.Pop();
				uint entriesIndex = rpfDirectoryEntry.EntriesIndex;
				int num3 = (int)(rpfDirectoryEntry.EntriesIndex + rpfDirectoryEntry.EntriesCount);
				for (int i = (int)entriesIndex; i < num3; i++)
				{
					RpfEntry rpfEntry2 = AllEntries[i];
					rpfEntry2.Parent = rpfDirectoryEntry;
					if (rpfEntry2 is RpfDirectoryEntry)
					{
						RpfDirectoryEntry rpfDirectoryEntry2 = rpfEntry2 as RpfDirectoryEntry;
						rpfDirectoryEntry2.Path = rpfDirectoryEntry.Path + "\\" + rpfDirectoryEntry2.NameLower;
						rpfDirectoryEntry.Directories.Add(rpfDirectoryEntry2);
						stack.Push(rpfDirectoryEntry2);
					}
					else if (rpfEntry2 is RpfFileEntry)
					{
						RpfFileEntry rpfFileEntry = rpfEntry2 as RpfFileEntry;
						rpfFileEntry.Path = rpfDirectoryEntry.Path + "\\" + rpfFileEntry.NameLower;
						rpfDirectoryEntry.Files.Add(rpfFileEntry);
					}
				}
			}
			br.BaseStream.Position = StartPos;
			CurrentFileReader = null;
		}

		public void ScanStructure(Action<string> updateStatus, Action<string> errorLog)
		{
			using (BinaryReader br = new BinaryReader(File.OpenRead(FilePath)))
			{
				try
				{
					ScanStructure(br, updateStatus, errorLog);
				}
				catch (Exception ex)
				{
					LastError = ex.ToString();
					LastException = ex;
					errorLog?.Invoke(FilePath + ": " + LastError);
				}
			}
		}

		private void ScanStructure(BinaryReader br, Action<string> updateStatus, Action<string> errorLog)
		{
			ReadHeader(br);
			GrandTotalRpfCount = 1u;
			GrandTotalFileCount = 1u;
			GrandTotalFolderCount = 0u;
			GrandTotalResourceCount = 0u;
			GrandTotalBinaryFileCount = 0u;
			Children = new List<RpfFile>();
			updateStatus?.Invoke("Scanning " + Path + "...");
			foreach (RpfEntry allEntry in AllEntries)
			{
				try
				{
					if (allEntry is RpfBinaryFileEntry)
					{
						RpfBinaryFileEntry rpfBinaryFileEntry = allEntry as RpfBinaryFileEntry;
						if (rpfBinaryFileEntry.NameLower.EndsWith(".rpf") && IsValidPath(rpfBinaryFileEntry.Path))
						{
							br.BaseStream.Position = StartPos + (long)rpfBinaryFileEntry.FileOffset * 512L;
							long fileSize = rpfBinaryFileEntry.GetFileSize();
							RpfFile rpfFile = new RpfFile(rpfBinaryFileEntry.Name, rpfBinaryFileEntry.Path, fileSize);
							rpfFile.Parent = this;
							rpfFile.ParentFileEntry = rpfBinaryFileEntry;
							rpfFile.ScanStructure(br, updateStatus, errorLog);
							GrandTotalRpfCount += rpfFile.GrandTotalRpfCount;
							GrandTotalFileCount += rpfFile.GrandTotalFileCount;
							GrandTotalFolderCount += rpfFile.GrandTotalFolderCount;
							GrandTotalResourceCount += rpfFile.GrandTotalResourceCount;
							GrandTotalBinaryFileCount += rpfFile.GrandTotalBinaryFileCount;
							Children.Add(rpfFile);
						}
						else
						{
							GrandTotalBinaryFileCount++;
							GrandTotalFileCount++;
						}
					}
					else if (allEntry is RpfResourceFileEntry)
					{
						GrandTotalResourceCount++;
						GrandTotalFileCount++;
					}
					else if (allEntry is RpfDirectoryEntry)
					{
						GrandTotalFolderCount++;
					}
				}
				catch (Exception ex)
				{
					errorLog?.Invoke(allEntry.Path + ": " + ex.ToString());
				}
			}
		}

		public byte[] ExtractFile(RpfFileEntry entry)
		{
			try
			{
				using (BinaryReader br = new BinaryReader(File.OpenRead(GetPhysicalFilePath())))
				{
					if (entry is RpfBinaryFileEntry)
					{
						return ExtractFileBinary(entry as RpfBinaryFileEntry, br);
					}
					if (entry is RpfResourceFileEntry)
					{
						return ExtractFileResource(entry as RpfResourceFileEntry, br);
					}
					return null;
				}
			}
			catch (Exception ex)
			{
				LastError = ex.ToString();
				LastException = ex;
				return null;
			}
		}

		public byte[] ExtractFileBinary(RpfBinaryFileEntry entry, BinaryReader br)
		{
			br.BaseStream.Position = StartPos + (long)entry.FileOffset * 512L;
			long fileSize = entry.GetFileSize();
			if (fileSize > 0)
			{
				uint num = 0u;
				uint num2 = (uint)(int)fileSize - num;
				byte[] array = new byte[num2];
				br.BaseStream.Position += num;
				br.Read(array, 0, (int)num2);
				byte[] array2 = array;
				if (entry.IsEncrypted)
				{
					array2 = ((!IsAESEncrypted) ? GTACrypto.DecryptNG(array, entry.Name, entry.FileUncompressedSize) : GTACrypto.DecryptAES(array));
				}
				byte[] result = array2;
				if (entry.FileSize != 0)
				{
					result = DecompressBytes(array2);
				}
				return result;
			}
			return null;
		}

		public byte[] ExtractFileResource(RpfResourceFileEntry entry, BinaryReader br)
		{
			br.BaseStream.Position = StartPos + (long)entry.FileOffset * 512L;
			if (entry.FileSize != 0)
			{
				uint num = 16u;
				uint num2 = entry.FileSize - num;
				byte[] array = new byte[num2];
				br.BaseStream.Position += num;
				br.Read(array, 0, (int)num2);
				byte[] array2 = array;
				if (entry.IsEncrypted)
				{
					array2 = ((!IsAESEncrypted) ? GTACrypto.DecryptNG(array, entry.Name, entry.FileSize) : GTACrypto.DecryptAES(array));
				}
				byte[] array3 = DecompressBytes(array2);
				byte[] array4 = null;
				if (array3 != null)
				{
					return array3;
				}
				entry.FileSize -= num;
				return array2;
			}
			return null;
		}

		public byte[] DecompressBytes(byte[] bytes)
		{
			try
			{
				using (DeflateStream deflateStream = new DeflateStream(new MemoryStream(bytes), CompressionMode.Decompress))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						deflateStream.CopyTo(memoryStream);
						byte[] buffer = memoryStream.GetBuffer();
						byte[] array = new byte[memoryStream.Length];
						Buffer.BlockCopy(buffer, 0, array, 0, array.Length);
						if (array.Length <= bytes.Length)
						{
							LastError = "Warning: Decompressed data was smaller than compressed data...";
						}
						return array;
					}
				}
			}
			catch (Exception lastException)
			{
				LastError = "Could not decompress.";
				LastException = lastException;
				return null;
			}
		}

		public static byte[] CompressBytes(byte[] data)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
				{
					deflateStream.Write(data, 0, data.Length);
					deflateStream.Close();
					byte[] buffer = memoryStream.GetBuffer();
					byte[] array = new byte[memoryStream.Length];
					Buffer.BlockCopy(buffer, 0, array, 0, array.Length);
					return array;
				}
			}
		}

		private void WriteHeader(BinaryWriter bw)
		{
			byte[] array = GetHeaderNamesData();
			NamesLength = (uint)array.Length;
			uint num = GetHeaderBlockCount() * 512;
			EnsureSpace(bw, null, num);
			byte[] array2 = GetHeaderEntriesData();
			switch (Encryption)
			{
			case RpfEncryption.AES:
				array2 = GTACrypto.EncryptAES(array2);
				array = GTACrypto.EncryptAES(array);
				IsAESEncrypted = true;
				break;
			case RpfEncryption.NG:
				array2 = GTACrypto.EncryptNG(array2, Name, (uint)FileSize);
				array = GTACrypto.EncryptNG(array, Name, (uint)FileSize);
				IsNGEncrypted = true;
				break;
			default:
				array2 = GTACrypto.DecryptNG(array2, Name, (uint)FileSize);
				array = GTACrypto.DecryptNG(array, Name, (uint)FileSize);
				IsNGEncrypted = true;
				break;
			case RpfEncryption.NONE:
			case RpfEncryption.OPEN:
				break;
			}
			bw.BaseStream.Position = StartPos;
			bw.Write(Version);
			bw.Write(EntryCount);
			bw.Write(NamesLength);
			bw.Write((uint)Encryption);
			bw.Write(array2);
			bw.Write(array);
			WritePadding(bw.BaseStream, StartPos + num);
		}

		private static void WritePadding(Stream s, long upto)
		{
			int num = (int)(upto - s.Position);
			if (num > 0)
			{
				s.Write(new byte[num], 0, num);
			}
		}

		private void EnsureAllEntries()
		{
			if (AllEntries == null)
			{
				AllEntries = new List<RpfEntry>();
				Root = new RpfDirectoryEntry();
				Root.File = this;
				Root.Name = string.Empty;
				Root.NameLower = string.Empty;
				Root.Path = Path.ToLowerInvariant();
			}
			if (Children == null)
			{
				Children = new List<RpfFile>();
			}
			List<RpfEntry> list = new List<RpfEntry>();
			AllEntries.Clear();
			AllEntries.Add(Root);
			Stack<RpfDirectoryEntry> stack = new Stack<RpfDirectoryEntry>();
			stack.Push(Root);
			while (stack.Count > 0)
			{
				RpfDirectoryEntry rpfDirectoryEntry = stack.Pop();
				rpfDirectoryEntry.EntriesCount = (uint)(rpfDirectoryEntry.Directories.Count + rpfDirectoryEntry.Files.Count);
				rpfDirectoryEntry.EntriesIndex = (uint)AllEntries.Count;
				list.Clear();
				list.AddRange(rpfDirectoryEntry.Directories);
				list.AddRange(rpfDirectoryEntry.Files);
				list.Sort((RpfEntry a, RpfEntry b) => string.CompareOrdinal(a.Name, b.Name));
				foreach (RpfEntry item2 in list)
				{
					AllEntries.Add(item2);
					if (item2 is RpfDirectoryEntry item)
					{
						stack.Push(item);
					}
				}
			}
			EntryCount = (uint)AllEntries.Count;
		}

		private byte[] GetHeaderNamesData()
		{
			MemoryStream memoryStream = new MemoryStream();
			DataWriter dataWriter = new DataWriter(memoryStream);
			Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
			foreach (RpfEntry allEntry in AllEntries)
			{
				string text = allEntry.Name ?? "";
				if (dictionary.TryGetValue(text, out var value))
				{
					allEntry.NameOffset = value;
					continue;
				}
				allEntry.NameOffset = (uint)memoryStream.Length;
				dictionary.Add(text, allEntry.NameOffset);
				dataWriter.Write(text);
			}
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Position = 0L;
			memoryStream.Read(array, 0, array.Length);
			return PadBuffer(array, 16u);
		}

		private byte[] GetHeaderEntriesData()
		{
			MemoryStream memoryStream = new MemoryStream();
			DataWriter writer = new DataWriter(memoryStream);
			foreach (RpfEntry allEntry in AllEntries)
			{
				allEntry.Write(writer);
			}
			byte[] array = new byte[memoryStream.Length];
			memoryStream.Position = 0L;
			memoryStream.Read(array, 0, array.Length);
			return array;
		}

		private uint GetHeaderBlockCount()
		{
			return GetBlockCount(16 + EntryCount * 16 + NamesLength);
		}

		private static byte[] PadBuffer(byte[] buf, uint n)
		{
			uint num = (uint)buf.Length;
			uint num2 = PadLength(num, n);
			if (num2 != num)
			{
				byte[] array = new byte[num2];
				Buffer.BlockCopy(buf, 0, array, 0, buf.Length);
				return array;
			}
			return buf;
		}

		private static uint PadLength(uint l, uint n)
		{
			uint num = l % n;
			return l + ((num != 0) ? (n - num) : 0);
		}

		private static uint GetBlockCount(long bytecount)
		{
			int num = (int)(bytecount & 0x1FF);
			uint num2 = (uint)(bytecount >> 9);
			if (num == 0)
			{
				return num2;
			}
			return num2 + 1;
		}

		private RpfFileEntry FindFirstFileAfter(uint block)
		{
			RpfFileEntry rpfFileEntry = null;
			foreach (RpfEntry allEntry in AllEntries)
			{
				if (allEntry is RpfFileEntry rpfFileEntry2 && rpfFileEntry2.FileOffset > block && (rpfFileEntry == null || rpfFileEntry2.FileOffset < rpfFileEntry.FileOffset))
				{
					rpfFileEntry = rpfFileEntry2;
				}
			}
			return rpfFileEntry;
		}

		private uint FindHole(uint reqblocks, uint ignorestart, uint ignoreend)
		{
			List<RpfFileEntry> list = new List<RpfFileEntry>();
			foreach (RpfEntry allEntry in AllEntries)
			{
				if (allEntry is RpfFileEntry item)
				{
					list.Add(item);
				}
			}
			list.Sort((RpfFileEntry e1, RpfFileEntry e2) => e1.FileOffset.CompareTo(e2.FileOffset));
			uint result = 0u;
			uint num = uint.MaxValue;
			uint headerBlockCount = GetHeaderBlockCount();
			uint num2 = headerBlockCount;
			for (int num3 = 0; num3 < list.Count(); num3++)
			{
				RpfFileEntry rpfFileEntry = list[num3];
				uint blockCount = GetBlockCount(rpfFileEntry.GetFileSize());
				uint fileOffset = rpfFileEntry.FileOffset;
				headerBlockCount = num2;
				num2 = rpfFileEntry.FileOffset + blockCount;
				if ((fileOffset <= ignorestart || headerBlockCount >= ignoreend) && headerBlockCount < fileOffset)
				{
					uint num4 = fileOffset - headerBlockCount;
					if (num4 >= reqblocks && num4 < num)
					{
						result = headerBlockCount;
						num = num4;
					}
				}
			}
			return result;
		}

		private uint FindEndBlock()
		{
			uint num = 0u;
			foreach (RpfEntry allEntry in AllEntries)
			{
				if (allEntry is RpfFileEntry rpfFileEntry)
				{
					uint blockCount = GetBlockCount(rpfFileEntry.GetFileSize());
					uint num2 = rpfFileEntry.FileOffset + blockCount;
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			if (num == 0)
			{
				num = GetHeaderBlockCount();
			}
			return num;
		}

		private void GrowArchive(BinaryWriter bw, uint newblockcount)
		{
			uint num = newblockcount * 512;
			if (num < FileSize || FileSize == num)
			{
				return;
			}
			FileSize = num;
			if (Parent != null)
			{
				if (ParentFileEntry == null)
				{
					throw new Exception("Can't grow archive " + Path + ": ParentFileEntry was null!");
				}
				ParentFileEntry.FileUncompressedSize = num;
				ParentFileEntry.FileSize = 0u;
				Parent.EnsureSpace(bw, ParentFileEntry, num);
			}
		}

		private void RelocateFile(BinaryWriter bw, RpfFileEntry f, uint newblock)
		{
			uint blockCount = GetBlockCount(f.GetFileSize());
			uint fileOffset = f.FileOffset;
			uint num = fileOffset + blockCount;
			if (newblock + blockCount > fileOffset && newblock < num)
			{
				throw new Exception("Unable to relocate file " + f.Path + ": new position was inside the original!");
			}
			Stream baseStream = bw.BaseStream;
			long position = baseStream.Position;
			long num2 = StartPos + (long)fileOffset * 512L;
			long num3 = StartPos + (long)newblock * 512L;
			long newpos = num3;
			long num4 = (long)blockCount * 512L;
			long upto = num3 + num4;
			byte[] buffer = new byte[16384];
			while (num4 > 0)
			{
				baseStream.Position = num2;
				int num5 = baseStream.Read(buffer, 0, (int)Math.Min(num4, 16384L));
				baseStream.Position = num3;
				baseStream.Write(buffer, 0, num5);
				num2 += num5;
				num3 += num5;
				num4 -= num5;
			}
			WritePadding(baseStream, upto);
			baseStream.Position = position;
			f.FileOffset = newblock;
			FindChildArchive(f)?.UpdateStartPos(newpos);
		}

		private void EnsureSpace(BinaryWriter bw, RpfFileEntry e, long bytecount)
		{
			uint blockCount = GetBlockCount(bytecount);
			uint num = e?.FileOffset ?? 0;
			uint num2 = num + blockCount;
			RpfFileEntry rpfFileEntry = FindFirstFileAfter(num);
			while (rpfFileEntry != null && rpfFileEntry.FileOffset < num2)
			{
				uint blockCount2 = GetBlockCount(rpfFileEntry.GetFileSize());
				uint num3 = FindHole(blockCount2, num, num2);
				if (num3 == 0)
				{
					num3 = FindEndBlock();
					GrowArchive(bw, num3 + blockCount2);
				}
				RelocateFile(bw, rpfFileEntry, num3);
				rpfFileEntry = FindFirstFileAfter(num);
			}
			if (rpfFileEntry == null)
			{
				uint num4 = FindEndBlock();
				GrowArchive(bw, num4 + ((e != null) ? blockCount : 0));
			}
			if (e != null)
			{
				WriteHeader(bw);
			}
		}

		private void InsertFileSpace(BinaryWriter bw, RpfFileEntry entry)
		{
			uint blockCount = GetBlockCount(entry.GetFileSize());
			entry.FileOffset = FindHole(blockCount, 0u, 0u);
			if (entry.FileOffset == 0)
			{
				entry.FileOffset = FindEndBlock();
				GrowArchive(bw, entry.FileOffset + blockCount);
			}
			EnsureAllEntries();
			WriteHeader(bw);
		}

		private void WriteNewArchive(BinaryWriter bw, RpfEncryption encryption)
		{
			Stream baseStream = bw.BaseStream;
			Encryption = encryption;
			Version = 1380992567u;
			IsAESEncrypted = encryption == RpfEncryption.AES;
			IsNGEncrypted = encryption == RpfEncryption.NG;
			StartPos = baseStream.Position;
			EnsureAllEntries();
			WriteHeader(bw);
			FileSize = baseStream.Position - StartPos;
		}

		public RpfFile FindChildArchive(RpfFileEntry f)
		{
			RpfFile result = null;
			if (Children != null)
			{
				foreach (RpfFile child in Children)
				{
					if (child.ParentFileEntry == f)
					{
						result = child;
						break;
					}
				}
			}
			return result;
		}

		private void UpdateStartPos(long newpos)
		{
			StartPos = newpos;
			if (Children == null)
			{
				return;
			}
			foreach (RpfFile child in Children)
			{
				if (child.ParentFileEntry != null)
				{
					long newpos2 = StartPos + (long)child.ParentFileEntry.FileOffset * 512L;
					child.UpdateStartPos(newpos2);
				}
			}
		}

		public static RpfFile CreateNew(string gtafolder, string relpath, RpfEncryption encryption = RpfEncryption.OPEN)
		{
			string text = gtafolder;
			text = (text.EndsWith("\\") ? text : (text + "\\"));
			text = (relpath.Contains(":") ? relpath : (text + relpath));
			if (File.Exists(text))
			{
				throw new Exception("File " + text + " already exists!");
			}
			File.Create(text).Dispose();
			RpfFile rpfFile = new RpfFile(text, relpath);
			using (FileStream output = File.Open(text, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryWriter bw = new BinaryWriter(output))
				{
					rpfFile.WriteNewArchive(bw, encryption);
					return rpfFile;
				}
			}
		}

		public static RpfFile CreateNew(RpfDirectoryEntry dir, string name, RpfEncryption encryption = RpfEncryption.OPEN)
		{
			string text = name.ToLowerInvariant();
			RpfFile file = dir.File;
			string physicalFilePath = file.GetPhysicalFilePath();
			string path = dir.Path + "\\" + text;
			if (!File.Exists(physicalFilePath))
			{
				throw new Exception("Root RPF file " + physicalFilePath + " does not exist!");
			}
			RpfFile rpfFile = new RpfFile(name, path, 512L);
			rpfFile.Parent = file;
			rpfFile.ParentFileEntry = new RpfBinaryFileEntry();
			RpfBinaryFileEntry parentFileEntry = rpfFile.ParentFileEntry;
			parentFileEntry.Parent = dir;
			parentFileEntry.FileOffset = 0u;
			parentFileEntry.FileSize = 0u;
			parentFileEntry.FileUncompressedSize = (uint)rpfFile.FileSize;
			parentFileEntry.EncryptionType = 0u;
			parentFileEntry.IsEncrypted = false;
			parentFileEntry.File = file;
			parentFileEntry.Path = path;
			parentFileEntry.Name = name;
			parentFileEntry.NameLower = text;
			parentFileEntry.NameHash = JenkHash.GenHash(name);
			parentFileEntry.ShortNameHash = JenkHash.GenHash(parentFileEntry.GetShortNameLower());
			dir.Files.Add(parentFileEntry);
			file.Children.Add(rpfFile);
			using (FileStream fileStream = File.Open(physicalFilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryWriter bw = new BinaryWriter(fileStream))
				{
					file.InsertFileSpace(bw, parentFileEntry);
					fileStream.Position = file.StartPos + (long)parentFileEntry.FileOffset * 512L;
					rpfFile.WriteNewArchive(bw, encryption);
					return rpfFile;
				}
			}
		}

		public static RpfFileEntry CreateFile(RpfDirectoryEntry dir, string name, byte[] data, bool overwrite = true)
		{
			string text = name.ToLowerInvariant();
			if (overwrite)
			{
				foreach (RpfFileEntry file2 in dir.Files)
				{
					if (file2.NameLower == text)
					{
						DeleteEntry(file2);
						break;
					}
				}
			}
			RpfFile file = dir.File;
			string physicalFilePath = file.GetPhysicalFilePath();
			string path = dir.Path + "\\" + text;
			if (!File.Exists(physicalFilePath))
			{
				throw new Exception("Root RPF file " + physicalFilePath + " does not exist!");
			}
			RpfFileEntry rpfFileEntry = null;
			uint num = (uint)data.Length;
			bool flag = false;
			bool flag2 = false;
			uint num2 = 0u;
			if (num >= 16)
			{
				num2 = BitConverter.ToUInt32(data, 0);
			}
			if (num2 == 927159122)
			{
				RpfResourceFileEntry rpfResourceFileEntry = new RpfResourceFileEntry();
				BitConverter.ToUInt32(data, 4);
				rpfResourceFileEntry.SystemFlags = BitConverter.ToUInt32(data, 8);
				rpfResourceFileEntry.GraphicsFlags = BitConverter.ToUInt32(data, 12);
				rpfResourceFileEntry.FileSize = num;
				if (num >= 16777215)
				{
					data[7] = (byte)(num & 0xFF);
					data[14] = (byte)((num >> 8) & 0xFF);
					data[5] = (byte)((num >> 16) & 0xFF);
					data[2] = (byte)((num >> 24) & 0xFF);
				}
				rpfFileEntry = rpfResourceFileEntry;
			}
			if (text.EndsWith(".rpf") && num2 == 1380992567)
			{
				flag = true;
			}
			if (text.EndsWith(".awc"))
			{
				flag2 = true;
			}
			if (rpfFileEntry == null)
			{
				byte[] array = ((flag || flag2) ? data : CompressBytes(data));
				RpfBinaryFileEntry rpfBinaryFileEntry = new RpfBinaryFileEntry();
				rpfBinaryFileEntry.EncryptionType = 0u;
				rpfBinaryFileEntry.IsEncrypted = false;
				rpfBinaryFileEntry.FileUncompressedSize = (uint)data.Length;
				rpfBinaryFileEntry.FileSize = ((!(flag || flag2)) ? ((uint)array.Length) : 0u);
				if (rpfBinaryFileEntry.FileSize > 16777215)
				{
					rpfBinaryFileEntry.FileSize = 0u;
					array = data;
				}
				data = array;
				rpfFileEntry = rpfBinaryFileEntry;
			}
			rpfFileEntry.Parent = dir;
			rpfFileEntry.File = file;
			rpfFileEntry.Path = path;
			rpfFileEntry.Name = name;
			rpfFileEntry.NameLower = name.ToLowerInvariant();
			rpfFileEntry.NameHash = JenkHash.GenHash(name);
			rpfFileEntry.ShortNameHash = JenkHash.GenHash(rpfFileEntry.GetShortNameLower());
			foreach (RpfFileEntry file3 in dir.Files)
			{
				if (file3.NameLower == rpfFileEntry.NameLower)
				{
					throw new Exception("File \"" + rpfFileEntry.Name + "\" already exists!");
				}
			}
			dir.Files.Add(rpfFileEntry);
			using (FileStream fileStream = File.Open(physicalFilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryWriter bw = new BinaryWriter(fileStream))
				{
					file.InsertFileSpace(bw, rpfFileEntry);
					long num3 = file.StartPos + (long)rpfFileEntry.FileOffset * 512L;
					long upto = num3 + (long)GetBlockCount(rpfFileEntry.GetFileSize()) * 512L;
					fileStream.Position = num3;
					fileStream.Write(data, 0, data.Length);
					WritePadding(fileStream, upto);
				}
			}
			if (flag)
			{
				RpfFile rpfFile = new RpfFile(name, path, data.LongLength);
				rpfFile.Parent = file;
				rpfFile.ParentFileEntry = rpfFileEntry as RpfBinaryFileEntry;
				rpfFile.StartPos = file.StartPos + (long)rpfFileEntry.FileOffset * 512L;
				file.Children.Add(rpfFile);
				using (FileStream fileStream2 = File.OpenRead(physicalFilePath))
				{
					using (BinaryReader br = new BinaryReader(fileStream2))
					{
						fileStream2.Position = rpfFile.StartPos;
						rpfFile.ScanStructure(br, null, null);
					}
				}
			}
			return rpfFileEntry;
		}

		public static void DeleteEntry(RpfEntry entry)
		{
			RpfFile file = entry.File;
			string physicalFilePath = file.GetPhysicalFilePath();
			if (!File.Exists(physicalFilePath))
			{
				throw new Exception("Root RPF file " + physicalFilePath + " does not exist!");
			}
			RpfDirectoryEntry rpfDirectoryEntry = entry as RpfDirectoryEntry;
			RpfFileEntry rpfFileEntry = entry as RpfFileEntry;
			if (rpfDirectoryEntry != null)
			{
				RpfDirectoryEntry[] array = rpfDirectoryEntry.Directories.ToArray();
				RpfFileEntry[] array2 = rpfDirectoryEntry.Files.ToArray();
				RpfDirectoryEntry[] array3 = array;
				for (int i = 0; i < array3.Length; i++)
				{
					DeleteEntry(array3[i]);
				}
				RpfFileEntry[] array4 = array2;
				for (int i = 0; i < array4.Length; i++)
				{
					DeleteEntry(array4[i]);
				}
			}
			if (entry.Parent == null)
			{
				throw new Exception("Parent directory is null! This shouldn't happen - please refresh the folder!");
			}
			if (rpfDirectoryEntry != null)
			{
				entry.Parent.Directories.Remove(rpfDirectoryEntry);
			}
			if (rpfFileEntry != null)
			{
				entry.Parent.Files.Remove(rpfFileEntry);
				RpfFile rpfFile = file.FindChildArchive(rpfFileEntry);
				if (rpfFile != null)
				{
					file.Children.Remove(rpfFile);
				}
			}
			using (FileStream output = File.Open(physicalFilePath, FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryWriter bw = new BinaryWriter(output))
				{
					file.EnsureAllEntries();
					file.WriteHeader(bw);
				}
			}
		}

		public static void Defragment(RpfFile file, Action<string, float> progress = null, bool recursive = true)
		{
			if (file?.AllEntries == null)
			{
				return;
			}
			if (recursive)
			{
				foreach (RpfEntry item2 in file?.AllEntries)
				{
					if (item2 is RpfFileEntry)
					{
						RpfFile rpfFile = file.FindChildArchive(item2 as RpfFileEntry);
						if (rpfFile != null)
						{
							Defragment(rpfFile);
						}
					}
				}
			}
			using (FileStream fileStream = File.Open(file.GetPhysicalFilePath(), FileMode.Open, FileAccess.ReadWrite))
			{
				using (BinaryWriter bw = new BinaryWriter(fileStream))
				{
					uint num = file.GetHeaderBlockCount();
					byte[] buffer = new byte[16384];
					List<RpfFileEntry> list = new List<RpfFileEntry>();
					for (int i = 0; i < file.AllEntries.Count; i++)
					{
						if (file.AllEntries[i] is RpfFileEntry item)
						{
							list.Add(item);
						}
					}
					list.Sort((RpfFileEntry a, RpfFileEntry b) => a.FileOffset.CompareTo(b.FileOffset));
					for (int num2 = 0; num2 < list.Count; num2++)
					{
						RpfFileEntry rpfFileEntry = list[num2];
						float arg = (float)num2 / (float)list.Count;
						string arg2 = "Relocating " + rpfFileEntry.Name + "...";
						progress?.Invoke(arg2, arg);
						uint fileOffset = rpfFileEntry.FileOffset;
						uint blockCount = GetBlockCount(rpfFileEntry.GetFileSize());
						if (fileOffset > num)
						{
							long num3 = file.StartPos + (long)fileOffset * 512L;
							long num4 = file.StartPos + (long)num * 512L;
							long num5 = (long)blockCount * 512L;
							while (num5 > 0)
							{
								fileStream.Position = num3;
								int num6 = fileStream.Read(buffer, 0, (int)Math.Min(num5, 16384L));
								fileStream.Position = num4;
								fileStream.Write(buffer, 0, num6);
								num3 += num6;
								num4 += num6;
								num5 -= num6;
							}
							rpfFileEntry.FileOffset = num;
							file.FindChildArchive(rpfFileEntry)?.UpdateStartPos(file.StartPos + (long)rpfFileEntry.FileOffset * 512L);
						}
						num += blockCount;
					}
					file.FileSize = (long)num * 512L;
					file.WriteHeader(bw);
					if (file.ParentFileEntry != null)
					{
						file.ParentFileEntry.FileUncompressedSize = (uint)file.FileSize;
						file.ParentFileEntry.FileSize = 0u;
						if (file.Parent != null)
						{
							file.Parent.WriteHeader(bw);
						}
					}
					if (file.Parent == null)
					{
						fileStream.SetLength(file.FileSize);
					}
				}
			}
		}

		private static bool IsValidPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			if (path.Length > 500)
			{
				return false;
			}
			int num = 0;
			foreach (char c in path)
			{
				switch (c)
				{
				case ':':
					return false;
				case ';':
					return false;
				case '/':
					num++;
					break;
				}
				if (c == '\\')
				{
					num++;
				}
			}
			if (num > 20)
			{
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return Path;
		}
	}
}
