using System;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class YftFile : GameFile
	{
		public FragType Fragment { get; set; }

		public YftFile(RpfFileEntry entry)
			: base(entry, GameFileType.Yft)
		{
		}

		public void Load(byte[] data, RpfFileEntry entry)
		{
			base.Name = entry.Name;
			base.RpfFileEntry = entry;
			if (!(entry is RpfResourceFileEntry rpfResourceFileEntry))
			{
				throw new Exception("File entry wasn't a resource! (is it binary data?)");
			}
			ResourceDataReader resourceDataReader = new ResourceDataReader(rpfResourceFileEntry, data);
			if (resourceDataReader.IsGen9)
			{
				switch (rpfResourceFileEntry.Version)
				{
				case 162:
					resourceDataReader.IsGen9 = false;
					break;
				}
			}
			Fragment = resourceDataReader.ReadBlock<FragType>(Array.Empty<object>());
			if (Fragment != null)
			{
				Fragment.Yft = this;
				if (Fragment.Drawable != null)
				{
					Fragment.Drawable.Owner = this;
				}
				if (Fragment.DrawableCloth != null)
				{
					Fragment.DrawableCloth.Owner = this;
				}
			}
			Loaded = true;
		}

		public byte[] Save()
		{
			bool isGen = RpfManager.IsGen9;
			if (isGen)
			{
				Fragment?.EnsureGen9();
			}
			return ResourceBuilder.Build(Fragment, GetVersion(isGen), compress: true, isGen);
		}

		public int GetVersion(bool gen9)
		{
			if (!gen9)
			{
				return 162;
			}
			return 171;
		}
	}
}
