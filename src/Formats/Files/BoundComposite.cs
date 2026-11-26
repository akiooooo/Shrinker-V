using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundComposite : Bounds
	{
		private ResourceSystemStructBlock<Matrix4F_s> ChildrenTransformation1Block;

		private ResourceSystemStructBlock<Matrix4F_s> ChildrenTransformation2Block;

		private ResourceSystemStructBlock<AABB_s> ChildrenBoundingBoxesBlock;

		private ResourceSystemStructBlock<BoundCompositeChildrenFlags> ChildrenFlags1Block;

		private ResourceSystemStructBlock<BoundCompositeChildrenFlags> ChildrenFlags2Block;

		public override long BlockLength => 176L;

		public ulong ChildrenPointer { get; set; }

		public ulong ChildrenTransformation1Pointer { get; set; }

		public ulong ChildrenTransformation2Pointer { get; set; }

		public ulong ChildrenBoundingBoxesPointer { get; set; }

		public ulong ChildrenFlags1Pointer { get; set; }

		public ulong ChildrenFlags2Pointer { get; set; }

		public ushort ChildrenCount1 { get; set; }

		public ushort ChildrenCount2 { get; set; }

		public uint Unknown_A4h { get; set; }

		public ulong BVHPointer { get; set; }

		public ResourcePointerArray64<Bounds> Children { get; set; }

		public Matrix4F_s[] ChildrenTransformation1 { get; set; }

		public Matrix4F_s[] ChildrenTransformation2 { get; set; }

		public AABB_s[] ChildrenBoundingBoxes { get; set; }

		public BoundCompositeChildrenFlags[] ChildrenFlags1 { get; set; }

		public BoundCompositeChildrenFlags[] ChildrenFlags2 { get; set; }

		public BVH BVH { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			ChildrenPointer = reader.ReadUInt64();
			ChildrenTransformation1Pointer = reader.ReadUInt64();
			ChildrenTransformation2Pointer = reader.ReadUInt64();
			ChildrenBoundingBoxesPointer = reader.ReadUInt64();
			ChildrenFlags1Pointer = reader.ReadUInt64();
			ChildrenFlags2Pointer = reader.ReadUInt64();
			ChildrenCount1 = reader.ReadUInt16();
			ChildrenCount2 = reader.ReadUInt16();
			Unknown_A4h = reader.ReadUInt32();
			BVHPointer = reader.ReadUInt64();
			Children = reader.ReadBlockAt<ResourcePointerArray64<Bounds>>(ChildrenPointer, new object[1] { ChildrenCount1 });
			ChildrenTransformation1 = reader.ReadStructsAt<Matrix4F_s>(ChildrenTransformation1Pointer, ChildrenCount1);
			ChildrenTransformation2 = reader.ReadStructsAt<Matrix4F_s>(ChildrenTransformation2Pointer, ChildrenCount1);
			ChildrenBoundingBoxes = reader.ReadStructsAt<AABB_s>(ChildrenBoundingBoxesPointer, ChildrenCount1);
			ChildrenFlags1 = reader.ReadStructsAt<BoundCompositeChildrenFlags>(ChildrenFlags1Pointer, ChildrenCount1);
			ChildrenFlags2 = reader.ReadStructsAt<BoundCompositeChildrenFlags>(ChildrenFlags2Pointer, ChildrenCount1);
			BVH = reader.ReadBlockAt<BVH>(BVHPointer, Array.Empty<object>());
			Matrix4F_s[] array = ChildrenTransformation1 ?? ChildrenTransformation2;
			if (Children == null || Children.data_items == null)
			{
				return;
			}
			for (int i = 0; i < Children.data_items.Length; i++)
			{
				Bounds bounds = Children.data_items[i];
				if (bounds != null)
				{
					bounds.Parent = this;
					Matrix value = (bounds.Transform = ((array != null && i < array.Length) ? array[i].ToMatrix() : Matrix.Identity));
					bounds.TransformInv = Matrix.Invert(value);
					bounds.CompositeFlags1 = ((ChildrenFlags1 != null && i < ChildrenFlags1.Length) ? ChildrenFlags1[i] : default(BoundCompositeChildrenFlags));
					bounds.CompositeFlags2 = ((ChildrenFlags2 != null && i < ChildrenFlags2.Length) ? ChildrenFlags2[i] : default(BoundCompositeChildrenFlags));
				}
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			ChildrenPointer = (ulong)((Children != null) ? Children.FilePosition : 0);
			ChildrenTransformation1Pointer = (ulong)((ChildrenTransformation1Block != null) ? ChildrenTransformation1Block.FilePosition : 0);
			ChildrenTransformation2Pointer = ((ChildrenTransformation2Block != null) ? ((ulong)ChildrenTransformation2Block.FilePosition) : ChildrenTransformation1Pointer);
			ChildrenBoundingBoxesPointer = (ulong)((ChildrenBoundingBoxesBlock != null) ? ChildrenBoundingBoxesBlock.FilePosition : 0);
			ChildrenFlags1Pointer = (ulong)((ChildrenFlags1Block != null) ? ChildrenFlags1Block.FilePosition : 0);
			ChildrenFlags2Pointer = (ulong)((ChildrenFlags2Block != null) ? ChildrenFlags2Block.FilePosition : 0);
			ChildrenCount1 = (ushort)((Children != null) ? ((uint)Children.Count) : 0u);
			ChildrenCount2 = (ushort)((Children != null) ? ((uint)Children.Count) : 0u);
			BVHPointer = (ulong)((BVH != null) ? BVH.FilePosition : 0);
			writer.Write(ChildrenPointer);
			writer.Write(ChildrenTransformation1Pointer);
			writer.Write(ChildrenTransformation2Pointer);
			writer.Write(ChildrenBoundingBoxesPointer);
			writer.Write(ChildrenFlags1Pointer);
			writer.Write(ChildrenFlags2Pointer);
			writer.Write(ChildrenCount1);
			writer.Write(ChildrenCount2);
			writer.Write(Unknown_A4h);
			writer.Write(BVHPointer);
		}

		public override IResourceBlock[] GetReferences()
		{
			BuildBVH();
			UpdateChildrenFlags();
			UpdateChildrenBounds();
			UpdateChildrenTransformations();
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (Children != null)
			{
				list.Add(Children);
			}
			if (ChildrenTransformation1 != null)
			{
				ChildrenTransformation1Block = new ResourceSystemStructBlock<Matrix4F_s>(ChildrenTransformation1);
				list.Add(ChildrenTransformation1Block);
			}
			if (ChildrenTransformation2 != null)
			{
				ChildrenTransformation2Block = new ResourceSystemStructBlock<Matrix4F_s>(ChildrenTransformation2);
				list.Add(ChildrenTransformation2Block);
			}
			if (ChildrenBoundingBoxes != null)
			{
				ChildrenBoundingBoxesBlock = new ResourceSystemStructBlock<AABB_s>(ChildrenBoundingBoxes);
				list.Add(ChildrenBoundingBoxesBlock);
			}
			if (ChildrenFlags1 != null)
			{
				ChildrenFlags1Block = new ResourceSystemStructBlock<BoundCompositeChildrenFlags>(ChildrenFlags1);
				list.Add(ChildrenFlags1Block);
			}
			if (ChildrenFlags2 != null)
			{
				ChildrenFlags2Block = new ResourceSystemStructBlock<BoundCompositeChildrenFlags>(ChildrenFlags2);
				list.Add(ChildrenFlags2Block);
			}
			if (BVH != null)
			{
				list.Add(BVH);
			}
			return list.ToArray();
		}

		public void BuildBVH()
		{
			if (Children?.data_items == null)
			{
				BVH = null;
				return;
			}
			if (Children.data_items.Length <= 5)
			{
				_ = BVH;
				BVH = null;
				return;
			}
			if (BVH == null && !(base.Owner is FragPhysicsLOD) && !(base.Owner is FragPhysArchetype))
			{
				_ = base.Owner is VerletCloth;
			}
			if (base.Owner is FragPhysArchetype fragPhysArchetype && fragPhysArchetype == fragPhysArchetype.Owner?.Archetype2)
			{
				BVH = null;
				return;
			}
			List<BVHBuilderItem> list = new List<BVHBuilderItem>();
			for (int i = 0; i < Children.data_items.Length; i++)
			{
				Bounds bounds = Children.data_items[i];
				if (bounds != null)
				{
					BoundingBox boundingBox = new BoundingBox(bounds.BoxMin, bounds.BoxMax).Transform(bounds.Transform);
					BVHBuilderItem bVHBuilderItem = new BVHBuilderItem();
					bVHBuilderItem.Min = boundingBox.Minimum;
					bVHBuilderItem.Max = boundingBox.Maximum;
					bVHBuilderItem.Index = i;
					bVHBuilderItem.Bounds = bounds;
					list.Add(bVHBuilderItem);
				}
				else
				{
					list.Add(null);
				}
			}
			BVH = BVHBuilder.Build(list, 1);
		}

		public void UpdateChildrenFlags()
		{
			if (Children?.data_items == null)
			{
				ChildrenFlags1 = null;
				ChildrenFlags2 = null;
				return;
			}
			if (base.OwnerIsFragment)
			{
				ChildrenFlags1 = null;
				ChildrenFlags2 = null;
				return;
			}
			List<BoundCompositeChildrenFlags> list = new List<BoundCompositeChildrenFlags>();
			List<BoundCompositeChildrenFlags> list2 = new List<BoundCompositeChildrenFlags>();
			Bounds[] data_items = Children.data_items;
			foreach (Bounds bounds in data_items)
			{
				BoundCompositeChildrenFlags item = default(BoundCompositeChildrenFlags);
				BoundCompositeChildrenFlags item2 = default(BoundCompositeChildrenFlags);
				if (bounds != null)
				{
					item = bounds.CompositeFlags1;
					item2 = bounds.CompositeFlags2;
				}
				list.Add(item);
				list2.Add(item2);
			}
			ChildrenFlags1 = list.ToArray();
			ChildrenFlags2 = list2.ToArray();
		}

		public void UpdateChildrenBounds()
		{
			if (Children?.data_items == null)
			{
				ChildrenBoundingBoxes = null;
				return;
			}
			List<AABB_s> list = new List<AABB_s>();
			Bounds[] data_items = Children.data_items;
			foreach (Bounds bounds in data_items)
			{
				AABB_s item = default(AABB_s);
				if (bounds != null)
				{
					item.Min = new Vector4(bounds.BoxMin, float.Epsilon);
					item.Max = new Vector4(bounds.BoxMax, bounds.Margin);
				}
				list.Add(item);
			}
			ChildrenBoundingBoxes = list.ToArray();
		}

		public void UpdateChildrenTransformations()
		{
			if (Children?.data_items == null)
			{
				ChildrenTransformation1 = null;
				ChildrenTransformation2 = null;
				return;
			}
			List<Matrix4F_s> list = new List<Matrix4F_s>();
			Bounds[] data_items = Children.data_items;
			foreach (Bounds bounds in data_items)
			{
				Matrix4F_s item = Matrix4F_s.Identity;
				if (bounds != null)
				{
					item = new Matrix4F_s(bounds.Transform);
				}
				if (base.OwnerIsFragment)
				{
					item.Flags1 = 2139095041u;
					item.Flags2 = 2139095041u;
					item.Flags3 = 2139095041u;
					item.Flags4 = 2139095041u;
				}
				else
				{
					item.Flags1 = 0u;
					item.Flags2 = 1u;
					item.Flags3 = 1u;
					item.Flags4 = 0u;
				}
				list.Add(item);
			}
			ChildrenTransformation1 = list.ToArray();
			ChildrenTransformation2 = null;
		}
	}
}
