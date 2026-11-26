using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct Matrix4F_s
	{





		public Vector3 Column1 { get; set; }

		public uint Flags1 { get; set; }

		public Vector3 Column2 { get; set; }

		public uint Flags2 { get; set; }

		public Vector3 Column3 { get; set; }

		public uint Flags3 { get; set; }

		public Vector3 Column4 { get; set; }

		public uint Flags4 { get; set; }

		public static Matrix4F_s Identity => new Matrix4F_s(identity: true);

		public Matrix4F_s(bool identity)
		{
			if (identity)
			{
				Column1 = Vector3.UnitX;
				Column2 = Vector3.UnitY;
				Column3 = Vector3.UnitZ;
				Column4 = Vector3.Zero;
			}
			else
			{
				Column1 = Vector3.Zero;
				Column2 = Vector3.Zero;
				Column3 = Vector3.Zero;
				Column4 = Vector3.Zero;
			}
			Flags1 = 2139095041u;
			Flags2 = 2139095041u;
			Flags3 = 2139095041u;
			Flags4 = 2139095041u;
		}

		public Matrix4F_s(Matrix m)
		{
			Column1 = new Vector3(m.M11, m.M12, m.M13);
			Column2 = new Vector3(m.M21, m.M22, m.M23);
			Column3 = new Vector3(m.M31, m.M32, m.M33);
			Column4 = new Vector3(m.M41, m.M42, m.M43);
			Flags1 = 2139095041u;
			Flags2 = 2139095041u;
			Flags3 = 2139095041u;
			Flags4 = 2139095041u;
		}

		public Matrix ToMatrix()
		{
			return new Matrix(Column1.X, Column1.Y, Column1.Z, 0f, Column2.X, Column2.Y, Column2.Z, 0f, Column3.X, Column3.Y, Column3.Z, 0f, Column4.X, Column4.Y, Column4.Z, 1f);
		}
	}
}
