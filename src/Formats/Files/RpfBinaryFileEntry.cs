using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RpfBinaryFileEntry : RpfFileEntry
	{
		public uint FileUncompressedSize { get; set; }

		public uint EncryptionType { get; set; }

		public override void Read(DataReader reader)
		{
			ulong num = reader.ReadUInt64();
			base.NameOffset = (uint)((int)num & 0xFFFF);
			base.FileSize = (uint)((int)(num >> 16) & 0xFFFFFF);
			base.FileOffset = (uint)((int)(num >> 40) & 0xFFFFFF);
			FileUncompressedSize = reader.ReadUInt32();
			EncryptionType = reader.ReadUInt32();
			switch (EncryptionType)
			{
			case 0u:
				base.IsEncrypted = false;
				break;
			case 1u:
				base.IsEncrypted = true;
				break;
			default:
				base.IsEncrypted = true;
				break;
			}
		}

		public override void Write(DataWriter writer)
		{
			writer.Write((ushort)base.NameOffset);
			byte[] value = new byte[3]
			{
				(byte)(base.FileSize & 0xFF),
				(byte)((base.FileSize >> 8) & 0xFF),
				(byte)((base.FileSize >> 16) & 0xFF)
			};
			writer.Write(value);
			byte[] value2 = new byte[3]
			{
				(byte)(base.FileOffset & 0xFF),
				(byte)((base.FileOffset >> 8) & 0xFF),
				(byte)((base.FileOffset >> 16) & 0xFF)
			};
			writer.Write(value2);
			writer.Write(FileUncompressedSize);
			if (base.IsEncrypted)
			{
				writer.Write(1u);
			}
			else
			{
				writer.Write(0u);
			}
		}

		public override string ToString()
		{
			return "Binary file: " + base.Path;
		}

		public override long GetFileSize()
		{
			return (base.FileSize == 0) ? FileUncompressedSize : base.FileSize;
		}
	}
}
