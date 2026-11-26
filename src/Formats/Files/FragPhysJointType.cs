using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysJointType : ResourceSystemBlock, IResourceXXSystemBlock, IResourceSystemBlock, IResourceBlock
	{
		public uint Unknown_04h = 1u;

		public ulong Unknown_08h;

		public float Unknown_10h = 0.825f;

		public byte Unknown_14h;

		public ulong Unknown_18h;

		public override long BlockLength => 32L;

		public uint VFT { get; set; }

		public FragJointType Type { get; set; }

		public byte FragIndex1 { get; set; }

		public byte FragIndex2 { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			Unknown_08h = reader.ReadUInt64();
			Unknown_10h = reader.ReadSingle();
			Unknown_14h = reader.ReadByte();
			Type = (FragJointType)reader.ReadByte();
			FragIndex1 = reader.ReadByte();
			FragIndex2 = reader.ReadByte();
			Unknown_18h = reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_14h);
			writer.Write((byte)Type);
			writer.Write(FragIndex1);
			writer.Write(FragIndex2);
			writer.Write(Unknown_18h);
		}

		public static FragPhysJointType Create(FragJointType type)
		{
			switch (type)
			{
			case FragJointType.DOF1:
				return new FragPhysJoint1DofType();
			case FragJointType.DOF3:
				return new FragPhysJoint3DofType();
			default:
				return null;
			}
		}

		public IResourceSystemBlock GetType(ResourceDataReader reader, params object[] parameters)
		{
			reader.Position += 21L;
			byte type = reader.ReadByte();
			reader.Position -= 22L;
			return Create((FragJointType)type);
		}
	}
}
