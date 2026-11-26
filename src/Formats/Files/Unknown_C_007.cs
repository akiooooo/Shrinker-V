using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Unknown_C_007 : ResourceSystemBlock
	{
		public ulong Unknown_0h;

		public ulong Unknown_8h;

		public override long BlockLength => 16L;

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_0h = reader.ReadUInt64();
			Unknown_8h = reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Unknown_0h);
			writer.Write(Unknown_8h);
		}

		public override string ToString()
		{
			return Unknown_0h + ", " + Unknown_8h;
		}
	}
}
