using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ShaderParamInfosG9 : ResourceSystemBlock
	{
		public override long BlockLength => 8 + NumParams * 8;

		public byte NumBuffers { get; set; }

		public byte NumTextures { get; set; }

		public byte NumUnknowns { get; set; }

		public byte NumSamplers { get; set; }

		public byte NumParams { get; set; }

		public byte Unknown0 { get; set; }

		public byte Unknown1 { get; set; }

		public byte Unknown2 { get; set; } = 12;

		public ShaderParamInfoG9[] Params { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			NumBuffers = reader.ReadByte();
			NumTextures = reader.ReadByte();
			NumUnknowns = reader.ReadByte();
			NumSamplers = reader.ReadByte();
			NumParams = reader.ReadByte();
			Unknown0 = reader.ReadByte();
			Unknown1 = reader.ReadByte();
			Unknown2 = reader.ReadByte();
			Params = reader.ReadStructs<ShaderParamInfoG9>(NumParams);
			_ = Unknown0;
			_ = Unknown1;
			_ = Unknown2;
			_ = 12;
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(NumBuffers);
			writer.Write(NumTextures);
			writer.Write(NumUnknowns);
			writer.Write(NumSamplers);
			writer.Write(NumParams);
			writer.Write(Unknown0);
			writer.Write(Unknown1);
			writer.Write(Unknown2);
			writer.WriteStructs(Params);
		}
	}
}
