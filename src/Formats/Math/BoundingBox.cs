using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameMath
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct BoundingBox
	{
		public Vector3 Minimum;

		public Vector3 Maximum;

		public BoundingBox(Vector3 minimum, Vector3 maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", new object[2]
			{
				Minimum.ToString(),
				Maximum.ToString()
			});
		}

		public override int GetHashCode()
		{
			return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ref BoundingBox value)
		{
			if (Minimum == value.Minimum)
			{
				return Maximum == value.Maximum;
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (!(value is BoundingBox value2))
			{
				return false;
			}
			return Equals(ref value2);
		}
	}
}
