using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ClothInstanceTuning : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public ulong Unknown_18h;

		public override long BlockLength => 64L;

		public uint VFT { get; set; }

		public float Unknown_10h { get; set; }

		public float Unknown_14h { get; set; }

		public float Unknown_20h { get; set; }

		public float Unknown_24h { get; set; }

		public float Unknown_28h { get; set; }

		public float Unknown_2Ch { get; set; }

		public uint Flags { get; set; }

		public float Unknown_34h { get; set; }

		public float Unknown_38h { get; set; }

		public uint Unknown_3Ch { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			Unknown_10h = reader.ReadSingle();
			Unknown_14h = reader.ReadSingle();
			Unknown_18h = reader.ReadUInt64();
			Unknown_20h = reader.ReadSingle();
			Unknown_24h = reader.ReadSingle();
			Unknown_28h = reader.ReadSingle();
			Unknown_2Ch = reader.ReadSingle();
			Flags = reader.ReadUInt32();
			Unknown_34h = reader.ReadSingle();
			Unknown_38h = reader.ReadSingle();
			Unknown_3Ch = reader.ReadUInt32();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_24h);
			writer.Write(Unknown_28h);
			writer.Write(Unknown_2Ch);
			writer.Write(Flags);
			writer.Write(Unknown_34h);
			writer.Write(Unknown_38h);
			writer.Write(Unknown_3Ch);
		}
	}
}
