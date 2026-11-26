using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ShaderParameter
	{
		public ushort Unknown_2h;

		public uint Unknown_4h;

		public byte DataType { get; set; }

		public byte Unknown_1h { get; set; }

		public ulong DataPointer { get; set; }

		public object Data { get; set; }

		public void Read(ResourceDataReader reader)
		{
			DataType = reader.ReadByte();
			Unknown_1h = reader.ReadByte();
			Unknown_2h = reader.ReadUInt16();
			Unknown_4h = reader.ReadUInt32();
			DataPointer = reader.ReadUInt64();
		}

		public void Write(ResourceDataWriter writer)
		{
			writer.Write(DataType);
			writer.Write(Unknown_1h);
			writer.Write(Unknown_2h);
			writer.Write(Unknown_4h);
			writer.Write(DataPointer);
		}

		public override string ToString()
		{
			if (Data == null)
			{
				return DataType + ": " + DataPointer;
			}
			return Data.ToString();
		}
	}
}
