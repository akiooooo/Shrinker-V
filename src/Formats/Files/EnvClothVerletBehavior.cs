using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class EnvClothVerletBehavior : ResourceSystemBlock
	{
		public ulong Unknown_0h;

		public ulong Unknown_8h;

		public ulong Unknown_10h;

		public ulong Unknown_18h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public ulong Unknown_30h;

		public ulong Unknown_38h;

		public override long BlockLength => 64L;

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_0h = reader.ReadUInt64();
			Unknown_8h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt64();
			Unknown_18h = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			Unknown_30h = reader.ReadUInt64();
			Unknown_38h = reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Unknown_0h);
			writer.Write(Unknown_8h);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
			writer.Write(Unknown_30h);
			writer.Write(Unknown_38h);
		}
	}
}
