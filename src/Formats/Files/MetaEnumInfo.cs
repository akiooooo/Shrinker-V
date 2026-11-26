using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MetaEnumInfo : ResourceSystemBlock
	{
		private ResourceSystemStructBlock<MetaEnumEntryInfo_s> EntriesBlock;

		public override long BlockLength => 24L;

		public MetaName EnumNameHash { get; set; }

		public uint EnumKey { get; set; }

		public long EntriesPointer { get; private set; }

		public int EntriesCount { get; private set; }

		public int Unknown_14h { get; set; }

		public MetaEnumEntryInfo_s[] Entries { get; private set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			EnumNameHash = (MetaName)reader.ReadInt32();
			EnumKey = reader.ReadUInt32();
			EntriesPointer = reader.ReadInt64();
			EntriesCount = reader.ReadInt32();
			Unknown_14h = reader.ReadInt32();
			Entries = reader.ReadStructsAt<MetaEnumEntryInfo_s>((ulong)EntriesPointer, (uint)EntriesCount);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			EntriesPointer = EntriesBlock?.FilePosition ?? 0;
			EntriesCount = (short)(EntriesBlock?.ItemCount ?? 0);
			writer.Write((int)EnumNameHash);
			writer.Write(EnumKey);
			writer.Write(EntriesPointer);
			writer.Write(EntriesCount);
			writer.Write(Unknown_14h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (EntriesBlock == null)
			{
				EntriesBlock = new ResourceSystemStructBlock<MetaEnumEntryInfo_s>(Entries);
			}
			if (EntriesBlock != null)
			{
				list.Add(EntriesBlock);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return EnumNameHash.ToString();
		}
	}
}
