using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GameMath
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[DefaultMember("Item")]
	public struct Vector3
	{
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3));

		public static readonly Vector3 Zero = default(Vector3);

		public static readonly Vector3 UnitX = new Vector3(1f, 0f, 0f);

		public static readonly Vector3 UnitY = new Vector3(0f, 1f, 0f);

		public static readonly Vector3 UnitZ = new Vector3(0f, 0f, 1f);

		public static readonly Vector3 One = new Vector3(1f, 1f, 1f);

		public static readonly Vector3 Up = new Vector3(0f, 1f, 0f);

		public static readonly Vector3 Down = new Vector3(0f, -1f, 0f);

		public static readonly Vector3 Left = new Vector3(-1f, 0f, 0f);

		public static readonly Vector3 Right = new Vector3(1f, 0f, 0f);

		public static readonly Vector3 ForwardRH = new Vector3(0f, 0f, -1f);

		public static readonly Vector3 ForwardLH = new Vector3(0f, 0f, 1f);

		public static readonly Vector3 BackwardRH = new Vector3(0f, 0f, 1f);

		public static readonly Vector3 BackwardLH = new Vector3(0f, 0f, -1f);

		public float X;

		public float Y;

		public float Z;

		public Vector3(float value)
		{
			X = value;
			Y = value;
			Z = value;
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public float Length()
		{
			return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
		}

		public void Normalize()
		{
			float num = Length();
			if (!MathUtil.IsZero(num))
			{
				float num2 = 1f / num;
				X *= num2;
				Y *= num2;
				Z *= num2;
			}
		}

		public static float Dot(Vector3 left, Vector3 right)
		{
			return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
		}

		public static Vector3 Normalize(Vector3 value)
		{
			value.Normalize();
			return value;
		}

		public static void Max(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result.X = ((left.X > right.X) ? left.X : right.X);
			result.Y = ((left.Y > right.Y) ? left.Y : right.Y);
			result.Z = ((left.Z > right.Z) ? left.Z : right.Z);
		}

		public static Vector3 Max(Vector3 left, Vector3 right)
		{
			Max(ref left, ref right, out var result);
			return result;
		}

		public static void Min(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
			result.X = ((left.X < right.X) ? left.X : right.X);
			result.Y = ((left.Y < right.Y) ? left.Y : right.Y);
			result.Z = ((left.Z < right.Z) ? left.Z : right.Z);
		}

		public static Vector3 Min(Vector3 left, Vector3 right)
		{
			Min(ref left, ref right, out var result);
			return result;
		}

		public static void Transform(ref Vector3 vector, ref Matrix transform, out Vector4 result)
		{
			result = new Vector4(vector.X * transform.M11 + vector.Y * transform.M21 + vector.Z * transform.M31 + transform.M41, vector.X * transform.M12 + vector.Y * transform.M22 + vector.Z * transform.M32 + transform.M42, vector.X * transform.M13 + vector.Y * transform.M23 + vector.Z * transform.M33 + transform.M43, vector.X * transform.M14 + vector.Y * transform.M24 + vector.Z * transform.M34 + transform.M44);
		}

		public static Vector4 Transform(Vector3 vector, Matrix transform)
		{
			Transform(ref vector, ref transform, out var result);
			return result;
		}

		public static void TransformCoordinate(ref Vector3 coordinate, ref Matrix transform, out Vector3 result)
		{
			Vector4 vector = new Vector4
			{
				X = coordinate.X * transform.M11 + coordinate.Y * transform.M21 + coordinate.Z * transform.M31 + transform.M41,
				Y = coordinate.X * transform.M12 + coordinate.Y * transform.M22 + coordinate.Z * transform.M32 + transform.M42,
				Z = coordinate.X * transform.M13 + coordinate.Y * transform.M23 + coordinate.Z * transform.M33 + transform.M43,
				W = 1f / (coordinate.X * transform.M14 + coordinate.Y * transform.M24 + coordinate.Z * transform.M34 + transform.M44)
			};
			result = new Vector3(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W);
		}

		public static Vector3 TransformCoordinate(Vector3 coordinate, Matrix transform)
		{
			TransformCoordinate(ref coordinate, ref transform, out var result);
			return result;
		}

		public static void TransformNormal(ref Vector3 normal, ref Matrix transform, out Vector3 result)
		{
			result = new Vector3(normal.X * transform.M11 + normal.Y * transform.M21 + normal.Z * transform.M31, normal.X * transform.M12 + normal.Y * transform.M22 + normal.Z * transform.M32, normal.X * transform.M13 + normal.Y * transform.M23 + normal.Z * transform.M33);
		}

		public static Vector3 TransformNormal(Vector3 normal, Matrix transform)
		{
			TransformNormal(ref normal, ref transform, out var result);
			return result;
		}

		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		public static Vector3 operator *(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
		}

		public static Vector3 operator -(Vector3 left, Vector3 right)
		{
			return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		public static Vector3 operator *(Vector3 value, float scale)
		{
			return new Vector3(value.X * scale, value.Y * scale, value.Z * scale);
		}

		public static Vector3 operator /(Vector3 value, float scale)
		{
			return new Vector3(value.X / scale, value.Y / scale, value.Z / scale);
		}

		public static Vector3 operator /(float scale, Vector3 value)
		{
			return new Vector3(scale / value.X, scale / value.Y, scale / value.Z);
		}

		public static Vector3 operator /(Vector3 value, Vector3 scale)
		{
			return new Vector3(value.X / scale.X, value.Y / scale.Y, value.Z / scale.Z);
		}

		public static Vector3 operator +(Vector3 value, float scalar)
		{
			return new Vector3(value.X + scalar, value.Y + scalar, value.Z + scalar);
		}

		public static Vector3 operator -(Vector3 value, float scalar)
		{
			return new Vector3(value.X - scalar, value.Y - scalar, value.Z - scalar);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Vector3 left, Vector3 right) { return !(left == right); }
        public static bool operator ==(Vector3 left, Vector3 right)
		{
			return left.Equals(ref right);
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", new object[3] { X, Y, Z });
		}

		public override int GetHashCode()
		{
			return (((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Z.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(ref Vector3 other)
		{
			if (MathUtil.NearEqual(other.X, X) && MathUtil.NearEqual(other.Y, Y))
			{
				return MathUtil.NearEqual(other.Z, Z);
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (!(value is Vector3 other))
			{
				return false;
			}
			return Equals(ref other);
		}
	}
}
