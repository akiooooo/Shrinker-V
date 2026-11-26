using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameFormats.Graphics;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TextureBase : ResourceSystemBlock
	{
		private string_r NameBlock;

		public override long BlockLength => 80L;

		public override long BlockLength_Gen9 => 80L;

		public uint VFT { get; set; }

		public uint Unknown_4h { get; set; } = 1u;

		public uint Unknown_8h { get; set; }

		public uint Unknown_Ch { get; set; }

		public uint Unknown_10h { get; set; }

		public uint Unknown_14h { get; set; }

		public uint Unknown_18h { get; set; }

		public uint Unknown_1Ch { get; set; }

		public uint Unknown_20h { get; set; }

		public uint Unknown_24h { get; set; }

		public ulong NamePointer { get; set; }

		public ushort Unknown_30h { get; set; } = 1;

		public ushort Unknown_32h { get; set; }

		public uint Unknown_34h { get; set; }

		public uint Unknown_38h { get; set; }

		public uint Unknown_3Ch { get; set; }

		public uint UsageData { get; set; }

		public uint Unknown_44h { get; set; }

		public uint ExtraFlags { get; set; }

		public uint Unknown_4Ch { get; set; }

		public ushort Width { get; set; }

		public ushort Height { get; set; }

		public ushort Depth { get; set; } = 1;

		public ushort Stride { get; set; }

		public TextureFormat Format { get; set; }

		public byte Unknown_5Ch { get; set; }

		public byte Levels { get; set; }

		public ushort Unknown_5Eh { get; set; }

		public uint Unknown_60h { get; set; }

		public uint Unknown_64h { get; set; }

		public uint Unknown_68h { get; set; }

		public uint Unknown_6Ch { get; set; }

		public ulong DataPointer { get; set; }

		public uint Unknown_78h { get; set; }

		public uint Unknown_7Ch { get; set; }

		public uint Unknown_80h { get; set; }

		public uint Unknown_84h { get; set; }

		public uint Unknown_88h { get; set; }

		public uint Unknown_8Ch { get; set; }

		public uint G9_BlockCount { get; set; }

		public uint G9_BlockStride { get; set; }

		public uint G9_Flags { get; set; }

		public TextureDimensionG9 G9_Dimension { get; set; } = TextureDimensionG9.Texture2D;

		public TextureFormatG9 G9_Format { get; set; }

		public TextureTileModeG9 G9_TileMode { get; set; } = TextureTileModeG9.Auto;

		public byte G9_AntiAliasType { get; set; }

		public byte G9_Unknown_23h { get; set; }

		public byte G9_Unknown_25h { get; set; }

		public ushort G9_UsageCount { get; set; } = 1;

		public ulong G9_SRVPointer { get; set; }

		public uint G9_UsageData { get; set; }

		public ulong G9_Unknown_48h { get; set; }

		public string Name { get; set; }

		public uint NameHash { get; set; }

		public TextureData Data { get; set; }

		public ShaderResourceViewG9 G9_SRV { get; set; }

		public TextureUsage Usage
		{
			get
			{
				return (TextureUsage)(UsageData & 0x1F);
			}
			set
			{
				UsageData = (UsageData & 0xFFFFFFE0u) + (uint)(value & (TextureUsage)31);
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			if (reader.IsGen9)
			{
				VFT = reader.ReadUInt32();
				Unknown_4h = reader.ReadUInt32();
				G9_BlockCount = reader.ReadUInt32();
				G9_BlockStride = reader.ReadUInt32();
				G9_Flags = reader.ReadUInt32();
				Unknown_14h = reader.ReadUInt32();
				Width = reader.ReadUInt16();
				Height = reader.ReadUInt16();
				Depth = reader.ReadUInt16();
				G9_Dimension = (TextureDimensionG9)reader.ReadByte();
				G9_Format = (TextureFormatG9)reader.ReadByte();
				G9_TileMode = (TextureTileModeG9)reader.ReadByte();
				G9_AntiAliasType = reader.ReadByte();
				Levels = reader.ReadByte();
				G9_Unknown_23h = reader.ReadByte();
				Unknown_24h = reader.ReadByte();
				G9_Unknown_25h = reader.ReadByte();
				G9_UsageCount = reader.ReadUInt16();
				NamePointer = reader.ReadUInt64();
				G9_SRVPointer = reader.ReadUInt64();
				DataPointer = reader.ReadUInt64();
				G9_UsageData = reader.ReadUInt32();
				Unknown_44h = reader.ReadUInt32();
				G9_Unknown_48h = reader.ReadUInt64();
				Format = GetLegacyFormat(G9_Format);
				Stride = CalculateStride();
				Usage = (TextureUsage)(G9_UsageData & 0x1F);
				Data = reader.ReadBlockAt<TextureData>(DataPointer, new object[1] { CalcDataSize() });
				G9_SRV = reader.ReadBlockAt<ShaderResourceViewG9>(G9_SRVPointer, Array.Empty<object>());
				Name = reader.ReadStringAt(NamePointer);
				if (!string.IsNullOrEmpty(Name))
				{
					NameHash = JenkHash.GenHash(Name.ToLowerInvariant());
				}
				uint g9_Flags = G9_Flags;
				if (g9_Flags != 2490368 && g9_Flags != 2490888)
				{
					_ = 2490920;
				}
				_ = Unknown_14h;
				TextureDimensionG9 g9_Dimension = G9_Dimension;
				if (g9_Dimension != TextureDimensionG9.Texture2D)
				{
					_ = 3;
				}
				_ = G9_TileMode;
				_ = 255;
				_ = G9_AntiAliasType;
				byte g9_Unknown_23h = G9_Unknown_23h;
				if (g9_Unknown_23h != 0 && g9_Unknown_23h != 40)
				{
					_ = 42;
				}
				_ = Unknown_24h;
				_ = G9_Unknown_25h;
				_ = G9_UsageCount;
				_ = 1;
				switch (Usage)
				{
				}
				g9_Flags = G9_UsageData >> 5;
				if (g9_Flags <= 98320)
				{
					switch (g9_Flags)
					{
					}
				}
				else if (g9_Flags <= 147472)
				{
					switch (g9_Flags)
					{
					}
				}
				else if (g9_Flags != 262144 && g9_Flags - 262160 > 4)
				{
					_ = 294928;
				}
				if (Unknown_44h != 0)
				{
					_ = 2;
				}
				_ = G9_Unknown_48h;
			}
			else
			{
				VFT = reader.ReadUInt32();
				Unknown_4h = reader.ReadUInt32();
				Unknown_8h = reader.ReadUInt32();
				Unknown_Ch = reader.ReadUInt32();
				Unknown_10h = reader.ReadUInt32();
				Unknown_14h = reader.ReadUInt32();
				Unknown_18h = reader.ReadUInt32();
				Unknown_1Ch = reader.ReadUInt32();
				Unknown_20h = reader.ReadUInt32();
				Unknown_24h = reader.ReadUInt32();
				NamePointer = reader.ReadUInt64();
				Unknown_30h = reader.ReadUInt16();
				Unknown_32h = reader.ReadUInt16();
				Unknown_34h = reader.ReadUInt32();
				Unknown_38h = reader.ReadUInt32();
				Unknown_3Ch = reader.ReadUInt32();
				UsageData = reader.ReadUInt32();
				Unknown_44h = reader.ReadUInt32();
				ExtraFlags = reader.ReadUInt32();
				Unknown_4Ch = reader.ReadUInt32();
				Name = reader.ReadStringAt(NamePointer);
				if (!string.IsNullOrEmpty(Name))
				{
					NameHash = JenkHash.GenHash(Name.ToLowerInvariant());
				}
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			if (writer.IsGen9)
			{
				NamePointer = (ulong)((NameBlock != null) ? NameBlock.FilePosition : 0);
				DataPointer = (ulong)((Data != null) ? Data.FilePosition : 0);
				G9_SRVPointer = (ulong)((G9_SRV != null) ? G9_SRV.FilePosition : 0);
				if (G9_Format == TextureFormatG9.UNKNOWN)
				{
					G9_Format = GetEnhancedFormat(Format);
				}
				if (G9_Dimension == (TextureDimensionG9)0)
				{
					G9_Dimension = TextureDimensionG9.Texture2D;
				}
				if (G9_TileMode == (TextureTileModeG9)0)
				{
					G9_TileMode = TextureTileModeG9.Auto;
				}
				G9_BlockCount = GetBlockCount(G9_Format, Width, Height, Depth, Levels, G9_Flags, G9_BlockCount);
				G9_BlockStride = GetBlockStride(G9_Format);
				G9_UsageData = (G9_UsageData & 0xFFFFFFE0u) + (uint)(Usage & (TextureUsage)31);
				writer.Write(VFT);
				writer.Write(Unknown_4h);
				writer.Write(G9_BlockCount);
				writer.Write(G9_BlockStride);
				writer.Write(G9_Flags);
				writer.Write(Unknown_14h);
				writer.Write(Width);
				writer.Write(Height);
				writer.Write(Depth);
				writer.Write((byte)G9_Dimension);
				writer.Write((byte)G9_Format);
				writer.Write((byte)G9_TileMode);
				writer.Write(G9_AntiAliasType);
				writer.Write(Levels);
				writer.Write(G9_Unknown_23h);
				writer.Write((byte)Unknown_24h);
				writer.Write(G9_Unknown_25h);
				writer.Write(G9_UsageCount);
				writer.Write(NamePointer);
				writer.Write(G9_SRVPointer);
				writer.Write(DataPointer);
				writer.Write(G9_UsageData);
				writer.Write(Unknown_44h);
				writer.Write(G9_Unknown_48h);
			}
			else
			{
				NamePointer = (ulong)((NameBlock != null) ? NameBlock.FilePosition : 0);
				writer.Write(VFT);
				writer.Write(Unknown_4h);
				writer.Write(Unknown_8h);
				writer.Write(Unknown_Ch);
				writer.Write(Unknown_10h);
				writer.Write(Unknown_14h);
				writer.Write(Unknown_18h);
				writer.Write(Unknown_1Ch);
				writer.Write(Unknown_20h);
				writer.Write(Unknown_24h);
				writer.Write(NamePointer);
				writer.Write(Unknown_30h);
				writer.Write(Unknown_32h);
				writer.Write(Unknown_34h);
				writer.Write(Unknown_38h);
				writer.Write(Unknown_3Ch);
				writer.Write(UsageData);
				writer.Write(Unknown_44h);
				writer.Write(ExtraFlags);
				writer.Write(Unknown_4Ch);
			}
		}

		public void EnsureGen9()
		{
			VFT = 0u;
			Unknown_4h = 1u;
			bool flag = this is Texture;
			Unknown_44h = (flag ? 2u : 0u);
			if (G9_Flags == 0)
			{
				G9_Flags = 2490888u;
				if (Name?.ToLowerInvariant()?.StartsWith("script_rt_") == true)
				{
					G9_Flags = 2490920u;
				}
			}
			if (G9_Unknown_23h == 0 && flag)
			{
				G9_Unknown_23h = 40;
			}
			if (G9_SRV == null)
			{
				G9_SRV = new ShaderResourceViewG9();
				G9_SRV.Dimension = ShaderResourceViewDimensionG9.Texture2D;
				if (Depth > 1)
				{
					G9_SRV.Dimension = ShaderResourceViewDimensionG9.Texture2DArray;
				}
			}
		}

		public int CalcDataSize()
		{
			if (Format == (TextureFormat)0u)
			{
				return 0;
			}
			DDSIO.DXGI_FORMAT dXGIFormat = DDSIO.GetDXGIFormat(Format);
			int num = 1;
			int num2 = 0;
			for (int i = 0; i < Levels; i++)
			{
				int width = Width / num;
				int height = Height / num;
				DDSIO.DXTex.ComputePitch(dXGIFormat, width, height, out var _, out var slicePitch, 0u);
				num2 += slicePitch;
				num *= 2;
			}
			return num2 * Depth;
		}

		public ushort CalculateStride()
		{
			if (Format == (TextureFormat)0u)
			{
				return 0;
			}
			DDSIO.DXTex.ComputePitch(DDSIO.GetDXGIFormat(Format), Width, Height, out var rowPitch, out var _, 0u);
			return (ushort)rowPitch;
		}

		public TextureFormat GetLegacyFormat(TextureFormatG9 format)
		{
			switch (format)
			{
			case TextureFormatG9.UNKNOWN:
				return (TextureFormat)0u;
			case TextureFormatG9.R8G8B8A8_UNORM:
				return TextureFormat.D3DFMT_A8B8G8R8;
			case TextureFormatG9.B8G8R8A8_UNORM:
				return TextureFormat.D3DFMT_A8R8G8B8;
			case TextureFormatG9.A8_UNORM:
				return TextureFormat.D3DFMT_A8;
			case TextureFormatG9.R8_UNORM:
				return TextureFormat.D3DFMT_L8;
			case TextureFormatG9.B5G5R5A1_UNORM:
				return TextureFormat.D3DFMT_A1R5G5B5;
			case TextureFormatG9.BC1_UNORM:
				return TextureFormat.D3DFMT_DXT1;
			case TextureFormatG9.BC2_UNORM:
				return TextureFormat.D3DFMT_DXT3;
			case TextureFormatG9.BC3_UNORM:
				return TextureFormat.D3DFMT_DXT5;
			case TextureFormatG9.BC4_UNORM:
				return TextureFormat.D3DFMT_ATI1;
			case TextureFormatG9.BC5_UNORM:
				return TextureFormat.D3DFMT_ATI2;
			case TextureFormatG9.BC7_UNORM:
				return TextureFormat.D3DFMT_BC7;
			case TextureFormatG9.BC7_UNORM_SRGB:
				return TextureFormat.D3DFMT_BC7;
			case TextureFormatG9.BC3_UNORM_SRGB:
				return TextureFormat.D3DFMT_DXT5;
			case TextureFormatG9.R16_UNORM:
				return TextureFormat.D3DFMT_A8;
			default:
				return TextureFormat.D3DFMT_A8R8G8B8;
			}
		}

		public TextureFormatG9 GetEnhancedFormat(TextureFormat format)
		{
			switch (format)
			{
			case (TextureFormat)0u:
				return TextureFormatG9.UNKNOWN;
			case TextureFormat.D3DFMT_A8B8G8R8:
				return TextureFormatG9.R8G8B8A8_UNORM;
			case TextureFormat.D3DFMT_A8R8G8B8:
				return TextureFormatG9.B8G8R8A8_UNORM;
			case TextureFormat.D3DFMT_A8:
				return TextureFormatG9.A8_UNORM;
			case TextureFormat.D3DFMT_L8:
				return TextureFormatG9.R8_UNORM;
			case TextureFormat.D3DFMT_A1R5G5B5:
				return TextureFormatG9.B5G5R5A1_UNORM;
			case TextureFormat.D3DFMT_DXT1:
				return TextureFormatG9.BC1_UNORM;
			case TextureFormat.D3DFMT_DXT3:
				return TextureFormatG9.BC2_UNORM;
			case TextureFormat.D3DFMT_DXT5:
				return TextureFormatG9.BC3_UNORM;
			case TextureFormat.D3DFMT_ATI1:
				return TextureFormatG9.BC4_UNORM;
			case TextureFormat.D3DFMT_ATI2:
				return TextureFormatG9.BC5_UNORM;
			case TextureFormat.D3DFMT_BC7:
				return TextureFormatG9.BC7_UNORM;
			default:
				return TextureFormatG9.B8G8R8A8_UNORM;
			}
		}

		public static uint GetBlockStride(TextureFormatG9 f)
		{
			switch (f)
			{
			default:
				return 8u;
			case TextureFormatG9.UNKNOWN:
				return 0u;
			case TextureFormatG9.BC1_UNORM:
				return 8u;
			case TextureFormatG9.BC2_UNORM:
				return 16u;
			case TextureFormatG9.BC3_UNORM:
				return 16u;
			case TextureFormatG9.BC4_UNORM:
				return 8u;
			case TextureFormatG9.BC5_UNORM:
				return 16u;
			case TextureFormatG9.BC6H_UF16:
				return 16u;
			case TextureFormatG9.BC7_UNORM:
				return 16u;
			case TextureFormatG9.BC1_UNORM_SRGB:
				return 8u;
			case TextureFormatG9.BC2_UNORM_SRGB:
				return 16u;
			case TextureFormatG9.BC3_UNORM_SRGB:
				return 16u;
			case TextureFormatG9.BC7_UNORM_SRGB:
				return 16u;
			case TextureFormatG9.R8G8B8A8_UNORM:
				return 4u;
			case TextureFormatG9.B8G8R8A8_UNORM:
				return 4u;
			case TextureFormatG9.R8G8B8A8_UNORM_SRGB:
				return 4u;
			case TextureFormatG9.B8G8R8A8_UNORM_SRGB:
				return 4u;
			case TextureFormatG9.B5G5R5A1_UNORM:
				return 2u;
			case TextureFormatG9.R10G10B10A2_UNORM:
				return 4u;
			case TextureFormatG9.R16G16B16A16_UNORM:
				return 8u;
			case TextureFormatG9.R16G16B16A16_FLOAT:
				return 8u;
			case TextureFormatG9.R16_UNORM:
				return 2u;
			case TextureFormatG9.R16_FLOAT:
				return 2u;
			case TextureFormatG9.R8_UNORM:
				return 1u;
			case TextureFormatG9.A8_UNORM:
				return 1u;
			case TextureFormatG9.R32_FLOAT:
				return 4u;
			case TextureFormatG9.R32G32B32A32_FLOAT:
				return 16u;
			case TextureFormatG9.R11G11B10_FLOAT:
				return 4u;
			}
		}

		public static uint GetBlockPixelCount(TextureFormatG9 f)
		{
			switch (f)
			{
			default:
				return 1u;
			case TextureFormatG9.BC1_UNORM:
			case TextureFormatG9.BC1_UNORM_SRGB:
			case TextureFormatG9.BC2_UNORM:
			case TextureFormatG9.BC2_UNORM_SRGB:
			case TextureFormatG9.BC3_UNORM:
			case TextureFormatG9.BC3_UNORM_SRGB:
			case TextureFormatG9.BC4_UNORM:
			case TextureFormatG9.BC5_UNORM:
			case TextureFormatG9.BC6H_UF16:
			case TextureFormatG9.BC7_UNORM:
			case TextureFormatG9.BC7_UNORM_SRGB:
				return 4u;
			}
		}

		public static uint GetBlockCount(TextureFormatG9 f, uint width, uint height, uint depth, uint mips, uint flags, uint oldval = 0u)
		{
			if (f == TextureFormatG9.UNKNOWN)
			{
				return 0u;
			}
			GetBlockStride(f);
			uint blockPixelCount = GetBlockPixelCount(f);
			uint num = width;
			uint num2 = height;
			uint num3 = depth;
			uint num4 = 1u;
			if (mips > 1)
			{
				for (num = 1u; num < width; num *= 2)
				{
				}
				for (num2 = 1u; num2 < height; num2 *= 2)
				{
				}
				for (num3 = 1u; num3 < depth; num3 *= 2)
				{
				}
			}
			uint num5 = 0u;
			for (int i = 0; i < mips; i++)
			{
				uint num6 = Math.Max(1u, (num + blockPixelCount - 1) / blockPixelCount);
				uint num7 = Math.Max(1u, (num2 + blockPixelCount - 1) / blockPixelCount);
				num6 += (num4 - num6 % num4) % num4;
				num7 += (num4 - num7 % num4) % num4;
				num5 += num6 * num7 * num3;
				num /= 2;
				num2 /= 2;
			}
			if (num5 != oldval && f != TextureFormatG9.A8_UNORM)
			{
				_ = 1;
				_ = 1;
			}
			return num5;
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (!string.IsNullOrEmpty(Name))
			{
				NameBlock = (string_r)Name;
				list.Add(NameBlock);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return "TextureBase: " + Name;
		}
	}
}
