using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class MorphController : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public ulong Unknown_10h;

		public ulong Unknown_30h;

		public ulong Unknown_38h;

		public override long BlockLength => 64L;

		public uint VFT { get; set; }

		public ulong Unknown_18h_Pointer { get; set; }

		public ulong Unknown_20h_Pointer { get; set; }

		public ulong Unknown_28h_Pointer { get; set; }

		public Unknown_C_006 Unknown_18h_Data { get; set; }

		public Unknown_C_006 Unknown_20h_Data { get; set; }

		public Unknown_C_006 Unknown_28h_Data { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt64();
			Unknown_18h_Pointer = reader.ReadUInt64();
			Unknown_20h_Pointer = reader.ReadUInt64();
			Unknown_28h_Pointer = reader.ReadUInt64();
			Unknown_30h = reader.ReadUInt64();
			Unknown_38h = reader.ReadUInt64();
			Unknown_18h_Data = reader.ReadBlockAt<Unknown_C_006>(Unknown_18h_Pointer, Array.Empty<object>());
			Unknown_20h_Data = reader.ReadBlockAt<Unknown_C_006>(Unknown_20h_Pointer, Array.Empty<object>());
			Unknown_28h_Data = reader.ReadBlockAt<Unknown_C_006>(Unknown_28h_Pointer, Array.Empty<object>());
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			Unknown_18h_Pointer = (ulong)((Unknown_18h_Data != null) ? Unknown_18h_Data.FilePosition : 0);
			Unknown_20h_Pointer = (ulong)((Unknown_20h_Data != null) ? Unknown_20h_Data.FilePosition : 0);
			Unknown_28h_Pointer = (ulong)((Unknown_28h_Data != null) ? Unknown_28h_Data.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_18h_Pointer);
			writer.Write(Unknown_20h_Pointer);
			writer.Write(Unknown_28h_Pointer);
			writer.Write(Unknown_30h);
			writer.Write(Unknown_38h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Unknown_18h_Data != null)
			{
				list.Add(Unknown_18h_Data);
			}
			if (Unknown_20h_Data != null)
			{
				list.Add(Unknown_20h_Data);
			}
			if (Unknown_28h_Data != null)
			{
				list.Add(Unknown_28h_Data);
			}
			return list.ToArray();
		}
	}
}
