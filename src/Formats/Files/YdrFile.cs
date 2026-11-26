using System;

namespace GameFormats.Files
{
	public class YdrFile : GameFile
	{
		public Drawable Drawable { get; set; }

		public YdrFile(RpfFileEntry entry)
			: base(entry, GameFileType.Ydr)
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
				if (version != 154 && version != 159 && version == 165)
				{
					resourceDataReader.IsGen9 = false;
				}
			}
			try
			{
				Drawable = resourceDataReader.ReadBlock<Drawable>(Array.Empty<object>());
				Drawable.Owner = this;
			}
			catch (Exception ex)
			{
				throw new System.IO.InvalidDataException("Could not read " + entry.Name, ex);
			}
			Loaded = true;
		}

		public byte[] Save()
		{
			bool isGen = RpfManager.IsGen9;
			if (isGen)
			{
				Drawable?.EnsureGen9();
			}
			return ResourceBuilder.Build(Drawable, GetVersion(isGen), compress: true, isGen);
		}

		public int GetVersion(bool gen9)
		{
			if (!gen9)
			{
				return 165;
			}
			return 159;
		}
	}
}
