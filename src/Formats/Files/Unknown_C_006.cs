using System;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Unknown_C_006 : ResourceSystemBlock
	{
		public ulong Unknown_0h;

		public ulong Unknown_8h;

		public ulong Unknown_10h;

		public ulong Unknown_18h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public ulong Unknown_30h;

		public ulong Unknown_38h;

		public ulong Unknown_40h;

		public ulong Unknown_48h;

		public ulong Unknown_F0h;

		public ulong Unknown_F8h;

		public ulong Unknown_100h;

		public ulong Unknown_108h;

		public ulong Unknown_110h;

		public ulong Unknown_118h;

		public ulong Unknown_120h;

		public ulong Unknown_128h;

		public ulong Unknown_130h;

		public ulong Unknown_138h;

		public ulong Unknown_140h;

		public ulong Unknown_148h;

		public ulong Unknown_170h;

		public ulong Unknown_178h;

		public uint Unknown_184h;

		public ulong Unknown_188h;

		public override long BlockLength => 400L;

		public ResourceSimpleList64_s<Vector4> Unknown_50h { get; set; }

		public ResourceSimpleList64_ushort Unknown_60h { get; set; }

		public ResourceSimpleList64_ushort Unknown_70h { get; set; }

		public ResourceSimpleList64_ushort Unknown_80h { get; set; }

		public ResourceSimpleList64_ushort Unknown_90h { get; set; }

		public ResourceSimpleList64_s<Vector4> Unknown_A0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_B0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_C0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_D0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_E0h { get; set; }

		public ResourceSimpleList64_ushort Unknown_150h { get; set; }

		public ResourceSimpleList64_ushort Unknown_160h { get; set; }

		public uint Unknown_180h { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_0h = reader.ReadUInt64();
			Unknown_8h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt64();
			Unknown_18h = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			Unknown_30h = reader.ReadUInt64();
			Unknown_38h = reader.ReadUInt64();
			Unknown_40h = reader.ReadUInt64();
			Unknown_48h = reader.ReadUInt64();
			Unknown_50h = reader.ReadBlock<ResourceSimpleList64_s<Vector4>>(Array.Empty<object>());
			Unknown_60h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_70h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_80h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_90h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_A0h = reader.ReadBlock<ResourceSimpleList64_s<Vector4>>(Array.Empty<object>());
			Unknown_B0h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_C0h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_D0h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_E0h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_F0h = reader.ReadUInt64();
			Unknown_F8h = reader.ReadUInt64();
			Unknown_100h = reader.ReadUInt64();
			Unknown_108h = reader.ReadUInt64();
			Unknown_110h = reader.ReadUInt64();
			Unknown_118h = reader.ReadUInt64();
			Unknown_120h = reader.ReadUInt64();
			Unknown_128h = reader.ReadUInt64();
			Unknown_130h = reader.ReadUInt64();
			Unknown_138h = reader.ReadUInt64();
			Unknown_140h = reader.ReadUInt64();
			Unknown_148h = reader.ReadUInt64();
			Unknown_150h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_160h = reader.ReadBlock<ResourceSimpleList64_ushort>(Array.Empty<object>());
			Unknown_170h = reader.ReadUInt64();
			Unknown_178h = reader.ReadUInt64();
			Unknown_180h = reader.ReadUInt32();
			Unknown_184h = reader.ReadUInt32();
			Unknown_188h = reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Unknown_0h);
			writer.Write(Unknown_8h);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
			writer.Write(Unknown_30h);
			writer.Write(Unknown_38h);
			writer.Write(Unknown_40h);
			writer.Write(Unknown_48h);
			writer.WriteBlock(Unknown_50h);
			writer.WriteBlock(Unknown_60h);
			writer.WriteBlock(Unknown_70h);
			writer.WriteBlock(Unknown_80h);
			writer.WriteBlock(Unknown_90h);
			writer.WriteBlock(Unknown_A0h);
			writer.WriteBlock(Unknown_B0h);
			writer.WriteBlock(Unknown_C0h);
			writer.WriteBlock(Unknown_D0h);
			writer.WriteBlock(Unknown_E0h);
			writer.Write(Unknown_F0h);
			writer.Write(Unknown_F8h);
			writer.Write(Unknown_100h);
			writer.Write(Unknown_108h);
			writer.Write(Unknown_110h);
			writer.Write(Unknown_118h);
			writer.Write(Unknown_120h);
			writer.Write(Unknown_128h);
			writer.Write(Unknown_130h);
			writer.Write(Unknown_138h);
			writer.Write(Unknown_140h);
			writer.Write(Unknown_148h);
			writer.WriteBlock(Unknown_150h);
			writer.WriteBlock(Unknown_160h);
			writer.Write(Unknown_170h);
			writer.Write(Unknown_178h);
			writer.Write(Unknown_180h);
			writer.Write(Unknown_184h);
			writer.Write(Unknown_188h);
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[12]
			{
				new Tuple<long, IResourceBlock>(80L, Unknown_50h),
				new Tuple<long, IResourceBlock>(96L, Unknown_60h),
				new Tuple<long, IResourceBlock>(112L, Unknown_70h),
				new Tuple<long, IResourceBlock>(128L, Unknown_80h),
				new Tuple<long, IResourceBlock>(144L, Unknown_90h),
				new Tuple<long, IResourceBlock>(160L, Unknown_A0h),
				new Tuple<long, IResourceBlock>(176L, Unknown_B0h),
				new Tuple<long, IResourceBlock>(192L, Unknown_C0h),
				new Tuple<long, IResourceBlock>(208L, Unknown_D0h),
				new Tuple<long, IResourceBlock>(224L, Unknown_E0h),
				new Tuple<long, IResourceBlock>(336L, Unknown_150h),
				new Tuple<long, IResourceBlock>(352L, Unknown_160h)
			};
		}
	}
}
