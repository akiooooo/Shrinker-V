using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragBoneTransforms : ResourceSystemBlock
	{
		public ulong Unknown_00h;

		public ulong Unknown_08h;

		public uint Unknown_14h;

		public ulong Unknown_18h;

		public override long BlockLength
		{
			get
			{
				Matrix3_s[] items = Items;
				return 32 + ((items != null) ? items.Length : 0) * 48;
			}
		}

		public byte ItemCount1 { get; set; }

		public byte ItemCount2 { get; set; }

		public ushort Unknown_12h { get; set; }

		public Matrix3_s[] Items { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_00h = reader.ReadUInt64();
			Unknown_08h = reader.ReadUInt64();
			ItemCount1 = reader.ReadByte();
			ItemCount2 = reader.ReadByte();
			Unknown_12h = reader.ReadUInt16();
			Unknown_14h = reader.ReadUInt32();
			Unknown_18h = reader.ReadUInt64();
			Items = reader.ReadStructs<Matrix3_s>(ItemCount1);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Unknown_00h);
			writer.Write(Unknown_08h);
			writer.Write(ItemCount1);
			writer.Write(ItemCount2);
			writer.Write(Unknown_12h);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.WriteStructs(Items);
		}
	}
}
