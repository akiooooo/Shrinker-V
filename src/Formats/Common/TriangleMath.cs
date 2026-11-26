using System;
using GameMath;

namespace GameFormats
{
	public static class TriangleMath
	{
		public static float AreaPart(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, out float angle)
		{
			Vector3 value = v2 - v1;
			Vector3 value2 = v3 - v1;
			Vector3 left = Vector3.Normalize(value);
			Vector3 right = Vector3.Normalize(value2);
			float num = value.Length();
			float num2 = value2.Length();
			double num3 = Math.Acos(Vector3.Dot(left, right));
			float result = (float)(0.5 * (double)num * (double)num2 * Math.Sin(num3));
			angle = (float)Math.Abs(num3);
			return result;
		}

		public static float Area(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3)
		{
			float angle;
			float num = AreaPart(ref v1, ref v2, ref v3, out angle);
			float angle2;
			float result = AreaPart(ref v2, ref v3, ref v1, out angle2);
			float angle3;
			float num2 = AreaPart(ref v3, ref v1, ref v2, out angle3);
			float num3 = (float)Math.PI;
			float num4 = Math.Min(angle, Math.Abs(angle - num3));
			float num5 = Math.Min(angle2, Math.Abs(angle2 - num3));
			float num6 = Math.Min(angle3, Math.Abs(angle3 - num3));
			if (num4 >= num5 && num != 0f)
			{
				if (num4 >= num6 || num2 == 0f)
				{
					return num;
				}
				return num2;
			}
			if (num5 >= num6 || num2 == 0f)
			{
				return result;
			}
			return num2;
		}
	}
}
