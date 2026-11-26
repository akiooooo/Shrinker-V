using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct BoundCompositeChildrenFlags
	{
		public EBoundCompositeFlags Flags1 { get; }

		public EBoundCompositeFlags Flags2 { get; }

		public override string ToString()
		{
			return Flags1.ToString() + ", " + Flags2;
		}
	}
}
