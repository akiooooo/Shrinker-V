using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct RpfResourcePageFlags
	{
		public uint Value { get; set; }

		public uint[] PageCounts => new uint[9]
		{
			(Value >> 4) & 1,
			(Value >> 5) & 3,
			(Value >> 7) & 0xF,
			(Value >> 11) & 0x3F,
			(Value >> 17) & 0x7F,
			(Value >> 24) & 1,
			(Value >> 25) & 1,
			(Value >> 26) & 1,
			(Value >> 27) & 1
		};

		public uint Count
		{
			get
			{
				uint[] pageCounts = PageCounts;
				return pageCounts[0] + pageCounts[1] + pageCounts[2] + pageCounts[3] + pageCounts[4] + pageCounts[5] + pageCounts[6] + pageCounts[7] + pageCounts[8];
			}
		}

		public uint Size
		{
			get
			{
				uint value = Value;
				uint num = (value >> 27) & 1;
				uint num2 = ((value >> 26) & 1) << 1;
				uint num3 = ((value >> 25) & 1) << 2;
				uint num4 = ((value >> 24) & 1) << 3;
				uint num5 = ((value >> 17) & 0x7F) << 4;
				uint num6 = ((value >> 11) & 0x3F) << 5;
				uint num7 = ((value >> 7) & 0xF) << 6;
				uint num8 = ((value >> 5) & 3) << 7;
				uint num9 = ((value >> 4) & 1) << 8;
				uint num10 = value & 0xF;
				return (uint)(512 << (int)num10) * (num + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9);
			}
		}

		public RpfResourcePageFlags(uint v)
		{
			Value = v;
		}

		public static implicit operator uint(RpfResourcePageFlags f)
		{
			return f.Value;
		}

		public static implicit operator RpfResourcePageFlags(uint v)
		{
			return new RpfResourcePageFlags(v);
		}

		public override string ToString()
		{
			return "Size: " + Size + ", Pages: " + Count;
		}
	}
}
