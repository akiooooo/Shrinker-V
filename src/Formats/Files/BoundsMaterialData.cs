using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundsMaterialData
	{
		public string Name { get; }

		public override string ToString()
		{
			return Name;
		}
	}
}
