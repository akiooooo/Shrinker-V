using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct MetaEnumEntryInfo_s
	{
		public MetaName EntryNameHash { get; }

		public int EntryValue { get; }

		public override string ToString()
		{
			return EntryNameHash.ToString() + ": " + EntryValue;
		}
	}
}
