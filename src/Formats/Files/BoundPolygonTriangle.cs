using System;
using System.ComponentModel;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundPolygonTriangle : BoundPolygon
	{
		public float triArea { get; set; }

		public ushort triIndex1 { get; set; }

		public ushort triIndex2 { get; set; }

		public ushort triIndex3 { get; set; }

		public ushort edgeIndex1 { get; set; }

		public ushort edgeIndex2 { get; set; }

		public ushort edgeIndex3 { get; set; }

		public int vertIndex1 => triIndex1 & 0x7FFF;

		public int vertIndex2 => triIndex2 & 0x7FFF;

		public int vertIndex3 => triIndex3 & 0x7FFF;

		public Vector3 Vertex1
		{
			get
			{
				if (base.Owner == null)
				{
					return Vector3.Zero;
				}
				return base.Owner.GetVertexPos(vertIndex1);
			}
		}

		public Vector3 Vertex2
		{
			get
			{
				if (base.Owner == null)
				{
					return Vector3.Zero;
				}
				return base.Owner.GetVertexPos(vertIndex2);
			}
		}

		public Vector3 Vertex3
		{
			get
			{
				if (base.Owner == null)
				{
					return Vector3.Zero;
				}
				return base.Owner.GetVertexPos(vertIndex3);
			}
		}

		public override Vector3 BoxMin => Vector3.Min(Vector3.Min(Vertex1, Vertex2), Vertex3);

		public override Vector3 BoxMax => Vector3.Max(Vector3.Max(Vertex1, Vertex2), Vertex3);

		public override Vector3 Position => (Vertex1 + Vertex2 + Vertex3) * (1f / 3f);

		public void SetEdgeIndex(int i, int polyindex)
		{
			ushort num = PackEdgeIndex(polyindex);
			switch (i)
			{
			case 0:
				_ = edgeIndex1;
				edgeIndex1 = num;
				break;
			case 1:
				_ = edgeIndex2;
				edgeIndex2 = num;
				break;
			case 2:
				_ = edgeIndex3;
				edgeIndex3 = num;
				break;
			}
		}

		public static ushort PackEdgeIndex(int polyIndex)
		{
			if (polyIndex < 0)
			{
				return ushort.MaxValue;
			}
			if (polyIndex > 65535)
			{
				return ushort.MaxValue;
			}
			return (ushort)polyIndex;
		}

		public BoundPolygonTriangle()
		{
			base.Type = BoundPolygonType.Triangle;
		}

		public override void Read(byte[] bytes, int offset)
		{
			triArea = BitConverter.ToSingle(bytes, offset);
			triIndex1 = BitConverter.ToUInt16(bytes, offset + 4);
			triIndex2 = BitConverter.ToUInt16(bytes, offset + 6);
			triIndex3 = BitConverter.ToUInt16(bytes, offset + 8);
			edgeIndex1 = BitConverter.ToUInt16(bytes, offset + 10);
			edgeIndex2 = BitConverter.ToUInt16(bytes, offset + 12);
			edgeIndex3 = BitConverter.ToUInt16(bytes, offset + 14);
		}

		public override void Write(BinaryWriter bw)
		{
			bw.Write(triArea);
			bw.Write(triIndex1);
			bw.Write(triIndex2);
			bw.Write(triIndex3);
			bw.Write(edgeIndex1);
			bw.Write(edgeIndex2);
			bw.Write(edgeIndex3);
		}

		public override string ToString()
		{
			return base.ToString() + ": " + vertIndex1 + ", " + vertIndex2 + ", " + vertIndex3;
		}
	}
}
