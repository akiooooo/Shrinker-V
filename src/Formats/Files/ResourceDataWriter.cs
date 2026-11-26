using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GameFormats.Files
{
	public class ResourceDataWriter : DataWriter
	{
		public bool IsGen9;

		private Stream systemStream;

		private Stream graphicsStream;

		public override long Position { get; set; }

		public ResourceDataWriter(Stream systemStream, Stream graphicsStream, Endianess endianess = Endianess.LittleEndian)
			: base(null, endianess)
		{
			this.systemStream = systemStream;
			this.graphicsStream = graphicsStream;
		}

		protected override void WriteToStream(byte[] value, bool ignoreEndianess = true)
		{
			if ((Position & 0x50000000) == 1342177280)
			{
				systemStream.Position = Position & -1342177281;
				if (!ignoreEndianess && base.Endianess == Endianess.BigEndian)
				{
					byte[] array = (byte[])value.Clone();
					Array.Reverse(array);
					systemStream.Write(array, 0, array.Length);
				}
				else
				{
					systemStream.Write(value, 0, value.Length);
				}
				Position = systemStream.Position | 0x50000000;
				return;
			}
			if ((Position & 0x60000000) == 1610612736)
			{
				graphicsStream.Position = Position & -1610612737;
				if (!ignoreEndianess && base.Endianess == Endianess.BigEndian)
				{
					byte[] array2 = (byte[])value.Clone();
					Array.Reverse(array2);
					graphicsStream.Write(array2, 0, array2.Length);
				}
				else
				{
					graphicsStream.Write(value, 0, value.Length);
				}
				Position = graphicsStream.Position | 0x60000000;
				return;
			}
			throw new Exception("illegal position!");
		}

		public void WriteBlock(IResourceBlock value)
		{
			value.Write(this);
		}

		public void WriteStruct<T>(T val) where T : struct
		{
			int num = Marshal.SizeOf(typeof(T));
			byte[] array = new byte[num];
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.StructureToPtr(val, intPtr, fDeleteOld: true);
			Marshal.Copy(intPtr, array, 0, num);
			Marshal.FreeHGlobal(intPtr);
			Write(array);
		}

		public void WriteStructs<T>(T[] val) where T : struct
		{
			if (val != null)
			{
				foreach (T val2 in val)
				{
					WriteStruct(val2);
				}
			}
		}

		public void WritePadding(int alignment)
		{
			long num = (alignment - Position % alignment) % alignment;
			if (num > 0)
			{
				Write(new byte[num]);
			}
		}

		public void WriteUlongs(ulong[] val)
		{
			if (val != null)
			{
				foreach (ulong value in val)
				{
					Write(value);
				}
			}
		}
	}
}
