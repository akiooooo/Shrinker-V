using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragDrawable : DrawableBase
	{
		public ulong Unknown_0A8h;

		public uint Unknown_104h;

		public ushort Unknown_112h = 1;

		public uint Unknown_114h;

		public ulong Unknown_118h;

		public ulong Unknown_120h;

		public ulong Unknown_128h;

		public ulong Unknown_138h;

		public ulong Unknown_140h;

		public ulong Unknown_148h;

		[CompilerGenerated]
		private FragType _003COwnerFragment_003Ek__BackingField;

		[CompilerGenerated]
		private FragPhysTypeChild _003COwnerFragmentPhys_003Ek__BackingField;

		[CompilerGenerated]
		private FragDrawable _003COwnerDrawable_003Ek__BackingField;

		private ResourceSystemStructBlock<ulong> FragMatricesIndsBlock;

		private ResourceSystemStructBlock<Matrix4F_s> FragMatricesBlock;

		private string_r NameBlock;

		public override long BlockLength => 336L;

		public Matrix4F_s FragMatrix { get; set; }

		public ulong BoundPointer { get; set; }

		public ulong FragMatricesIndsPointer { get; set; }

		public ushort FragMatricesIndsCount { get; set; }

		public ushort FragMatricesCapacity { get; set; }

		public ulong FragMatricesPointer { get; set; }

		public ushort FragMatricesCount { get; set; }

		public ulong NamePointer { get; set; }

		public Bounds Bound { get; set; }

		public ulong[] FragMatricesInds { get; set; }

		public Matrix4F_s[] FragMatrices { get; set; }

		public string Name { get; set; }

		public FragType OwnerFragment
		{
			[CompilerGenerated]
			set
			{
				_003COwnerFragment_003Ek__BackingField = value;
			}
		}

		public EnvironmentCloth OwnerCloth { get; set; }

		public FragPhysTypeChild OwnerFragmentPhys
		{
			[CompilerGenerated]
			set
			{
				_003COwnerFragmentPhys_003Ek__BackingField = value;
			}
		}

		public FragDrawable OwnerDrawable
		{
			[CompilerGenerated]
			set
			{
				_003COwnerDrawable_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_0A8h = reader.ReadUInt64();
			FragMatrix = reader.ReadStruct<Matrix4F_s>();
			BoundPointer = reader.ReadUInt64();
			FragMatricesIndsPointer = reader.ReadUInt64();
			FragMatricesIndsCount = reader.ReadUInt16();
			FragMatricesCapacity = reader.ReadUInt16();
			Unknown_104h = reader.ReadUInt32();
			FragMatricesPointer = reader.ReadUInt64();
			FragMatricesCount = reader.ReadUInt16();
			Unknown_112h = reader.ReadUInt16();
			Unknown_114h = reader.ReadUInt32();
			Unknown_118h = reader.ReadUInt64();
			Unknown_120h = reader.ReadUInt64();
			Unknown_128h = reader.ReadUInt64();
			NamePointer = reader.ReadUInt64();
			Unknown_138h = reader.ReadUInt64();
			Unknown_140h = reader.ReadUInt64();
			Unknown_148h = reader.ReadUInt64();
			Bound = reader.ReadBlockAt<Bounds>(BoundPointer, Array.Empty<object>());
			FragMatricesInds = reader.ReadUlongsAt(FragMatricesIndsPointer, FragMatricesIndsCount);
			FragMatrices = reader.ReadStructsAt<Matrix4F_s>(FragMatricesPointer, FragMatricesCapacity);
			Name = reader.ReadStringAt(NamePointer);
			if (Bound != null)
			{
				Bound.Owner = this;
			}
			_ = FragMatricesInds;
			_ = FragMatrices;
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			BoundPointer = (ulong)((Bound != null) ? Bound.FilePosition : 0);
			FragMatricesIndsPointer = (ulong)((FragMatricesIndsBlock != null) ? FragMatricesIndsBlock.FilePosition : 0);
			FragMatricesIndsCount = (ushort)((FragMatricesIndsBlock != null) ? ((uint)FragMatricesIndsBlock.ItemCount) : 0u);
			FragMatricesCapacity = (ushort)((FragMatricesBlock != null) ? ((uint)FragMatricesBlock.ItemCount) : 0u);
			FragMatricesPointer = (ulong)((FragMatricesBlock != null) ? FragMatricesBlock.FilePosition : 0);
			NamePointer = (ulong)((NameBlock != null) ? NameBlock.FilePosition : 0);
			writer.Write(Unknown_0A8h);
			writer.WriteStruct(FragMatrix);
			writer.Write(BoundPointer);
			writer.Write(FragMatricesIndsPointer);
			writer.Write(FragMatricesIndsCount);
			writer.Write(FragMatricesCapacity);
			writer.Write(Unknown_104h);
			writer.Write(FragMatricesPointer);
			writer.Write(FragMatricesCount);
			writer.Write(Unknown_112h);
			writer.Write(Unknown_114h);
			writer.Write(Unknown_118h);
			writer.Write(Unknown_120h);
			writer.Write(Unknown_128h);
			writer.Write(NamePointer);
			writer.Write(Unknown_138h);
			writer.Write(Unknown_140h);
			writer.Write(Unknown_148h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (Bound != null)
			{
				list.Add(Bound);
			}
			if (FragMatricesInds != null)
			{
				FragMatricesIndsBlock = new ResourceSystemStructBlock<ulong>(FragMatricesInds);
				list.Add(FragMatricesIndsBlock);
			}
			if (FragMatrices != null)
			{
				FragMatricesBlock = new ResourceSystemStructBlock<Matrix4F_s>(FragMatrices);
				list.Add(FragMatricesBlock);
			}
			if (Name != null)
			{
				NameBlock = (string_r)Name;
				list.Add(NameBlock);
			}
			return list.ToArray();
		}
	}
}
