using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class VertexDeclarationG9 : ResourceSystemBlock
	{
		public override long BlockLength => 320L;

		public uint[] Offsets { get; set; }

		public byte[] Sizes { get; set; }

		public byte[] Types { get; set; }

		public ulong Data { get; set; }

		public byte VertexSize
		{
			get
			{
				return (byte)((Data >> 2) & 0xFF);
			}
			set
			{
				Data = (Data & 0xFFFFFC03u) + (uint)((value & 0xFF) << 2);
			}
		}

		public uint VertexCount
		{
			get
			{
				return (uint)((Data >> 10) & 0x3FFFFF);
			}
			set
			{
				Data = (Data & 0x3FF) + ((value & 0x3FFFFF) << 10);
			}
		}

		public uint ElementCount
		{
			get
			{
				if (Types == null)
				{
					return 0u;
				}
				uint num = 0u;
				byte[] types = Types;
				for (int i = 0; i < types.Length; i++)
				{
					if (types[i] != 0)
					{
						num++;
					}
				}
				return num;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Offsets = reader.ReadStructs<uint>(52u);
			Sizes = reader.ReadBytes(52);
			Types = reader.ReadBytes(52);
			Data = reader.ReadUInt64();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.WriteStructs(Offsets);
			writer.Write(Sizes);
			writer.Write(Types);
			writer.Write(Data);
		}

		public VertexDeclaration GetLegacyDeclaration(VertexDeclarationTypes vdtypes = VertexDeclarationTypes.GTAV1)
		{
			uint num = 0u;
			byte[] types = Types;
			_ = Sizes;
			_ = Offsets;
			uint elementCount = ElementCount;
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] != 0)
				{
					int legacyComponentIndex = GetLegacyComponentIndex(i, vdtypes);
					if (legacyComponentIndex >= 0)
					{
						num = BitUtil.SetBit(num, legacyComponentIndex);
					}
				}
			}
			VertexType vertexType = (VertexType)num;
			if (vertexType <= VertexType.PBBNCCTT)
			{
				switch (vertexType)
				{
				}
			}
			else if (vertexType <= VertexType.PBBNCTTX)
			{
				switch (vertexType)
				{
				}
			}
			else if (vertexType <= VertexType.PBBNCTTTX)
			{
				switch (vertexType)
				{
				}
			}
			else if (vertexType <= VertexType.PNCTTTX_3)
			{
				switch (vertexType)
				{
				}
			}
			else if (vertexType != VertexType.PNCCTTTX && vertexType != VertexType.PNCTTTX_2)
			{
				_ = 19673;
			}
			VertexDeclaration vertexDeclaration = new VertexDeclaration();
			vertexDeclaration.Types = vdtypes;
			vertexDeclaration.Flags = num;
			vertexDeclaration.UpdateCountAndStride();
			_ = vertexDeclaration.Count;
			_ = vertexDeclaration.Stride;
			_ = VertexSize;
			return vertexDeclaration;
		}

		public static VertexDeclarationG9 FromLegacyDeclaration(VertexDeclaration vd)
		{
			byte b = (byte)vd.Stride;
			VertexDeclarationTypes types = vd.Types;
			uint flags = vd.Flags;
			uint[] array = new uint[52];
			byte[] array2 = new byte[52];
			byte[] array3 = new byte[52];
			_ = 8598872888530528662L;
			uint num = 0u;
			for (int i = 0; i < 52; i++)
			{
				array[i] = num;
				int legacyComponentIndex = GetLegacyComponentIndex(i, types);
				if (legacyComponentIndex >= 0 && (flags & (uint)(1 << legacyComponentIndex)) != 0)
				{
					int sizeInBytes = VertexComponentTypes.GetSizeInBytes((VertexComponentType)(((ulong)types >> legacyComponentIndex * 4) & 0xF));
					num += (uint)sizeInBytes;
					array2[i] = b;
					array3[i] = (byte)GetGen9ComponentType(legacyComponentIndex, types);
				}
			}
			return new VertexDeclarationG9
			{
				Offsets = array,
				Sizes = array2,
				Types = array3,
				VertexSize = b,
				VertexCount = 0u
			};
		}

		public static int GetLegacyComponentIndex(int i, VertexDeclarationTypes vdtypes)
		{
			_ = 8598872888530528662L;
			switch (i)
			{
			case 0:
				return 0;
			case 4:
				return 3;
			case 8:
				return 14;
			case 16:
				return 1;
			case 20:
				return 2;
			case 24:
				return 4;
			case 25:
				return 5;
			case 28:
				return 6;
			case 29:
				return 7;
			case 30:
				return 8;
			case 31:
				return 9;
			case 32:
				return 10;
			case 33:
				return 11;
			default:
				return -1;
			}
		}

		public static VertexDeclarationG9ElementFormat GetGen9ComponentTypeGTAV1(int lci)
		{
			switch (lci)
			{
			case 0:
				return VertexDeclarationG9ElementFormat.R32G32B32_FLOAT;
			case 1:
				return VertexDeclarationG9ElementFormat.R8G8B8A8_UNORM;
			case 2:
				return VertexDeclarationG9ElementFormat.R8G8B8A8_UINT;
			case 3:
				return VertexDeclarationG9ElementFormat.R32G32B32_FLOAT;
			case 4:
			case 5:
				return VertexDeclarationG9ElementFormat.R8G8B8A8_UNORM;
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
				return VertexDeclarationG9ElementFormat.R32G32_TYPELESS;
			case 14:
			case 15:
				return VertexDeclarationG9ElementFormat.R32G32B32A32_FLOAT;
			default:
				return VertexDeclarationG9ElementFormat.R32G32B32A32_FLOAT;
			}
		}

		public static VertexDeclarationG9ElementFormat GetGen9ComponentType(int lci, VertexDeclarationTypes vdtypes)
		{
			if (vdtypes == VertexDeclarationTypes.GTAV1)
			{
				return GetGen9ComponentTypeGTAV1(lci);
			}
			switch (lci)
			{
			case 1:
				return VertexDeclarationG9ElementFormat.R8G8B8A8_UNORM;
			case 2:
				return VertexDeclarationG9ElementFormat.R8G8B8A8_UINT;
			default:
				switch ((VertexComponentType)(byte)(((ulong)vdtypes >> lci * 4) & 0xF))
				{
				case VertexComponentType.Half2:
					return VertexDeclarationG9ElementFormat.R16G16_FLOAT;
				case VertexComponentType.Float:
					return VertexDeclarationG9ElementFormat.NONE;
				case VertexComponentType.Half4:
					return VertexDeclarationG9ElementFormat.R16G16B16A16_FLOAT;
				case VertexComponentType.FloatUnk:
					return VertexDeclarationG9ElementFormat.NONE;
				case VertexComponentType.Float2:
					return VertexDeclarationG9ElementFormat.R32G32_TYPELESS;
				case VertexComponentType.Float3:
					return VertexDeclarationG9ElementFormat.R32G32B32_FLOAT;
				case VertexComponentType.Float4:
					return VertexDeclarationG9ElementFormat.R32G32B32A32_FLOAT;
				case VertexComponentType.UByte4:
					return VertexDeclarationG9ElementFormat.R8G8B8A8_UINT;
				case VertexComponentType.Colour:
					return VertexDeclarationG9ElementFormat.R8G8B8A8_UNORM;
				case VertexComponentType.RGBA8SNorm:
					return VertexDeclarationG9ElementFormat.R8G8B8A8_UNORM;
				default:
					return VertexDeclarationG9ElementFormat.NONE;
				}
			}
		}
	}
}
