using System.Collections.Generic;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysArticulatedBodyType : ResourceSystemBlock
	{
		public uint Unknown_04h = 1u;

		public ulong Unknown_08h;

		public uint Unknown_68h;

		public float Unknown_6Ch = 1f;

		public ulong Unknown_70h;

		public ulong Unknown_A0h;

		public ulong Unknown_A8h;

		private ResourceSystemStructBlock<Vector4> UnknownVectorsBlock;

		public override long BlockLength => 176L;

		public uint VFT { get; set; } = 1080211704u;

		public uint[] ItemIndices { get; set; }

		public ulong JointsPointer { get; set; }

		public ulong UnknownVectorsPointer { get; set; }

		public byte UnknownVectorsCount { get; set; }

		public byte JointsCount { get; set; }

		public byte[] ItemFlags { get; set; }

		public ResourcePointerArray64<FragPhysJointType> Joints { get; set; }

		public Vector4[] UnknownVectors { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			Unknown_08h = reader.ReadUInt64();
			ItemIndices = reader.ReadStructs<uint>(22u);
			Unknown_68h = reader.ReadUInt32();
			Unknown_6Ch = reader.ReadSingle();
			Unknown_70h = reader.ReadUInt64();
			JointsPointer = reader.ReadUInt64();
			UnknownVectorsPointer = reader.ReadUInt64();
			UnknownVectorsCount = reader.ReadByte();
			JointsCount = reader.ReadByte();
			ItemFlags = reader.ReadBytes(22);
			Unknown_A0h = reader.ReadUInt64();
			Unknown_A8h = reader.ReadUInt64();
			Joints = reader.ReadBlockAt<ResourcePointerArray64<FragPhysJointType>>(JointsPointer, new object[1] { JointsCount });
			UnknownVectors = reader.ReadStructsAt<Vector4>(UnknownVectorsPointer, UnknownVectorsCount);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			JointsPointer = (ulong)((Joints != null) ? Joints.FilePosition : 0);
			UnknownVectorsPointer = (ulong)((UnknownVectorsBlock != null) ? UnknownVectorsBlock.FilePosition : 0);
			UnknownVectorsCount = (byte)((UnknownVectorsBlock != null) ? ((uint)UnknownVectorsBlock.ItemCount) : 0u);
			JointsCount = (byte)((Joints != null) ? ((uint)Joints.Count) : 0u);
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.WriteStructs(ItemIndices);
			writer.Write(Unknown_68h);
			writer.Write(Unknown_6Ch);
			writer.Write(Unknown_70h);
			writer.Write(JointsPointer);
			writer.Write(UnknownVectorsPointer);
			writer.Write(UnknownVectorsCount);
			writer.Write(JointsCount);
			writer.Write(ItemFlags);
			writer.Write(Unknown_A0h);
			writer.Write(Unknown_A8h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Joints != null)
			{
				list.Add(Joints);
			}
			if (UnknownVectors != null)
			{
				UnknownVectorsBlock = new ResourceSystemStructBlock<Vector4>(UnknownVectors);
				list.Add(UnknownVectorsBlock);
			}
			return list.ToArray();
		}
	}
}
