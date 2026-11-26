using GameMath;

namespace GameFormats.Files
{
	public class BVHBuilderItem
	{
		public Vector3 Min;

		public Vector3 Max;

		public int Index;

		public Bounds Bounds;

		public BoundPolygon Polygon;
	}
}
