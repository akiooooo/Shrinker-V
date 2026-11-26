using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct BoundMaterialColour
	{
		public byte R { get; }

		public byte G { get; }

		public byte B { get; }

		public byte A { get; }

		public override string ToString()
		{
			return R + ", " + G + ", " + B + ", " + A;
		}
	}
}
