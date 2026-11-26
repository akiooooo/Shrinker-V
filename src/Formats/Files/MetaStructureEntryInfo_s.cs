using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct MetaStructureEntryInfo_s
	{



		public MetaName EntryNameHash { get; }

		public int DataOffset { get; }

		public MetaStructureEntryDataType DataType { get; }

		public byte Unknown_9h { get; }
        public short ReferenceTypeIndex { get; }
        public MetaName ReferenceKey { get; }

		public override string ToString()
		{
			return DataOffset + ": " + DataType.ToString() + ": " + ReferenceKey.ToString() + ": " + EntryNameHash;
		}
	}
}
