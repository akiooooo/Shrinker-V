using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RpfDirectoryEntry : RpfEntry
	{
		public List<RpfDirectoryEntry> Directories = new List<RpfDirectoryEntry>();

		public List<RpfFileEntry> Files = new List<RpfFileEntry>();

		public uint EntriesIndex { get; set; }

		public uint EntriesCount { get; set; }

		public override void Read(DataReader reader)
		{
			base.NameOffset = reader.ReadUInt32();
			if (reader.ReadUInt32() != 2147483392)
			{
				throw new Exception("Error in RPF7 directory entry.");
			}
			EntriesIndex = reader.ReadUInt32();
			EntriesCount = reader.ReadUInt32();
		}

		public override void Write(DataWriter writer)
		{
			writer.Write(base.NameOffset);
			writer.Write(2147483392u);
			writer.Write(EntriesIndex);
			writer.Write(EntriesCount);
		}

		public override string ToString()
		{
			return "Directory: " + base.Path;
		}
	}
}
