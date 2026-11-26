using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundDisc : Bounds
	{
		public override long BlockLength => 128L;

		public uint Unknown_70h { get; set; }

		public uint Unknown_74h { get; set; }

		public uint Unknown_78h { get; set; }

		public uint Unknown_7Ch { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_70h = reader.ReadUInt32();
			Unknown_74h = reader.ReadUInt32();
			Unknown_78h = reader.ReadUInt32();
			Unknown_7Ch = reader.ReadUInt32();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			writer.Write(Unknown_70h);
			writer.Write(Unknown_74h);
			writer.Write(Unknown_78h);
			writer.Write(Unknown_7Ch);
		}
	}
}
