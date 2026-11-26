using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace GameFormats
{
	public static class Xml
	{
		public static string GetStringAttribute(XmlNode node, string attribute)
		{
			return node?.Attributes[attribute]?.InnerText;
		}

		public static int GetIntAttribute(XmlNode node, string attribute)
		{
			if (node == null)
			{
				return 0;
			}
			int.TryParse(node.Attributes[attribute]?.InnerText, out var result);
			return result;
		}

		public static uint GetUIntAttribute(XmlNode node, string attribute)
		{
			if (node == null)
			{
				return 0u;
			}
			uint.TryParse(node.Attributes[attribute]?.InnerText, out var result);
			return result;
		}

		public static string GetChildInnerText(XmlNode node, string name)
		{
			return node?.SelectSingleNode(name)?.InnerText;
		}

		public static int[] GetRawIntArray(XmlNode node)
		{
			if (node == null)
			{
				return new int[0];
			}
			List<int> list = new List<int>();
			string[] array = Regex.Split(node.InnerText, "[\\s\\r\\n\\t]");
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					string text = array[i];
					if (!string.IsNullOrEmpty(text))
					{
						int result = 0;
						int.TryParse(text, out result);
						list.Add(result);
					}
				}
			}
			return list.ToArray();
		}

		public static int[] GetChildRawIntArray(XmlNode node, string name)
		{
			return GetRawIntArray(node.SelectSingleNode(name));
		}
	}
}
