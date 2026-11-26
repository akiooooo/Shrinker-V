using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragVehicleGlassWindows : ResourceSystemBlock
	{
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public struct WindowOffset
		{
			public uint ItemID { get; set; }

			public uint Offset { get; set; }

			public override string ToString()
			{
				return ItemID + ": " + Offset;
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class Window
		{
			public ushort UnkUshort3;

			public uint UnkUint2;

			public uint UnkUint3;

			public uint UnkUint4;

			public uint UnkUint5;

			[CompilerGenerated]
			private byte[] _003CPadding_003Ek__BackingField;

			public Matrix Projection { get; set; }

			public uint UnkUint1 { get; set; } = 1447515971u;

			public ushort ItemID { get; set; }

			public ushort UnkUshort1 { get; set; }

			public ushort ShatterMapWidth { get; set; }

			public ushort ItemDataCount { get; set; }

			public ushort ItemDataByteLength { get; set; }

			public float UnkFloat17 { get; set; }

			public float UnkFloat18 { get; set; }

			public ushort UnkUshort4 { get; set; }

			public ushort UnkUshort5 { get; set; }

			public float CracksTextureTiling { get; set; }

			public ushort[] ShatterMapRowOffsets { get; set; }

			public WindowShatterMapRow[] ShatterMap { get; set; }

			public byte[] Padding
			{
				[CompilerGenerated]
				set
				{
					_003CPadding_003Ek__BackingField = value;
				}
			}

			public uint ItemDataLength
			{
				get
				{
					uint num = ((ShatterMapRowOffsets != null) ? ((uint)(ItemDataCount * 2)) : 0u);
					if (ShatterMap != null)
					{
						WindowShatterMapRow[] shatterMap = ShatterMap;
						foreach (WindowShatterMapRow windowShatterMapRow in shatterMap)
						{
							num += windowShatterMapRow.TotalLength;
						}
					}
					return num;
				}
			}

			public uint TotalLength => 112 + ItemDataLength;

			public void Read(ResourceDataReader reader)
			{
				Projection = reader.ReadMatrix();
				UnkUint1 = reader.ReadUInt32();
				ItemID = reader.ReadUInt16();
				UnkUshort1 = reader.ReadUInt16();
				ShatterMapWidth = reader.ReadUInt16();
				ItemDataCount = reader.ReadUInt16();
				ItemDataByteLength = reader.ReadUInt16();
				UnkUshort3 = reader.ReadUInt16();
				UnkUint2 = reader.ReadUInt32();
				UnkUint3 = reader.ReadUInt32();
				UnkFloat17 = reader.ReadSingle();
				UnkFloat18 = reader.ReadSingle();
				UnkUshort4 = reader.ReadUInt16();
				UnkUshort5 = reader.ReadUInt16();
				CracksTextureTiling = reader.ReadSingle();
				UnkUint4 = reader.ReadUInt32();
				UnkUint5 = reader.ReadUInt32();
				if (ItemDataByteLength != 0)
				{
					ShatterMapRowOffsets = reader.ReadStructs<ushort>(ItemDataCount);
					ShatterMap = new WindowShatterMapRow[ItemDataCount];
					for (int i = 0; i < ItemDataCount; i++)
					{
						WindowShatterMapRow windowShatterMapRow = new WindowShatterMapRow();
						windowShatterMapRow.Read(reader);
						ShatterMap[i] = windowShatterMapRow;
					}
				}
			}

			public void Write(ResourceDataWriter writer)
			{
				writer.Write(Projection);
				writer.Write(UnkUint1);
				writer.Write(ItemID);
				writer.Write(UnkUshort1);
				writer.Write(ShatterMapWidth);
				writer.Write(ItemDataCount);
				writer.Write(ItemDataByteLength);
				writer.Write(UnkUshort3);
				writer.Write(UnkUint2);
				writer.Write(UnkUint3);
				writer.Write(UnkFloat17);
				writer.Write(UnkFloat18);
				writer.Write(UnkUshort4);
				writer.Write(UnkUshort5);
				writer.Write(CracksTextureTiling);
				writer.Write(UnkUint4);
				writer.Write(UnkUint5);
				writer.WriteStructs(ShatterMapRowOffsets);
				if (ShatterMap != null)
				{
					WindowShatterMapRow[] shatterMap = ShatterMap;
					for (int i = 0; i < shatterMap.Length; i++)
					{
						shatterMap[i].Write(writer);
					}
				}
			}

			public override string ToString()
			{
				return ItemID + ": " + UnkUshort1 + ": " + ShatterMapWidth + ": " + ItemDataCount + ": " + ItemDataByteLength + ": " + UnkUshort3;
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class WindowShatterMapRow
		{
			public byte Start1 { get; set; }

			public byte End1 { get; set; }

			public byte[] Data1 { get; set; }

			public byte Start2 { get; set; }

			public byte End2 { get; set; }

			public byte[] Data2 { get; set; }

			public uint DataLength
			{
				get
				{
					byte[] data = Data1;
					int num = 0 + ((data != null) ? data.Length : 0);
					byte[] data2 = Data2;
					return (uint)(num + ((data2 != null) ? data2.Length : 0));
				}
			}

			public uint TotalLength
			{
				get
				{
					uint num = 2u;
					num += DataLength;
					if (Data1 != null)
					{
						num++;
					}
					if (Start2 != byte.MaxValue)
					{
						num++;
					}
					return num;
				}
			}

			public void Read(ResourceDataReader reader)
			{
				Start1 = reader.ReadByte();
				End1 = reader.ReadByte();
				int num = End1 - Start1 + 1;
				if (num <= 0)
				{
					return;
				}
				Data1 = reader.ReadBytes(num);
				Start2 = reader.ReadByte();
				if (Start2 != byte.MaxValue)
				{
					End2 = reader.ReadByte();
					int num2 = End2 - Start2 + 1;
					if (num2 > 0)
					{
						Data2 = reader.ReadBytes(num2);
					}
				}
			}

			public void Write(ResourceDataWriter writer)
			{
				writer.Write(Start1);
				writer.Write(End1);
				if (Data1 == null)
				{
					return;
				}
				writer.Write(Data1);
				writer.Write(Start2);
				if (Start2 != byte.MaxValue)
				{
					writer.Write(End2);
					if (Data2 != null)
					{
						writer.Write(Data2);
					}
				}
			}

			public override string ToString()
			{
				return Start1 + ": " + End1 + ", " + Start2 + ": " + End2;
			}
		}

		public override long BlockLength => TotalLength;

		public uint Unknown_0h { get; set; } = 1447515976u;

		public ushort Unknown_4h { get; set; } = 112;

		public ushort ItemCount { get; set; }

		public uint TotalLength { get; set; }

		public WindowOffset[] WindowOffsets { get; set; }

		public uint UnkUint0 { get; set; }

		public Window[] Windows { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Unknown_0h = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt16();
			ItemCount = reader.ReadUInt16();
			TotalLength = reader.ReadUInt32();
			WindowOffsets = reader.ReadStructs<WindowOffset>((uint)(ItemCount + (ItemCount & 1)));
			UnkUint0 = reader.ReadUInt32();
			long num = 16 + WindowOffsets.Length * 8;
			Windows = new Window[ItemCount];
			for (int i = 0; i < ItemCount; i++)
			{
				long num2 = reader.Position;
				Window window = new Window();
				window.Read(reader);
				Windows[i] = window;
				num += reader.Position - num2;
				long num3 = (16 - num % 16) % 16;
				if (num3 > 0)
				{
					window.Padding = reader.ReadBytes((int)num3);
					num += num3;
				}
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			BuildOffsets();
			writer.Write(Unknown_0h);
			writer.Write(Unknown_4h);
			writer.Write(ItemCount);
			writer.Write(TotalLength);
			writer.WriteStructs(WindowOffsets);
			writer.Write(UnkUint0);
			long num = 16 + WindowOffsets.Length * 8;
			Window[] windows = Windows;
			foreach (Window obj in windows)
			{
				long num2 = writer.Position;
				obj.Write(writer);
				num += writer.Position - num2;
				long num3 = (16 - num % 16) % 16;
				if (num3 > 0)
				{
					writer.Write(new byte[num3]);
					num += num3;
				}
			}
		}

		public void BuildOffsets()
		{
			List<WindowOffset> list = new List<WindowOffset>();
			uint num = 16u;
			if (Windows != null)
			{
				num += (uint)((Windows.Length + (Windows.Length & 1)) * 8);
				Window[] windows = Windows;
				foreach (Window window in windows)
				{
					list.Add(new WindowOffset
					{
						ItemID = window.ItemID,
						Offset = num
					});
					num += window.TotalLength;
					num += (16 - num % 16) % 16;
				}
				if ((list.Count & 1) != 0)
				{
					list.Add(default(WindowOffset));
				}
			}
			TotalLength = num;
			WindowOffsets = list.ToArray();
		}
	}
}
