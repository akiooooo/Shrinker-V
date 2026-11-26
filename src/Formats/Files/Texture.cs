using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Texture : TextureBase
	{
		public override long BlockLength => 144L;

		public override long BlockLength_Gen9 => 128L;

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			if (reader.IsGen9)
			{
				reader.ReadUInt64();
				reader.ReadUInt64();
				reader.ReadUInt64();
				reader.ReadUInt64();
				reader.ReadUInt64();
				reader.ReadUInt64();
				return;
			}
			base.Width = reader.ReadUInt16();
			base.Height = reader.ReadUInt16();
			base.Depth = reader.ReadUInt16();
			base.Stride = reader.ReadUInt16();
			base.Format = (TextureFormat)reader.ReadUInt32();
			base.Unknown_5Ch = reader.ReadByte();
			base.Levels = reader.ReadByte();
			base.Unknown_5Eh = reader.ReadUInt16();
			base.Unknown_60h = reader.ReadUInt32();
			base.Unknown_64h = reader.ReadUInt32();
			base.Unknown_68h = reader.ReadUInt32();
			base.Unknown_6Ch = reader.ReadUInt32();
			base.DataPointer = reader.ReadUInt64();
			base.Unknown_78h = reader.ReadUInt32();
			base.Unknown_7Ch = reader.ReadUInt32();
			base.Unknown_80h = reader.ReadUInt32();
			base.Unknown_84h = reader.ReadUInt32();
			base.Unknown_88h = reader.ReadUInt32();
			base.Unknown_8Ch = reader.ReadUInt32();
			base.Data = reader.ReadBlockAt<TextureData>(base.DataPointer, new object[5] { base.Format, base.Width, base.Height, base.Levels, base.Stride });
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			if (writer.IsGen9)
			{
				writer.Write(0uL);
				writer.WriteBlock(base.G9_SRV);
				writer.Write(0uL);
				return;
			}
			base.DataPointer = (ulong)base.Data.FilePosition;
			writer.Write(base.Width);
			writer.Write(base.Height);
			writer.Write(base.Depth);
			writer.Write(base.Stride);
			writer.Write((uint)base.Format);
			writer.Write(base.Unknown_5Ch);
			writer.Write(base.Levels);
			writer.Write(base.Unknown_5Eh);
			writer.Write(base.Unknown_60h);
			writer.Write(base.Unknown_64h);
			writer.Write(base.Unknown_68h);
			writer.Write(base.Unknown_6Ch);
			writer.Write(base.DataPointer);
			writer.Write(base.Unknown_78h);
			writer.Write(base.Unknown_7Ch);
			writer.Write(base.Unknown_80h);
			writer.Write(base.Unknown_84h);
			writer.Write(base.Unknown_88h);
			writer.Write(base.Unknown_8Ch);
		}

		public override IResourceBlock[] GetReferences()
		{
			return new List<IResourceBlock>(base.GetReferences()) { base.Data }.ToArray();
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			if (base.G9_SRV != null)
			{
				return new Tuple<long, IResourceBlock>[1]
				{
					new Tuple<long, IResourceBlock>(88L, base.G9_SRV)
				};
			}
			return base.GetParts();
		}

		public override string ToString()
		{
			return "Texture: " + base.Width + "x" + base.Height + ": " + base.Name;
		}
	}
}
