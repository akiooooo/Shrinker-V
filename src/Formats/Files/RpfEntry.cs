using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class RpfEntry
	{
		[CompilerGenerated]
		private uint _003CNameHash_003Ek__BackingField;

		[CompilerGenerated]
		private uint _003CShortNameHash_003Ek__BackingField;

		public uint H1;

		public uint H2;

		public RpfFile File { get; set; }

		public RpfDirectoryEntry Parent { get; set; }

		public uint NameHash
		{
			[CompilerGenerated]
			set
			{
				_003CNameHash_003Ek__BackingField = value;
			}
		}

		public uint ShortNameHash
		{
			[CompilerGenerated]
			set
			{
				_003CShortNameHash_003Ek__BackingField = value;
			}
		}

		public uint NameOffset { get; set; }

		public string Name { get; set; }

		public string NameLower { get; set; }

		public string Path { get; set; }

		public abstract void Read(DataReader reader);

		public abstract void Write(DataWriter writer);

		public override string ToString()
		{
			return Path;
		}

		public string GetShortNameLower()
		{
			if (NameLower == null)
			{
				NameLower = Name.ToLowerInvariant();
			}
			int num = NameLower.LastIndexOf('.');
			if (num > 0)
			{
				return NameLower.Substring(0, num);
			}
			return NameLower;
		}
	}
}
