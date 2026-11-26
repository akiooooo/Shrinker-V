using System;
using System.ComponentModel;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundPolygonCylinder : BoundPolygon
	{
		public ushort cylinderType { get; set; }

		public ushort cylinderIndex1 { get; set; }

		public float cylinderRadius { get; set; }

		public ushort cylinderIndex2 { get; set; }

		public ushort unused0 { get; set; }

		public uint unused1 { get; set; }

		public Vector3 Vertex1
		{
			get
			{
				if (base.Owner == null)
				{
					return Vector3.Zero;
				}
				return base.Owner.GetVertexPos(cylinderIndex1);
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
				return base.Owner.GetVertexPos(cylinderIndex2);
			}
		}

		public override Vector3 BoxMin => Vector3.Min(Vertex1, Vertex2) - cylinderRadius;

		public override Vector3 BoxMax => Vector3.Max(Vertex1, Vertex2) + cylinderRadius;

		public override Vector3 Position => (Vertex1 + Vertex2) * 0.5f;

		public BoundPolygonCylinder()
		{
			base.Type = BoundPolygonType.Cylinder;
		}

		public override void Read(byte[] bytes, int offset)
		{
			cylinderType = BitConverter.ToUInt16(bytes, offset);
			cylinderIndex1 = BitConverter.ToUInt16(bytes, offset + 2);
			cylinderRadius = BitConverter.ToSingle(bytes, offset + 4);
			cylinderIndex2 = BitConverter.ToUInt16(bytes, offset + 8);
			unused0 = BitConverter.ToUInt16(bytes, offset + 10);
			unused1 = BitConverter.ToUInt32(bytes, offset + 12);
		}

		public override void Write(BinaryWriter bw)
		{
			bw.Write(cylinderType);
			bw.Write(cylinderIndex1);
			bw.Write(cylinderRadius);
			bw.Write(cylinderIndex2);
			bw.Write(unused0);
			bw.Write(unused1);
		}

		public override string ToString()
		{
			return base.ToString() + ": " + cylinderIndex1 + ", " + cylinderIndex2 + ", " + cylinderRadius;
		}
	}
}
