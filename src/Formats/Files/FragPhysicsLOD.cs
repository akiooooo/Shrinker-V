using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysicsLOD : ResourceSystemBlock
	{
		public uint Unknown_04h = 1u;

		public ulong Unknown_08h;

		public uint Unknown_10h;

		public uint Unknown_5Ch = 2139095041u;

		public uint Unknown_6Ch = 2139095041u;

		public uint Unknown_7Ch = 2139095041u;

		public uint Unknown_8Ch = 2139095041u;

		public uint Unknown_9Ch = 2139095041u;

		public uint Unknown_ACh = 2139095041u;

		public uint Unknown_BCh = 2139095041u;

		public byte Unknown_11Ch = 1;

		public byte Unknown_11Fh;

		public ulong Unknown_120h;

		public ulong Unknown_128h;

		private ResourceSystemStructBlock<float> ChildrenUnkFloatsBlock;

		private ResourceSystemStructBlock<Vector4> ChildrenInertiaTensorsBlock;

		private ResourceSystemStructBlock<Vector4> ChildrenUnkVecsBlock;

		private ResourceSystemStructBlock<byte> UnknownData1Block;

		private ResourceSystemStructBlock<byte> UnknownData2Block;

		public override long BlockLength => 304L;

		public uint VFT { get; set; } = 1080055512u;

		public float Unknown_14h { get; set; }

		public float Unknown_18h { get; set; }

		public float Unknown_1Ch { get; set; }

		public ulong ArticulatedBodyTypePointer { get; set; }

		public ulong ChildrenUnkFloatsPointer { get; set; }

		public Vector3 PositionOffset { get; set; }

		public uint Unknown_3Ch { get; set; } = 2143289345u;

		public Vector3 Unknown_40h { get; set; }

		public uint Unknown_4Ch { get; set; } = 2143289345u;

		public Vector3 Unknown_50h { get; set; }

		public Vector3 DampingLinearC { get; set; }

		public Vector3 DampingLinearV { get; set; }

		public Vector3 DampingLinearV2 { get; set; }

		public Vector3 DampingAngularC { get; set; }

		public Vector3 DampingAngularV { get; set; }

		public Vector3 DampingAngularV2 { get; set; }

		public ulong GroupNamesPointer { get; set; }

		public ulong GroupsPointer { get; set; }

		public ulong ChildrenPointer { get; set; }

		public ulong Archetype1Pointer { get; set; }

		public ulong Archetype2Pointer { get; set; }

		public ulong BoundPointer { get; set; }

		public ulong ChildrenInertiaTensorsPointer { get; set; }

		public ulong ChildrenUnkVecsPointer { get; set; }

		public ulong FragTransformsPointer { get; set; }

		public ulong UnknownData1Pointer { get; set; }

		public ulong UnknownData2Pointer { get; set; }

		public byte UnknownData1Count { get; set; }

		public byte UnknownData2Count { get; set; }

		public byte GroupsCount { get; set; }

		public byte RootGroupsCount { get; set; }

		public byte ChildrenCount { get; set; }

		public byte ChildrenCount2 { get; set; }

		public FragPhysArticulatedBodyType ArticulatedBodyType { get; set; }

		public float[] ChildrenUnkFloats { get; set; }

		public FragPhysGroupNamesBlock GroupNames { get; set; }

		public ResourcePointerArray64<FragPhysTypeGroup> Groups { get; set; }

		public ResourcePointerArray64<FragPhysTypeChild> Children { get; set; }

		public FragPhysArchetype Archetype1 { get; set; }

		public FragPhysArchetype Archetype2 { get; set; }

		public Bounds Bound { get; set; }

		public Vector4[] ChildrenInertiaTensors { get; set; }

		public Vector4[] ChildrenUnkVecs { get; set; }

		public FragPhysTransforms FragTransforms { get; set; }

		public byte[] UnknownData1 { get; set; }

		public byte[] UnknownData2 { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			Unknown_08h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt32();
			Unknown_14h = reader.ReadSingle();
			Unknown_18h = reader.ReadSingle();
			Unknown_1Ch = reader.ReadSingle();
			ArticulatedBodyTypePointer = reader.ReadUInt64();
			ChildrenUnkFloatsPointer = reader.ReadUInt64();
			PositionOffset = reader.ReadVector3();
			Unknown_3Ch = reader.ReadUInt32();
			Unknown_40h = reader.ReadVector3();
			Unknown_4Ch = reader.ReadUInt32();
			Unknown_50h = reader.ReadVector3();
			Unknown_5Ch = reader.ReadUInt32();
			DampingLinearC = reader.ReadVector3();
			Unknown_6Ch = reader.ReadUInt32();
			DampingLinearV = reader.ReadVector3();
			Unknown_7Ch = reader.ReadUInt32();
			DampingLinearV2 = reader.ReadVector3();
			Unknown_8Ch = reader.ReadUInt32();
			DampingAngularC = reader.ReadVector3();
			Unknown_9Ch = reader.ReadUInt32();
			DampingAngularV = reader.ReadVector3();
			Unknown_ACh = reader.ReadUInt32();
			DampingAngularV2 = reader.ReadVector3();
			Unknown_BCh = reader.ReadUInt32();
			GroupNamesPointer = reader.ReadUInt64();
			GroupsPointer = reader.ReadUInt64();
			ChildrenPointer = reader.ReadUInt64();
			Archetype1Pointer = reader.ReadUInt64();
			Archetype2Pointer = reader.ReadUInt64();
			BoundPointer = reader.ReadUInt64();
			ChildrenInertiaTensorsPointer = reader.ReadUInt64();
			ChildrenUnkVecsPointer = reader.ReadUInt64();
			FragTransformsPointer = reader.ReadUInt64();
			UnknownData1Pointer = reader.ReadUInt64();
			UnknownData2Pointer = reader.ReadUInt64();
			UnknownData1Count = reader.ReadByte();
			UnknownData2Count = reader.ReadByte();
			GroupsCount = reader.ReadByte();
			RootGroupsCount = reader.ReadByte();
			Unknown_11Ch = reader.ReadByte();
			ChildrenCount = reader.ReadByte();
			ChildrenCount2 = reader.ReadByte();
			Unknown_11Fh = reader.ReadByte();
			Unknown_120h = reader.ReadUInt64();
			Unknown_128h = reader.ReadUInt64();
			ArticulatedBodyType = reader.ReadBlockAt<FragPhysArticulatedBodyType>(ArticulatedBodyTypePointer, Array.Empty<object>());
			ChildrenUnkFloats = reader.ReadFloatsAt(ChildrenUnkFloatsPointer, ChildrenCount);
			Groups = reader.ReadBlockAt<ResourcePointerArray64<FragPhysTypeGroup>>(GroupsPointer, new object[1] { GroupsCount });
			GroupNames = reader.ReadBlockAt<FragPhysGroupNamesBlock>(GroupNamesPointer, new object[2]
			{
				GroupsCount,
				Groups?.data_items
			});
			Children = reader.ReadBlockAt<ResourcePointerArray64<FragPhysTypeChild>>(ChildrenPointer, new object[1] { ChildrenCount });
			Archetype1 = reader.ReadBlockAt<FragPhysArchetype>(Archetype1Pointer, Array.Empty<object>());
			Archetype2 = reader.ReadBlockAt<FragPhysArchetype>(Archetype2Pointer, Array.Empty<object>());
			Bound = reader.ReadBlockAt<Bounds>(BoundPointer, Array.Empty<object>());
			ChildrenInertiaTensors = reader.ReadStructsAt<Vector4>(ChildrenInertiaTensorsPointer, ChildrenCount);
			ChildrenUnkVecs = reader.ReadStructsAt<Vector4>(ChildrenUnkVecsPointer, ChildrenCount);
			FragTransforms = reader.ReadBlockAt<FragPhysTransforms>(FragTransformsPointer, Array.Empty<object>());
			UnknownData1 = reader.ReadBytesAt(UnknownData1Pointer, UnknownData1Count);
			UnknownData2 = reader.ReadBytesAt(UnknownData2Pointer, UnknownData2Count);
			if (Children != null && Children.data_items != null)
			{
				for (int i = 0; i < Children.data_items.Length; i++)
				{
					FragPhysTypeChild fragPhysTypeChild = Children.data_items[i];
					fragPhysTypeChild.OwnerFragPhysLod = this;
					fragPhysTypeChild.OwnerFragPhysIndex = i;
					fragPhysTypeChild.UnkFloatFromParent = ((ChildrenUnkFloats != null && i < ChildrenUnkFloats.Length) ? ChildrenUnkFloats[i] : 0f);
					fragPhysTypeChild.UnkVecFromParent = ((ChildrenUnkVecs != null && i < ChildrenUnkVecs.Length) ? ChildrenUnkVecs[i] : Vector4.Zero);
					fragPhysTypeChild.InertiaTensorFromParent = ((ChildrenInertiaTensors != null && i < ChildrenInertiaTensors.Length) ? ChildrenInertiaTensors[i] : Vector4.Zero);
					ushort groupIndex = fragPhysTypeChild.GroupIndex;
					if (Groups?.data_items != null && groupIndex < Groups.data_items.Length)
					{
						FragPhysTypeGroup fragPhysTypeGroup = Groups.data_items[groupIndex];
						fragPhysTypeChild.Group = fragPhysTypeGroup;
					}
				}
			}
			if (Groups?.data_items != null && GroupNames?.data_items != null && Groups.data_items.Length == GroupNames.data_items.Length)
			{
				for (int j = 0; j < Groups.data_items.Length; j++)
				{
					Groups.data_items[j].Name = GroupNames.data_items[j];
				}
			}
			if (Archetype1 != null)
			{
				Archetype1.Owner = this;
			}
			if (Archetype2 != null)
			{
				Archetype2.Owner = this;
			}
			if (Bound != null)
			{
				Bound.Owner = this;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			ArticulatedBodyTypePointer = (ulong)((ArticulatedBodyType != null) ? ArticulatedBodyType.FilePosition : 0);
			ChildrenUnkFloatsPointer = (ulong)((ChildrenUnkFloatsBlock != null) ? ChildrenUnkFloatsBlock.FilePosition : 0);
			GroupNamesPointer = (ulong)((GroupNames != null) ? GroupNames.FilePosition : 0);
			GroupsPointer = (ulong)((Groups != null) ? Groups.FilePosition : 0);
			ChildrenPointer = (ulong)((Children != null) ? Children.FilePosition : 0);
			Archetype1Pointer = (ulong)((Archetype1 != null) ? Archetype1.FilePosition : 0);
			Archetype2Pointer = (ulong)((Archetype2 != null) ? Archetype2.FilePosition : 0);
			BoundPointer = (ulong)((Bound != null) ? Bound.FilePosition : 0);
			ChildrenInertiaTensorsPointer = (ulong)((ChildrenInertiaTensorsBlock != null) ? ChildrenInertiaTensorsBlock.FilePosition : 0);
			ChildrenUnkVecsPointer = (ulong)((ChildrenUnkVecsBlock != null) ? ChildrenUnkVecsBlock.FilePosition : 0);
			FragTransformsPointer = (ulong)((FragTransforms != null) ? FragTransforms.FilePosition : 0);
			UnknownData1Pointer = (ulong)((UnknownData1Block != null) ? UnknownData1Block.FilePosition : 0);
			UnknownData2Pointer = (ulong)((UnknownData2Block != null) ? UnknownData2Block.FilePosition : 0);
			UnknownData1Count = (byte)((UnknownData1Block != null) ? ((uint)UnknownData1Block.ItemCount) : 0u);
			UnknownData2Count = (byte)((UnknownData2Block != null) ? ((uint)UnknownData2Block.ItemCount) : 0u);
			GroupsCount = (byte)((Groups != null) ? ((uint)Groups.Count) : 0u);
			ChildrenCount = (byte)((Children != null) ? ((uint)Children.Count) : 0u);
			ChildrenCount2 = ChildrenCount;
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_1Ch);
			writer.Write(ArticulatedBodyTypePointer);
			writer.Write(ChildrenUnkFloatsPointer);
			writer.Write(PositionOffset);
			writer.Write(Unknown_3Ch);
			writer.Write(Unknown_40h);
			writer.Write(Unknown_4Ch);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_5Ch);
			writer.Write(DampingLinearC);
			writer.Write(Unknown_6Ch);
			writer.Write(DampingLinearV);
			writer.Write(Unknown_7Ch);
			writer.Write(DampingLinearV2);
			writer.Write(Unknown_8Ch);
			writer.Write(DampingAngularC);
			writer.Write(Unknown_9Ch);
			writer.Write(DampingAngularV);
			writer.Write(Unknown_ACh);
			writer.Write(DampingAngularV2);
			writer.Write(Unknown_BCh);
			writer.Write(GroupNamesPointer);
			writer.Write(GroupsPointer);
			writer.Write(ChildrenPointer);
			writer.Write(Archetype1Pointer);
			writer.Write(Archetype2Pointer);
			writer.Write(BoundPointer);
			writer.Write(ChildrenInertiaTensorsPointer);
			writer.Write(ChildrenUnkVecsPointer);
			writer.Write(FragTransformsPointer);
			writer.Write(UnknownData1Pointer);
			writer.Write(UnknownData2Pointer);
			writer.Write(UnknownData1Count);
			writer.Write(UnknownData2Count);
			writer.Write(GroupsCount);
			writer.Write(RootGroupsCount);
			writer.Write(Unknown_11Ch);
			writer.Write(ChildrenCount);
			writer.Write(ChildrenCount2);
			writer.Write(Unknown_11Fh);
			writer.Write(Unknown_120h);
			writer.Write(Unknown_128h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (ArticulatedBodyType != null)
			{
				list.Add(ArticulatedBodyType);
			}
			if (ChildrenUnkFloats != null)
			{
				ChildrenUnkFloatsBlock = new ResourceSystemStructBlock<float>(ChildrenUnkFloats);
				list.Add(ChildrenUnkFloatsBlock);
			}
			if (Groups != null)
			{
				list.Add(Groups);
			}
			if (Children != null)
			{
				list.Add(Children);
			}
			if (Archetype1 != null)
			{
				list.Add(Archetype1);
			}
			if (Archetype2 != null)
			{
				list.Add(Archetype2);
			}
			if (Bound != null)
			{
				list.Add(Bound);
			}
			if (ChildrenInertiaTensors != null)
			{
				ChildrenInertiaTensorsBlock = new ResourceSystemStructBlock<Vector4>(ChildrenInertiaTensors);
				list.Add(ChildrenInertiaTensorsBlock);
			}
			if (ChildrenUnkVecs != null)
			{
				ChildrenUnkVecsBlock = new ResourceSystemStructBlock<Vector4>(ChildrenUnkVecs);
				list.Add(ChildrenUnkVecsBlock);
			}
			if (FragTransforms != null)
			{
				list.Add(FragTransforms);
			}
			if (UnknownData1 != null)
			{
				UnknownData1Block = new ResourceSystemStructBlock<byte>(UnknownData1);
				list.Add(UnknownData1Block);
			}
			if (UnknownData2 != null)
			{
				UnknownData2Block = new ResourceSystemStructBlock<byte>(UnknownData2);
				list.Add(UnknownData2Block);
			}
			if (GroupNames != null)
			{
				list.Add(GroupNames);
			}
			return list.ToArray();
		}
	}
}
