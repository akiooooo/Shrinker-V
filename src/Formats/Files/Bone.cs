using System.Collections.Generic;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Bone : ResourceSystemBlock
	{
		public uint Unknown_1Ch;

		public uint Unknown_34h;

		public ulong Unknown_48h;

		private string_r NameBlock;

		public Quaternion AnimRotation;

		public Vector3 AnimTranslation;

		public Vector3 AnimScale;

		public Matrix AnimTransform;

		public Matrix BindTransformInv;

		public Matrix SkinTransform;

		public Matrix AbsTransform;

		public override long BlockLength => 80L;

		public Quaternion Rotation { get; set; }

		public Vector3 Translation { get; set; }

		public Vector3 Scale { get; set; }

		public float Unknown_2Ch { get; set; } = 1f;

		public short NextSiblingIndex { get; set; }

		public short ParentIndex { get; set; }

		public ulong NamePointer { get; set; }

		public EBoneFlags Flags { get; set; }

		public short Index { get; set; }

		public ushort Tag { get; set; }

		public short Index2 { get; set; }

		public string Name { get; set; }

		public Bone Parent { get; set; }

		public Vector4 TransformUnk { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Rotation = new Quaternion(reader.ReadVector4());
			Translation = reader.ReadVector3();
			Unknown_1Ch = reader.ReadUInt32();
			Scale = reader.ReadVector3();
			Unknown_2Ch = reader.ReadSingle();
			NextSiblingIndex = reader.ReadInt16();
			ParentIndex = reader.ReadInt16();
			Unknown_34h = reader.ReadUInt32();
			NamePointer = reader.ReadUInt64();
			Flags = (EBoneFlags)reader.ReadUInt16();
			Index = reader.ReadInt16();
			Tag = reader.ReadUInt16();
			Index2 = reader.ReadInt16();
			Unknown_48h = reader.ReadUInt64();
			Name = reader.ReadStringAt(NamePointer);
			AnimRotation = Rotation;
			AnimTranslation = Translation;
			AnimScale = Scale;
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			NamePointer = (ulong)((NameBlock != null) ? NameBlock.FilePosition : 0);
			writer.Write(Rotation.ToVector4());
			writer.Write(Translation);
			writer.Write(Unknown_1Ch);
			writer.Write(Scale);
			writer.Write(Unknown_2Ch);
			writer.Write(NextSiblingIndex);
			writer.Write(ParentIndex);
			writer.Write(Unknown_34h);
			writer.Write(NamePointer);
			writer.Write((ushort)Flags);
			writer.Write(Index);
			writer.Write(Tag);
			writer.Write(Index2);
			writer.Write(Unknown_48h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Name != null)
			{
				NameBlock = (string_r)Name;
				list.Add(NameBlock);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return Tag + ": " + Name;
		}

		public void UpdateAnimTransform()
		{
			AnimTransform = Matrix.AffineTransformation(1f, AnimRotation, AnimTranslation);
			AnimTransform.ScaleVector *= AnimScale;
			if (Parent != null)
			{
				AnimTransform *= Parent.AnimTransform;
			}
		}

		public void UpdateSkinTransform()
		{
			SkinTransform = BindTransformInv * AnimTransform;
		}
	}
}
