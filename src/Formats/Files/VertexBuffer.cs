using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class VertexBuffer : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public uint Unknown_Ch;

		public uint Unknown_1Ch;

		public ulong Unknown_28h;

		public ulong Unknown_38h;

		public ulong Unknown_40h;

		public ulong Unknown_48h;

		public ulong Unknown_50h;

		public ulong Unknown_58h;

		public ulong Unknown_60h;

		public ulong Unknown_68h;

		public ulong Unknown_70h;

		public ulong Unknown_78h;

		public ushort G9_Unknown_Eh;

		public uint G9_Unknown_14h;

		public ulong G9_Unknown_20h;

		public override long BlockLength => 128L;

		public override long BlockLength_Gen9 => 64L;

		public uint VFT { get; set; } = 1080153080u;

		public ushort VertexStride { get; set; }

		public ushort Flags { get; set; }

		public ulong DataPointer1 { get; set; }

		public uint VertexCount { get; set; }

		public ulong DataPointer2 { get; set; }

		public ulong InfoPointer { get; set; }

		public uint G9_BindFlags { get; set; }

		public ulong G9_SRVPointer { get; set; }

		public ShaderResourceViewG9 G9_SRV { get; set; }

		public VertexDeclarationG9 G9_Info { get; set; }

		public VertexData Data1 { get; set; }

		public VertexData Data2 { get; set; }

		public VertexDeclaration Info { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			if (reader.IsGen9)
			{
				VertexCount = reader.ReadUInt32();
				VertexStride = reader.ReadUInt16();
				G9_Unknown_Eh = reader.ReadUInt16();
				G9_BindFlags = reader.ReadUInt32();
				G9_Unknown_14h = reader.ReadUInt32();
				DataPointer1 = reader.ReadUInt64();
				G9_Unknown_20h = reader.ReadUInt64();
				Unknown_28h = reader.ReadUInt64();
				G9_SRVPointer = reader.ReadUInt64();
				InfoPointer = reader.ReadUInt64();
				G9_SRV = reader.ReadBlockAt<ShaderResourceViewG9>(G9_SRVPointer, Array.Empty<object>());
				G9_Info = reader.ReadBlockAt<VertexDeclarationG9>(InfoPointer, Array.Empty<object>());
				uint count = VertexCount * VertexStride;
				byte[] gen9bytes = reader.ReadBytesAt(DataPointer1, count);
				InitVertexDataFromGen9Data(gen9bytes);
				_ = G9_Unknown_Eh;
				uint g9_BindFlags = G9_BindFlags;
				if (g9_BindFlags != 5768201)
				{
					_ = 5792777;
				}
				_ = G9_Unknown_14h;
				_ = G9_Unknown_20h;
				_ = Unknown_28h;
			}
			else
			{
				VertexStride = reader.ReadUInt16();
				Flags = reader.ReadUInt16();
				Unknown_Ch = reader.ReadUInt32();
				DataPointer1 = reader.ReadUInt64();
				VertexCount = reader.ReadUInt32();
				Unknown_1Ch = reader.ReadUInt32();
				DataPointer2 = reader.ReadUInt64();
				Unknown_28h = reader.ReadUInt64();
				InfoPointer = reader.ReadUInt64();
				Unknown_38h = reader.ReadUInt64();
				Unknown_40h = reader.ReadUInt64();
				Unknown_48h = reader.ReadUInt64();
				Unknown_50h = reader.ReadUInt64();
				Unknown_58h = reader.ReadUInt64();
				Unknown_60h = reader.ReadUInt64();
				Unknown_68h = reader.ReadUInt64();
				Unknown_70h = reader.ReadUInt64();
				Unknown_78h = reader.ReadUInt64();
				Info = reader.ReadBlockAt<VertexDeclaration>(InfoPointer, Array.Empty<object>());
				Data1 = reader.ReadBlockAt<VertexData>(DataPointer1, new object[3] { VertexStride, VertexCount, Info });
				Data2 = reader.ReadBlockAt<VertexData>(DataPointer2, new object[3] { VertexStride, VertexCount, Info });
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			VertexCount = ((Data1 != null) ? ((uint)Data1.VertexCount) : ((Data2 != null) ? ((uint)Data2.VertexCount) : 0u));
			DataPointer1 = (ulong)((Data1 != null) ? Data1.FilePosition : 0);
			DataPointer2 = (ulong)((Data2 != null) ? Data2.FilePosition : 0);
			InfoPointer = (ulong)((Info != null) ? Info.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			if (writer.IsGen9)
			{
				G9_SRVPointer = (ulong)((G9_SRV != null) ? G9_SRV.FilePosition : 0);
				InfoPointer = (ulong)((G9_Info != null) ? G9_Info.FilePosition : 0);
				if (G9_BindFlags == 0)
				{
					G9_BindFlags = 5768201u;
				}
				writer.Write(VertexCount);
				writer.Write(VertexStride);
				writer.Write(G9_Unknown_Eh);
				writer.Write(G9_BindFlags);
				writer.Write(G9_Unknown_14h);
				writer.Write(DataPointer1);
				writer.Write(G9_Unknown_20h);
				writer.Write(Unknown_28h);
				writer.Write(G9_SRVPointer);
				writer.Write(InfoPointer);
			}
			else
			{
				writer.Write(VertexStride);
				writer.Write(Flags);
				writer.Write(Unknown_Ch);
				writer.Write(DataPointer1);
				writer.Write(VertexCount);
				writer.Write(Unknown_1Ch);
				writer.Write(DataPointer2);
				writer.Write(Unknown_28h);
				writer.Write(InfoPointer);
				writer.Write(Unknown_38h);
				writer.Write(Unknown_40h);
				writer.Write(Unknown_48h);
				writer.Write(Unknown_50h);
				writer.Write(Unknown_58h);
				writer.Write(Unknown_60h);
				writer.Write(Unknown_68h);
				writer.Write(Unknown_70h);
				writer.Write(Unknown_78h);
			}
		}

		public void InitVertexDataFromGen9Data(byte[] gen9bytes)
		{
			if (gen9bytes == null || G9_Info == null)
			{
				return;
			}
			byte[] types = G9_Info.Types;
			byte[] sizes = G9_Info.Sizes;
			uint[] offsets = G9_Info.Offsets;
			VertexDeclaration legacyDeclaration = G9_Info.GetLegacyDeclaration();
			VertexDeclarationTypes types2 = legacyDeclaration.Types;
			VertexType flags = (VertexType)legacyDeclaration.Flags;
			byte[] array = new byte[gen9bytes.Length];
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == 0)
				{
					continue;
				}
				int legacyComponentIndex = VertexDeclarationG9.GetLegacyComponentIndex(i, types2);
				if (legacyComponentIndex >= 0)
				{
					int num = sizes[i];
					int num2 = (int)offsets[i];
					int componentOffset = legacyDeclaration.GetComponentOffset(legacyComponentIndex);
					int sizeInBytes = VertexComponentTypes.GetSizeInBytes(legacyDeclaration.GetComponentType(legacyComponentIndex));
					for (int j = 0; j < VertexCount; j++)
					{
						int srcOffset = num2 + num * j;
						int dstOffset = componentOffset + VertexStride * j;
						Buffer.BlockCopy(gen9bytes, srcOffset, array, dstOffset, sizeInBytes);
					}
				}
			}
			VertexData vertexData = new VertexData();
			vertexData.VertexStride = VertexStride;
			vertexData.VertexCount = (int)VertexCount;
			vertexData.Info = legacyDeclaration;
			vertexData.VertexType = flags;
			vertexData.VertexBytes = array;
			Data1 = vertexData;
			Data2 = vertexData;
			Info = legacyDeclaration;
		}

		public byte[] InitGen9DataFromVertexData()
		{
			if (Info == null)
			{
				return null;
			}
			if (Data1?.VertexBytes == null)
			{
				return null;
			}
			VertexDeclaration info = Info;
			VertexDeclarationTypes types = info.Types;
			VertexDeclarationG9 vertexDeclarationG = VertexDeclarationG9.FromLegacyDeclaration(info);
			uint[] offsets = vertexDeclarationG.Offsets;
			byte[] sizes = vertexDeclarationG.Sizes;
			byte[] types2 = vertexDeclarationG.Types;
			if (G9_Info != null)
			{
				_ = vertexDeclarationG.VertexSize;
				_ = G9_Info.VertexSize;
				_ = vertexDeclarationG.VertexCount;
				_ = G9_Info.VertexCount;
				_ = vertexDeclarationG.ElementCount;
				_ = G9_Info.ElementCount;
				for (int i = 0; i < 52; i++)
				{
					_ = vertexDeclarationG.Offsets[i];
					_ = G9_Info.Offsets[i];
					_ = vertexDeclarationG.Sizes[i];
					_ = G9_Info.Sizes[i];
					_ = vertexDeclarationG.Types[i];
					_ = G9_Info.Types[i];
				}
			}
			G9_Info = vertexDeclarationG;
			byte[] vertexBytes = Data1.VertexBytes;
			byte[] array = new byte[vertexBytes.Length];
			for (int j = 0; j < types2.Length; j++)
			{
				if (types2[j] == 0)
				{
					continue;
				}
				int legacyComponentIndex = VertexDeclarationG9.GetLegacyComponentIndex(j, types);
				if (legacyComponentIndex >= 0)
				{
					int num = sizes[j];
					int num2 = (int)offsets[j];
					int componentOffset = info.GetComponentOffset(legacyComponentIndex);
					int sizeInBytes = VertexComponentTypes.GetSizeInBytes(info.GetComponentType(legacyComponentIndex));
					for (int k = 0; k < VertexCount; k++)
					{
						int srcOffset = componentOffset + VertexStride * k;
						int dstOffset = num2 + num * k;
						Buffer.BlockCopy(vertexBytes, srcOffset, array, dstOffset, sizeInBytes);
					}
				}
			}
			return array;
		}

		public void EnsureGen9()
		{
			VFT = 1080153080u;
			Unknown_4h = 1u;
			if (Data1 == null && Data2 != null)
			{
				Data1 = Data2;
			}
			if (Data1 != null)
			{
				Data1.G9_VertexBytes = InitGen9DataFromVertexData();
			}
			Data2 = Data1;
			if (G9_SRV == null)
			{
				G9_SRV = new ShaderResourceViewG9();
				G9_SRV.Dimension = ShaderResourceViewDimensionG9.Buffer;
			}
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Data1 != null)
			{
				list.Add(Data1);
			}
			if (Data2 != null)
			{
				list.Add(Data2);
			}
			if (G9_SRV != null)
			{
				list.Add(G9_SRV);
			}
			if (G9_Info != null)
			{
				list.Add(G9_Info);
			}
			else if (Info != null)
			{
				list.Add(Info);
			}
			return list.ToArray();
		}
	}
}
