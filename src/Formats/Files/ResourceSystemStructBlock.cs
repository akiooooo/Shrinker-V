using System.Runtime.InteropServices;

namespace GameFormats.Files
{
	public class ResourceSystemStructBlock<T> : ResourceSystemBlock where T : struct
	{
		public T[] Items { get; set; }

		public int ItemCount { get; set; }

		public int StructureSize { get; set; }

		public override long BlockLength => ((Items != null) ? Items.Length : ItemCount) * StructureSize;

		public ResourceSystemStructBlock(T[] items)
		{
			Items = items;
			ItemCount = ((Items != null) ? Items.Length : 0);
			StructureSize = Marshal.SizeOf(typeof(T));
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			int count = ItemCount * StructureSize;
			byte[] data = reader.ReadBytes(count);
			Items = MetaTypes.ConvertDataArray<T>(data, 0, ItemCount);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			byte[] array = MetaTypes.ConvertArrayToBytes(Items);
			if (array != null)
			{
				writer.Write(array);
			}
		}
	}
}
