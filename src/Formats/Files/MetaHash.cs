using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct MetaHash
	{
		public uint Hash { get; set; }

		public string Hex => Hash.ToString("X").PadLeft(8, '0');

		public MetaHash(uint h)
		{
			Hash = h;
		}

		public override string ToString()
		{
			string str = JenkIndex.TryGetString(Hash);
			if (!string.IsNullOrEmpty(str))
			{
				return str;
			}
			if (MetaNames.TryGetString(Hash, out str))
			{
				return str;
			}
			return GlobalText.GetString(Hash);
		}

		public string ToCleanString()
		{
			if (Hash == 0)
			{
				return string.Empty;
			}
			return ToString();
		}

		public static implicit operator uint(MetaHash h)
		{
			return h.Hash;
		}

		public static implicit operator MetaHash(uint v)
		{
			return new MetaHash(v);
		}
	}
}
