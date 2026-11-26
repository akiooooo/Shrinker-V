using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourceSimpleList64_s<T> : ResourceSystemBlock, IResourceNoCacheBlock, IResourceBlock where T : struct
	{
		private ResourceSystemStructBlock<T> data_block;

		public override long BlockLength => 16L;

		public ulong EntriesPointer { get; private set; }

		public ushort EntriesCount { get; private set; }

		public ushort EntriesCapacity { get; private set; }

		public T[] data_items { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			EntriesPointer = reader.ReadUInt64();
			EntriesCount = reader.ReadUInt16();
			EntriesCapacity = reader.ReadUInt16();
			reader.Position += 4L;
			data_items = reader.ReadStructsAt<T>(EntriesPointer, EntriesCount);
			_ = EntriesCount;
			_ = EntriesCapacity;
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			EntriesPointer = (ulong)((data_block != null) ? data_block.FilePosition : 0);
			EntriesCount = (ushort)((data_block != null) ? ((uint)data_block.ItemCount) : 0u);
			EntriesCapacity = (ushort)((data_block != null) ? ((uint)data_block.ItemCount) : 0u);
			writer.Write(EntriesPointer);
			writer.Write(EntriesCount);
			writer.Write(EntriesCapacity);
			writer.Write(0u);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			T[] array = data_items;
			if (array != null && array.Length != 0)
			{
				data_block = new ResourceSystemStructBlock<T>(data_items);
				list.Add(data_block);
			}
			else
			{
				data_block = null;
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return "(Count: " + EntriesCount + ")";
		}
	}
}
