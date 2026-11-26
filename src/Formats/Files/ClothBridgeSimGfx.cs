using System;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ClothBridgeSimGfx : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public uint Unknown_1Ch;

		public ulong Unknown_50h;

		public ulong Unknown_58h;

		public ulong Unknown_90h;

		public ulong Unknown_98h;

		public ulong Unknown_D0h;

		public ulong Unknown_D8h;

		public ulong Unknown_110h;

		public ulong Unknown_118h;

		public ulong Unknown_120h;

		public ulong Unknown_138h;

		public override long BlockLength => 320L;

		public uint VFT { get; set; }

		public uint VertexCount { get; set; }

		public uint Unknown_14h { get; set; }

		public uint Unknown_18h { get; set; }

		public ResourceSimpleList64_float Unknown_20h { get; set; }

		public ResourceSimpleList64_float Unknown_30h { get; set; }

		public ResourceSimpleList64_float Unknown_40h { get; set; }

		public ResourceSimpleList64_float Unknown_60h { get; set; }

		public ResourceSimpleList64_uint Unknown_70h { get; set; }

		public ResourceSimpleList64_uint Unknown_80h { get; set; }

		public ResourceSimpleList64_float Unknown_A0h { get; set; }

		public ResourceSimpleList64_uint Unknown_B0h { get; set; }

		public ResourceSimpleList64_uint Unknown_C0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_E0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_F0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_100h { get; set; }

		public ResourceSimpleList64_uint Unknown_128h { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			VertexCount = reader.ReadUInt32();
			Unknown_14h = reader.ReadUInt32();
			Unknown_18h = reader.ReadUInt32();
			Unknown_1Ch = reader.ReadUInt32();
			Unknown_20h = reader.ReadBlock<ResourceSimpleList64_float>(Array.Empty<object>());
			Unknown_30h = reader.ReadBlock<ResourceSimpleList64_float>(Array.Empty<object>());
			Unknown_40h = reader.ReadBlock<ResourceSimpleList64_float>(Array.Empty<object>());
			Unknown_50h = reader.ReadUInt64();
			Unknown_58h = reader.ReadUInt64();
			Unknown_60h = reader.ReadBlock<ResourceSimpleList64_float>(Array.Empty<object>());
			Unknown_70h = reader.ReadBlock<ResourceSimpleList64_uint>(Array.Empty<object>());
			Unknown_80h = reader.ReadBlock<ResourceSimpleList64_uint>(Array.Empty<object>());
			Unknown_90h = reader.ReadUInt64();
			Unknown_98h = reader.ReadUInt64();
			Unknown_A0h = reader.ReadBlock<ResourceSimpleList64_float>(Array.Empty<object>());
			Unknown_B0h = reader.ReadBlock<ResourceSimpleList64_uint>(Array.Empty<object>());
			Unknown_C0h = reader.ReadBlock<ResourceSimpleList64_uint>(Array.Empty<object>());
			Unknown_D0h = reader.ReadUInt64();
			Unknown_D8h = reader.ReadUInt64();
			Unknown_E0h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_F0h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_100h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_110h = reader.ReadUInt64();
			Unknown_118h = reader.ReadUInt64();
			Unknown_120h = reader.ReadUInt64();
			Unknown_128h = reader.ReadBlock<ResourceSimpleList64_uint>(Array.Empty<object>());
			Unknown_138h = reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(VertexCount);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_1Ch);
			writer.WriteBlock(Unknown_20h);
			writer.WriteBlock(Unknown_30h);
			writer.WriteBlock(Unknown_40h);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_58h);
			writer.WriteBlock(Unknown_60h);
			writer.WriteBlock(Unknown_70h);
			writer.WriteBlock(Unknown_80h);
			writer.Write(Unknown_90h);
			writer.Write(Unknown_98h);
			writer.WriteBlock(Unknown_A0h);
			writer.WriteBlock(Unknown_B0h);
			writer.WriteBlock(Unknown_C0h);
			writer.Write(Unknown_D0h);
			writer.Write(Unknown_D8h);
			writer.WriteBlock(Unknown_E0h);
			writer.WriteBlock(Unknown_F0h);
			writer.WriteBlock(Unknown_100h);
			writer.Write(Unknown_110h);
			writer.Write(Unknown_118h);
			writer.Write(Unknown_120h);
			writer.WriteBlock(Unknown_128h);
			writer.Write(Unknown_138h);
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[13]
			{
				new Tuple<long, IResourceBlock>(32L, Unknown_20h),
				new Tuple<long, IResourceBlock>(48L, Unknown_30h),
				new Tuple<long, IResourceBlock>(64L, Unknown_40h),
				new Tuple<long, IResourceBlock>(96L, Unknown_60h),
				new Tuple<long, IResourceBlock>(112L, Unknown_70h),
				new Tuple<long, IResourceBlock>(128L, Unknown_80h),
				new Tuple<long, IResourceBlock>(160L, Unknown_A0h),
				new Tuple<long, IResourceBlock>(176L, Unknown_B0h),
				new Tuple<long, IResourceBlock>(192L, Unknown_C0h),
				new Tuple<long, IResourceBlock>(224L, Unknown_E0h),
				new Tuple<long, IResourceBlock>(240L, Unknown_F0h),
				new Tuple<long, IResourceBlock>(256L, Unknown_100h),
				new Tuple<long, IResourceBlock>(296L, Unknown_128h)
			};
		}
	}
}
