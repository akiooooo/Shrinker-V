using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourceFileBase : ResourceSystemBlock
	{
		public override long BlockLength => 16L;

		public uint FileVFT { get; set; }

		public uint FileUnknown { get; set; } = 1u;

		public ulong FilePagesInfoPointer { get; set; }

		public ResourcePagesInfo FilePagesInfo { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			FileVFT = reader.ReadUInt32();
			FileUnknown = reader.ReadUInt32();
			FilePagesInfoPointer = reader.ReadUInt64();
			FilePagesInfo = reader.ReadBlockAt<ResourcePagesInfo>(FilePagesInfoPointer, Array.Empty<object>());
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			FilePagesInfoPointer = (ulong)((FilePagesInfo != null) ? FilePagesInfo.FilePosition : 0);
			writer.Write(FileVFT);
			writer.Write(FileUnknown);
			writer.Write(FilePagesInfoPointer);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (FilePagesInfo != null)
			{
				list.Add(FilePagesInfo);
			}
			return list.ToArray();
		}
	}
}
