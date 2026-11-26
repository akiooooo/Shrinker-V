using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameFormats.Files
{
	public class ResourceDataReader : DataReader
	{
		public bool IsGen9 = RpfManager.IsGen9;

		private Stream systemStream;

		private Stream graphicsStream;

		[CompilerGenerated]
		private RpfResourceFileEntry _003CFileEntry_003Ek__BackingField;

		public Dictionary<long, IResourceBlock> blockPool = new Dictionary<long, IResourceBlock>();

		public Dictionary<long, object> arrayPool = new Dictionary<long, object>();

		public RpfResourceFileEntry FileEntry
		{
			[CompilerGenerated]
			set
			{
				_003CFileEntry_003Ek__BackingField = value;
			}
		}

		public override long Position { get; set; }

		public ResourceDataReader(RpfResourceFileEntry resentry, byte[] data, Endianess endianess = Endianess.LittleEndian)
			: base(null, endianess)
		{
			FileEntry = resentry;
			int systemSize = resentry.SystemSize;
			int graphicsSize = resentry.GraphicsSize;
			systemStream = new MemoryStream(data, 0, systemSize);
			graphicsStream = new MemoryStream(data, systemSize, graphicsSize);
			Position = 1342177280L;
		}

		protected override byte[] ReadFromStream(int count, bool ignoreEndianess = false)
		{
			if ((Position & 0x50000000) == 1342177280)
			{
				systemStream.Position = Position & -1342177281;
				byte[] array = new byte[count];
				systemStream.Read(array, 0, count);
				if (!ignoreEndianess && base.Endianess == Endianess.BigEndian)
				{
					Array.Reverse(array);
				}
				Position = systemStream.Position | 0x50000000;
				return array;
			}
			if ((Position & 0x60000000) == 1610612736)
			{
				graphicsStream.Position = Position & -1610612737;
				byte[] array2 = new byte[count];
				graphicsStream.Read(array2, 0, count);
				if (!ignoreEndianess && base.Endianess == Endianess.BigEndian)
				{
					Array.Reverse(array2);
				}
				Position = graphicsStream.Position | 0x60000000;
				return array2;
			}
			throw new Exception("illegal position!");
		}

		public T ReadBlock<T>(params object[] parameters) where T : IResourceBlock, new()
		{
			bool flag = !typeof(IResourceNoCacheBlock).IsAssignableFrom(typeof(T));
			if (flag && blockPool.ContainsKey(Position))
			{
				IResourceBlock resourceBlock = blockPool[Position];
				if (resourceBlock is T result)
				{
					Position += resourceBlock.BlockLength;
					return result;
				}
				flag = false;
			}
			T val = new T();
			if (val is IResourceXXSystemBlock)
			{
				val = (T)((IResourceXXSystemBlock)(object)val).GetType(this, parameters);
			}
			if (val == null)
			{
				return default(T);
			}
			if (flag)
			{
				blockPool[Position] = val;
			}
			val.Read(this, parameters);
			return val;
		}

		public T ReadBlockAt<T>(ulong position, params object[] parameters) where T : IResourceBlock, new()
		{
			if (position != 0L)
			{
				long position2 = Position;
				Position = (long)position;
				T result = ReadBlock<T>(parameters);
				Position = position2;
				return result;
			}
			return default(T);
		}

		public T[] ReadBlocks<T>(ulong[] pointers) where T : IResourceBlock, new()
		{
			if (pointers == null)
			{
				return null;
			}
			int num = pointers.Length;
			T[] array = new T[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = ReadBlockAt<T>(pointers[i], Array.Empty<object>());
			}
			return array;
		}

		public byte[] ReadBytesAt(ulong position, uint count, bool cache = true)
		{
			if ((long)position <= 0L || count == 0)
			{
				return null;
			}
			long position2 = Position;
			Position = (long)position;
			byte[] array = ReadBytes((int)count);
			Position = position2;
			if (cache)
			{
				arrayPool[(long)position] = array;
			}
			return array;
		}

		public ushort[] ReadUshortsAt(ulong position, uint count, bool cache = true)
		{
			if (position == 0 || count == 0)
			{
				return null;
			}
			ushort[] array = new ushort[count];
			uint count2 = count * 2;
			Buffer.BlockCopy(ReadBytesAt(position, count2, cache: false), 0, array, 0, (int)count2);
			if (cache)
			{
				arrayPool[(long)position] = array;
			}
			return array;
		}

		public short[] ReadShortsAt(ulong position, uint count, bool cache = true)
		{
			if (position == 0 || count == 0)
			{
				return null;
			}
			short[] array = new short[count];
			uint count2 = count * 2;
			Buffer.BlockCopy(ReadBytesAt(position, count2, cache: false), 0, array, 0, (int)count2);
			if (cache)
			{
				arrayPool[(long)position] = array;
			}
			return array;
		}

		public uint[] ReadUintsAt(ulong position, uint count, bool cache = true)
		{
			if (position == 0 || count == 0)
			{
				return null;
			}
			uint[] array = new uint[count];
			uint count2 = count * 4;
			Buffer.BlockCopy(ReadBytesAt(position, count2, cache: false), 0, array, 0, (int)count2);
			if (cache)
			{
				arrayPool[(long)position] = array;
			}
			return array;
		}

		public ulong[] ReadUlongsAt(ulong position, uint count, bool cache = true)
		{
			if (position == 0 || count == 0)
			{
				return null;
			}
			ulong[] array = new ulong[count];
			uint count2 = count * 8;
			Buffer.BlockCopy(ReadBytesAt(position, count2, cache: false), 0, array, 0, (int)count2);
			if (cache)
			{
				arrayPool[(long)position] = array;
			}
			return array;
		}

		public float[] ReadFloatsAt(ulong position, uint count, bool cache = true)
		{
			if (position == 0 || count == 0)
			{
				return null;
			}
			float[] array = new float[count];
			uint count2 = count * 4;
			Buffer.BlockCopy(ReadBytesAt(position, count2, cache: false), 0, array, 0, (int)count2);
			if (cache)
			{
				arrayPool[(long)position] = array;
			}
			return array;
		}

		public T[] ReadStructsAt<T>(ulong position, uint count, bool cache = true)
		{
			if (position == 0 || count == 0)
			{
				return null;
			}
			uint num = (uint)Marshal.SizeOf(typeof(T));
			uint num2 = count * num;
			byte[] source = ReadBytesAt(position, num2, cache: false);
			T[] array = new T[count];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			IntPtr destination = gCHandle.AddrOfPinnedObject();
			Marshal.Copy(source, 0, destination, (int)num2);
			gCHandle.Free();
			if (cache)
			{
				arrayPool[(long)position] = array;
			}
			return array;
		}

		public T[] ReadStructs<T>(uint count)
		{
			uint num = (uint)Marshal.SizeOf(typeof(T));
			T[] array = new T[count];
			uint num2 = count * num;
			byte[] source = ReadBytes((int)num2);
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			IntPtr destination = gCHandle.AddrOfPinnedObject();
			Marshal.Copy(source, 0, destination, (int)num2);
			gCHandle.Free();
			return array;
		}

		public T ReadStruct<T>() where T : struct
		{
			uint count = (uint)Marshal.SizeOf(typeof(T));
			GCHandle gCHandle = GCHandle.Alloc(ReadBytes((int)count), GCHandleType.Pinned);
			T result = Marshal.PtrToStructure<T>(gCHandle.AddrOfPinnedObject());
			gCHandle.Free();
			return result;
		}

		public T ReadStructAt<T>(long position) where T : struct
		{
			if (position <= 0)
			{
				return default(T);
			}
			long position2 = Position;
			Position = position;
			T result = ReadStruct<T>();
			Position = position2;
			return result;
		}

		public string ReadStringAt(ulong position)
		{
			if ((long)position <= 0L)
			{
				return null;
			}
			long position2 = Position;
			Position = (long)position;
			string text = ReadString();
			Position = position2;
			arrayPool[(long)position] = text;
			return text;
		}
	}
}
