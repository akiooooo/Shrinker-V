using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundGeometry : Bounds
	{
		private ResourceSystemStructBlock<BoundVertex_s> VerticesShrunkBlock;

		private ResourceSystemDataBlock PolygonsBlock;

		private ResourceSystemStructBlock<BoundVertex_s> VerticesBlock;

		private ResourceSystemStructBlock<BoundMaterialColour> VertexColoursBlock;

		private ResourceSystemStructBlock<BoundMaterial_s> MaterialsBlock;

		private ResourceSystemStructBlock<BoundMaterialColour> MaterialColoursBlock;

		private ResourceSystemStructBlock<byte> PolygonMaterialIndicesBlock;

		public override long BlockLength => 304L;

		public uint Unknown_70h { get; set; }

		public uint Unknown_74h { get; set; }

		public ulong VerticesShrunkPointer { get; set; }

		public ushort Unknown_80h { get; set; }

		public ushort Unknown_82h { get; set; }

		public uint VerticesShrunkCount { get; set; }

		public ulong PolygonsPointer { get; set; }

		public Vector3 Quantum { get; set; }

		public float Unknown_9Ch { get; set; }

		public Vector3 CenterGeom { get; set; }

		public float Unknown_ACh { get; set; }

		public ulong VerticesPointer { get; set; }

		public ulong VertexColoursPointer { get; set; }

		public ulong OctantsPointer { get; set; }

		public ulong OctantItemsPointer { get; set; }

		public uint VerticesCount { get; set; }

		public uint PolygonsCount { get; set; }

		public uint Unknown_D8h { get; set; }

		public uint Unknown_DCh { get; set; }

		public uint Unknown_E0h { get; set; }

		public uint Unknown_E4h { get; set; }

		public uint Unknown_E8h { get; set; }

		public uint Unknown_ECh { get; set; }

		public ulong MaterialsPointer { get; set; }

		public ulong MaterialColoursPointer { get; set; }

		public uint Unknown_100h { get; set; }

		public uint Unknown_104h { get; set; }

		public uint Unknown_108h { get; set; }

		public uint Unknown_10Ch { get; set; }

		public uint Unknown_110h { get; set; }

		public uint Unknown_114h { get; set; }

		public ulong PolygonMaterialIndicesPointer { get; set; }

		public byte MaterialsCount { get; set; }

		public byte MaterialColoursCount { get; set; }

		public ushort Unknown_122h { get; set; }

		public uint Unknown_124h { get; set; }

		public uint Unknown_128h { get; set; }

		public uint Unknown_12Ch { get; set; }

		public Vector3[] VerticesShrunk { get; set; }

		public BoundPolygon[] Polygons { get; set; }

		public Vector3[] Vertices { get; set; }

		public BoundMaterialColour[] VertexColours { get; set; }

		public BoundGeomOctants Octants { get; set; }

		public BoundMaterial_s[] Materials { get; set; }

		public BoundMaterialColour[] MaterialColours { get; set; }

		public byte[] PolygonMaterialIndices { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_70h = reader.ReadUInt32();
			Unknown_74h = reader.ReadUInt32();
			VerticesShrunkPointer = reader.ReadUInt64();
			Unknown_80h = reader.ReadUInt16();
			Unknown_82h = reader.ReadUInt16();
			VerticesShrunkCount = reader.ReadUInt32();
			PolygonsPointer = reader.ReadUInt64();
			Quantum = reader.ReadVector3();
			Unknown_9Ch = reader.ReadSingle();
			CenterGeom = reader.ReadVector3();
			Unknown_ACh = reader.ReadSingle();
			VerticesPointer = reader.ReadUInt64();
			VertexColoursPointer = reader.ReadUInt64();
			OctantsPointer = reader.ReadUInt64();
			OctantItemsPointer = reader.ReadUInt64();
			VerticesCount = reader.ReadUInt32();
			PolygonsCount = reader.ReadUInt32();
			Unknown_D8h = reader.ReadUInt32();
			Unknown_DCh = reader.ReadUInt32();
			Unknown_E0h = reader.ReadUInt32();
			Unknown_E4h = reader.ReadUInt32();
			Unknown_E8h = reader.ReadUInt32();
			Unknown_ECh = reader.ReadUInt32();
			MaterialsPointer = reader.ReadUInt64();
			MaterialColoursPointer = reader.ReadUInt64();
			Unknown_100h = reader.ReadUInt32();
			Unknown_104h = reader.ReadUInt32();
			Unknown_108h = reader.ReadUInt32();
			Unknown_10Ch = reader.ReadUInt32();
			Unknown_110h = reader.ReadUInt32();
			Unknown_114h = reader.ReadUInt32();
			PolygonMaterialIndicesPointer = reader.ReadUInt64();
			MaterialsCount = reader.ReadByte();
			MaterialColoursCount = reader.ReadByte();
			Unknown_122h = reader.ReadUInt16();
			Unknown_124h = reader.ReadUInt32();
			Unknown_128h = reader.ReadUInt32();
			Unknown_12Ch = reader.ReadUInt32();
			BoundVertex_s[] array = reader.ReadStructsAt<BoundVertex_s>(VerticesShrunkPointer, VerticesShrunkCount);
			if (array != null)
			{
				VerticesShrunk = new Vector3[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					BoundVertex_s boundVertex_s = array[i];
					VerticesShrunk[i] = boundVertex_s.Vector * Quantum;
				}
			}
			ReadPolygons(reader);
			BoundVertex_s[] array2 = reader.ReadStructsAt<BoundVertex_s>(VerticesPointer, VerticesCount);
			if (array2 != null)
			{
				Vertices = new Vector3[array2.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					BoundVertex_s boundVertex_s2 = array2[j];
					Vertices[j] = boundVertex_s2.Vector * Quantum;
				}
			}
			VertexColours = reader.ReadStructsAt<BoundMaterialColour>(VertexColoursPointer, VerticesCount);
			Octants = reader.ReadBlockAt<BoundGeomOctants>(OctantsPointer, new object[1] { OctantItemsPointer });
			Materials = reader.ReadStructsAt<BoundMaterial_s>(MaterialsPointer, (uint)((MaterialsCount < 4) ? 4 : MaterialsCount));
			MaterialColours = reader.ReadStructsAt<BoundMaterialColour>(MaterialColoursPointer, MaterialColoursCount);
			PolygonMaterialIndices = reader.ReadBytesAt(PolygonMaterialIndicesPointer, PolygonsCount);
			if (MaterialsPointer != 0L && MaterialsCount < 4)
			{
				BoundMaterial_s[] array3 = new BoundMaterial_s[MaterialsCount];
				for (int k = 0; k < MaterialsCount; k++)
				{
					array3[k] = Materials[k];
				}
				Materials = array3;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			VerticesShrunkPointer = (ulong)((VerticesShrunkBlock != null) ? VerticesShrunkBlock.FilePosition : 0);
			PolygonsPointer = (ulong)((PolygonsBlock != null) ? PolygonsBlock.FilePosition : 0);
			VerticesPointer = (ulong)((VerticesBlock != null) ? VerticesBlock.FilePosition : 0);
			VertexColoursPointer = (ulong)((VertexColoursBlock != null) ? VertexColoursBlock.FilePosition : 0);
			OctantsPointer = (ulong)((Octants != null) ? Octants.FilePosition : 0);
			OctantItemsPointer = ((OctantsPointer != 0L) ? (OctantsPointer + 32) : 0);
			VerticesCount = ((VerticesBlock != null) ? ((uint)VerticesBlock.ItemCount) : 0u);
			VerticesShrunkCount = VerticesCount;
			PolygonsCount = ((Polygons != null) ? ((uint)Polygons.Length) : 0u);
			MaterialsPointer = (ulong)((MaterialsBlock != null) ? MaterialsBlock.FilePosition : 0);
			MaterialColoursPointer = (ulong)((MaterialColoursBlock != null) ? MaterialColoursBlock.FilePosition : 0);
			PolygonMaterialIndicesPointer = (ulong)((PolygonMaterialIndicesBlock != null) ? PolygonMaterialIndicesBlock.FilePosition : 0);
			MaterialColoursCount = (byte)((MaterialColoursBlock != null) ? ((uint)MaterialColoursBlock.ItemCount) : 0u);
			writer.Write(Unknown_70h);
			writer.Write(Unknown_74h);
			writer.Write(VerticesShrunkPointer);
			writer.Write(Unknown_80h);
			writer.Write(Unknown_82h);
			writer.Write(VerticesShrunkCount);
			writer.Write(PolygonsPointer);
			writer.Write(Quantum);
			writer.Write(Unknown_9Ch);
			writer.Write(CenterGeom);
			writer.Write(Unknown_ACh);
			writer.Write(VerticesPointer);
			writer.Write(VertexColoursPointer);
			writer.Write(OctantsPointer);
			writer.Write(OctantItemsPointer);
			writer.Write(VerticesCount);
			writer.Write(PolygonsCount);
			writer.Write(Unknown_D8h);
			writer.Write(Unknown_DCh);
			writer.Write(Unknown_E0h);
			writer.Write(Unknown_E4h);
			writer.Write(Unknown_E8h);
			writer.Write(Unknown_ECh);
			writer.Write(MaterialsPointer);
			writer.Write(MaterialColoursPointer);
			writer.Write(Unknown_100h);
			writer.Write(Unknown_104h);
			writer.Write(Unknown_108h);
			writer.Write(Unknown_10Ch);
			writer.Write(Unknown_110h);
			writer.Write(Unknown_114h);
			writer.Write(PolygonMaterialIndicesPointer);
			writer.Write(MaterialsCount);
			writer.Write(MaterialColoursCount);
			writer.Write(Unknown_122h);
			writer.Write(Unknown_124h);
			writer.Write(Unknown_128h);
			writer.Write(Unknown_12Ch);
		}

		public override IResourceBlock[] GetReferences()
		{
			BuildMaterials();
			CalculateQuantum();
			UpdateEdgeIndices();
			UpdateTriangleAreas();
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (VerticesShrunk != null)
			{
				List<BoundVertex_s> list2 = new List<BoundVertex_s>();
				Vector3[] verticesShrunk = VerticesShrunk;
				for (int i = 0; i < verticesShrunk.Length; i++)
				{
					Vector3 v = verticesShrunk[i] / Quantum;
					BoundVertex_s item = new BoundVertex_s(v);
					list2.Add(item);
				}
				VerticesShrunkBlock = new ResourceSystemStructBlock<BoundVertex_s>(list2.ToArray());
				list.Add(VerticesShrunkBlock);
			}
			if (Polygons != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryWriter bw = new BinaryWriter(memoryStream);
				BoundPolygon[] polygons = Polygons;
				for (int i = 0; i < polygons.Length; i++)
				{
					polygons[i].Write(bw);
				}
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				memoryStream.Read(array, 0, array.Length);
				for (int j = 0; j < Polygons.Length; j++)
				{
					int num = j * 16;
					byte type = (byte)Polygons[j].Type;
					byte b = array[num];
					array[num] = (byte)((b & 0xF8) | (type & 7));
				}
				_ = array.Length;
				_ = Polygons.Length * 16;
				PolygonsBlock = new ResourceSystemDataBlock(array);
				list.Add(PolygonsBlock);
			}
			if (Vertices != null)
			{
				List<BoundVertex_s> list3 = new List<BoundVertex_s>();
				Vector3[] verticesShrunk = Vertices;
				for (int i = 0; i < verticesShrunk.Length; i++)
				{
					Vector3 v2 = verticesShrunk[i] / Quantum;
					BoundVertex_s item2 = new BoundVertex_s(v2);
					list3.Add(item2);
				}
				VerticesBlock = new ResourceSystemStructBlock<BoundVertex_s>(list3.ToArray());
				list.Add(VerticesBlock);
			}
			if (VertexColours != null)
			{
				VertexColoursBlock = new ResourceSystemStructBlock<BoundMaterialColour>(VertexColours);
				list.Add(VertexColoursBlock);
			}
			if (Octants != null)
			{
				list.Add(Octants);
			}
			if (Materials != null)
			{
				BoundMaterial_s[] array2 = Materials;
				if (array2.Length < 4)
				{
					array2 = new BoundMaterial_s[4];
					for (int k = 0; k < Materials.Length; k++)
					{
						array2[k] = Materials[k];
					}
				}
				MaterialsBlock = new ResourceSystemStructBlock<BoundMaterial_s>(array2);
				list.Add(MaterialsBlock);
			}
			if (MaterialColours != null)
			{
				MaterialColoursBlock = new ResourceSystemStructBlock<BoundMaterialColour>(MaterialColours);
				list.Add(MaterialColoursBlock);
			}
			if (PolygonMaterialIndices != null)
			{
				PolygonMaterialIndicesBlock = new ResourceSystemStructBlock<byte>(PolygonMaterialIndices);
				list.Add(PolygonMaterialIndicesBlock);
			}
			return list.ToArray();
		}

		private void ReadPolygons(ResourceDataReader reader)
		{
			if (PolygonsCount == 0)
			{
				return;
			}
			Polygons = new BoundPolygon[PolygonsCount];
			uint count = PolygonsCount * 16;
			byte[] array = reader.ReadBytesAt(PolygonsPointer, count);
			for (int i = 0; i < PolygonsCount; i++)
			{
				int num = i * 16;
				byte b = array[num];
				array[num] = (byte)(b & 0xF8);
				BoundPolygonType type = (BoundPolygonType)(b & 7);
				BoundPolygon boundPolygon = CreatePolygon(type);
				if (boundPolygon != null)
				{
					boundPolygon.Index = i;
					boundPolygon.Read(array, num);
				}
				Polygons[i] = boundPolygon;
			}
		}

		public Vector3 GetVertex(int index)
		{
			if (index < 0 || index >= Vertices.Length)
			{
				return Vector3.Zero;
			}
			return Vertices[index];
		}

		public Vector3 GetVertexPos(int index)
		{
			return Vector3.Transform(GetVertex(index) + CenterGeom, base.Transform).XYZ();
		}

		public int GetMaterialIndex(int polyIndex)
		{
			int result = 0;
			if (PolygonMaterialIndices != null && polyIndex < PolygonMaterialIndices.Length)
			{
				result = PolygonMaterialIndices[polyIndex];
			}
			return result;
		}

		public BoundMaterial_s GetMaterialByIndex(int matIndex)
		{
			if (Materials != null && matIndex < Materials.Length)
			{
				return Materials[matIndex];
			}
			return default(BoundMaterial_s);
		}

		public BoundMaterial_s GetMaterial(int polyIndex)
		{
			int materialIndex = GetMaterialIndex(polyIndex);
			return GetMaterialByIndex(materialIndex);
		}

		public void CalculateQuantum()
		{
			Quantum = (base.BoxMax - base.BoxMin) * 0.5f / 32767f;
		}

		public void BuildMaterials()
		{
			Dictionary<BoundMaterial_s, byte> dictionary = new Dictionary<BoundMaterial_s, byte>();
			List<BoundMaterial_s> list = new List<BoundMaterial_s>();
			List<byte> list2 = new List<byte>();
			if (Polygons != null)
			{
				BoundPolygon[] polygons = Polygons;
				for (int i = 0; i < polygons.Length; i++)
				{
					BoundMaterial_s material = polygons[i].Material;
					if (dictionary.TryGetValue(material, out var value))
					{
						list2.Add(value);
						continue;
					}
					value = (byte)list.Count;
					dictionary.Add(material, value);
					list.Add(material);
					list2.Add(value);
				}
			}
			MaterialsCount = (byte)list.Count;
			Materials = list.ToArray();
			PolygonMaterialIndices = list2.ToArray();
		}

		public void UpdateEdgeIndices()
		{
			if (Polygons == null)
			{
				return;
			}
			for (int i = 0; i < Polygons.Length; i++)
			{
				BoundPolygon boundPolygon = Polygons[i];
				if (boundPolygon != null)
				{
					boundPolygon.Index = i;
				}
			}
			Dictionary<BoundEdgeRef, BoundEdge> dictionary = new Dictionary<BoundEdgeRef, BoundEdge>();
			BoundPolygon[] polygons = Polygons;
			for (int j = 0; j < polygons.Length; j++)
			{
				if (!(polygons[j] is BoundPolygonTriangle boundPolygonTriangle))
				{
					continue;
				}
				BoundEdgeRef key = new BoundEdgeRef(boundPolygonTriangle.vertIndex1, boundPolygonTriangle.vertIndex2);
				BoundEdgeRef key2 = new BoundEdgeRef(boundPolygonTriangle.vertIndex2, boundPolygonTriangle.vertIndex3);
				BoundEdgeRef key3 = new BoundEdgeRef(boundPolygonTriangle.vertIndex3, boundPolygonTriangle.vertIndex1);
				if (dictionary.TryGetValue(key, out var value))
				{
					if (value.Triangle2 != null)
					{
						boundPolygonTriangle.SetEdgeIndex(0, value.Triangle1.Index);
					}
					else
					{
						value.Triangle2 = boundPolygonTriangle;
						value.EdgeID2 = 0;
					}
				}
				else
				{
					dictionary[key] = new BoundEdge(boundPolygonTriangle, 0);
				}
				if (dictionary.TryGetValue(key2, out var value2))
				{
					if (value2.Triangle2 != null)
					{
						boundPolygonTriangle.SetEdgeIndex(1, value2.Triangle1.Index);
					}
					else
					{
						value2.Triangle2 = boundPolygonTriangle;
						value2.EdgeID2 = 1;
					}
				}
				else
				{
					dictionary[key2] = new BoundEdge(boundPolygonTriangle, 1);
				}
				if (dictionary.TryGetValue(key3, out var value3))
				{
					if (value3.Triangle2 != null)
					{
						boundPolygonTriangle.SetEdgeIndex(2, value3.Triangle1.Index);
						continue;
					}
					value3.Triangle2 = boundPolygonTriangle;
					value3.EdgeID2 = 2;
				}
				else
				{
					dictionary[key3] = new BoundEdge(boundPolygonTriangle, 2);
				}
			}
			foreach (KeyValuePair<BoundEdgeRef, BoundEdge> item in dictionary)
			{
				_ = item.Key;
				BoundEdge value4 = item.Value;
				if (value4.Triangle1 != null)
				{
					if (value4.Triangle2 == null)
					{
						value4.Triangle1.SetEdgeIndex(value4.EdgeID1, -1);
						continue;
					}
					value4.Triangle1.SetEdgeIndex(value4.EdgeID1, value4.Triangle2.Index);
					value4.Triangle2.SetEdgeIndex(value4.EdgeID2, value4.Triangle1.Index);
				}
			}
			polygons = Polygons;
			for (int j = 0; j < polygons.Length; j++)
			{
				if (polygons[j] is BoundPolygonTriangle boundPolygonTriangle2)
				{
					_ = boundPolygonTriangle2.edgeIndex1;
					_ = Polygons.Length;
					_ = boundPolygonTriangle2.edgeIndex2;
					_ = Polygons.Length;
					_ = boundPolygonTriangle2.edgeIndex3;
					_ = Polygons.Length;
				}
			}
		}

		public void UpdateTriangleAreas()
		{
			if (Polygons == null)
			{
				return;
			}
			new Dictionary<BoundEdgeRef, BoundEdge>();
			BoundPolygon[] polygons = Polygons;
			for (int i = 0; i < polygons.Length; i++)
			{
				if (polygons[i] is BoundPolygonTriangle boundPolygonTriangle)
				{
					Vector3 v = boundPolygonTriangle.Vertex1;
					Vector3 v2 = boundPolygonTriangle.Vertex2;
					Vector3 v3 = boundPolygonTriangle.Vertex3;
					float triArea = TriangleMath.Area(ref v, ref v2, ref v3);
					boundPolygonTriangle.triArea = triArea;
				}
			}
		}

		private BoundPolygon CreatePolygon(BoundPolygonType type)
		{
			BoundPolygon boundPolygon = null;
			switch (type)
			{
			case BoundPolygonType.Triangle:
				boundPolygon = new BoundPolygonTriangle();
				break;
			case BoundPolygonType.Sphere:
				boundPolygon = new BoundPolygonSphere();
				break;
			case BoundPolygonType.Capsule:
				boundPolygon = new BoundPolygonCapsule();
				break;
			case BoundPolygonType.Box:
				boundPolygon = new BoundPolygonBox();
				break;
			case BoundPolygonType.Cylinder:
				boundPolygon = new BoundPolygonCylinder();
				break;
			}
			if (boundPolygon != null)
			{
				boundPolygon.Owner = this;
			}
			return boundPolygon;
		}
	}
}
