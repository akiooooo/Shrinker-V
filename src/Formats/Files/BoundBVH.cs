using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundBVH : BoundGeometry
	{
		public override long BlockLength => 336L;

		public ulong BvhPointer { get; set; }

		public uint Unknown_138h { get; set; }

		public uint Unknown_13Ch { get; set; }

		public ushort Unknown_140h { get; set; } = ushort.MaxValue;

		public ushort Unknown_142h { get; set; }

		public uint Unknown_144h { get; set; }

		public uint Unknown_148h { get; set; }

		public uint Unknown_14Ch { get; set; }

		public BVH BVH { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			BvhPointer = reader.ReadUInt64();
			Unknown_138h = reader.ReadUInt32();
			Unknown_13Ch = reader.ReadUInt32();
			Unknown_140h = reader.ReadUInt16();
			Unknown_142h = reader.ReadUInt16();
			Unknown_144h = reader.ReadUInt32();
			Unknown_148h = reader.ReadUInt32();
			Unknown_14Ch = reader.ReadUInt32();
			if (BvhPointer > 65535)
			{
				BVH = reader.ReadBlockAt<BVH>(BvhPointer, Array.Empty<object>());
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			BvhPointer = (ulong)((BVH != null) ? BVH.FilePosition : 0);
			writer.Write(BvhPointer);
			writer.Write(Unknown_138h);
			writer.Write(Unknown_13Ch);
			writer.Write(Unknown_140h);
			writer.Write(Unknown_142h);
			writer.Write(Unknown_144h);
			writer.Write(Unknown_148h);
			writer.Write(Unknown_14Ch);
		}

		public override IResourceBlock[] GetReferences()
		{
			BuildBVH(updateParent: false);
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (BVH != null)
			{
				list.Add(BVH);
			}
			return list.ToArray();
		}

		public void BuildBVH(bool updateParent = true)
		{
			BoundPolygon[] polygons = base.Polygons;
			if (((polygons != null && polygons.Length != 0) ? 1 : 0) <= (false ? 1 : 0))
			{
				_ = BVH;
				BVH = null;
				return;
			}
			_ = BVH;
			List<BVHBuilderItem> list = new List<BVHBuilderItem>();
			for (int i = 0; i < base.Polygons.Length; i++)
			{
				BoundPolygon boundPolygon = base.Polygons[i];
				if (boundPolygon != null)
				{
					BVHBuilderItem bVHBuilderItem = new BVHBuilderItem();
					bVHBuilderItem.Min = boundPolygon.BoxMin;
					bVHBuilderItem.Max = boundPolygon.BoxMax;
					bVHBuilderItem.Index = i;
					bVHBuilderItem.Polygon = boundPolygon;
					list.Add(bVHBuilderItem);
				}
			}
			BVH bVH = BVHBuilder.Build(list, 4);
			BoundPolygon[] array = new BoundPolygon[list.Count];
			byte[] array2 = new byte[list.Count];
			int[] array3 = new int[list.Count];
			for (int j = 0; j < list.Count; j++)
			{
				BoundPolygon boundPolygon2 = list[j]?.Polygon;
				if (boundPolygon2 != null && boundPolygon2.Index < array3.Length)
				{
					array3[boundPolygon2.Index] = j;
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				BoundPolygon boundPolygon3 = list[k]?.Polygon;
				if (boundPolygon3 != null)
				{
					array[k] = boundPolygon3;
					array2[k] = (byte)((boundPolygon3.Index < base.PolygonMaterialIndices.Length) ? base.PolygonMaterialIndices[boundPolygon3.Index] : 0);
					boundPolygon3.Index = k;
					if (boundPolygon3 is BoundPolygonTriangle boundPolygonTriangle)
					{
						int polyindex = ((boundPolygonTriangle.edgeIndex1 >= 0 && boundPolygonTriangle.edgeIndex1 < array3.Length) ? array3[boundPolygonTriangle.edgeIndex1] : (-1));
						int polyindex2 = ((boundPolygonTriangle.edgeIndex2 >= 0 && boundPolygonTriangle.edgeIndex2 < array3.Length) ? array3[boundPolygonTriangle.edgeIndex2] : (-1));
						int polyindex3 = ((boundPolygonTriangle.edgeIndex3 >= 0 && boundPolygonTriangle.edgeIndex3 < array3.Length) ? array3[boundPolygonTriangle.edgeIndex3] : (-1));
						boundPolygonTriangle.SetEdgeIndex(0, polyindex);
						boundPolygonTriangle.SetEdgeIndex(1, polyindex2);
						boundPolygonTriangle.SetEdgeIndex(2, polyindex3);
					}
				}
			}
			base.Polygons = array;
			base.PolygonMaterialIndices = array2;
			base.BoxMin = bVH.BoundingBoxMin.XYZ();
			base.BoxMax = bVH.BoundingBoxMax.XYZ();
			base.BoxCenter = bVH.BoundingBoxCenter.XYZ();
			base.SphereCenter = base.BoxCenter;
			base.SphereRadius = (base.BoxMax - base.BoxCenter).Length();
			BVH = bVH;
			if (updateParent && base.Parent != null)
			{
				base.Parent.BuildBVH();
			}
		}
	}
}
