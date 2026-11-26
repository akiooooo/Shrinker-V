using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Bounds : ResourceFileBase, IResourceXXSystemBlock, IResourceSystemBlock, IResourceBlock
	{
		[CompilerGenerated]
		private Matrix _003CTransformInv_003Ek__BackingField = Matrix.Identity;

		public override long BlockLength => 112L;

		public BoundsType Type { get; set; }

		public byte Unknown_11h { get; set; }

		public ushort Unknown_12h { get; set; }

		public float SphereRadius { get; set; }

		public uint Unknown_18h { get; set; }

		public uint Unknown_1Ch { get; set; }

		public Vector3 BoxMax { get; set; }

		public float Margin { get; set; }

		public Vector3 BoxMin { get; set; }

		public uint Unknown_3Ch { get; set; } = 1u;

		public Vector3 BoxCenter { get; set; }

		public byte MaterialIndex { get; set; }

		public byte ProceduralId { get; set; }

		public byte RoomId_and_PedDensity { get; set; }

		public byte UnkFlags { get; set; }

		public Vector3 SphereCenter { get; set; }

		public byte PolyFlags { get; set; }

		public byte MaterialColorIndex { get; set; }

		public ushort Unknown_5Eh { get; set; }

		public Vector3 Unknown_60h { get; set; }

		public float Volume { get; set; }

		public BoundComposite Parent { get; set; }

		public object Owner { get; set; }

		public bool OwnerIsFragment
		{
			get
			{
				if (!(Owner is FragPhysicsLOD) && !(Owner is FragPhysArchetype))
				{
					return Owner is FragDrawable;
				}
				return true;
			}
		}

		public Matrix Transform { get; set; } = Matrix.Identity;

		public Matrix TransformInv
		{
			[CompilerGenerated]
			set
			{
				_003CTransformInv_003Ek__BackingField = value;
			}
		}

		public BoundCompositeChildrenFlags CompositeFlags1 { get; set; }

		public BoundCompositeChildrenFlags CompositeFlags2 { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Type = (BoundsType)reader.ReadByte();
			Unknown_11h = reader.ReadByte();
			Unknown_12h = reader.ReadUInt16();
			SphereRadius = reader.ReadSingle();
			Unknown_18h = reader.ReadUInt32();
			Unknown_1Ch = reader.ReadUInt32();
			BoxMax = reader.ReadVector3();
			Margin = reader.ReadSingle();
			BoxMin = reader.ReadVector3();
			Unknown_3Ch = reader.ReadUInt32();
			BoxCenter = reader.ReadVector3();
			MaterialIndex = reader.ReadByte();
			ProceduralId = reader.ReadByte();
			RoomId_and_PedDensity = reader.ReadByte();
			UnkFlags = reader.ReadByte();
			SphereCenter = reader.ReadVector3();
			PolyFlags = reader.ReadByte();
			MaterialColorIndex = reader.ReadByte();
			Unknown_5Eh = reader.ReadUInt16();
			Unknown_60h = reader.ReadVector3();
			Volume = reader.ReadSingle();
			_ = Unknown_11h;
			_ = Unknown_12h;
			_ = Unknown_18h;
			_ = Unknown_1Ch;
			_ = Unknown_5Eh;
			uint unknown_3Ch = Unknown_3Ch;
			_ = 2;
			switch (UnkFlags)
			{
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
			case 37:
			case 38:
			case 39:
			case 40:
			case 41:
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
			case 47:
			case 48:
				_ = 48;
				break;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			writer.Write((byte)Type);
			writer.Write(Unknown_11h);
			writer.Write(Unknown_12h);
			writer.Write(SphereRadius);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_1Ch);
			writer.Write(BoxMax);
			writer.Write(Margin);
			writer.Write(BoxMin);
			writer.Write(Unknown_3Ch);
			writer.Write(BoxCenter);
			writer.Write(MaterialIndex);
			writer.Write(ProceduralId);
			writer.Write(RoomId_and_PedDensity);
			writer.Write(UnkFlags);
			writer.Write(SphereCenter);
			writer.Write(PolyFlags);
			writer.Write(MaterialColorIndex);
			writer.Write(Unknown_5Eh);
			writer.Write(Unknown_60h);
			writer.Write(Volume);
		}

		public IResourceSystemBlock GetType(ResourceDataReader reader, params object[] parameters)
		{
			reader.Position += 16L;
			byte type = reader.ReadByte();
			reader.Position -= 17L;
			return Create((BoundsType)type);
		}

		public static Bounds Create(BoundsType type)
		{
			switch (type)
			{
			default:
				_ = 255;
				return null;
			case BoundsType.Sphere:
				return new BoundSphere();
			case BoundsType.Capsule:
				return new BoundCapsule();
			case BoundsType.Box:
				return new BoundBox();
			case BoundsType.Geometry:
				return new BoundGeometry();
			case BoundsType.GeometryBVH:
				return new BoundBVH();
			case BoundsType.Composite:
				return new BoundComposite();
			case BoundsType.Disc:
				return new BoundDisc();
			case BoundsType.Cylinder:
				return new BoundCylinder();
			case BoundsType.Cloth:
				return new BoundCloth();
			case (BoundsType)2:
			case (BoundsType)5:
			case (BoundsType)6:
			case (BoundsType)7:
			case (BoundsType)9:
			case (BoundsType)11:
			case (BoundsType)14:
				return null;
			}
		}
	}
}
