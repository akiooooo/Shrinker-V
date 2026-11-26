using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GameMath
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	[DefaultMember("Item")]
	public struct Matrix
	{
		public static readonly int SizeInBytes = 64;

		public static readonly Matrix Zero = default(Matrix);

		public static readonly Matrix Identity = new Matrix
		{
			M11 = 1f,
			M22 = 1f,
			M33 = 1f,
			M44 = 1f
		};

		public float M11;

		public float M12;

		public float M13;

		public float M14;

		public float M21;

		public float M22;

		public float M23;

		public float M24;

		public float M31;

		public float M32;

		public float M33;

		public float M34;

		public float M41;

		public float M42;

		public float M43;

		public float M44;

		public Vector4 Column1
		{
			get
			{
				return new Vector4(M11, M21, M31, M41);
			}
			set
			{
				M11 = value.X;
				M21 = value.Y;
				M31 = value.Z;
				M41 = value.W;
			}
		}

		public Vector4 Column2
		{
			get
			{
				return new Vector4(M12, M22, M32, M42);
			}
			set
			{
				M12 = value.X;
				M22 = value.Y;
				M32 = value.Z;
				M42 = value.W;
			}
		}

		public Vector4 Column3
		{
			get
			{
				return new Vector4(M13, M23, M33, M43);
			}
			set
			{
				M13 = value.X;
				M23 = value.Y;
				M33 = value.Z;
				M43 = value.W;
			}
		}

		public Vector4 Column4
		{
			get
			{
				return new Vector4(M14, M24, M34, M44);
			}
			set
			{
				M14 = value.X;
				M24 = value.Y;
				M34 = value.Z;
				M44 = value.W;
			}
		}

		public Vector3 ScaleVector
		{
			get
			{
				return new Vector3(M11, M22, M33);
			}
			set
			{
				M11 = value.X;
				M22 = value.Y;
				M33 = value.Z;
			}
		}

		public Matrix(float M11, float M12, float M13, float M14, float M21, float M22, float M23, float M24, float M31, float M32, float M33, float M34, float M41, float M42, float M43, float M44)
		{
			this.M11 = M11;
			this.M12 = M12;
			this.M13 = M13;
			this.M14 = M14;
			this.M21 = M21;
			this.M22 = M22;
			this.M23 = M23;
			this.M24 = M24;
			this.M31 = M31;
			this.M32 = M32;
			this.M33 = M33;
			this.M34 = M34;
			this.M41 = M41;
			this.M42 = M42;
			this.M43 = M43;
			this.M44 = M44;
		}

		public void Invert()
		{
			Invert(ref this, out this);
		}

		public static void Multiply(ref Matrix left, ref Matrix right, out Matrix result)
		{
			result = new Matrix
			{
				M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41,
				M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42,
				M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43,
				M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44,
				M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41,
				M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42,
				M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43,
				M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44,
				M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41,
				M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42,
				M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43,
				M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44,
				M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41,
				M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42,
				M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43,
				M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44
			};
		}

		public static void Invert(ref Matrix value, out Matrix result)
		{
			float num = value.M31 * value.M42 - value.M32 * value.M41;
			float num2 = value.M31 * value.M43 - value.M33 * value.M41;
			float num3 = value.M34 * value.M41 - value.M31 * value.M44;
			float num4 = value.M32 * value.M43 - value.M33 * value.M42;
			float num5 = value.M34 * value.M42 - value.M32 * value.M44;
			float num6 = value.M33 * value.M44 - value.M34 * value.M43;
			float num7 = value.M22 * num6 + value.M23 * num5 + value.M24 * num4;
			float num8 = value.M21 * num6 + value.M23 * num3 + value.M24 * num2;
			float num9 = value.M21 * (-num5) + value.M22 * num3 + value.M24 * num;
			float num10 = value.M21 * num4 + value.M22 * (-num2) + value.M23 * num;
			float num11 = value.M11 * num7 - value.M12 * num8 + value.M13 * num9 - value.M14 * num10;
			if (Math.Abs(num11) == 0f)
			{
				result = Zero;
				return;
			}
			num11 = 1f / num11;
			float num12 = value.M11 * value.M22 - value.M12 * value.M21;
			float num13 = value.M11 * value.M23 - value.M13 * value.M21;
			float num14 = value.M14 * value.M21 - value.M11 * value.M24;
			float num15 = value.M12 * value.M23 - value.M13 * value.M22;
			float num16 = value.M14 * value.M22 - value.M12 * value.M24;
			float num17 = value.M13 * value.M24 - value.M14 * value.M23;
			float num18 = value.M12 * num6 + value.M13 * num5 + value.M14 * num4;
			float num19 = value.M11 * num6 + value.M13 * num3 + value.M14 * num2;
			float num20 = value.M11 * (-num5) + value.M12 * num3 + value.M14 * num;
			float num21 = value.M11 * num4 + value.M12 * (-num2) + value.M13 * num;
			float num22 = value.M42 * num17 + value.M43 * num16 + value.M44 * num15;
			float num23 = value.M41 * num17 + value.M43 * num14 + value.M44 * num13;
			float num24 = value.M41 * (-num16) + value.M42 * num14 + value.M44 * num12;
			float num25 = value.M41 * num15 + value.M42 * (-num13) + value.M43 * num12;
			float num26 = value.M32 * num17 + value.M33 * num16 + value.M34 * num15;
			float num27 = value.M31 * num17 + value.M33 * num14 + value.M34 * num13;
			float num28 = value.M31 * (-num16) + value.M32 * num14 + value.M34 * num12;
			float num29 = value.M31 * num15 + value.M32 * (-num13) + value.M33 * num12;
			result.M11 = num7 * num11;
			result.M12 = (-num18) * num11;
			result.M13 = num22 * num11;
			result.M14 = (-num26) * num11;
			result.M21 = (-num8) * num11;
			result.M22 = num19 * num11;
			result.M23 = (-num23) * num11;
			result.M24 = num27 * num11;
			result.M31 = num9 * num11;
			result.M32 = (-num20) * num11;
			result.M33 = num24 * num11;
			result.M34 = (-num28) * num11;
			result.M41 = (-num10) * num11;
			result.M42 = num21 * num11;
			result.M43 = (-num25) * num11;
			result.M44 = num29 * num11;
		}

		public static Matrix Invert(Matrix value)
		{
			value.Invert();
			return value;
		}

		public static void Scaling(float scale, out Matrix result)
		{
			result = Identity;
			result.M11 = (result.M22 = (result.M33 = scale));
		}

		public static Matrix Scaling(float scale)
		{
			Scaling(scale, out var result);
			return result;
		}

		public static void RotationQuaternion(ref Quaternion rotation, out Matrix result)
		{
			float num = rotation.X * rotation.X;
			float num2 = rotation.Y * rotation.Y;
			float num3 = rotation.Z * rotation.Z;
			float num4 = rotation.X * rotation.Y;
			float num5 = rotation.Z * rotation.W;
			float num6 = rotation.Z * rotation.X;
			float num7 = rotation.Y * rotation.W;
			float num8 = rotation.Y * rotation.Z;
			float num9 = rotation.X * rotation.W;
			result = Identity;
			result.M11 = 1f - 2f * (num2 + num3);
			result.M12 = 2f * (num4 + num5);
			result.M13 = 2f * (num6 - num7);
			result.M21 = 2f * (num4 - num5);
			result.M22 = 1f - 2f * (num3 + num);
			result.M23 = 2f * (num8 + num9);
			result.M31 = 2f * (num6 + num7);
			result.M32 = 2f * (num8 - num9);
			result.M33 = 1f - 2f * (num2 + num);
		}

		public static Matrix RotationQuaternion(Quaternion rotation)
		{
			RotationQuaternion(ref rotation, out var result);
			return result;
		}

		public static void Translation(ref Vector3 value, out Matrix result)
		{
			Translation(value.X, value.Y, value.Z, out result);
		}

		public static Matrix Translation(Vector3 value)
		{
			Translation(ref value, out var result);
			return result;
		}

		public static void Translation(float x, float y, float z, out Matrix result)
		{
			result = Identity;
			result.M41 = x;
			result.M42 = y;
			result.M43 = z;
		}

		public static void AffineTransformation(float scaling, ref Quaternion rotation, ref Vector3 translation, out Matrix result)
		{
			result = Scaling(scaling) * RotationQuaternion(rotation) * Translation(translation);
		}

		public static Matrix AffineTransformation(float scaling, Quaternion rotation, Vector3 translation)
		{
			AffineTransformation(scaling, ref rotation, ref translation, out var result);
			return result;
		}

		public static Matrix operator *(Matrix left, Matrix right)
		{
			Multiply(ref left, ref right, out var result);
			return result;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]", M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
		}

		public override int GetHashCode()
		{
			return (((((((((((((((((((((((((((((M11.GetHashCode() * 397) ^ M12.GetHashCode()) * 397) ^ M13.GetHashCode()) * 397) ^ M14.GetHashCode()) * 397) ^ M21.GetHashCode()) * 397) ^ M22.GetHashCode()) * 397) ^ M23.GetHashCode()) * 397) ^ M24.GetHashCode()) * 397) ^ M31.GetHashCode()) * 397) ^ M32.GetHashCode()) * 397) ^ M33.GetHashCode()) * 397) ^ M34.GetHashCode()) * 397) ^ M41.GetHashCode()) * 397) ^ M42.GetHashCode()) * 397) ^ M43.GetHashCode()) * 397) ^ M44.GetHashCode();
		}

		public bool Equals(ref Matrix other)
		{
			if (MathUtil.NearEqual(other.M11, M11) && MathUtil.NearEqual(other.M12, M12) && MathUtil.NearEqual(other.M13, M13) && MathUtil.NearEqual(other.M14, M14) && MathUtil.NearEqual(other.M21, M21) && MathUtil.NearEqual(other.M22, M22) && MathUtil.NearEqual(other.M23, M23) && MathUtil.NearEqual(other.M24, M24) && MathUtil.NearEqual(other.M31, M31) && MathUtil.NearEqual(other.M32, M32) && MathUtil.NearEqual(other.M33, M33) && MathUtil.NearEqual(other.M34, M34) && MathUtil.NearEqual(other.M41, M41) && MathUtil.NearEqual(other.M42, M42) && MathUtil.NearEqual(other.M43, M43))
			{
				return MathUtil.NearEqual(other.M44, M44);
			}
			return false;
		}

		public override bool Equals(object value)
		{
			if (!(value is Matrix other))
			{
				return false;
			}
			return Equals(ref other);
		}
	}
}
