using System.ComponentModel;
using System.IO;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RpfResourceFileEntry : RpfFileEntry
	{
		public RpfResourcePageFlags SystemFlags { get; set; }

		public RpfResourcePageFlags GraphicsFlags { get; set; }

		public int Version => GetVersionFromFlags(SystemFlags, GraphicsFlags);

		public int SystemSize => (int)SystemFlags.Size;

		public int GraphicsSize => (int)GraphicsFlags.Size;

		public static uint GetFlagsFromBlocks(uint blockCount, uint blockSize, uint version)
		{
			uint num = 0u;
			uint num2 = 0u;
			uint num3 = 0u;
			uint num4 = 0u;
			uint num5 = 0u;
			uint num6 = 0u;
			uint num7 = 0u;
			uint num8 = 0u;
			uint num9 = 0u;
			uint num10 = 0u;
			uint num11 = blockSize;
			if (blockCount != 0)
			{
				while (num11 > 512)
				{
					num10++;
					num11 >>= 1;
				}
			}
			num = blockCount & 1;
			num2 = (blockCount >> 1) & 1;
			num3 = (blockCount >> 2) & 1;
			num4 = (blockCount >> 3) & 1;
			num5 = (blockCount >> 4) & 0x7F;
			_ = 15;
			_ = 127;
			return 0 | ((version & 0xF) << 28) | ((num & 1) << 27) | ((num2 & 1) << 26) | ((num3 & 1) << 25) | ((num4 & 1) << 24) | ((num5 & 0x7F) << 17) | ((num6 & 0x3F) << 11) | ((num7 & 0xF) << 7) | ((num8 & 3) << 5) | ((num9 & 1) << 4) | (num10 & 0xF);
		}

		public static int GetVersionFromFlags(uint sysFlags, uint gfxFlags)
		{
			uint num = (sysFlags >> 28) & 0xF;
			uint num2 = (gfxFlags >> 28) & 0xF;
			return (int)((num << 4) + num2);
		}

		public override void Read(DataReader reader)
		{
			base.NameOffset = reader.ReadUInt16();
			byte[] array = reader.ReadBytes(3);
			base.FileSize = (uint)(array[0] + (array[1] << 8) + (array[2] << 16));
			byte[] array2 = reader.ReadBytes(3);
			base.FileOffset = (uint)((array2[0] + (array2[1] << 8) + (array2[2] << 16)) & 0x7FFFFF);
			SystemFlags = reader.ReadUInt32();
			GraphicsFlags = reader.ReadUInt32();
			if (base.FileSize == 16777215)
			{
				BinaryReader currentFileReader = base.File.CurrentFileReader;
				long position = currentFileReader.BaseStream.Position;
				currentFileReader.BaseStream.Position = base.File.StartPos + (long)base.FileOffset * 512L;
				byte[] array3 = currentFileReader.ReadBytes(16);
				base.FileSize = (uint)(array3[7] | (array3[14] << 8) | (array3[5] << 16) | (array3[2] << 24));
				currentFileReader.BaseStream.Position = position;
			}
		}

		public override void Write(DataWriter writer)
		{
			writer.Write((ushort)base.NameOffset);
			uint num = base.FileSize;
			if (num > 16777215)
			{
				num = 16777215u;
			}
			byte[] value = new byte[3]
			{
				(byte)(num & 0xFF),
				(byte)((num >> 8) & 0xFF),
				(byte)((num >> 16) & 0xFF)
			};
			writer.Write(value);
			byte[] value2 = new byte[3]
			{
				(byte)(base.FileOffset & 0xFF),
				(byte)((base.FileOffset >> 8) & 0xFF),
				(byte)(((base.FileOffset >> 16) & 0xFF) | 0x80)
			};
			writer.Write(value2);
			writer.Write(SystemFlags);
			writer.Write(GraphicsFlags);
		}

		public override string ToString()
		{
			return "Resource file: " + base.Path;
		}

		public override long GetFileSize()
		{
			if (base.FileSize != 0)
			{
				return base.FileSize;
			}
			return SystemSize + GraphicsSize;
		}
	}
}
