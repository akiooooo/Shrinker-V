using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GameMath
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[DefaultMember("Item")]
	public struct Vector4
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector4));

		public static readonly Vector4 Zero = default(Vector4);

		public static readonly Vector4 UnitX = new Vector4(1f, 0f, 0f, 0f);

		public static readonly Vector4 UnitY = new Vector4(0f, 1f, 0f, 0f);

		public static readonly Vector4 UnitZ = new Vector4(0f, 0f, 1f, 0f);

		public static readonly Vector4 UnitW = new Vector4(0f, 0f, 0f, 1f);

		public static readonly Vector4 One = new Vector4(1f, 1f, 1f, 1f);

		public float X;

		public float Y;

		public float Z;

		public float W;

		public Vector4(float value)
		{
			X = value;
			Y = value;
			Z = value;
			W = value;
		}

		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4(Vector3 value, float w)
		{
			X = value.X;
			Y = value.Y;
			Z = value.Z;
			W = w;
		}

		public Vector4(Vector2 value, float z, float w)
		{
			X = value.X;
			Y = value.Y;
			Z = z;
			W = w;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
		}

		public override int GetHashCode()
		{
			return (((((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Z.GetHashCode()) * 397) ^ W.GetHashCode();
		}

		public bool Equals(ref Vector4 other)
		{
			if (MathUtil.NearEqual(other.X, X) && MathUtil.NearEqual(other.Y, Y) && MathUtil.NearEqual(other.Z, Z))
			{
				return MathUtil.NearEqual(other.W, W);
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (!(value is Vector4 other))
			{
				return false;
			}
			return Equals(ref other);
		}
	}
}
