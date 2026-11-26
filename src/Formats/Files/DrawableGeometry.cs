using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DrawableGeometry : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public ulong Unknown_10h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public ulong Unknown_30h;

		public ulong Unknown_40h;

		public ulong Unknown_48h;

		public ulong Unknown_50h;

		public ushort Unknown_62h = 3;

		public uint Unknown_64h;

		public uint Unknown_74h;

		public ulong Unknown_80h;

		public ulong Unknown_88h;

		public ulong Unknown_90h;

		[CompilerGenerated]
		private AABB_s _003CAABB_003Ek__BackingField;

		public override long BlockLength
		{
			get
			{
				long num = 152L;
				if (BoneIds != null)
				{
					if (BoneIds.Length > 4)
					{
						num += 8;
					}
					num += BoneIds.Length * 2;
				}
				return num;
			}
		}

		public uint VFT { get; set; } = 1080133528u;

		public ulong VertexBufferPointer { get; set; }

		public ulong IndexBufferPointer { get; set; }

		public uint IndicesCount { get; set; }

		public uint TrianglesCount { get; set; }

		public ushort VerticesCount { get; set; }

		public ulong BoneIdsPointer { get; set; }

		public ushort VertexStride { get; set; }

		public ushort BoneIdsCount { get; set; }

		public ulong VertexDataPointer { get; set; }

		public VertexBuffer VertexBuffer { get; set; }

		public IndexBuffer IndexBuffer { get; set; }

		public VertexData VertexData { get; set; }

		public ushort[] BoneIds { get; set; }

		public ShaderFX Shader { get; set; }

		public ushort ShaderID { get; set; }

		public AABB_s AABB
		{
			[CompilerGenerated]
			set
			{
				_003CAABB_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt64();
			VertexBufferPointer = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			Unknown_30h = reader.ReadUInt64();
			IndexBufferPointer = reader.ReadUInt64();
			Unknown_40h = reader.ReadUInt64();
			Unknown_48h = reader.ReadUInt64();
			Unknown_50h = reader.ReadUInt64();
			IndicesCount = reader.ReadUInt32();
			TrianglesCount = reader.ReadUInt32();
			VerticesCount = reader.ReadUInt16();
			Unknown_62h = reader.ReadUInt16();
			Unknown_64h = reader.ReadUInt32();
			BoneIdsPointer = reader.ReadUInt64();
			VertexStride = reader.ReadUInt16();
			BoneIdsCount = reader.ReadUInt16();
			Unknown_74h = reader.ReadUInt32();
			VertexDataPointer = reader.ReadUInt64();
			Unknown_80h = reader.ReadUInt64();
			Unknown_88h = reader.ReadUInt64();
			Unknown_90h = reader.ReadUInt64();
			VertexBuffer = reader.ReadBlockAt<VertexBuffer>(VertexBufferPointer, Array.Empty<object>());
			IndexBuffer = reader.ReadBlockAt<IndexBuffer>(IndexBufferPointer, Array.Empty<object>());
			BoneIds = reader.ReadUshortsAt(BoneIdsPointer, BoneIdsCount, cache: false);
			_ = BoneIds;
			if (VertexBuffer != null)
			{
				VertexData = VertexBuffer.Data1 ?? VertexBuffer.Data2;
				if (VerticesCount == 0)
				{
					VerticesCount = (ushort)(VertexData?.VertexCount ?? 0);
				}
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			VertexBufferPointer = (ulong)((VertexBuffer != null) ? VertexBuffer.FilePosition : 0);
			IndexBufferPointer = (ulong)((IndexBuffer != null) ? IndexBuffer.FilePosition : 0);
			VertexDataPointer = (ulong)((VertexData != null) ? VertexData.FilePosition : 0);
			VerticesCount = (ushort)((VertexData != null) ? ((uint)VertexData.VertexCount) : 0u);
			VertexStride = (ushort)((VertexBuffer != null) ? VertexBuffer.VertexStride : 0);
			IndicesCount = ((IndexBuffer != null) ? IndexBuffer.IndicesCount : 0u);
			TrianglesCount = IndicesCount / 3;
			BoneIdsPointer = (ulong)((BoneIds != null) ? (writer.Position + 152 + ((BoneIds.Length > 4) ? 8 : 0)) : 0);
			ushort[] boneIds = BoneIds;
			BoneIdsCount = (ushort)((boneIds != null) ? ((uint)boneIds.Length) : 0u);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(Unknown_10h);
			writer.Write(VertexBufferPointer);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
			writer.Write(Unknown_30h);
			writer.Write(IndexBufferPointer);
			writer.Write(Unknown_40h);
			writer.Write(Unknown_48h);
			writer.Write(Unknown_50h);
			writer.Write(IndicesCount);
			writer.Write(TrianglesCount);
			writer.Write(VerticesCount);
			writer.Write(Unknown_62h);
			writer.Write(Unknown_64h);
			writer.Write(BoneIdsPointer);
			writer.Write(VertexStride);
			writer.Write(BoneIdsCount);
			writer.Write(Unknown_74h);
			writer.Write(VertexDataPointer);
			writer.Write(Unknown_80h);
			writer.Write(Unknown_88h);
			writer.Write(Unknown_90h);
			if (BoneIds != null)
			{
				if (BoneIds.Length > 4)
				{
					writer.Write(0uL);
				}
				for (int i = 0; i < BoneIds.Length; i++)
				{
					writer.Write(BoneIds[i]);
				}
			}
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (VertexBuffer != null)
			{
				list.Add(VertexBuffer);
			}
			if (IndexBuffer != null)
			{
				list.Add(IndexBuffer);
			}
			if (VertexData != null)
			{
				list.Add(VertexData);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return VerticesCount + " verts, " + Shader.ToString();
		}
	}
}
