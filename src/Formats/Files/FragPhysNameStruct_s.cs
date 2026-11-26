using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct FragPhysNameStruct_s
	{
		public uint Unknown_00h { get; }

		public uint Unknown_04h { get; }

		public uint Unknown_08h { get; }

		public uint Unknown_0Ch { get; }

		public uint Unknown_10h { get; }

		public uint Unknown_14h { get; }

		public uint Unknown_18h { get; }

		public uint Unknown_1Ch { get; }

		public uint Unknown_20h { get; }

		public uint Unknown_24h { get; }

		public override string ToString()
		{
			UintStringBuilder uintStringBuilder = new UintStringBuilder();
			uintStringBuilder.Add(Unknown_00h);
			uintStringBuilder.Add(Unknown_04h);
			uintStringBuilder.Add(Unknown_08h);
			uintStringBuilder.Add(Unknown_0Ch);
			uintStringBuilder.Add(Unknown_10h);
			uintStringBuilder.Add(Unknown_14h);
			uintStringBuilder.Add(Unknown_18h);
			uintStringBuilder.Add(Unknown_1Ch);
			uintStringBuilder.Add(Unknown_20h);
			uintStringBuilder.Add(Unknown_24h);
			return uintStringBuilder.ToString().Replace("\0", "");
		}
	}
}
