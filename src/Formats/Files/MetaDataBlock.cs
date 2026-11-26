using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MetaDataBlock : ResourceSystemBlock
	{
		private ResourceSystemDataBlock DataBlock;

		public override long BlockLength => 16L;

		public MetaName StructureNameHash { get; set; }

		public int DataLength { get; set; }

		public long DataPointer { get; private set; }

		public byte[] Data { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			StructureNameHash = (MetaName)reader.ReadInt32();
			DataLength = reader.ReadInt32();
			DataPointer = reader.ReadInt64();
			Data = reader.ReadBytesAt((ulong)DataPointer, (uint)DataLength);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			byte[] data = Data;
			DataLength = ((data != null) ? data.Length : 0);
			DataPointer = DataBlock?.FilePosition ?? 0;
			writer.Write((int)StructureNameHash);
			writer.Write(DataLength);
			writer.Write(DataPointer);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (DataBlock == null)
			{
				DataBlock = new ResourceSystemDataBlock(Data);
			}
			if (DataBlock != null)
			{
				list.Add(DataBlock);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return StructureNameHash.ToString() + ": " + DataPointer + " (" + DataLength + ")";
		}
	}
}
