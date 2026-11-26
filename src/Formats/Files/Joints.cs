using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class Joints : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public ushort Unknown_34h;

		public ushort Unknown_36h = 1;

		public ulong Unknown_38h;

		private ResourceSystemStructBlock<JointRotationLimit_s> RotationLimitsBlock;

		private ResourceSystemStructBlock<JointTranslationLimit_s> TranslationLimitsBlock;

		public override long BlockLength => 64L;

		public uint VFT { get; set; } = 1080130656u;

		public ulong RotationLimitsPointer { get; set; }

		public ulong TranslationLimitsPointer { get; set; }

		public ushort RotationLimitsCount { get; set; }

		public ushort TranslationLimitsCount { get; set; }

		public JointRotationLimit_s[] RotationLimits { get; set; }

		public JointTranslationLimit_s[] TranslationLimits { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			RotationLimitsPointer = reader.ReadUInt64();
			TranslationLimitsPointer = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			RotationLimitsCount = reader.ReadUInt16();
			TranslationLimitsCount = reader.ReadUInt16();
			Unknown_34h = reader.ReadUInt16();
			Unknown_36h = reader.ReadUInt16();
			Unknown_38h = reader.ReadUInt64();
			RotationLimits = reader.ReadStructsAt<JointRotationLimit_s>(RotationLimitsPointer, RotationLimitsCount);
			TranslationLimits = reader.ReadStructsAt<JointTranslationLimit_s>(TranslationLimitsPointer, TranslationLimitsCount);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			RotationLimitsPointer = (ulong)((RotationLimitsBlock != null) ? RotationLimitsBlock.FilePosition : 0);
			TranslationLimitsPointer = (ulong)((TranslationLimitsBlock != null) ? TranslationLimitsBlock.FilePosition : 0);
			RotationLimitsCount = (ushort)((RotationLimitsBlock != null) ? ((uint)RotationLimitsBlock.ItemCount) : 0u);
			TranslationLimitsCount = (ushort)((TranslationLimitsBlock != null) ? ((uint)TranslationLimitsBlock.ItemCount) : 0u);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(RotationLimitsPointer);
			writer.Write(TranslationLimitsPointer);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
			writer.Write(RotationLimitsCount);
			writer.Write(TranslationLimitsCount);
			writer.Write(Unknown_34h);
			writer.Write(Unknown_36h);
			writer.Write(Unknown_38h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (RotationLimits != null)
			{
				RotationLimitsBlock = new ResourceSystemStructBlock<JointRotationLimit_s>(RotationLimits);
				list.Add(RotationLimitsBlock);
			}
			if (TranslationLimits != null)
			{
				TranslationLimitsBlock = new ResourceSystemStructBlock<JointTranslationLimit_s>(TranslationLimits);
				list.Add(TranslationLimitsBlock);
			}
			return list.ToArray();
		}
	}
}
