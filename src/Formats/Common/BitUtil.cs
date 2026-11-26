namespace GameFormats
{
	public static class BitUtil
	{
		public static uint SetBit(uint value, int bit)
		{
			return value | (uint)(1 << bit);
		}
	}
}
