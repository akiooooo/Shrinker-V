using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Drawable : DrawableBase
	{
		[CompilerGenerated]
		private string _003CErrorMessage_003Ek__BackingField;

		private string_r NameBlock;

		public override long BlockLength => 208L;

		public ulong NamePointer { get; set; }

		public ResourceSimpleList64<LightAttributes> LightAttributes { get; set; }

		public ulong UnkPointer { get; set; }

		public ulong BoundPointer { get; set; }

		public string Name { get; set; }

		public Bounds Bound { get; set; }

		public string ErrorMessage
		{
			[CompilerGenerated]
			set
			{
				_003CErrorMessage_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			NamePointer = reader.ReadUInt64();
			LightAttributes = reader.ReadBlock<ResourceSimpleList64<LightAttributes>>(Array.Empty<object>());
			UnkPointer = reader.ReadUInt64();
			BoundPointer = reader.ReadUInt64();
			try
			{
				Name = reader.ReadStringAt(NamePointer);
				Bound = reader.ReadBlockAt<Bounds>(BoundPointer, Array.Empty<object>());
				if (Bound != null)
				{
					Bound.Owner = this;
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.ToString();
			}
			_ = UnkPointer;
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			NamePointer = (ulong)((NameBlock != null) ? NameBlock.FilePosition : 0);
			BoundPointer = (ulong)((Bound != null) ? Bound.FilePosition : 0);
			writer.Write(NamePointer);
			writer.WriteBlock(LightAttributes);
			writer.Write(UnkPointer);
			writer.Write(BoundPointer);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (Name != null)
			{
				NameBlock = (string_r)Name;
				list.Add(NameBlock);
			}
			if (Bound != null)
			{
				list.Add(Bound);
			}
			return list.ToArray();
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[1]
			{
				new Tuple<long, IResourceBlock>(176L, LightAttributes)
			};
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
