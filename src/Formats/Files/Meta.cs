using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Meta : ResourceFileBase
	{
		private string_r NameBlock;

		public override long BlockLength => 112L;

		public int Unknown_10h { get; set; }

		public short Unknown_14h { get; set; }

		public byte HasEncryptedStrings { get; set; }

		public byte Unknown_17h { get; set; }

		public int Unknown_18h { get; set; }

		public int RootBlockIndex { get; set; }

		public long StructureInfosPointer { get; set; }

		public long EnumInfosPointer { get; set; }

		public long DataBlocksPointer { get; set; }

		public long NamePointer { get; set; }

		public long EncryptedStringsPointer { get; set; }

		public short StructureInfosCount { get; set; }

		public short EnumInfosCount { get; set; }

		public short DataBlocksCount { get; set; }

		public short Unknown_4Eh { get; set; }

		public uint Unknown_50h { get; set; }

		public uint Unknown_54h { get; set; }

		public uint Unknown_58h { get; set; }

		public uint Unknown_5Ch { get; set; }

		public uint Unknown_60h { get; set; }

		public uint Unknown_64h { get; set; }

		public uint Unknown_68h { get; set; }

		public uint Unknown_6Ch { get; set; }

		public ResourceSimpleArray<MetaStructureInfo> StructureInfos { get; set; }

		public ResourceSimpleArray<MetaEnumInfo> EnumInfos { get; set; }

		public ResourceSimpleArray<MetaDataBlock> DataBlocks { get; set; }

		public string Name { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_10h = reader.ReadInt32();
			Unknown_14h = reader.ReadInt16();
			HasEncryptedStrings = reader.ReadByte();
			Unknown_17h = reader.ReadByte();
			Unknown_18h = reader.ReadInt32();
			RootBlockIndex = reader.ReadInt32();
			StructureInfosPointer = reader.ReadInt64();
			EnumInfosPointer = reader.ReadInt64();
			DataBlocksPointer = reader.ReadInt64();
			NamePointer = reader.ReadInt64();
			EncryptedStringsPointer = reader.ReadInt64();
			StructureInfosCount = reader.ReadInt16();
			EnumInfosCount = reader.ReadInt16();
			DataBlocksCount = reader.ReadInt16();
			Unknown_4Eh = reader.ReadInt16();
			Unknown_50h = reader.ReadUInt32();
			Unknown_54h = reader.ReadUInt32();
			Unknown_58h = reader.ReadUInt32();
			Unknown_5Ch = reader.ReadUInt32();
			Unknown_60h = reader.ReadUInt32();
			Unknown_64h = reader.ReadUInt32();
			Unknown_68h = reader.ReadUInt32();
			Unknown_6Ch = reader.ReadUInt32();
			StructureInfos = reader.ReadBlockAt<ResourceSimpleArray<MetaStructureInfo>>((ulong)StructureInfosPointer, new object[1] { StructureInfosCount });
			EnumInfos = reader.ReadBlockAt<ResourceSimpleArray<MetaEnumInfo>>((ulong)EnumInfosPointer, new object[1] { EnumInfosCount });
			DataBlocks = reader.ReadBlockAt<ResourceSimpleArray<MetaDataBlock>>((ulong)DataBlocksPointer, new object[1] { DataBlocksCount });
			Name = reader.ReadStringAt((ulong)NamePointer);
			string.IsNullOrEmpty(Name);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			StructureInfosPointer = StructureInfos?.FilePosition ?? 0;
			EnumInfosPointer = EnumInfos?.FilePosition ?? 0;
			DataBlocksPointer = DataBlocks?.FilePosition ?? 0;
			NamePointer = NameBlock?.FilePosition ?? 0;
			EncryptedStringsPointer = 0L;
			StructureInfosCount = (short)(StructureInfos?.Count ?? 0);
			EnumInfosCount = (short)(EnumInfos?.Count ?? 0);
			DataBlocksCount = (short)(DataBlocks?.Count ?? 0);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_14h);
			writer.Write(HasEncryptedStrings);
			writer.Write(Unknown_17h);
			writer.Write(Unknown_18h);
			writer.Write(RootBlockIndex);
			writer.Write(StructureInfosPointer);
			writer.Write(EnumInfosPointer);
			writer.Write(DataBlocksPointer);
			writer.Write(NamePointer);
			writer.Write(EncryptedStringsPointer);
			writer.Write(StructureInfosCount);
			writer.Write(EnumInfosCount);
			writer.Write(DataBlocksCount);
			writer.Write(Unknown_4Eh);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_54h);
			writer.Write(Unknown_58h);
			writer.Write(Unknown_5Ch);
			writer.Write(Unknown_60h);
			writer.Write(Unknown_64h);
			writer.Write(Unknown_68h);
			writer.Write(Unknown_6Ch);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (StructureInfos != null && StructureInfos.Count > 0)
			{
				list.Add(StructureInfos);
			}
			if (EnumInfos != null && EnumInfos.Count > 0)
			{
				list.Add(EnumInfos);
			}
			if (DataBlocks != null && DataBlocks.Count > 0)
			{
				list.Add(DataBlocks);
			}
			if (!string.IsNullOrEmpty(Name))
			{
				NameBlock = (string_r)Name;
				list.Add(NameBlock);
			}
			return list.ToArray();
		}
	}
}
