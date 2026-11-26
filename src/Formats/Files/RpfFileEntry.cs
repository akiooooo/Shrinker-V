using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class RpfFileEntry : RpfEntry
	{
		public uint FileOffset { get; set; }

		public uint FileSize { get; set; }

		public bool IsEncrypted { get; set; }

		public abstract long GetFileSize();
	}
}
