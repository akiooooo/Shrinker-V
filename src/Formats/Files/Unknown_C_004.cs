using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct Unknown_C_004
	{
		public ushort Unknown_0h { get; }

		public ushort Unknown_2h { get; }

		public float Unknown_4h { get; }

		public float Unknown_8h { get; }

		public float Unknown_Ch { get; }

		public override string ToString()
		{
			return Unknown_0h + ", " + Unknown_2h + ", " + Unknown_4h + ", " + Unknown_8h + ", " + Unknown_Ch;
		}
	}
}
