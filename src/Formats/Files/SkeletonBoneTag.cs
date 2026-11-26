using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SkeletonBoneTag : ResourceSystemBlock
	{
		public override long BlockLength => 16L;

		public uint BoneTag { get; set; }

		public uint BoneIndex { get; set; }

		public ulong NextPointer { get; set; }

		public SkeletonBoneTag Next { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			BoneTag = reader.ReadUInt32();
			BoneIndex = reader.ReadUInt32();
			NextPointer = reader.ReadUInt64();
			Next = reader.ReadBlockAt<SkeletonBoneTag>(NextPointer, Array.Empty<object>());
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			NextPointer = (ulong)((Next != null) ? Next.FilePosition : 0);
			writer.Write(BoneTag);
			writer.Write(BoneIndex);
			writer.Write(NextPointer);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Next != null)
			{
				list.Add(Next);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return BoneTag + ": " + BoneIndex;
		}
	}
}
