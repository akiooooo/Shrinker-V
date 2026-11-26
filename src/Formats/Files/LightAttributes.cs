using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class LightAttributes : ResourceSystemBlock
	{
		public override long BlockLength => 168L;

		public uint Unknown_0h { get; set; }

		public uint Unknown_4h { get; set; }

		public Vector3 Position { get; set; }

		public uint Unknown_14h { get; set; }

		public byte ColorR { get; set; }

		public byte ColorG { get; set; }

		public byte ColorB { get; set; }

		public byte Flashiness { get; set; }

		public float Intensity { get; set; }

		public uint Flags { get; set; }

		public ushort BoneId { get; set; }

		public LightType Type { get; set; }

		public byte GroupId { get; set; }

		public uint TimeFlags { get; set; }

		public float Falloff { get; set; }

		public float FalloffExponent { get; set; }

		public Vector3 CullingPlaneNormal { get; set; }

		public float CullingPlaneOffset { get; set; }

		public byte ShadowBlur { get; set; }

		public byte Unknown_45h { get; set; }

		public ushort Unknown_46h { get; set; }

		public uint Unknown_48h { get; set; }

		public float VolumeIntensity { get; set; }

		public float VolumeSizeScale { get; set; }

		public byte VolumeOuterColorR { get; set; }

		public byte VolumeOuterColorG { get; set; }

		public byte VolumeOuterColorB { get; set; }

		public byte LightHash { get; set; }

		public float VolumeOuterIntensity { get; set; }

		public float CoronaSize { get; set; }

		public float VolumeOuterExponent { get; set; }

		public byte LightFadeDistance { get; set; }

		public byte ShadowFadeDistance { get; set; }

		public byte SpecularFadeDistance { get; set; }

		public byte VolumetricFadeDistance { get; set; }

		public float ShadowNearClip { get; set; }

		public float CoronaIntensity { get; set; }

		public float CoronaZBias { get; set; }

		public Vector3 Direction { get; set; }

		public Vector3 Tangent { get; set; }

		public float ConeInnerAngle { get; set; }

		public float ConeOuterAngle { get; set; }

		public Vector3 Extent { get; set; }

		public MetaHash ProjectedTextureHash { get; set; }

		public uint Unknown_A4h { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_0h = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Position = reader.ReadVector3();
			Unknown_14h = reader.ReadUInt32();
			ColorR = reader.ReadByte();
			ColorG = reader.ReadByte();
			ColorB = reader.ReadByte();
			Flashiness = reader.ReadByte();
			Intensity = reader.ReadSingle();
			Flags = reader.ReadUInt32();
			BoneId = reader.ReadUInt16();
			Type = (LightType)reader.ReadByte();
			GroupId = reader.ReadByte();
			TimeFlags = reader.ReadUInt32();
			Falloff = reader.ReadSingle();
			FalloffExponent = reader.ReadSingle();
			CullingPlaneNormal = reader.ReadVector3();
			CullingPlaneOffset = reader.ReadSingle();
			ShadowBlur = reader.ReadByte();
			Unknown_45h = reader.ReadByte();
			Unknown_46h = reader.ReadUInt16();
			Unknown_48h = reader.ReadUInt32();
			VolumeIntensity = reader.ReadSingle();
			VolumeSizeScale = reader.ReadSingle();
			VolumeOuterColorR = reader.ReadByte();
			VolumeOuterColorG = reader.ReadByte();
			VolumeOuterColorB = reader.ReadByte();
			LightHash = reader.ReadByte();
			VolumeOuterIntensity = reader.ReadSingle();
			CoronaSize = reader.ReadSingle();
			VolumeOuterExponent = reader.ReadSingle();
			LightFadeDistance = reader.ReadByte();
			ShadowFadeDistance = reader.ReadByte();
			SpecularFadeDistance = reader.ReadByte();
			VolumetricFadeDistance = reader.ReadByte();
			ShadowNearClip = reader.ReadSingle();
			CoronaIntensity = reader.ReadSingle();
			CoronaZBias = reader.ReadSingle();
			Direction = reader.ReadVector3();
			Tangent = reader.ReadVector3();
			ConeInnerAngle = reader.ReadSingle();
			ConeOuterAngle = reader.ReadSingle();
			Extent = reader.ReadVector3();
			ProjectedTextureHash = new MetaHash(reader.ReadUInt32());
			Unknown_A4h = reader.ReadUInt32();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Unknown_0h);
			writer.Write(Unknown_4h);
			writer.Write(Position);
			writer.Write(Unknown_14h);
			writer.Write(ColorR);
			writer.Write(ColorG);
			writer.Write(ColorB);
			writer.Write(Flashiness);
			writer.Write(Intensity);
			writer.Write(Flags);
			writer.Write(BoneId);
			writer.Write((byte)Type);
			writer.Write(GroupId);
			writer.Write(TimeFlags);
			writer.Write(Falloff);
			writer.Write(FalloffExponent);
			writer.Write(CullingPlaneNormal);
			writer.Write(CullingPlaneOffset);
			writer.Write(ShadowBlur);
			writer.Write(Unknown_45h);
			writer.Write(Unknown_46h);
			writer.Write(Unknown_48h);
			writer.Write(VolumeIntensity);
			writer.Write(VolumeSizeScale);
			writer.Write(VolumeOuterColorR);
			writer.Write(VolumeOuterColorG);
			writer.Write(VolumeOuterColorB);
			writer.Write(LightHash);
			writer.Write(VolumeOuterIntensity);
			writer.Write(CoronaSize);
			writer.Write(VolumeOuterExponent);
			writer.Write(LightFadeDistance);
			writer.Write(ShadowFadeDistance);
			writer.Write(SpecularFadeDistance);
			writer.Write(VolumetricFadeDistance);
			writer.Write(ShadowNearClip);
			writer.Write(CoronaIntensity);
			writer.Write(CoronaZBias);
			writer.Write(Direction);
			writer.Write(Tangent);
			writer.Write(ConeInnerAngle);
			writer.Write(ConeOuterAngle);
			writer.Write(Extent);
			writer.Write(ProjectedTextureHash.Hash);
			writer.Write(Unknown_A4h);
		}
	}
}
