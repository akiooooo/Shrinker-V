using System.Text;

namespace GameFormats.Files
{
	public struct PsoChar32
	{
		public byte b00;

		public byte b01;

		public byte b02;

		public byte b03;

		public byte b04;

		public byte b05;

		public byte b06;

		public byte b07;

		public byte b08;

		public byte b09;

		public byte b10;

		public byte b11;

		public byte b12;

		public byte b13;

		public byte b14;

		public byte b15;

		public byte b16;

		public byte b17;

		public byte b18;

		public byte b19;

		public byte b20;

		public byte b21;

		public byte b22;

		public byte b23;

		public byte b24;

		public byte b25;

		public byte b26;

		public byte b27;

		public byte b28;

		public byte b29;

		public byte b30;

		public byte b31;

		public override string ToString()
		{
			byte[] bytes = new byte[32]
			{
				b00, b01, b02, b03, b04, b05, b06, b07, b08, b09,
				b10, b11, b12, b13, b14, b15, b16, b17, b18, b19,
				b20, b21, b22, b23, b24, b25, b26, b27, b28, b29,
				b30, b31
			};
			return Encoding.ASCII.GetString(bytes).Replace("\0", string.Empty);
		}
	}
}
