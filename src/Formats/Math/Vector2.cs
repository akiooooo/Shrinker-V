using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameMath
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[DefaultMember("Item")]
	public struct Vector2
	{
		public float X;

		public float Y;

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", new object[2] { X, Y });
		}

		public override int GetHashCode()
		{
			return (X.GetHashCode() * 397) ^ Y.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ref Vector2 other)
		{
			if (MathUtil.NearEqual(other.X, X))
			{
				return MathUtil.NearEqual(other.Y, Y);
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (!(value is Vector2 other))
			{
				return false;
			}
			return Equals(ref other);
		}
	}
}
