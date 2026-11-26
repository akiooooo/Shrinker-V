using System.ComponentModel;
using System.IO;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class BoundPolygon
	{
		public BoundMaterial_s? MaterialCustom;

		public BoundPolygonType Type { get; set; }

		public BoundGeometry Owner { get; set; }

		public BoundMaterial_s Material
		{
			get
			{
				if (MaterialCustom.HasValue)
				{
					return MaterialCustom.Value;
				}
				return Owner?.GetMaterial(Index) ?? default(BoundMaterial_s);
			}
		}

		public int Index { get; set; }

		public abstract Vector3 BoxMin { get; }

		public abstract Vector3 BoxMax { get; }

		public abstract Vector3 Position { get; }

		public abstract void Read(byte[] bytes, int offset);

		public abstract void Write(BinaryWriter bw);

		public override string ToString()
		{
			return Type.ToString();
		}
	}
}
