using System.Collections.Generic;

namespace GameFormats.Files
{
	public static class GlobalText
	{
		public static Dictionary<uint, string> Index = new Dictionary<uint, string>();

		private static object syncRoot = new object();

		public static volatile bool FullIndexBuilt = false;

		public static string GetString(uint hash)
		{
			string value;
			lock (syncRoot)
			{
				if (!Index.TryGetValue(hash, out value))
				{
					value = hash.ToString();
					return value;
				}
			}
			return value;
		}
	}
}
