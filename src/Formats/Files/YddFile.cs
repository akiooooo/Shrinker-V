using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class YddFile : GameFile
	{
		[CompilerGenerated]
		private Drawable[] _003CDrawables_003Ek__BackingField;

		public DrawableDictionary DrawableDict { get; set; }

		public Dictionary<uint, Drawable> Dict { get; set; }

		public Drawable[] Drawables
		{
			[CompilerGenerated]
			set
			{
				_003CDrawables_003Ek__BackingField = value;
			}
		}

		public YddFile(RpfFileEntry entry)
			: base(entry, GameFileType.Ydd)
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
				if (version != 159 && version == 165)
				{
					resourceDataReader.IsGen9 = false;
				}
			}
			DrawableDict = resourceDataReader.ReadBlock<DrawableDictionary>(Array.Empty<object>());
			if (DrawableDict != null && DrawableDict.Drawables != null && DrawableDict.Drawables.data_items != null && DrawableDict.Hashes != null)
			{
				Dict = new Dictionary<uint, Drawable>();
				Drawable[] data_items = DrawableDict.Drawables.data_items;
				uint[] hashes = DrawableDict.Hashes;
				for (int i = 0; i < data_items.Length && i < hashes.Length; i++)
				{
					Drawable drawable = data_items[i];
					uint key = hashes[i];
					Dict[key] = drawable;
					drawable.Owner = this;
				}
				for (int j = 0; j < data_items.Length && j < hashes.Length; j++)
				{
					Drawable drawable2 = data_items[j];
					uint num = hashes[j];
					if (drawable2.Name == null || JenkHash.GenHash(drawable2.Name) != num)
					{
						drawable2.Name = MetaXmlBase.HashString(num);
					}
				}
				Drawables = Dict.Values.ToArray();
			}
			Loaded = true;
		}

		public byte[] Save()
		{
			Drawable[] array = DrawableDict?.Drawables?.data_items;
			bool isGen = RpfManager.IsGen9;
			if (isGen && array != null)
			{
				Drawable[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.EnsureGen9();
				}
			}
			return ResourceBuilder.Build(DrawableDict, GetVersion(isGen), compress: true, isGen);
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
