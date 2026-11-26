namespace GameFormats.Files
{
	public class JenkHash
	{
		public static uint GenHash(string text)
		{
			if (text == null)
			{
				return 0u;
			}
			uint num = 0u;
			for (int i = 0; i < text.Length; i++)
			{
				num += (byte)text[i];
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}

		public static uint GenHash(byte[] data)
		{
			uint num = 0u;
			for (uint num2 = 0u; num2 < data.Length; num2++)
			{
				num += data[num2];
				num += num << 10;
				num ^= num >> 6;
			}
			num += num << 3;
			num ^= num >> 11;
			return num + (num << 15);
		}
	}
}
