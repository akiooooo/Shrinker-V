using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourcePagesInfo : ResourceSystemBlock
	{
		public override long BlockLength => 16 + 8 * (SystemPagesCount + GraphicsPagesCount);

		public uint Unknown_0h { get; set; }

		public uint Unknown_4h { get; set; }

		public byte SystemPagesCount { get; set; } = 128;

		public byte GraphicsPagesCount { get; set; }

		public ushort Unknown_Ah { get; set; }

		public uint Unknown_Ch { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_0h = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			SystemPagesCount = reader.ReadByte();
			GraphicsPagesCount = reader.ReadByte();
			Unknown_Ah = reader.ReadUInt16();
			Unknown_Ch = reader.ReadUInt32();
			reader.Position += 8 * (SystemPagesCount + GraphicsPagesCount);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Unknown_0h);
			writer.Write(Unknown_4h);
			writer.Write(SystemPagesCount);
			writer.Write(GraphicsPagesCount);
			writer.Write(Unknown_Ah);
			writer.Write(Unknown_Ch);
			int num = 8 * (SystemPagesCount + GraphicsPagesCount);
			writer.Write(new byte[num]);
		}

		public override string ToString()
		{
			return SystemPagesCount + ", " + GraphicsPagesCount;
		}
	}
}
