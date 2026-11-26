using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameMath
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[DefaultMember("Item")]
	public struct Quaternion
	{
		public float X;

		public float Y;

		public float Z;

		public float W;

		public Quaternion(Vector4 value)
		{
			X = value.X;
			Y = value.Y;
			Z = value.Z;
			W = value.W;
		}

		public static void Multiply(ref Quaternion left, ref Quaternion right, out Quaternion result)
		{
			float x = left.X;
			float y = left.Y;
			float z = left.Z;
			float w = left.W;
			float x2 = right.X;
			float y2 = right.Y;
			float z2 = right.Z;
			float w2 = right.W;
			float num = y * z2 - z * y2;
			float num2 = z * x2 - x * z2;
			float num3 = x * y2 - y * x2;
			float num4 = x * x2 + y * y2 + z * z2;
			result.X = x * w2 + x2 * w + num;
			result.Y = y * w2 + y2 * w + num2;
			result.Z = z * w2 + z2 * w + num3;
			result.W = w * w2 - num4;
		}

		public static Quaternion operator *(Quaternion left, Quaternion right)
		{
			Multiply(ref left, ref right, out var result);
			return result;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
		}

		public override int GetHashCode()
		{
			return (((((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Z.GetHashCode()) * 397) ^ W.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ref Quaternion other)
		{
			if (MathUtil.NearEqual(other.X, X) && MathUtil.NearEqual(other.Y, Y) && MathUtil.NearEqual(other.Z, Z))
			{
				return MathUtil.NearEqual(other.W, W);
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (!(value is Quaternion other))
			{
				return false;
			}
			return Equals(ref other);
		}
	}
}
