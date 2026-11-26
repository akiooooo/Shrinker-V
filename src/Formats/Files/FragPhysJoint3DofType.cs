using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysJoint3DofType : FragPhysJointType
	{
		public override long BlockLength => 240L;

		public Vector4 Unknown_20h { get; set; }

		public Vector4 Unknown_30h { get; set; }

		public Vector4 Unknown_40h { get; set; }

		public Vector4 Unknown_50h { get; set; }

		public Vector4 Unknown_60h { get; set; }

		public Vector4 Unknown_70h { get; set; }

		public Vector4 Unknown_80h { get; set; }

		public Vector4 Unknown_90h { get; set; }

		public Vector4 Unknown_A0h { get; set; }

		public Vector4 Unknown_B0h { get; set; }

		public Vector4 Unknown_C0h { get; set; } = new Vector4(100000000f);

		public Vector4 Unknown_D0h { get; set; } = new Vector4(-100000000f);

		public Vector4 Unknown_E0h { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_20h = reader.ReadVector4();
			Unknown_30h = reader.ReadVector4();
			Unknown_40h = reader.ReadVector4();
			Unknown_50h = reader.ReadVector4();
			Unknown_60h = reader.ReadVector4();
			Unknown_70h = reader.ReadVector4();
			Unknown_80h = reader.ReadVector4();
			Unknown_90h = reader.ReadVector4();
			Unknown_A0h = reader.ReadVector4();
			Unknown_B0h = reader.ReadVector4();
			Unknown_C0h = reader.ReadVector4();
			Unknown_D0h = reader.ReadVector4();
			Unknown_E0h = reader.ReadVector4();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_30h);
			writer.Write(Unknown_40h);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_60h);
			writer.Write(Unknown_70h);
			writer.Write(Unknown_80h);
			writer.Write(Unknown_90h);
			writer.Write(Unknown_A0h);
			writer.Write(Unknown_B0h);
			writer.Write(Unknown_C0h);
			writer.Write(Unknown_D0h);
			writer.Write(Unknown_E0h);
		}
	}
}
