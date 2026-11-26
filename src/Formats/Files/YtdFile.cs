using System;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class YtdFile : GameFile
	{
		public TextureDictionary TextureDict { get; set; }

		public YtdFile()
			: base(null, GameFileType.Ytd)
		{
		}

		public YtdFile(RpfFileEntry entry)
			: base(entry, GameFileType.Ytd)
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
				int version = rpfResourceFileEntry.Version;
				if (version != 5 && version == 13)
				{
					resourceDataReader.IsGen9 = false;
				}
			}
			TextureDict = resourceDataReader.ReadBlock<TextureDictionary>(Array.Empty<object>());
		}

		public byte[] Save()
		{
			bool isGen = RpfManager.IsGen9;
			if (isGen)
			{
				TextureDict?.EnsureGen9();
			}
			return ResourceBuilder.Build(TextureDict, GetVersion(isGen), compress: true, isGen);
		}

		public int GetVersion(bool gen9)
		{
			if (!gen9)
			{
				return 13;
			}
			return 5;
		}
	}
}
