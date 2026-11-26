using System;
using System.ComponentModel;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundPolygonSphere : BoundPolygon
	{
		public ushort sphereType { get; set; }

		public ushort sphereIndex { get; set; }

		public float sphereRadius { get; set; }

		public uint unused0 { get; set; }

		public uint unused1 { get; set; }

		public override Vector3 BoxMin => Position - sphereRadius;

		public override Vector3 BoxMax => Position + sphereRadius;

		public override Vector3 Position
		{
			get
			{
				if (base.Owner == null)
				{
					return Vector3.Zero;
				}
				return base.Owner.GetVertexPos(sphereIndex);
			}
		}

		public BoundPolygonSphere()
		{
			base.Type = BoundPolygonType.Sphere;
		}

		public override void Read(byte[] bytes, int offset)
		{
			sphereType = BitConverter.ToUInt16(bytes, offset);
			sphereIndex = BitConverter.ToUInt16(bytes, offset + 2);
			sphereRadius = BitConverter.ToSingle(bytes, offset + 4);
			unused0 = BitConverter.ToUInt32(bytes, offset + 8);
			unused1 = BitConverter.ToUInt32(bytes, offset + 12);
		}

		public override void Write(BinaryWriter bw)
		{
			bw.Write(sphereType);
			bw.Write(sphereIndex);
			bw.Write(sphereRadius);
			bw.Write(unused0);
			bw.Write(unused1);
		}

		public override string ToString()
		{
			return base.ToString() + ": " + sphereIndex + ", " + sphereRadius;
		}
	}
}
