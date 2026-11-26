using System;

namespace GameMath
{
	public static class MathUtil
	{
		public unsafe static bool NearEqual(float a, float b)
		{
			if (IsZero(a - b))
			{
				return true;
			}
			int num = *(int*)(&a);
			int num2 = *(int*)(&b);
			if (num < 0 != num2 < 0)
			{
				return false;
			}
			return Math.Abs(num - num2) <= 4;
		}

		public static bool IsZero(float a)
		{
			return Math.Abs(a) < 1E-06f;
		}
	}
}
