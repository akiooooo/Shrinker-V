using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class SkeletonBonesBlock : ResourceSystemBlock
	{
		public uint Unk0;

		public uint Unk1;

		public uint Unk2;

		public override long BlockLength
		{
			get
			{
				long num = 16L;
				if (Items != null)
				{
					Bone[] items = Items;
					foreach (Bone bone in items)
					{
						num += bone.BlockLength;
					}
				}
				return num;
			}
		}

		public uint Count { get; set; }

		public Bone[] Items { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Count = reader.ReadUInt32();
			Unk0 = reader.ReadUInt32();
			Unk1 = reader.ReadUInt32();
			Unk2 = reader.ReadUInt32();
			uint num = (uint)parameters[0];
			Bone[] array = new Bone[num];
			for (uint num2 = 0u; num2 < num; num2++)
			{
				array[num2] = reader.ReadBlock<Bone>(Array.Empty<object>());
			}
			Items = array;
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			Bone[] items = Items;
			Count = ((items != null) ? ((uint)items.Length) : 0u);
			writer.Write(Count);
			writer.Write(Unk0);
			writer.Write(Unk1);
			writer.Write(Unk2);
			Bone[] items2 = Items;
			for (int i = 0; i < items2.Length; i++)
			{
				items2[i].Write(writer);
			}
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			List<Tuple<long, IResourceBlock>> list = new List<Tuple<long, IResourceBlock>>();
			long num = 16L;
			if (Items != null)
			{
				Bone[] items = Items;
				foreach (Bone bone in items)
				{
					list.Add(new Tuple<long, IResourceBlock>(num, bone));
					num += bone.BlockLength;
				}
			}
			return list.ToArray();
		}
	}
}
