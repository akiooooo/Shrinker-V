using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundEdge
	{
		public BoundPolygonTriangle Triangle1 { get; set; }

		public BoundPolygonTriangle Triangle2 { get; set; }

		public int EdgeID1 { get; set; }

		public int EdgeID2 { get; set; }

		public BoundEdge(BoundPolygonTriangle t1, int e1)
		{
			Triangle1 = t1;
			EdgeID1 = e1;
		}
	}
}
