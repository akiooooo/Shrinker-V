using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysGroupNamesBlock : ResourceSystemBlock
	{
		public FragPhysTypeGroup[] Groups;

		public override long BlockLength
		{
			get
			{
				FragPhysNameStruct_s[] array = data_items;
				return ((array != null) ? array.Length : 0) * 8 + 8;
			}
		}

		public ulong[] data_pointers { get; set; }

		public FragPhysNameStruct_s[] data_items { get; set; }

		public uint UnkVFT { get; set; } = 1095046985u;

		public uint UnkUint1 { get; set; } = 1u;

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			int num = Convert.ToInt32(parameters[0]);
			Groups = parameters[1] as FragPhysTypeGroup[];
			data_pointers = new ulong[num];
			for (int i = 0; i < num; i++)
			{
				data_pointers[i] = reader.ReadUInt64();
			}
			UnkVFT = reader.ReadUInt32();
			UnkUint1 = reader.ReadUInt32();
			data_items = new FragPhysNameStruct_s[num];
			for (int j = 0; j < num; j++)
			{
				data_items[j] = reader.ReadStructAt<FragPhysNameStruct_s>((long)data_pointers[j]);
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			List<ulong> list = new List<ulong>();
			FragPhysTypeGroup[] groups = Groups;
			foreach (FragPhysTypeGroup fragPhysTypeGroup in groups)
			{
				list.Add((ulong)(fragPhysTypeGroup.FilePosition + 128));
			}
			data_pointers = list.ToArray();
			ulong[] array = data_pointers;
			foreach (ulong value in array)
			{
				writer.Write(value);
			}
			writer.Write(UnkVFT);
			writer.Write(UnkUint1);
		}

		public override string ToString()
		{
			return "(Count: " + ((data_items != null) ? data_items.Length : 0) + ")";
		}
	}
}
