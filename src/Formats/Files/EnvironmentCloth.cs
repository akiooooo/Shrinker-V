using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class EnvironmentCloth : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public ulong Unknown_20h;

		public ulong Unknown_30h;

		public ulong Unknown_38h;

		public ulong Unknown_40h;

		public ulong Unknown_48h;

		public ulong Unknown_50h;

		public ulong Unknown_58h;

		public uint Unknown_6Ch;

		public ulong Unknown_70h;

		public uint Unknown_7Ch;

		private ResourceSystemStructBlock<uint> UnknownDataBlock;

		public override long BlockLength => 128L;

		public uint VFT { get; set; } = 1080059352u;

		public ulong InstanceTuningPointer { get; set; }

		public ulong DrawablePointer { get; set; }

		public ulong ControllerPointer { get; set; }

		public ulong UnknownPointer { get; set; }

		public ushort UnknownCount1 { get; set; }

		public ushort UnknownCount2 { get; set; }

		public uint Unknown_78h { get; set; }

		public ClothInstanceTuning InstanceTuning { get; set; }

		public FragDrawable Drawable { get; set; }

		public ClothController Controller { get; set; }

		public uint[] UnknownData { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			InstanceTuningPointer = reader.ReadUInt64();
			DrawablePointer = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			ControllerPointer = reader.ReadUInt64();
			Unknown_30h = reader.ReadUInt64();
			Unknown_38h = reader.ReadUInt64();
			Unknown_40h = reader.ReadUInt64();
			Unknown_48h = reader.ReadUInt64();
			Unknown_50h = reader.ReadUInt64();
			Unknown_58h = reader.ReadUInt64();
			UnknownPointer = reader.ReadUInt64();
			UnknownCount1 = reader.ReadUInt16();
			UnknownCount2 = reader.ReadUInt16();
			Unknown_6Ch = reader.ReadUInt32();
			Unknown_70h = reader.ReadUInt64();
			Unknown_78h = reader.ReadUInt32();
			Unknown_7Ch = reader.ReadUInt32();
			InstanceTuning = reader.ReadBlockAt<ClothInstanceTuning>(InstanceTuningPointer, Array.Empty<object>());
			Drawable = reader.ReadBlockAt<FragDrawable>(DrawablePointer, Array.Empty<object>());
			Controller = reader.ReadBlockAt<ClothController>(ControllerPointer, Array.Empty<object>());
			UnknownData = reader.ReadUintsAt(UnknownPointer, UnknownCount1);
			if (Drawable != null)
			{
				Drawable.OwnerCloth = this;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			InstanceTuningPointer = (ulong)((InstanceTuning != null) ? InstanceTuning.FilePosition : 0);
			DrawablePointer = (ulong)((Drawable != null) ? Drawable.FilePosition : 0);
			ControllerPointer = (ulong)((Controller != null) ? Controller.FilePosition : 0);
			UnknownPointer = (ulong)((UnknownDataBlock != null) ? UnknownDataBlock.FilePosition : 0);
			UnknownCount1 = (ushort)((UnknownDataBlock != null) ? ((uint)UnknownDataBlock.ItemCount) : 0u);
			UnknownCount2 = UnknownCount1;
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(InstanceTuningPointer);
			writer.Write(DrawablePointer);
			writer.Write(Unknown_20h);
			writer.Write(ControllerPointer);
			writer.Write(Unknown_30h);
			writer.Write(Unknown_38h);
			writer.Write(Unknown_40h);
			writer.Write(Unknown_48h);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_58h);
			writer.Write(UnknownPointer);
			writer.Write(UnknownCount1);
			writer.Write(UnknownCount2);
			writer.Write(Unknown_6Ch);
			writer.Write(Unknown_70h);
			writer.Write(Unknown_78h);
			writer.Write(Unknown_7Ch);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (InstanceTuning != null)
			{
				list.Add(InstanceTuning);
			}
			if (Drawable != null)
			{
				list.Add(Drawable);
			}
			if (Controller != null)
			{
				list.Add(Controller);
			}
			if (UnknownData != null)
			{
				UnknownDataBlock = new ResourceSystemStructBlock<uint>(UnknownData);
				list.Add(UnknownDataBlock);
			}
			return list.ToArray();
		}
	}
}
