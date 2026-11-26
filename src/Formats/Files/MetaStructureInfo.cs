using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MetaStructureInfo : ResourceSystemBlock
	{
		private ResourceSystemStructBlock<MetaStructureEntryInfo_s> EntriesBlock;

		public override long BlockLength => 32L;

		public MetaName StructureNameHash { get; set; }

		public uint StructureKey { get; set; }

		public uint Unknown_8h { get; set; }

		public uint Unknown_Ch { get; set; }

		public long EntriesPointer { get; private set; }

		public int StructureSize { get; set; }

		public short Unknown_1Ch { get; set; }

		public short EntriesCount { get; private set; }

		public MetaStructureEntryInfo_s[] Entries { get; private set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			StructureNameHash = (MetaName)reader.ReadInt32();
			StructureKey = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt32();
			Unknown_Ch = reader.ReadUInt32();
			EntriesPointer = reader.ReadInt64();
			StructureSize = reader.ReadInt32();
			Unknown_1Ch = reader.ReadInt16();
			EntriesCount = reader.ReadInt16();
			Entries = reader.ReadStructsAt<MetaStructureEntryInfo_s>((ulong)EntriesPointer, (uint)EntriesCount);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			EntriesPointer = EntriesBlock?.FilePosition ?? 0;
			EntriesCount = (short)(EntriesBlock?.ItemCount ?? 0);
			writer.Write((int)StructureNameHash);
			writer.Write(StructureKey);
			writer.Write(Unknown_8h);
			writer.Write(Unknown_Ch);
			writer.Write(EntriesPointer);
			writer.Write(StructureSize);
			writer.Write(Unknown_1Ch);
			writer.Write(EntriesCount);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (EntriesBlock == null)
			{
				EntriesBlock = new ResourceSystemStructBlock<MetaStructureEntryInfo_s>(Entries);
			}
			if (EntriesBlock != null)
			{
				list.Add(EntriesBlock);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return StructureNameHash.ToString();
		}
	}
}
