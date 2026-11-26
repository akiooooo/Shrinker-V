using System;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct BoundVertex_s
	{
		public short X { get; set; }

		public short Y { get; set; }

		public short Z { get; set; }

		public Vector3 Vector => new Vector3(X, Y, Z);

		public BoundVertex_s(Vector3 v)
		{
			X = (short)Math.Min(Math.Max(v.X, -32767f), 32767f);
			Y = (short)Math.Min(Math.Max(v.Y, -32767f), 32767f);
			Z = (short)Math.Min(Math.Max(v.Z, -32767f), 32767f);
		}
	}
}
