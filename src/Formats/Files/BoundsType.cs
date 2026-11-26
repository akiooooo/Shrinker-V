namespace GameFormats.Files
{
	public enum BoundsType : byte
	{
		None = byte.MaxValue,
		Sphere = 0,
		Capsule = 1,
		Box = 3,
		Geometry = 4,
		GeometryBVH = 8,
		Composite = 10,
		Disc = 12,
		Cylinder = 13,
		Cloth = 15
	}
}
