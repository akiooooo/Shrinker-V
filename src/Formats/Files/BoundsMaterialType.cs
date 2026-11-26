using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct BoundsMaterialType
	{
		public byte Index { get; set; }

		public override string ToString()
		{
			return BoundsMaterialTypes.GetMaterialName(this);
		}

		public static implicit operator BoundsMaterialType(byte b)
		{
			return new BoundsMaterialType
			{
				Index = b
			};
		}
	}
}
