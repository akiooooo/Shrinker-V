using System;

namespace GameFormats.Files
{
	public static class MetaNames
	{
		public static bool TryGetString(uint h, out string str)
		{
			if (Enum.IsDefined(typeof(MetaName), h))
			{
				MetaName metaName = (MetaName)h;
				str = metaName.ToString();
				if (str.StartsWith("@"))
				{
					str = str.Substring(1);
				}
				return true;
			}
			str = null;
			return false;
		}
	}
}
