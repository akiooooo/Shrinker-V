using System.Text;

namespace GameFormats.Files
{
	public class UintStringBuilder
	{
		public StringBuilder sb = new StringBuilder();

		public void Add(uint u)
		{
			sb.Append((char)(u & 0xFF));
			sb.Append((char)((u & 0xFF00) >> 8));
			sb.Append((char)((u & 0xFF0000) >> 16));
			sb.Append((char)((u & 0xFF000000u) >> 24));
		}

		public override string ToString()
		{
			return sb.ToString().Replace("\0", "");
		}
	}
}
