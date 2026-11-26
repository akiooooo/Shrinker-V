using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DrawableModelsBlock : ResourceSystemBlock
	{
		public DrawableBase Owner;

		public override long BlockLength
		{
			get
			{
				long num = 0L;
				num += ListLength(High, num);
				num += ListLength(Med, num);
				num += ListLength(Low, num);
				num += ListLength(VLow, num);
				return num + ListLength(Extra, num);
			}
		}

		public DrawableModel[] High { get; set; }

		public DrawableModel[] Med { get; set; }

		public DrawableModel[] Low { get; set; }

		public DrawableModel[] VLow { get; set; }

		public DrawableModel[] Extra { get; set; }

		public ResourcePointerListHeader HighHeader { get; set; }

		public ResourcePointerListHeader MedHeader { get; set; }

		public ResourcePointerListHeader LowHeader { get; set; }

		public ResourcePointerListHeader VLowHeader { get; set; }

		public ResourcePointerListHeader ExtraHeader { get; set; }

		public ulong[] HighPointers { get; set; }

		public ulong[] MedPointers { get; set; }

		public ulong[] LowPointers { get; set; }

		public ulong[] VLowPointers { get; set; }

		public ulong[] ExtraPointers { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Owner = parameters[0] as DrawableBase;
			ulong num = (ulong)reader.Position;
			ulong num2 = Owner?.DrawableModelsHighPointer ?? 0;
			ulong num3 = Owner?.DrawableModelsMediumPointer ?? 0;
			ulong num4 = Owner?.DrawableModelsLowPointer ?? 0;
			ulong num5 = Owner?.DrawableModelsVeryLowPointer ?? 0;
			ulong num6 = ((num != num2) ? num : 0);
			if (num2 != 0L)
			{
				HighHeader = reader.ReadStructAt<ResourcePointerListHeader>((long)num2);
				HighPointers = reader.ReadUlongsAt(HighHeader.Pointer, HighHeader.Capacity, cache: false);
				High = reader.ReadBlocks<DrawableModel>(HighPointers);
			}
			if (num3 != 0L)
			{
				MedHeader = reader.ReadStructAt<ResourcePointerListHeader>((long)num3);
				MedPointers = reader.ReadUlongsAt(MedHeader.Pointer, MedHeader.Capacity, cache: false);
				Med = reader.ReadBlocks<DrawableModel>(MedPointers);
			}
			if (num4 != 0L)
			{
				LowHeader = reader.ReadStructAt<ResourcePointerListHeader>((long)num4);
				LowPointers = reader.ReadUlongsAt(LowHeader.Pointer, LowHeader.Capacity, cache: false);
				Low = reader.ReadBlocks<DrawableModel>(LowPointers);
			}
			if (num5 != 0L)
			{
				VLowHeader = reader.ReadStructAt<ResourcePointerListHeader>((long)num5);
				VLowPointers = reader.ReadUlongsAt(VLowHeader.Pointer, VLowHeader.Capacity, cache: false);
				VLow = reader.ReadBlocks<DrawableModel>(VLowPointers);
			}
			if (num6 != 0L)
			{
				ExtraHeader = reader.ReadStructAt<ResourcePointerListHeader>((long)num6);
				ExtraPointers = reader.ReadUlongsAt(ExtraHeader.Pointer, ExtraHeader.Capacity, cache: false);
				Extra = reader.ReadBlocks<DrawableModel>(ExtraPointers);
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			long p = writer.Position;
			if (High != null)
			{
				HighHeader = makeHeader(ref p, High.Length);
				HighPointers = makePointers(ref p, High);
				write(HighHeader, HighPointers, High);
			}
			if (Med != null)
			{
				MedHeader = makeHeader(ref p, Med.Length);
				MedPointers = makePointers(ref p, Med);
				write(MedHeader, MedPointers, Med);
			}
			if (Low != null)
			{
				LowHeader = makeHeader(ref p, Low.Length);
				LowPointers = makePointers(ref p, Low);
				write(LowHeader, LowPointers, Low);
			}
			if (VLow != null)
			{
				VLowHeader = makeHeader(ref p, VLow.Length);
				VLowPointers = makePointers(ref p, VLow);
				write(VLowHeader, VLowPointers, VLow);
			}
			if (Extra != null)
			{
				ExtraHeader = makeHeader(ref p, Extra.Length);
				ExtraPointers = makePointers(ref p, Extra);
				write(ExtraHeader, ExtraPointers, Extra);
			}
			ResourcePointerListHeader makeHeader(ref long reference, int c)
			{
				reference += Pad(reference);
				ResourcePointerListHeader result = new ResourcePointerListHeader
				{
					Pointer = (ulong)(reference + 16),
					Count = (ushort)c,
					Capacity = (ushort)c
				};
				reference += HeaderLength(c);
				return result;
			}
			ulong[] makePointers(ref long reference, DrawableModel[] a)
			{
				ulong[] array = new ulong[a.Length];
				for (int i = 0; i < a.Length; i++)
				{
					reference += Pad(reference);
					array[i] = (ulong)reference;
					reference += a[i].BlockLength;
				}
				return array;
			}
			void write(ResourcePointerListHeader h, ulong[] val, DrawableModel[] a)
			{
				writer.WritePadding(16);
				writer.WriteStruct(h);
				writer.WriteUlongs(val);
				for (int i = 0; i < a.Length; i++)
				{
					writer.WritePadding(16);
					writer.WriteBlock(a[i]);
				}
			}
		}

		private long Pad(long o)
		{
			return (16 - o % 16) % 16;
		}

		private long HeaderLength(int listlength)
		{
			return 16 + listlength * 8;
		}

		private long ListLength(DrawableModel[] list, long o)
		{
			if (list == null)
			{
				return 0L;
			}
			long num = 0L;
			num += HeaderLength(list.Length);
			foreach (DrawableModel drawableModel in list)
			{
				num += Pad(num) + drawableModel.BlockLength;
			}
			return Pad(o) + num;
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			List<Tuple<long, IResourceBlock>> parts = new List<Tuple<long, IResourceBlock>>();
			parts.AddRange(base.GetParts());
			long p = 0L;
			addParts(ref p, High);
			addParts(ref p, Med);
			addParts(ref p, Low);
			addParts(ref p, VLow);
			addParts(ref p, Extra);
			return parts.ToArray();
			void addParts(ref long reference, DrawableModel[] a)
			{
				if (a != null)
				{
					reference += Pad(reference);
					reference += HeaderLength(a.Length);
					foreach (DrawableModel drawableModel in a)
					{
						reference += Pad(reference);
						parts.Add(new Tuple<long, IResourceBlock>(reference, drawableModel));
						reference += drawableModel.BlockLength;
					}
				}
			}
		}

		public long GetHighPointer()
		{
			if (High == null)
			{
				return 0L;
			}
			return FilePosition;
		}

		public long GetMedPointer()
		{
			if (Med == null)
			{
				return 0L;
			}
			long filePosition = FilePosition;
			filePosition += ListLength(High, filePosition);
			return filePosition + Pad(filePosition);
		}

		public long GetLowPointer()
		{
			if (Low == null)
			{
				return 0L;
			}
			long medPointer = GetMedPointer();
			medPointer += ListLength(Med, medPointer);
			return medPointer + Pad(medPointer);
		}

		public long GetVLowPointer()
		{
			if (VLow == null)
			{
				return 0L;
			}
			long lowPointer = GetLowPointer();
			lowPointer += ListLength(Low, lowPointer);
			return lowPointer + Pad(lowPointer);
		}
	}
}
