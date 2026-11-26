using GameMath;

namespace GameFormats
{
	public static class QuaternionExtension
	{
		public static Vector3 Multiply(this Quaternion a, Vector3 b)
		{
			float num = a.X * 2f;
			float num2 = a.Y * 2f;
			float num3 = a.Z * 2f;
			float num4 = a.W * num;
			float num5 = a.W * num2;
			float num6 = a.W * num3;
			float num7 = a.X * num;
			float num8 = a.X * num2;
			float num9 = a.X * num3;
			float num10 = a.Y * num2;
			float num11 = a.Y * num3;
			float num12 = a.Z * num3;
			return new Vector3(b.X * (1f - num10 - num12) + b.Y * (num8 - num6) + b.Z * (num9 + num5), b.X * (num8 + num6) + b.Y * (1f - num7 - num12) + b.Z * (num11 - num4), b.X * (num9 - num5) + b.Y * (num11 + num4) + b.Z * (1f - num7 - num10));
		}

		public static Vector4 ToVector4(this Quaternion q)
		{
			return new Vector4(q.X, q.Y, q.Z, q.W);
		}
	}
}
