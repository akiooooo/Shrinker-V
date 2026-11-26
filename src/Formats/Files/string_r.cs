using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class string_r : ResourceSystemBlock
	{
		public override long BlockLength => Value.Length + 1;

		public string Value { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Value = reader.ReadString();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(Value);
		}

		public static explicit operator string_r(string value)
		{
			return new string_r
			{
				Value = value
			};
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
