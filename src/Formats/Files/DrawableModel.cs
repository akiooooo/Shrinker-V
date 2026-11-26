using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DrawableModel : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public uint Unknown_14h;

		public override long BlockLength
		{
			get
			{
				long num = 48L;
				num += GeometriesCount1 * 2;
				num = ((GeometriesCount1 != 1) ? (num + (16 - num % 16) % 16) : (num + 6));
				num += GeometriesCount1 * 8;
				num += (16 - num % 16) % 16;
				num += ((int)GeometriesCount1 + ((GeometriesCount1 > 1) ? 1 : 0)) * 32;
				for (int i = 0; i < GeometriesCount1; i++)
				{
					DrawableGeometry drawableGeometry = ((Geometries != null) ? Geometries[i] : null);
					if (drawableGeometry != null)
					{
						num += (16 - num % 16) % 16;
						num += drawableGeometry.BlockLength;
					}
				}
				return num;
			}
		}

		public uint VFT { get; set; } = 1080101528u;

		public ulong GeometriesPointer { get; set; }

		public ushort GeometriesCount1 { get; set; }

		public ushort GeometriesCount2 { get; set; }

		public ulong BoundsPointer { get; set; }

		public ulong ShaderMappingPointer { get; set; }

		public uint SkeletonBinding { get; set; }

		public ushort RenderMaskFlags { get; set; }

		public ushort GeometriesCount3 { get; set; }

		public ushort[] ShaderMapping { get; set; }

		public ulong[] GeometryPointers { get; set; }

		public AABB_s[] BoundsData { get; set; }

		public DrawableGeometry[] Geometries { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			GeometriesPointer = reader.ReadUInt64();
			GeometriesCount1 = reader.ReadUInt16();
			GeometriesCount2 = reader.ReadUInt16();
			Unknown_14h = reader.ReadUInt32();
			BoundsPointer = reader.ReadUInt64();
			ShaderMappingPointer = reader.ReadUInt64();
			SkeletonBinding = reader.ReadUInt32();
			RenderMaskFlags = reader.ReadUInt16();
			GeometriesCount3 = reader.ReadUInt16();
			ShaderMapping = reader.ReadUshortsAt(ShaderMappingPointer, GeometriesCount1, cache: false);
			GeometryPointers = reader.ReadUlongsAt(GeometriesPointer, GeometriesCount1, cache: false);
			BoundsData = reader.ReadStructsAt<AABB_s>(BoundsPointer, (uint)((GeometriesCount1 > 1) ? (GeometriesCount1 + 1) : GeometriesCount1), cache: false);
			Geometries = reader.ReadBlocks<DrawableGeometry>(GeometryPointers);
			if (Geometries == null)
			{
				return;
			}
			for (int i = 0; i < Geometries.Length; i++)
			{
				DrawableGeometry drawableGeometry = Geometries[i];
				if (drawableGeometry != null)
				{
					drawableGeometry.ShaderID = (ushort)((ShaderMapping != null && i < ShaderMapping.Length) ? ShaderMapping[i] : 0);
					drawableGeometry.AABB = ((BoundsData == null) ? default(AABB_s) : ((BoundsData.Length > 1 && i + 1 < BoundsData.Length) ? BoundsData[i + 1] : BoundsData[0]));
				}
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			GeometriesCount1 = (ushort)((Geometries != null) ? ((uint)Geometries.Length) : 0u);
			GeometriesCount2 = GeometriesCount1;
			GeometriesCount3 = GeometriesCount1;
			long num = (long)(ShaderMappingPointer = (ulong)(writer.Position + 48));
			num += GeometriesCount1 * 2;
			num = (long)(GeometriesPointer = (ulong)((GeometriesCount1 != 1) ? (num + pad(num)) : (num + 6)));
			num += GeometriesCount1 * 8;
			num = (long)(BoundsPointer = (ulong)(num + pad(num)));
			num += BoundsData.Length * 32;
			GeometryPointers = new ulong[GeometriesCount1];
			for (int i = 0; i < GeometriesCount1; i++)
			{
				DrawableGeometry drawableGeometry = ((Geometries != null) ? Geometries[i] : null);
				if (drawableGeometry != null)
				{
					num += pad(num);
					GeometryPointers[i] = (ulong)num;
					num += drawableGeometry.BlockLength;
				}
			}
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(GeometriesPointer);
			writer.Write(GeometriesCount1);
			writer.Write(GeometriesCount2);
			writer.Write(Unknown_14h);
			writer.Write(BoundsPointer);
			writer.Write(ShaderMappingPointer);
			writer.Write(SkeletonBinding);
			writer.Write(RenderMaskFlags);
			writer.Write(GeometriesCount3);
			for (int j = 0; j < GeometriesCount1; j++)
			{
				writer.Write(ShaderMapping[j]);
			}
			if (GeometriesCount1 == 1)
			{
				writer.Write(new byte[6]);
			}
			else
			{
				writer.WritePadding(16);
			}
			for (int k = 0; k < GeometriesCount1; k++)
			{
				writer.Write(GeometryPointers[k]);
			}
			writer.WritePadding(16);
			for (int l = 0; l < BoundsData.Length; l++)
			{
				writer.WriteStruct(BoundsData[l]);
			}
			for (int m = 0; m < GeometriesCount1; m++)
			{
				DrawableGeometry drawableGeometry2 = ((Geometries != null) ? Geometries[m] : null);
				if (drawableGeometry2 != null)
				{
					writer.WritePadding(16);
					writer.WriteBlock(drawableGeometry2);
				}
			}
			long pad(long o)
			{
				return (16 - o % 16) % 16;
			}
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			List<Tuple<long, IResourceBlock>> list = new List<Tuple<long, IResourceBlock>>();
			list.AddRange(base.GetParts());
			long num = 48L;
			num += GeometriesCount1 * 2;
			num = ((GeometriesCount1 != 1) ? (num + (16 - num % 16) % 16) : (num + 6));
			num += GeometriesCount1 * 8;
			num += (16 - num % 16) % 16;
			num += ((int)GeometriesCount1 + ((GeometriesCount1 > 1) ? 1 : 0)) * 32;
			for (int i = 0; i < GeometriesCount1; i++)
			{
				DrawableGeometry drawableGeometry = ((Geometries != null) ? Geometries[i] : null);
				if (drawableGeometry != null)
				{
					num += (16 - num % 16) % 16;
					list.Add(new Tuple<long, IResourceBlock>(num, drawableGeometry));
					num += drawableGeometry.BlockLength;
				}
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			DrawableGeometry[] geometries = Geometries;
			string text = ((geometries != null) ? geometries.Length : 0).ToString();
			DrawableGeometry[] geometries2 = Geometries;
			return "(" + text + " geometr" + ((((geometries2 != null) ? geometries2.Length : 0) != 1) ? "ies)" : "y)");
		}
	}
}
