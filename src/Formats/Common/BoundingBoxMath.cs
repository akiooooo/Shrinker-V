using GameMath;

namespace GameFormats
{
	public static class BoundingBoxMath
	{
		public static BoundingBox Transform(this BoundingBox b, Matrix mat)
		{
			Matrix transform = mat;
			transform.Column1 = mat.Column1.Abs();
			transform.Column2 = mat.Column2.Abs();
			transform.Column3 = mat.Column3.Abs();
			transform.Column4 = mat.Column4.Abs();
			Vector3 coordinate = (b.Maximum + b.Minimum) * 0.5f;
			Vector3 normal = (b.Maximum - b.Minimum) * 0.5f;
			Vector3 vector = Vector3.TransformCoordinate(coordinate, mat);
			Vector3 vector2 = Vector3.TransformNormal(normal, transform).Abs();
			return new BoundingBox(vector - vector2, vector + vector2);
		}
	}
}
