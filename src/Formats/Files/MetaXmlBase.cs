namespace GameFormats.Files
{
	public class MetaXmlBase
	{
		public static string HashString(MetaHash h)
		{
			if ((uint)h == 0)
			{
				return "";
			}
			string str = JenkIndex.TryGetString(h);
			if (string.IsNullOrEmpty(str) && MetaNames.TryGetString(h, out str))
			{
				return str;
			}
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			return "hash_" + h.Hex;
		}
	}
}
