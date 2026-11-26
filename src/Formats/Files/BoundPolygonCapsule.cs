using System;
using System.ComponentModel;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundPolygonCapsule : BoundPolygon
	{
		public ushort capsuleType { get; set; }

		public ushort capsuleIndex1 { get; set; }

		public float capsuleRadius { get; set; }

		public ushort capsuleIndex2 { get; set; }

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
				return base.Owner.GetVertexPos(capsuleIndex1);
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
				return base.Owner.GetVertexPos(capsuleIndex2);
			}
		}

		public override Vector3 BoxMin => Vector3.Min(Vertex1, Vertex2) - capsuleRadius;

		public override Vector3 BoxMax => Vector3.Max(Vertex1, Vertex2) + capsuleRadius;

		public override Vector3 Position => (Vertex1 + Vertex2) * 0.5f;

		public BoundPolygonCapsule()
		{
			base.Type = BoundPolygonType.Capsule;
		}

		public override void Read(byte[] bytes, int offset)
		{
			capsuleType = BitConverter.ToUInt16(bytes, offset);
			capsuleIndex1 = BitConverter.ToUInt16(bytes, offset + 2);
			capsuleRadius = BitConverter.ToSingle(bytes, offset + 4);
			capsuleIndex2 = BitConverter.ToUInt16(bytes, offset + 8);
			unused0 = BitConverter.ToUInt16(bytes, offset + 10);
			unused1 = BitConverter.ToUInt32(bytes, offset + 12);
		}

		public override void Write(BinaryWriter bw)
		{
			bw.Write(capsuleType);
			bw.Write(capsuleIndex1);
			bw.Write(capsuleRadius);
			bw.Write(capsuleIndex2);
			bw.Write(unused0);
			bw.Write(unused1);
		}

		public override string ToString()
		{
			return base.ToString() + ": " + capsuleIndex1 + ", " + capsuleIndex2 + ", " + capsuleRadius;
		}
	}
}
