namespace GameFormats.Files
{
	public static class VertexComponentTypes
	{
		public static int GetSizeInBytes(VertexComponentType type)
		{
			switch (type)
			{
			case VertexComponentType.Nothing:
				return 0;
			case VertexComponentType.Half2:
				return 4;
			case VertexComponentType.Float:
				return 4;
			case VertexComponentType.Half4:
				return 8;
			case VertexComponentType.FloatUnk:
				return 0;
			case VertexComponentType.Float2:
				return 8;
			case VertexComponentType.Float3:
				return 12;
			case VertexComponentType.Float4:
				return 16;
			case VertexComponentType.UByte4:
				return 4;
			case VertexComponentType.Colour:
				return 4;
			case VertexComponentType.RGBA8SNorm:
				return 4;
			default:
				return 0;
			}
		}
	}
}
