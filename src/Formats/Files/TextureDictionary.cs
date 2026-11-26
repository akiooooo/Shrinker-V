using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TextureDictionary : ResourceFileBase
	{
		[CompilerGenerated]
		private Dictionary<uint, Texture> _003CDict_003Ek__BackingField;

		public override long BlockLength => 64L;

		public uint Unknown_10h { get; set; }

		public uint Unknown_14h { get; set; }

		public uint Unknown_18h { get; set; } = 1u;

		public uint Unknown_1Ch { get; set; }

		public ResourceSimpleList64_uint TextureNameHashes { get; set; }

		public ResourcePointerList64<Texture> Textures { get; set; }

		public Dictionary<uint, Texture> Dict
		{
			[CompilerGenerated]
			set
			{
				_003CDict_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_10h = reader.ReadUInt32();
			Unknown_14h = reader.ReadUInt32();
			Unknown_18h = reader.ReadUInt32();
			Unknown_1Ch = reader.ReadUInt32();
			TextureNameHashes = reader.ReadBlock<ResourceSimpleList64_uint>(Array.Empty<object>());
			Textures = reader.ReadBlock<ResourcePointerList64<Texture>>(Array.Empty<object>());
			BuildDict();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_1Ch);
			writer.WriteBlock(TextureNameHashes);
			writer.WriteBlock(Textures);
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[2]
			{
				new Tuple<long, IResourceBlock>(32L, TextureNameHashes),
				new Tuple<long, IResourceBlock>(48L, Textures)
			};
		}

		private void BuildDict()
		{
			Dictionary<uint, Texture> dictionary = new Dictionary<uint, Texture>();
			if (Textures?.data_items != null && TextureNameHashes?.data_items != null)
			{
				for (int i = 0; i < Textures.data_items.Length && i < TextureNameHashes.data_items.Length; i++)
				{
					Texture value = Textures.data_items[i];
					uint key = TextureNameHashes.data_items[i];
					dictionary[key] = value;
				}
			}
			Dict = dictionary;
		}

		public void EnsureGen9()
		{
			base.FileVFT = 0u;
			base.FileUnknown = 1u;
			Texture[] array = Textures?.data_items;
			if (array != null)
			{
				Texture[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.EnsureGen9();
				}
			}
		}
	}
}
