using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysEvtSet : ResourceSystemBlock
	{
		public uint Unknown_04h = 1u;

		public ulong Unknown_08h;

		public ulong Unknown_10h;

		public ulong Unknown_18h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public override long BlockLength => 48L;

		public uint VFT { get; set; } = 1080060072u;

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			Unknown_08h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt64();
			Unknown_18h = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
		}
	}
}
