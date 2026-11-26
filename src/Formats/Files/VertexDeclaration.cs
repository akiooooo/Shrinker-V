using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class VertexDeclaration : ResourceSystemBlock
	{
		public override long BlockLength => 16L;

		public uint Flags { get; set; }

		public ushort Stride { get; set; }

		public byte Unknown_6h { get; set; }

		public byte Count { get; set; }

		public VertexDeclarationTypes Types { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Flags = reader.ReadUInt32();
			Stride = reader.ReadUInt16();
			Unknown_6h = reader.ReadByte();
			Count = reader.ReadByte();
			Types = (VertexDeclarationTypes)reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Flags);
			writer.Write(Stride);
			writer.Write(Unknown_6h);
			writer.Write(Count);
			writer.Write((ulong)Types);
		}

		public ulong GetDeclarationId()
		{
			ulong num = 0uL;
			for (int i = 0; i < 16; i++)
			{
				if (((Flags >> i) & 1) == 1)
				{
					num += (ulong)Types & (ulong)(uint)(15 << i * 4);
				}
			}
			return num;
		}

		public VertexComponentType GetComponentType(int index)
		{
			return (VertexComponentType)(((ulong)Types >> index * 4) & 0xF);
		}

		public int GetComponentOffset(int index)
		{
			int num = 0;
			for (int i = 0; i < index; i++)
			{
				if (((Flags >> i) & 1) == 1)
				{
					VertexComponentType componentType = GetComponentType(i);
					num += VertexComponentTypes.GetSizeInBytes(componentType);
				}
			}
			return num;
		}

		public void UpdateCountAndStride()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 16; i++)
			{
				if (((Flags >> i) & 1) == 1)
				{
					VertexComponentType componentType = GetComponentType(i);
					num2 += VertexComponentTypes.GetSizeInBytes(componentType);
					num++;
				}
			}
			Count = (byte)num;
			Stride = (ushort)num2;
		}

		public override string ToString()
		{
			return Stride + ": " + Count + ": " + Flags + ": " + Types;
		}
	}
}
