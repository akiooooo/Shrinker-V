using System;
using GameMath;

namespace GameFormats
{
	public static class Vectors
	{
		public static Vector3 XYZ(this Vector4 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static Vector3 Abs(this Vector3 v)
		{
			return new Vector3(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z));
		}

		public static int CompareTo(this Vector3 a, Vector3 b)
		{
			int num = a.X.CompareTo(b.X);
			if (num != 0)
			{
				return num;
			}
			num = a.Y.CompareTo(b.Y);
			if (num != 0)
			{
				return num;
			}
			num = a.Z.CompareTo(b.Z);
			if (num != 0)
			{
				return num;
			}
			return 0;
		}

		public static Vector4 Abs(this Vector4 v)
		{
			return new Vector4(Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z), Math.Abs(v.W));
		}
	}
}
