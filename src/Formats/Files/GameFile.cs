using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	public abstract class GameFile : Cacheable<GameFileCacheKey>
	{
		public volatile bool Loaded;

		[CompilerGenerated]
		private RpfFileEntry _003CRpfFileEntry_003Ek__BackingField;

		[CompilerGenerated]
		private GameFileType _003CType_003Ek__BackingField;

		public RpfFileEntry RpfFileEntry
		{
			[CompilerGenerated]
			set
			{
				_003CRpfFileEntry_003Ek__BackingField = value;
			}
		}

		public string Name { get; set; }

		public GameFileType Type
		{
			[CompilerGenerated]
			set
			{
				_003CType_003Ek__BackingField = value;
			}
		}

		public GameFile(RpfFileEntry entry, GameFileType type)
		{
			RpfFileEntry = entry;
			Type = type;
			MemoryUsage = entry?.GetFileSize() ?? 0;
			if (entry is RpfResourceFileEntry)
			{
				RpfResourceFileEntry rpfResourceFileEntry = entry as RpfResourceFileEntry;
				int num = rpfResourceFileEntry.SystemSize + rpfResourceFileEntry.GraphicsSize;
				MemoryUsage = num;
			}
			else if (entry is RpfBinaryFileEntry)
			{
				uint fileUncompressedSize = (entry as RpfBinaryFileEntry).FileUncompressedSize;
				if (fileUncompressedSize > MemoryUsage)
				{
					MemoryUsage = fileUncompressedSize;
				}
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(Name))
			{
				return Name;
			}
			return JenkIndex.GetString(Key.Hash);
		}
	}
}
