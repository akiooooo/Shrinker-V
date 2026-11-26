using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct ResourcePointerListHeader
	{



		public ulong Pointer { get; set; }

		public ushort Count { get; set; }

		public ushort Capacity { get; set; }
        public uint Unknown { get; set; }
	}
}
