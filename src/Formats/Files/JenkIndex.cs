using System.Collections.Generic;

namespace GameFormats.Files
{
	public static class JenkIndex
	{
		public static Dictionary<uint, string> Index = new Dictionary<uint, string>();

		private static object syncRoot = new object();

		public static bool Ensure(string str)
		{
			uint num = JenkHash.GenHash(str);
			if (num == 0)
			{
				return true;
			}
			lock (syncRoot)
			{
				if (!Index.ContainsKey(num))
				{
					Index.Add(num, str);
					return false;
				}
			}
			return true;
		}

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

		public static string TryGetString(uint hash)
		{
			string value;
			lock (syncRoot)
			{
				if (!Index.TryGetValue(hash, out value))
				{
					value = string.Empty;
					return value;
				}
			}
			return value;
		}
	}
}
