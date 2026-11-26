namespace GameFormats.Files
{
	public enum TextureTileModeG9 : byte
	{
		Depth = 4,
		Linear = 8,
		Display = 10,
		Standard = 13,
		RenderTarget = 14,
		VolumeStandard = 19,
		VolumeRenderTarget = 20,
		Auto = byte.MaxValue
	}
}
