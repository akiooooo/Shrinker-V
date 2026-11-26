using System;
using System.ComponentModel;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundPolygonBox : BoundPolygon
	{
		public uint boxType { get; set; }

		public short boxIndex1 { get; set; }

		public short boxIndex2 { get; set; }

		public short boxIndex3 { get; set; }

		public short boxIndex4 { get; set; }

		public uint unused0 { get; set; }

		public Vector3 Vertex1
		{
			get
			{
				if (base.Owner == null)
				{
					return Vector3.Zero;
				}
				return base.Owner.GetVertexPos(boxIndex1);
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
				return base.Owner.GetVertexPos(boxIndex2);
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
				return base.Owner.GetVertexPos(boxIndex3);
			}
		}

		public Vector3 Vertex4
		{
			get
			{
				if (base.Owner == null)
				{
					return Vector3.Zero;
				}
				return base.Owner.GetVertexPos(boxIndex4);
			}
		}

		public override Vector3 BoxMin => Vector3.Min(Vector3.Min(Vector3.Min(Vertex1, Vertex2), Vertex3), Vertex4);

		public override Vector3 BoxMax => Vector3.Max(Vector3.Max(Vector3.Max(Vertex1, Vertex2), Vertex3), Vertex4);

		public override Vector3 Position => (Vertex1 + Vertex2 + Vertex3 + Vertex4) * 0.25f;

		public BoundPolygonBox()
		{
			base.Type = BoundPolygonType.Box;
		}

		public override void Read(byte[] bytes, int offset)
		{
			boxType = BitConverter.ToUInt32(bytes, offset);
			boxIndex1 = BitConverter.ToInt16(bytes, offset + 4);
			boxIndex2 = BitConverter.ToInt16(bytes, offset + 6);
			boxIndex3 = BitConverter.ToInt16(bytes, offset + 8);
			boxIndex4 = BitConverter.ToInt16(bytes, offset + 10);
			unused0 = BitConverter.ToUInt32(bytes, offset + 12);
		}

		public override void Write(BinaryWriter bw)
		{
			bw.Write(boxType);
			bw.Write(boxIndex1);
			bw.Write(boxIndex2);
			bw.Write(boxIndex3);
			bw.Write(boxIndex4);
			bw.Write(unused0);
		}

		public override string ToString()
		{
			return base.ToString() + ": " + boxIndex1 + ", " + boxIndex2 + ", " + boxIndex3 + ", " + boxIndex4;
		}
	}
}
