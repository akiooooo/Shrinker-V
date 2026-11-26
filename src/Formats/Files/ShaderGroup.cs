using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ShaderGroup : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public uint Unknown_1Ch;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public uint Unknown_34h;

		public ulong Unknown_38h;

		public override long BlockLength => 64L;

		public uint VFT { get; set; } = 1080113136u;

		public ulong TextureDictionaryPointer { get; set; }

		public ulong ShadersPointer { get; set; }

		public ushort ShadersCount1 { get; set; }

		public ushort ShadersCount2 { get; set; }

		public uint ShaderGroupBlocksSize { get; set; }

		public TextureDictionary TextureDictionary { get; set; }

		public ResourcePointerArray64<ShaderFX> Shaders { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			TextureDictionaryPointer = reader.ReadUInt64();
			ShadersPointer = reader.ReadUInt64();
			ShadersCount1 = reader.ReadUInt16();
			ShadersCount2 = reader.ReadUInt16();
			Unknown_1Ch = reader.ReadUInt32();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			ShaderGroupBlocksSize = reader.ReadUInt32();
			Unknown_34h = reader.ReadUInt32();
			Unknown_38h = reader.ReadUInt64();
			TextureDictionary = reader.ReadBlockAt<TextureDictionary>(TextureDictionaryPointer, Array.Empty<object>());
			Shaders = reader.ReadBlockAt<ResourcePointerArray64<ShaderFX>>(ShadersPointer, new object[1] { ShadersCount1 });
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			TextureDictionaryPointer = (ulong)((TextureDictionary != null) ? TextureDictionary.FilePosition : 0);
			ShadersPointer = (ulong)((Shaders != null) ? Shaders.FilePosition : 0);
			ShadersCount1 = (ushort)((Shaders != null) ? ((uint)Shaders.Count) : 0u);
			ShadersCount2 = ShadersCount1;
			ShaderGroupBlocksSize = ((!writer.IsGen9) ? ((uint)BlockLength / 16) : 0u);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(TextureDictionaryPointer);
			writer.Write(ShadersPointer);
			writer.Write(ShadersCount1);
			writer.Write(ShadersCount2);
			writer.Write(Unknown_1Ch);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
			writer.Write(ShaderGroupBlocksSize);
			writer.Write(Unknown_34h);
			writer.Write(Unknown_38h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (TextureDictionary != null)
			{
				list.Add(TextureDictionary);
			}
			if (Shaders != null)
			{
				list.Add(Shaders);
			}
			return list.ToArray();
		}
	}
}
