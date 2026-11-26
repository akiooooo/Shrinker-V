using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Skeleton : ResourceSystemBlock
	{
		public ulong Unknown_8h;

		public ulong Unknown_48h;

		public ushort Unknown_62h;

		public uint Unknown_64h;

		public ulong Unknown_68h;

		private ResourceSystemStructBlock<Matrix> TransformationsInvertedBlock;

		private ResourceSystemStructBlock<Matrix> TransformationsBlock;

		private ResourceSystemStructBlock<short> ParentIndicesBlock;

		private ResourceSystemStructBlock<short> ChildIndicesBlock;

		[CompilerGenerated]
		private Bone[] _003CBonesSorted_003Ek__BackingField;

		public override long BlockLength => 112L;

		public uint VFT { get; set; } = 1080114336u;

		public uint Unknown_4h { get; set; } = 1u;

		public ulong BoneTagsPointer { get; set; }

		public ushort BoneTagsCapacity { get; set; }

		public ushort BoneTagsCount { get; set; }

		public FlagsUint Unknown_1Ch { get; set; }

		public ulong BonesPointer { get; set; }

		public ulong TransformationsInvertedPointer { get; set; }

		public ulong TransformationsPointer { get; set; }

		public ulong ParentIndicesPointer { get; set; }

		public ulong ChildIndicesPointer { get; set; }

		public MetaHash Unknown_50h { get; set; }

		public MetaHash Unknown_54h { get; set; }

		public MetaHash Unknown_58h { get; set; }

		public ushort Unknown_5Ch { get; set; } = 1;

		public ushort BonesCount { get; set; }

		public ushort ChildIndicesCount { get; set; }

		public ResourcePointerArray64<SkeletonBoneTag> BoneTags { get; set; }

		public SkeletonBonesBlock Bones { get; set; }

		public Matrix[] TransformationsInverted { get; set; }

		public Matrix[] Transformations { get; set; }

		public short[] ParentIndices { get; set; }

		public short[] ChildIndices { get; set; }

		public Dictionary<ushort, Bone> BonesMap { get; set; }

		public Bone[] BonesSorted
		{
			[CompilerGenerated]
			set
			{
				_003CBonesSorted_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			BoneTagsPointer = reader.ReadUInt64();
			BoneTagsCapacity = reader.ReadUInt16();
			BoneTagsCount = reader.ReadUInt16();
			Unknown_1Ch = reader.ReadUInt32();
			BonesPointer = reader.ReadUInt64();
			TransformationsInvertedPointer = reader.ReadUInt64();
			TransformationsPointer = reader.ReadUInt64();
			ParentIndicesPointer = reader.ReadUInt64();
			ChildIndicesPointer = reader.ReadUInt64();
			Unknown_48h = reader.ReadUInt64();
			Unknown_50h = new MetaHash(reader.ReadUInt32());
			Unknown_54h = new MetaHash(reader.ReadUInt32());
			Unknown_58h = new MetaHash(reader.ReadUInt32());
			Unknown_5Ch = reader.ReadUInt16();
			BonesCount = reader.ReadUInt16();
			ChildIndicesCount = reader.ReadUInt16();
			Unknown_62h = reader.ReadUInt16();
			Unknown_64h = reader.ReadUInt32();
			Unknown_68h = reader.ReadUInt64();
			BoneTags = reader.ReadBlockAt<ResourcePointerArray64<SkeletonBoneTag>>(BoneTagsPointer, new object[1] { BoneTagsCapacity });
			Bones = reader.ReadBlockAt<SkeletonBonesBlock>((BonesPointer != 0L) ? (BonesPointer - 16) : 0, new object[1] { (uint)BonesCount });
			TransformationsInverted = reader.ReadStructsAt<Matrix>(TransformationsInvertedPointer, BonesCount);
			Transformations = reader.ReadStructsAt<Matrix>(TransformationsPointer, BonesCount);
			ParentIndices = reader.ReadShortsAt(ParentIndicesPointer, BonesCount);
			ChildIndices = reader.ReadShortsAt(ChildIndicesPointer, ChildIndicesCount);
			AssignBoneParents();
			BuildBonesMap();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			BoneTagsPointer = (ulong)((BoneTags != null) ? BoneTags.FilePosition : 0);
			BoneTagsCapacity = (ushort)((BoneTags != null) ? ((uint)BoneTags.Count) : 0u);
			BonesPointer = (ulong)((Bones != null) ? (Bones.FilePosition + 16) : 0);
			TransformationsInvertedPointer = (ulong)((TransformationsInvertedBlock != null) ? TransformationsInvertedBlock.FilePosition : 0);
			TransformationsPointer = (ulong)((TransformationsBlock != null) ? TransformationsBlock.FilePosition : 0);
			ParentIndicesPointer = (ulong)((ParentIndicesBlock != null) ? ParentIndicesBlock.FilePosition : 0);
			ChildIndicesPointer = (ulong)((ChildIndicesBlock != null) ? ChildIndicesBlock.FilePosition : 0);
			BonesCount = (ushort)((Bones?.Items != null) ? ((uint)Bones.Items.Length) : 0u);
			ChildIndicesCount = (ushort)((ChildIndicesBlock != null) ? ((uint)ChildIndicesBlock.ItemCount) : 0u);
			BoneTagsCount = Math.Min(BonesCount, BoneTagsCapacity);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(BoneTagsPointer);
			writer.Write(BoneTagsCapacity);
			writer.Write(BoneTagsCount);
			writer.Write(Unknown_1Ch);
			writer.Write(BonesPointer);
			writer.Write(TransformationsInvertedPointer);
			writer.Write(TransformationsPointer);
			writer.Write(ParentIndicesPointer);
			writer.Write(ChildIndicesPointer);
			writer.Write(Unknown_48h);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_54h);
			writer.Write(Unknown_58h);
			writer.Write(Unknown_5Ch);
			writer.Write(BonesCount);
			writer.Write(ChildIndicesCount);
			writer.Write(Unknown_62h);
			writer.Write(Unknown_64h);
			writer.Write(Unknown_68h);
		}

		public override IResourceBlock[] GetReferences()
		{
			BuildTransformations();
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (BoneTags != null)
			{
				list.Add(BoneTags);
			}
			if (Bones != null)
			{
				list.Add(Bones);
			}
			if (TransformationsInverted != null)
			{
				TransformationsInvertedBlock = new ResourceSystemStructBlock<Matrix>(TransformationsInverted);
				list.Add(TransformationsInvertedBlock);
			}
			if (Transformations != null)
			{
				TransformationsBlock = new ResourceSystemStructBlock<Matrix>(Transformations);
				list.Add(TransformationsBlock);
			}
			if (ParentIndices != null)
			{
				ParentIndicesBlock = new ResourceSystemStructBlock<short>(ParentIndices);
				list.Add(ParentIndicesBlock);
			}
			if (ChildIndices != null)
			{
				ChildIndicesBlock = new ResourceSystemStructBlock<short>(ChildIndices);
				list.Add(ChildIndicesBlock);
			}
			return list.ToArray();
		}

		public void AssignBoneParents()
		{
			if (Bones?.Items == null || ParentIndices == null)
			{
				return;
			}
			int num = Math.Min(Bones.Items.Length, ParentIndices.Length);
			for (int i = 0; i < num; i++)
			{
				Bone bone = Bones.Items[i];
				short num2 = ParentIndices[i];
				if (num2 >= 0 && num2 < Bones.Items.Length)
				{
					bone.Parent = Bones.Items[num2];
				}
			}
		}

		public void BuildBonesMap()
		{
			BonesMap = new Dictionary<ushort, Bone>();
			if (Bones?.Items != null)
			{
				List<Bone> list = new List<Bone>();
				for (int i = 0; i < Bones.Items.Length; i++)
				{
					Bone bone = Bones.Items[i];
					BonesMap[bone.Tag] = bone;
					list.Add(bone);
					bone.UpdateAnimTransform();
					bone.AbsTransform = bone.AnimTransform;
					int num = i;
					Matrix[] transformationsInverted = TransformationsInverted;
					bone.BindTransformInv = ((num < ((transformationsInverted != null) ? transformationsInverted.Length : 0)) ? TransformationsInverted[i] : Matrix.Invert(bone.AnimTransform));
					bone.BindTransformInv.M44 = 1f;
					bone.UpdateSkinTransform();
					int num2 = i;
					Matrix[] transformations = Transformations;
					bone.TransformUnk = ((num2 < ((transformations != null) ? transformations.Length : 0)) ? Transformations[i].Column4 : Vector4.Zero);
				}
				list.Sort((Bone a, Bone b) => a.Index.CompareTo(b.Index));
				BonesSorted = list.ToArray();
			}
		}

		public void BuildTransformations()
		{
			List<Matrix> list = new List<Matrix>();
			List<Matrix> list2 = new List<Matrix>();
			if (Bones?.Items != null)
			{
				Bone[] items = Bones.Items;
				foreach (Bone bone in items)
				{
					Vector3 vector = bone.Translation;
					Quaternion quaternion = bone.Rotation;
					Vector3 scale = bone.Scale;
					Matrix item = Matrix.AffineTransformation(1f, quaternion, vector);
					item.ScaleVector *= scale;
					item.Column4 = bone.TransformUnk;
					for (Bone parent = bone.Parent; parent != null; parent = parent.Parent)
					{
						vector = parent.Rotation.Multiply(vector) + parent.Translation;
						quaternion = parent.Rotation * quaternion;
					}
					Matrix item2 = Matrix.Invert(Matrix.AffineTransformation(1f, quaternion, vector));
					item2.Column4 = Vector4.Zero;
					list.Add(item);
					list2.Add(item2);
				}
			}
			Transformations = ((list.Count > 0) ? list.ToArray() : null);
			TransformationsInverted = ((list2.Count > 0) ? list2.ToArray() : null);
		}
	}
}
