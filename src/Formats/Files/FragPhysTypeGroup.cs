using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysTypeGroup : ResourceSystemBlock
	{
		public float Unknown_00h;

		public float Unknown_04h;

		public float Unknown_08h;

		public float Unknown_0Ch;

		public float Unknown_48h;

		public float Unknown_7Ch;

		public float Unknown_ACh;

		public override long BlockLength => 176L;

		public float Strength { get; set; }

		public float ForceTransmissionScaleUp { get; set; }

		public float ForceTransmissionScaleDown { get; set; }

		public float JointStiffness { get; set; }

		public float MinSoftAngle1 { get; set; }

		public float MaxSoftAngle1 { get; set; }

		public float MaxSoftAngle2 { get; set; }

		public float MaxSoftAngle3 { get; set; }

		public float RotationSpeed { get; set; }

		public float RotationStrength { get; set; }

		public float RestoringStrength { get; set; }

		public float RestoringMaxTorque { get; set; }

		public float LatchStrength { get; set; }

		public float Mass { get; set; }

		public byte ChildGroupIndex { get; set; }

		public byte ParentIndex { get; set; }

		public byte ChildIndex { get; set; }

		public byte ChildCount { get; set; }

		public byte ChildGroupCount { get; set; }

		public byte UnkByte51 { get; set; } = byte.MaxValue;

		public byte GlassWindowIndex { get; set; }

		public byte GlassFlags { get; set; }

		public float MinDamageForce { get; set; }

		public float DamageHealth { get; set; }

		public float UnkFloat5C { get; set; }

		public float UnkFloat60 { get; set; }

		public float UnkFloat64 { get; set; }

		public float UnkFloat68 { get; set; }

		public float UnkFloat6C { get; set; }

		public float UnkFloat70 { get; set; }

		public float UnkFloat74 { get; set; }

		public float UnkFloat78 { get; set; }

		public FragPhysNameStruct_s Name { get; set; }

		public float UnkFloatA8 { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_00h = reader.ReadSingle();
			Unknown_04h = reader.ReadSingle();
			Unknown_08h = reader.ReadSingle();
			Unknown_0Ch = reader.ReadSingle();
			Strength = reader.ReadSingle();
			ForceTransmissionScaleUp = reader.ReadSingle();
			ForceTransmissionScaleDown = reader.ReadSingle();
			JointStiffness = reader.ReadSingle();
			MinSoftAngle1 = reader.ReadSingle();
			MaxSoftAngle1 = reader.ReadSingle();
			MaxSoftAngle2 = reader.ReadSingle();
			MaxSoftAngle3 = reader.ReadSingle();
			RotationSpeed = reader.ReadSingle();
			RotationStrength = reader.ReadSingle();
			RestoringStrength = reader.ReadSingle();
			RestoringMaxTorque = reader.ReadSingle();
			LatchStrength = reader.ReadSingle();
			Mass = reader.ReadSingle();
			Unknown_48h = reader.ReadSingle();
			ChildGroupIndex = reader.ReadByte();
			ParentIndex = reader.ReadByte();
			ChildIndex = reader.ReadByte();
			ChildCount = reader.ReadByte();
			ChildGroupCount = reader.ReadByte();
			UnkByte51 = reader.ReadByte();
			GlassWindowIndex = reader.ReadByte();
			GlassFlags = reader.ReadByte();
			MinDamageForce = reader.ReadSingle();
			DamageHealth = reader.ReadSingle();
			UnkFloat5C = reader.ReadSingle();
			UnkFloat60 = reader.ReadSingle();
			UnkFloat64 = reader.ReadSingle();
			UnkFloat68 = reader.ReadSingle();
			UnkFloat6C = reader.ReadSingle();
			UnkFloat70 = reader.ReadSingle();
			UnkFloat74 = reader.ReadSingle();
			UnkFloat78 = reader.ReadSingle();
			Unknown_7Ch = reader.ReadSingle();
			Name = reader.ReadStruct<FragPhysNameStruct_s>();
			UnkFloatA8 = reader.ReadSingle();
			Unknown_ACh = reader.ReadSingle();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Unknown_00h);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.Write(Unknown_0Ch);
			writer.Write(Strength);
			writer.Write(ForceTransmissionScaleUp);
			writer.Write(ForceTransmissionScaleDown);
			writer.Write(JointStiffness);
			writer.Write(MinSoftAngle1);
			writer.Write(MaxSoftAngle1);
			writer.Write(MaxSoftAngle2);
			writer.Write(MaxSoftAngle3);
			writer.Write(RotationSpeed);
			writer.Write(RotationStrength);
			writer.Write(RestoringStrength);
			writer.Write(RestoringMaxTorque);
			writer.Write(LatchStrength);
			writer.Write(Mass);
			writer.Write(Unknown_48h);
			writer.Write(ChildGroupIndex);
			writer.Write(ParentIndex);
			writer.Write(ChildIndex);
			writer.Write(ChildCount);
			writer.Write(ChildGroupCount);
			writer.Write(UnkByte51);
			writer.Write(GlassWindowIndex);
			writer.Write(GlassFlags);
			writer.Write(MinDamageForce);
			writer.Write(DamageHealth);
			writer.Write(UnkFloat5C);
			writer.Write(UnkFloat60);
			writer.Write(UnkFloat64);
			writer.Write(UnkFloat68);
			writer.Write(UnkFloat6C);
			writer.Write(UnkFloat70);
			writer.Write(UnkFloat74);
			writer.Write(UnkFloat78);
			writer.Write(Unknown_7Ch);
			writer.WriteStruct(Name);
			writer.Write(UnkFloatA8);
			writer.Write(Unknown_ACh);
		}

		public override string ToString()
		{
			return Name.ToString();
		}
	}
}
