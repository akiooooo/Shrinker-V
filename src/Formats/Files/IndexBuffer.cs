using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class IndexBuffer : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public uint Unknown_Ch;

		public ulong Unknown_18h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public ulong Unknown_30h;

		public ulong Unknown_38h;

		public ulong Unknown_40h;

		public ulong Unknown_48h;

		public ulong Unknown_50h;

		public ulong Unknown_58h;

		public ushort G9_Unknown_Eh;

		public uint G9_Unknown_14h;

		private ResourceSystemStructBlock<ushort> IndicesBlock;

		public override long BlockLength => 96L;

		public override long BlockLength_Gen9 => 64L;

		public uint VFT { get; set; } = 1080152408u;

		public uint IndicesCount { get; set; }

		public ulong IndicesPointer { get; set; }

		public ushort G9_IndexSize { get; set; } = 2;

		public uint G9_BindFlags { get; set; }

		public ulong G9_SRVPointer { get; set; }

		public ShaderResourceViewG9 G9_SRV { get; set; }

		public ushort[] Indices { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			IndicesCount = reader.ReadUInt32();
			if (reader.IsGen9)
			{
				G9_IndexSize = reader.ReadUInt16();
				G9_Unknown_Eh = reader.ReadUInt16();
				G9_BindFlags = reader.ReadUInt32();
				G9_Unknown_14h = reader.ReadUInt32();
				IndicesPointer = reader.ReadUInt64();
				Unknown_20h = reader.ReadUInt64();
				Unknown_28h = reader.ReadUInt64();
				G9_SRVPointer = reader.ReadUInt64();
				Unknown_38h = reader.ReadUInt64();
				Indices = reader.ReadUshortsAt(IndicesPointer, IndicesCount);
				G9_SRV = reader.ReadBlockAt<ShaderResourceViewG9>(G9_SRVPointer, Array.Empty<object>());
				_ = G9_IndexSize;
				_ = 2;
				_ = G9_Unknown_Eh;
				_ = G9_BindFlags;
				_ = 5767690;
				_ = G9_Unknown_14h;
				_ = Unknown_20h;
				_ = Unknown_28h;
				_ = Unknown_38h;
			}
			else
			{
				Unknown_Ch = reader.ReadUInt32();
				IndicesPointer = reader.ReadUInt64();
				Unknown_18h = reader.ReadUInt64();
				Unknown_20h = reader.ReadUInt64();
				Unknown_28h = reader.ReadUInt64();
				Unknown_30h = reader.ReadUInt64();
				Unknown_38h = reader.ReadUInt64();
				Unknown_40h = reader.ReadUInt64();
				Unknown_48h = reader.ReadUInt64();
				Unknown_50h = reader.ReadUInt64();
				Unknown_58h = reader.ReadUInt64();
				Indices = reader.ReadUshortsAt(IndicesPointer, IndicesCount);
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			IndicesCount = ((IndicesBlock != null) ? ((uint)IndicesBlock.ItemCount) : 0u);
			IndicesPointer = (ulong)((IndicesBlock != null) ? IndicesBlock.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(IndicesCount);
			if (writer.IsGen9)
			{
				G9_SRVPointer = (ulong)((G9_SRV != null) ? G9_SRV.FilePosition : 0);
				if (G9_BindFlags == 0)
				{
					G9_BindFlags = 5767690u;
				}
				writer.Write(G9_IndexSize);
				writer.Write(G9_Unknown_Eh);
				writer.Write(G9_BindFlags);
				writer.Write(G9_Unknown_14h);
				writer.Write(IndicesPointer);
				writer.Write(Unknown_20h);
				writer.Write(Unknown_28h);
				writer.Write(G9_SRVPointer);
				writer.Write(Unknown_38h);
			}
			else
			{
				writer.Write(Unknown_Ch);
				writer.Write(IndicesPointer);
				writer.Write(Unknown_18h);
				writer.Write(Unknown_20h);
				writer.Write(Unknown_28h);
				writer.Write(Unknown_30h);
				writer.Write(Unknown_38h);
				writer.Write(Unknown_40h);
				writer.Write(Unknown_48h);
				writer.Write(Unknown_50h);
				writer.Write(Unknown_58h);
			}
		}

		public void EnsureGen9()
		{
			VFT = 1080152408u;
			Unknown_4h = 1u;
			if (G9_SRV == null)
			{
				G9_SRV = new ShaderResourceViewG9();
				G9_SRV.Dimension = ShaderResourceViewDimensionG9.Buffer;
			}
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Indices != null)
			{
				IndicesBlock = new ResourceSystemStructBlock<ushort>(Indices);
				list.Add(IndicesBlock);
			}
			if (G9_SRV != null)
			{
				list.Add(G9_SRV);
			}
			return list.ToArray();
		}
	}
}
