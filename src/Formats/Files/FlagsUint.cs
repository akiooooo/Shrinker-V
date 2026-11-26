using System;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct FlagsUint
	{
		public uint Value { get; set; }

		public string Hex => Convert.ToString(Value, 16).ToUpper().PadLeft(8, '0');

		public string Bin => Convert.ToString(Value, 2).PadLeft(32, '0');

		public FlagsUint(uint v)
		{
			Value = v;
		}

		public override string ToString()
		{
			return Bin + " | 0x" + Hex + " | " + Value;
		}

		public static implicit operator FlagsUint(uint v)
		{
			return new FlagsUint(v);
		}

		public static implicit operator uint(FlagsUint v)
		{
			return v.Value;
		}
	}
}
