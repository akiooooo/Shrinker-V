using System;
using System.IO;
using GameFormats.Files;

namespace GameFormats.Graphics
{
	public static class DDSIO
	{
		public struct TexMetadata
		{
			public int width;

			public int height;

			public int depth;

			public int arraySize;

			public int mipLevels;

			public uint miscFlags;

			public uint miscFlags2;

			public DXGI_FORMAT format;

			public TEX_DIMENSION dimension;

			public bool IsCubemap()
			{
				return (miscFlags & 4) != 0;
			}

			public bool IsPMAlpha()
			{
				return (miscFlags2 & 7) == 2;
			}
		}

		public struct Image
		{
			public int width;

			public int height;

			public DXGI_FORMAT format;

			public int rowPitch;

			public int slicePitch;

			public int pixels;
		}

		public static class DXTex
		{
			public static uint DDS_HEADER_FLAGS_TEXTURE = 4103u;

			public static uint DDS_HEADER_FLAGS_MIPMAP = 131072u;

			public static uint DDS_HEADER_FLAGS_VOLUME = 8388608u;

			public static uint DDS_HEADER_FLAGS_PITCH = 8u;

			public static uint DDS_HEADER_FLAGS_LINEARSIZE = 524288u;

			public static uint DDS_HEIGHT = 2u;

			public static uint DDS_WIDTH = 4u;

			public static uint DDS_SURFACE_FLAGS_TEXTURE = 4096u;

			public static uint DDS_SURFACE_FLAGS_MIPMAP = 4194312u;

			public static uint DDS_SURFACE_FLAGS_CUBEMAP = 8u;

			public static uint DDS_CUBEMAP_POSITIVEX = 1536u;

			public static uint DDS_CUBEMAP_NEGATIVEX = 2560u;

			public static uint DDS_CUBEMAP_POSITIVEY = 4608u;

			public static uint DDS_CUBEMAP_NEGATIVEY = 8704u;

			public static uint DDS_CUBEMAP_POSITIVEZ = 16896u;

			public static uint DDS_CUBEMAP_NEGATIVEZ = 33280u;

			public static uint DDS_CUBEMAP_ALLFACES = DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX | DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY | DDS_CUBEMAP_POSITIVEZ | DDS_CUBEMAP_NEGATIVEZ;

			public static uint DDS_CUBEMAP = 512u;

			public static uint DDS_FLAGS_VOLUME = 2097152u;

			public static uint DDS_MAGIC = 542327876u;

			public static void ComputePitch(DXGI_FORMAT fmt, int width, int height, out int rowPitch, out int slicePitch, uint flags)
			{
				switch (fmt)
				{
				case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
				{
					assert(IsCompressed(fmt));
					int num3 = Math.Max(1, (width + 3) / 4);
					int num4 = Math.Max(1, (height + 3) / 4);
					rowPitch = num3 * 8;
					slicePitch = rowPitch * num4;
					return;
				}
				case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
				{
					assert(IsCompressed(fmt));
					int num = Math.Max(1, (width + 3) / 4);
					int num2 = Math.Max(1, (height + 3) / 4);
					rowPitch = num * 16;
					slicePitch = rowPitch * num2;
					return;
				}
				case DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_YUY2:
					assert(IsPacked(fmt));
					rowPitch = (width + 1 >> 1) * 4;
					slicePitch = rowPitch * height;
					return;
				case DXGI_FORMAT.DXGI_FORMAT_Y210:
				case DXGI_FORMAT.DXGI_FORMAT_Y216:
					assert(IsPacked(fmt));
					rowPitch = (width + 1 >> 1) * 8;
					slicePitch = rowPitch * height;
					return;
				case DXGI_FORMAT.DXGI_FORMAT_NV12:
				case DXGI_FORMAT.DXGI_FORMAT_420_OPAQUE:
					assert(IsPlanar(fmt));
					rowPitch = (width + 1 >> 1) * 2;
					slicePitch = rowPitch * (height + (height + 1 >> 1));
					return;
				case DXGI_FORMAT.DXGI_FORMAT_P010:
				case DXGI_FORMAT.DXGI_FORMAT_P016:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_D16_UNORM_S8_UINT:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R16_UNORM_X8_TYPELESS:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_X16_TYPELESS_G8_UINT:
					assert(IsPlanar(fmt));
					rowPitch = (width + 1 >> 1) * 4;
					slicePitch = rowPitch * (height + (height + 1 >> 1));
					return;
				case DXGI_FORMAT.DXGI_FORMAT_NV11:
					assert(IsPlanar(fmt));
					rowPitch = (width + 3 >> 2) * 4;
					slicePitch = rowPitch * height * 2;
					return;
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_P208:
					assert(IsPlanar(fmt));
					rowPitch = (width + 1 >> 1) * 2;
					slicePitch = rowPitch * height * 2;
					return;
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V208:
					assert(IsPlanar(fmt));
					rowPitch = width;
					slicePitch = rowPitch * (height + (height + 1 >> 1) * 2);
					return;
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V408:
					assert(IsPlanar(fmt));
					rowPitch = width;
					slicePitch = rowPitch * (height + (height >> 1) * 4);
					return;
				}
				assert(IsValid(fmt));
				assert(!IsCompressed(fmt) && !IsPacked(fmt) && !IsPlanar(fmt));
				int num5 = (((flags & 0x10000) != 0) ? 24 : (((flags & 0x20000) != 0) ? 16 : (((flags & 0x40000) == 0) ? BitsPerPixel(fmt) : 8)));
				if ((flags & 0x20F) != 0)
				{
					if ((flags & 0x200) != 0)
					{
						rowPitch = (width * num5 + 32767) / 32768 * 4096;
						slicePitch = rowPitch * height;
					}
					else if ((flags & 8) != 0)
					{
						rowPitch = (width * num5 + 511) / 512 * 64;
						slicePitch = rowPitch * height;
					}
					else if ((flags & 4) != 0)
					{
						rowPitch = (width * num5 + 255) / 256 * 32;
						slicePitch = rowPitch * height;
					}
					else if ((flags & 2) != 0)
					{
						rowPitch = (width * num5 + 127) / 128 * 16;
						slicePitch = rowPitch * height;
					}
					else
					{
						rowPitch = (width * num5 + 31) / 32 * 4;
						slicePitch = rowPitch * height;
					}
				}
				else
				{
					rowPitch = (width * num5 + 7) / 8;
					slicePitch = rowPitch * height;
				}
			}

			public static bool IsCompressed(DXGI_FORMAT fmt)
			{
				if (fmt - 70 <= DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SINT || fmt - 94 <= DXGI_FORMAT.DXGI_FORMAT_R32G32B32_TYPELESS)
				{
					return true;
				}
				return false;
			}

			public static bool IsPlanar(DXGI_FORMAT fmt)
			{
				switch (fmt)
				{
				case DXGI_FORMAT.DXGI_FORMAT_NV12:
				case DXGI_FORMAT.DXGI_FORMAT_P010:
				case DXGI_FORMAT.DXGI_FORMAT_P016:
				case DXGI_FORMAT.DXGI_FORMAT_420_OPAQUE:
				case DXGI_FORMAT.DXGI_FORMAT_NV11:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_D16_UNORM_S8_UINT:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R16_UNORM_X8_TYPELESS:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_X16_TYPELESS_G8_UINT:
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_P208:
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V208:
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V408:
					return true;
				default:
					return false;
				}
			}

			public static bool IsPacked(DXGI_FORMAT fmt)
			{
				if (fmt - 68 <= DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_TYPELESS || fmt - 107 <= DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT)
				{
					return true;
				}
				return false;
			}

			public static void assert(bool b)
			{
				if (!b)
				{
					throw new Exception();
				}
			}

			public static bool IsValid(DXGI_FORMAT fmt)
			{
				if ((int)fmt >= 1)
				{
					return (int)fmt <= 190;
				}
				return false;
			}

			public static bool IsPalettized(DXGI_FORMAT fmt)
			{
				if (fmt - 111 <= DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_UINT)
				{
					return true;
				}
				return false;
			}

			public static int BitsPerPixel(DXGI_FORMAT fmt)
			{
				switch (fmt)
				{
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_SINT:
					return 128;
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32B32_SINT:
					return 96;
				case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G32_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32G8X24_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT_S8X24_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_X32_TYPELESS_G8X24_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_Y416:
				case DXGI_FORMAT.DXGI_FORMAT_Y210:
				case DXGI_FORMAT.DXGI_FORMAT_Y216:
					return 64;
				case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R10G10B10A2_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R11G11B10_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R16G16_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_R32_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R32_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_R24G8_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_D24_UNORM_S8_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R24_UNORM_X8_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_X24_TYPELESS_G8_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R9G9B9E5_SHAREDEXP:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_AYUV:
				case DXGI_FORMAT.DXGI_FORMAT_Y410:
				case DXGI_FORMAT.DXGI_FORMAT_YUY2:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R10G10B10_7E3_A2_FLOAT:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R10G10B10_6E4_A2_FLOAT:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R10G10B10_SNORM_A2_UNORM:
					return 32;
				case DXGI_FORMAT.DXGI_FORMAT_P010:
				case DXGI_FORMAT.DXGI_FORMAT_P016:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_D16_UNORM_S8_UINT:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R16_UNORM_X8_TYPELESS:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_X16_TYPELESS_G8_UINT:
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V408:
					return 24;
				case DXGI_FORMAT.DXGI_FORMAT_R8G8_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R8G8_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_R16_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT:
				case DXGI_FORMAT.DXGI_FORMAT_D16_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R16_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R16_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R16_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R16_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_A8P8:
				case DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM:
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_P208:
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V208:
					return 16;
				case DXGI_FORMAT.DXGI_FORMAT_NV12:
				case DXGI_FORMAT.DXGI_FORMAT_420_OPAQUE:
				case DXGI_FORMAT.DXGI_FORMAT_NV11:
					return 12;
				case DXGI_FORMAT.DXGI_FORMAT_R8_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_R8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R8_UINT:
				case DXGI_FORMAT.DXGI_FORMAT_R8_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_R8_SINT:
				case DXGI_FORMAT.DXGI_FORMAT_A8_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_AI44:
				case DXGI_FORMAT.DXGI_FORMAT_IA44:
				case DXGI_FORMAT.DXGI_FORMAT_P8:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R4G4_UNORM:
					return 8;
				case DXGI_FORMAT.DXGI_FORMAT_R1_UNORM:
					return 1;
				case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
					return 4;
				case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
					return 8;
				default:
					return 0;
				}
			}

			public static int ComputeScanlines(DXGI_FORMAT fmt, int height)
			{
				switch (fmt)
				{
				case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
				case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
				case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
					assert(IsCompressed(fmt));
					return Math.Max(1, (height + 3) / 4);
				case DXGI_FORMAT.DXGI_FORMAT_NV11:
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_P208:
					assert(IsPlanar(fmt));
					return height * 2;
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V208:
					assert(IsPlanar(fmt));
					return height + (height + 1 >> 1) * 2;
				case DXGI_FORMAT.WIN10_DXGI_FORMAT_V408:
					assert(IsPlanar(fmt));
					return height + (height >> 1) * 4;
				case DXGI_FORMAT.DXGI_FORMAT_NV12:
				case DXGI_FORMAT.DXGI_FORMAT_P010:
				case DXGI_FORMAT.DXGI_FORMAT_P016:
				case DXGI_FORMAT.DXGI_FORMAT_420_OPAQUE:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_D16_UNORM_S8_UINT:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_R16_UNORM_X8_TYPELESS:
				case DXGI_FORMAT.XBOX_DXGI_FORMAT_X16_TYPELESS_G8_UINT:
					assert(IsPlanar(fmt));
					return height + (height + 1 >> 1);
				default:
					assert(IsValid(fmt));
					assert(!IsCompressed(fmt) && !IsPlanar(fmt));
					return height;
				}
			}

			public static bool _EncodeDDSHeader(TexMetadata metadata, uint flags, BinaryWriter bw, out int required)
			{
				required = 0;
				if (!IsValid(metadata.format))
				{
					return false;
				}
				if (IsPalettized(metadata.format))
				{
					return false;
				}
				if (metadata.arraySize > 1 && (metadata.arraySize != 6 || metadata.dimension != TEX_DIMENSION.TEX_DIMENSION_TEXTURE2D || !metadata.IsCubemap()))
				{
					flags |= 0x10000;
				}
				if ((flags & 0x20000) != 0)
				{
					flags |= 0x10000;
				}
				DDS_PIXELFORMAT ddspf = new DDS_PIXELFORMAT(0u);
				if ((flags & 0x10000) == 0)
				{
					switch (metadata.format)
					{
					case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_A8B8G8R8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R16G16_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_G16R16;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_A8L8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R16_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_L16;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_L8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_A8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_A8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R8G8_B8G8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_R8G8_B8G8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_G8R8_G8B8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_G8R8_G8B8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_DXT1;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
						ddspf = (metadata.IsPMAlpha() ? DDS_PIXELFORMAT.DDSPF_DXT2 : DDS_PIXELFORMAT.DDSPF_DXT3);
						break;
					case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
						ddspf = (metadata.IsPMAlpha() ? DDS_PIXELFORMAT.DDSPF_DXT4 : DDS_PIXELFORMAT.DDSPF_DXT5);
						break;
					case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_BC4_UNORM;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_BC4_SNORM;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_BC5_UNORM;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_BC5_SNORM;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_B5G6R5_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_R5G6B5;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_A1R5G5B5;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_A8R8G8B8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_X8R8G8B8;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_B4G4R4A4_UNORM:
						ddspf = DDS_PIXELFORMAT.DDSPF_A4R4G4B4;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_YUY2:
						ddspf = DDS_PIXELFORMAT.DDSPF_YUY2;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 116u;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_FLOAT:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 113u;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_UNORM:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 36u;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R16G16B16A16_SNORM:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 110u;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 115u;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R16G16_FLOAT:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 112u;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 114u;
						break;
					case DXGI_FORMAT.DXGI_FORMAT_R16_FLOAT:
						ddspf.dwSize = 32u;
						ddspf.dwFlags = DDS_PIXELFORMAT.DDS_FOURCC;
						ddspf.dwFourCC = 111u;
						break;
					}
				}
				int num = 124;
				int num2 = 20;
				required = 4 + num;
				if (ddspf.dwSize == 0)
				{
					required += num2;
				}
				DDS_HEADER dDS_HEADER = new DDS_HEADER
				{
					dwSize = (uint)num,
					dwFlags = DDS_HEADER_FLAGS_TEXTURE,
					dwCaps = DDS_SURFACE_FLAGS_TEXTURE
				};
				if (metadata.mipLevels > 0)
				{
					dDS_HEADER.dwFlags |= DDS_HEADER_FLAGS_MIPMAP;
					dDS_HEADER.dwMipMapCount = (uint)metadata.mipLevels;
					if (dDS_HEADER.dwMipMapCount > 1)
					{
						dDS_HEADER.dwCaps |= DDS_SURFACE_FLAGS_MIPMAP;
					}
				}
				switch (metadata.dimension)
				{
				case TEX_DIMENSION.TEX_DIMENSION_TEXTURE1D:
					dDS_HEADER.dwWidth = (uint)metadata.width;
					dDS_HEADER.dwHeight = (dDS_HEADER.dwDepth = 1u);
					break;
				case TEX_DIMENSION.TEX_DIMENSION_TEXTURE2D:
					dDS_HEADER.dwHeight = (uint)metadata.height;
					dDS_HEADER.dwWidth = (uint)metadata.width;
					dDS_HEADER.dwDepth = 1u;
					if (metadata.IsCubemap())
					{
						dDS_HEADER.dwCaps |= DDS_SURFACE_FLAGS_CUBEMAP;
						dDS_HEADER.dwCaps2 |= DDS_CUBEMAP_ALLFACES;
					}
					break;
				case TEX_DIMENSION.TEX_DIMENSION_TEXTURE3D:
					dDS_HEADER.dwFlags |= DDS_HEADER_FLAGS_VOLUME;
					dDS_HEADER.dwCaps2 |= DDS_FLAGS_VOLUME;
					dDS_HEADER.dwHeight = (uint)metadata.height;
					dDS_HEADER.dwWidth = (uint)metadata.width;
					dDS_HEADER.dwDepth = (uint)metadata.depth;
					break;
				default:
					return false;
				}
				ComputePitch(metadata.format, metadata.width, metadata.height, out var rowPitch, out var slicePitch, 0u);
				if (IsCompressed(metadata.format))
				{
					dDS_HEADER.dwFlags |= DDS_HEADER_FLAGS_LINEARSIZE;
					dDS_HEADER.dwPitchOrLinearSize = (uint)slicePitch;
				}
				else
				{
					dDS_HEADER.dwFlags |= DDS_HEADER_FLAGS_PITCH;
					dDS_HEADER.dwPitchOrLinearSize = (uint)rowPitch;
				}
				DDS_HEADER_DXT10 dDS_HEADER_DXT = default(DDS_HEADER_DXT10);
				if (ddspf.dwSize == 0)
				{
					dDS_HEADER.ddspf = DDS_PIXELFORMAT.DDSPF_DX10;
					dDS_HEADER_DXT.dxgiFormat = metadata.format;
					dDS_HEADER_DXT.resourceDimension = (uint)metadata.dimension;
					dDS_HEADER_DXT.miscFlag = metadata.miscFlags & 0xFFFFFFFBu;
					if ((metadata.miscFlags & 4) != 0)
					{
						dDS_HEADER_DXT.miscFlag |= 4u;
						assert(metadata.arraySize % 6 == 0);
						dDS_HEADER_DXT.arraySize = (uint)(metadata.arraySize / 6);
					}
					else
					{
						dDS_HEADER_DXT.arraySize = (uint)metadata.arraySize;
					}
					if ((flags & 0x20000) != 0)
					{
						dDS_HEADER_DXT.miscFlags2 = metadata.miscFlags2;
					}
				}
				else
				{
					dDS_HEADER.ddspf = ddspf;
				}
				bw.Write(DDS_MAGIC);
				bw.Write(dDS_HEADER.dwSize);
				bw.Write(dDS_HEADER.dwFlags);
				bw.Write(dDS_HEADER.dwHeight);
				bw.Write(dDS_HEADER.dwWidth);
				bw.Write(dDS_HEADER.dwPitchOrLinearSize);
				bw.Write(dDS_HEADER.dwDepth);
				bw.Write(dDS_HEADER.dwMipMapCount);
				for (int i = 0; i < 11; i++)
				{
					bw.Write(0);
				}
				bw.Write(dDS_HEADER.ddspf.dwSize);
				bw.Write(dDS_HEADER.ddspf.dwFlags);
				bw.Write(dDS_HEADER.ddspf.dwFourCC);
				bw.Write(dDS_HEADER.ddspf.dwRGBBitCount);
				bw.Write(dDS_HEADER.ddspf.dwRBitMask);
				bw.Write(dDS_HEADER.ddspf.dwGBitMask);
				bw.Write(dDS_HEADER.ddspf.dwBBitMask);
				bw.Write(dDS_HEADER.ddspf.dwABitMask);
				bw.Write(dDS_HEADER.dwCaps);
				bw.Write(dDS_HEADER.dwCaps2);
				bw.Write(dDS_HEADER.dwCaps3);
				bw.Write(dDS_HEADER.dwCaps4);
				bw.Write(dDS_HEADER.dwReserved2);
				if (ddspf.dwSize == 0)
				{
					bw.Write((uint)dDS_HEADER_DXT.dxgiFormat);
					bw.Write(dDS_HEADER_DXT.resourceDimension);
					bw.Write(dDS_HEADER_DXT.miscFlag);
					bw.Write(dDS_HEADER_DXT.arraySize);
					bw.Write(dDS_HEADER_DXT.miscFlags2);
				}
				return true;
			}
		}

		public class ImageStruct
		{
			public int Width;

			public int Height;

			public int Format;

			public int MipMapLevels;

			public byte[] Data;
		}

		public enum DXGI_FORMAT : uint
		{
			DXGI_FORMAT_UNKNOWN = 0u,
			DXGI_FORMAT_R32G32B32A32_TYPELESS = 1u,
			DXGI_FORMAT_R32G32B32A32_FLOAT = 2u,
			DXGI_FORMAT_R32G32B32A32_UINT = 3u,
			DXGI_FORMAT_R32G32B32A32_SINT = 4u,
			DXGI_FORMAT_R32G32B32_TYPELESS = 5u,
			DXGI_FORMAT_R32G32B32_FLOAT = 6u,
			DXGI_FORMAT_R32G32B32_UINT = 7u,
			DXGI_FORMAT_R32G32B32_SINT = 8u,
			DXGI_FORMAT_R16G16B16A16_TYPELESS = 9u,
			DXGI_FORMAT_R16G16B16A16_FLOAT = 10u,
			DXGI_FORMAT_R16G16B16A16_UNORM = 11u,
			DXGI_FORMAT_R16G16B16A16_UINT = 12u,
			DXGI_FORMAT_R16G16B16A16_SNORM = 13u,
			DXGI_FORMAT_R16G16B16A16_SINT = 14u,
			DXGI_FORMAT_R32G32_TYPELESS = 15u,
			DXGI_FORMAT_R32G32_FLOAT = 16u,
			DXGI_FORMAT_R32G32_UINT = 17u,
			DXGI_FORMAT_R32G32_SINT = 18u,
			DXGI_FORMAT_R32G8X24_TYPELESS = 19u,
			DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20u,
			DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21u,
			DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22u,
			DXGI_FORMAT_R10G10B10A2_TYPELESS = 23u,
			DXGI_FORMAT_R10G10B10A2_UNORM = 24u,
			DXGI_FORMAT_R10G10B10A2_UINT = 25u,
			DXGI_FORMAT_R11G11B10_FLOAT = 26u,
			DXGI_FORMAT_R8G8B8A8_TYPELESS = 27u,
			DXGI_FORMAT_R8G8B8A8_UNORM = 28u,
			DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29u,
			DXGI_FORMAT_R8G8B8A8_UINT = 30u,
			DXGI_FORMAT_R8G8B8A8_SNORM = 31u,
			DXGI_FORMAT_R8G8B8A8_SINT = 32u,
			DXGI_FORMAT_R16G16_TYPELESS = 33u,
			DXGI_FORMAT_R16G16_FLOAT = 34u,
			DXGI_FORMAT_R16G16_UNORM = 35u,
			DXGI_FORMAT_R16G16_UINT = 36u,
			DXGI_FORMAT_R16G16_SNORM = 37u,
			DXGI_FORMAT_R16G16_SINT = 38u,
			DXGI_FORMAT_R32_TYPELESS = 39u,
			DXGI_FORMAT_D32_FLOAT = 40u,
			DXGI_FORMAT_R32_FLOAT = 41u,
			DXGI_FORMAT_R32_UINT = 42u,
			DXGI_FORMAT_R32_SINT = 43u,
			DXGI_FORMAT_R24G8_TYPELESS = 44u,
			DXGI_FORMAT_D24_UNORM_S8_UINT = 45u,
			DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46u,
			DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47u,
			DXGI_FORMAT_R8G8_TYPELESS = 48u,
			DXGI_FORMAT_R8G8_UNORM = 49u,
			DXGI_FORMAT_R8G8_UINT = 50u,
			DXGI_FORMAT_R8G8_SNORM = 51u,
			DXGI_FORMAT_R8G8_SINT = 52u,
			DXGI_FORMAT_R16_TYPELESS = 53u,
			DXGI_FORMAT_R16_FLOAT = 54u,
			DXGI_FORMAT_D16_UNORM = 55u,
			DXGI_FORMAT_R16_UNORM = 56u,
			DXGI_FORMAT_R16_UINT = 57u,
			DXGI_FORMAT_R16_SNORM = 58u,
			DXGI_FORMAT_R16_SINT = 59u,
			DXGI_FORMAT_R8_TYPELESS = 60u,
			DXGI_FORMAT_R8_UNORM = 61u,
			DXGI_FORMAT_R8_UINT = 62u,
			DXGI_FORMAT_R8_SNORM = 63u,
			DXGI_FORMAT_R8_SINT = 64u,
			DXGI_FORMAT_A8_UNORM = 65u,
			DXGI_FORMAT_R1_UNORM = 66u,
			DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67u,
			DXGI_FORMAT_R8G8_B8G8_UNORM = 68u,
			DXGI_FORMAT_G8R8_G8B8_UNORM = 69u,
			DXGI_FORMAT_BC1_TYPELESS = 70u,
			DXGI_FORMAT_BC1_UNORM = 71u,
			DXGI_FORMAT_BC1_UNORM_SRGB = 72u,
			DXGI_FORMAT_BC2_TYPELESS = 73u,
			DXGI_FORMAT_BC2_UNORM = 74u,
			DXGI_FORMAT_BC2_UNORM_SRGB = 75u,
			DXGI_FORMAT_BC3_TYPELESS = 76u,
			DXGI_FORMAT_BC3_UNORM = 77u,
			DXGI_FORMAT_BC3_UNORM_SRGB = 78u,
			DXGI_FORMAT_BC4_TYPELESS = 79u,
			DXGI_FORMAT_BC4_UNORM = 80u,
			DXGI_FORMAT_BC4_SNORM = 81u,
			DXGI_FORMAT_BC5_TYPELESS = 82u,
			DXGI_FORMAT_BC5_UNORM = 83u,
			DXGI_FORMAT_BC5_SNORM = 84u,
			DXGI_FORMAT_B5G6R5_UNORM = 85u,
			DXGI_FORMAT_B5G5R5A1_UNORM = 86u,
			DXGI_FORMAT_B8G8R8A8_UNORM = 87u,
			DXGI_FORMAT_B8G8R8X8_UNORM = 88u,
			DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89u,
			DXGI_FORMAT_B8G8R8A8_TYPELESS = 90u,
			DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91u,
			DXGI_FORMAT_B8G8R8X8_TYPELESS = 92u,
			DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93u,
			DXGI_FORMAT_BC6H_TYPELESS = 94u,
			DXGI_FORMAT_BC6H_UF16 = 95u,
			DXGI_FORMAT_BC6H_SF16 = 96u,
			DXGI_FORMAT_BC7_TYPELESS = 97u,
			DXGI_FORMAT_BC7_UNORM = 98u,
			DXGI_FORMAT_BC7_UNORM_SRGB = 99u,
			DXGI_FORMAT_AYUV = 100u,
			DXGI_FORMAT_Y410 = 101u,
			DXGI_FORMAT_Y416 = 102u,
			DXGI_FORMAT_NV12 = 103u,
			DXGI_FORMAT_P010 = 104u,
			DXGI_FORMAT_P016 = 105u,
			DXGI_FORMAT_420_OPAQUE = 106u,
			DXGI_FORMAT_YUY2 = 107u,
			DXGI_FORMAT_Y210 = 108u,
			DXGI_FORMAT_Y216 = 109u,
			DXGI_FORMAT_NV11 = 110u,
			DXGI_FORMAT_AI44 = 111u,
			DXGI_FORMAT_IA44 = 112u,
			DXGI_FORMAT_P8 = 113u,
			DXGI_FORMAT_A8P8 = 114u,
			DXGI_FORMAT_B4G4R4A4_UNORM = 115u,
			DXGI_FORMAT_FORCE_UINT = uint.MaxValue,
			XBOX_DXGI_FORMAT_R10G10B10_7E3_A2_FLOAT = 116u,
			XBOX_DXGI_FORMAT_R10G10B10_6E4_A2_FLOAT = 117u,
			XBOX_DXGI_FORMAT_D16_UNORM_S8_UINT = 118u,
			XBOX_DXGI_FORMAT_R16_UNORM_X8_TYPELESS = 119u,
			XBOX_DXGI_FORMAT_X16_TYPELESS_G8_UINT = 120u,
			WIN10_DXGI_FORMAT_P208 = 130u,
			WIN10_DXGI_FORMAT_V208 = 131u,
			WIN10_DXGI_FORMAT_V408 = 132u,
			XBOX_DXGI_FORMAT_R10G10B10_SNORM_A2_UNORM = 189u,
			XBOX_DXGI_FORMAT_R4G4_UNORM = 190u
		}

		public enum TEX_DIMENSION : uint
		{
			TEX_DIMENSION_TEXTURE1D = 2u,
			TEX_DIMENSION_TEXTURE2D,
			TEX_DIMENSION_TEXTURE3D
		}

		public struct DDS_HEADER
		{
			public uint dwSize;

			public uint dwFlags;

			public uint dwHeight;

			public uint dwWidth;

			public uint dwPitchOrLinearSize;

			public uint dwDepth;

			public uint dwMipMapCount;

			public DDS_PIXELFORMAT ddspf;

			public uint dwCaps;

			public uint dwCaps2;

			public uint dwCaps3;

			public uint dwCaps4;

			public uint dwReserved2;
		}

		public struct DDS_HEADER_DXT10
		{
			public DXGI_FORMAT dxgiFormat;

			public uint resourceDimension;

			public uint miscFlag;

			public uint arraySize;

			public uint miscFlags2;
		}

		public struct DDS_PIXELFORMAT
		{
			public uint dwSize;

			public uint dwFlags;

			public uint dwFourCC;

			public uint dwRGBBitCount;

			public uint dwRBitMask;

			public uint dwGBitMask;

			public uint dwBBitMask;

			public uint dwABitMask;

			public static uint DDS_FOURCC = 4u;

			public static uint DDS_RGB = 64u;

			public static uint DDS_RGBA = 65u;

			public static uint DDS_LUMINANCE = 131072u;

			public static uint DDS_LUMINANCEA = 131073u;

			public static uint DDS_ALPHA = 2u;

			public static uint DDS_PAL8 = 32u;

			public static DDS_PIXELFORMAT DDSPF_DXT1 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('D', 'X', 'T', '1'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_DXT2 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('D', 'X', 'T', '2'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_DXT3 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('D', 'X', 'T', '3'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_DXT4 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('D', 'X', 'T', '4'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_DXT5 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('D', 'X', 'T', '5'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_BC4_UNORM = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('B', 'C', '4', 'U'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_BC4_SNORM = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('B', 'C', '4', 'S'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_BC5_UNORM = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('B', 'C', '5', 'U'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_BC5_SNORM = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('B', 'C', '5', 'S'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_R8G8_B8G8 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('R', 'G', 'B', 'G'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_G8R8_G8B8 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('G', 'R', 'G', 'B'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_YUY2 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('Y', 'U', 'Y', '2'), 0u, 0u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_A8R8G8B8 = new DDS_PIXELFORMAT(32u, DDS_RGBA, 0u, 32u, 16711680u, 65280u, 255u, 4278190080u);

			public static DDS_PIXELFORMAT DDSPF_X8R8G8B8 = new DDS_PIXELFORMAT(32u, DDS_RGB, 0u, 32u, 16711680u, 65280u, 255u, 0u);

			public static DDS_PIXELFORMAT DDSPF_A8B8G8R8 = new DDS_PIXELFORMAT(32u, DDS_RGBA, 0u, 32u, 255u, 65280u, 16711680u, 4278190080u);

			public static DDS_PIXELFORMAT DDSPF_X8B8G8R8 = new DDS_PIXELFORMAT(32u, DDS_RGB, 0u, 32u, 255u, 65280u, 16711680u, 0u);

			public static DDS_PIXELFORMAT DDSPF_G16R16 = new DDS_PIXELFORMAT(32u, DDS_RGB, 0u, 32u, 65535u, 4294901760u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_R5G6B5 = new DDS_PIXELFORMAT(32u, DDS_RGB, 0u, 16u, 63488u, 2016u, 31u, 0u);

			public static DDS_PIXELFORMAT DDSPF_A1R5G5B5 = new DDS_PIXELFORMAT(32u, DDS_RGBA, 0u, 16u, 31744u, 992u, 31u, 32768u);

			public static DDS_PIXELFORMAT DDSPF_A4R4G4B4 = new DDS_PIXELFORMAT(32u, DDS_RGBA, 0u, 16u, 3840u, 240u, 15u, 61440u);

			public static DDS_PIXELFORMAT DDSPF_R8G8B8 = new DDS_PIXELFORMAT(32u, DDS_RGB, 0u, 24u, 16711680u, 65280u, 255u, 0u);

			public static DDS_PIXELFORMAT DDSPF_L8 = new DDS_PIXELFORMAT(32u, DDS_LUMINANCE, 0u, 8u, 255u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_L16 = new DDS_PIXELFORMAT(32u, DDS_LUMINANCE, 0u, 16u, 65535u, 0u, 0u, 0u);

			public static DDS_PIXELFORMAT DDSPF_A8L8 = new DDS_PIXELFORMAT(32u, DDS_LUMINANCEA, 0u, 16u, 255u, 0u, 0u, 65280u);

			public static DDS_PIXELFORMAT DDSPF_A8 = new DDS_PIXELFORMAT(32u, DDS_ALPHA, 0u, 8u, 0u, 0u, 0u, 255u);

			public static DDS_PIXELFORMAT DDSPF_DX10 = new DDS_PIXELFORMAT(32u, DDS_FOURCC, MAKEFOURCC('D', 'X', '1', '0'), 0u, 0u, 0u, 0u, 0u);

			public DDS_PIXELFORMAT(uint val)
			{
				dwSize = val;
				dwFlags = val;
				dwFourCC = val;
				dwRGBBitCount = val;
				dwRBitMask = val;
				dwGBitMask = val;
				dwBBitMask = val;
				dwABitMask = val;
			}

			public DDS_PIXELFORMAT(uint size, uint flags, uint fourcc, uint rgbbitcount, uint rmask, uint gmask, uint bmask, uint amask)
			{
				dwSize = size;
				dwFlags = flags;
				dwFourCC = fourcc;
				dwRGBBitCount = rgbbitcount;
				dwRBitMask = rmask;
				dwGBitMask = gmask;
				dwBBitMask = bmask;
				dwABitMask = amask;
			}

			public static uint MAKEFOURCC(char ch0, char ch1, char ch2, char ch3)
			{
				return (uint)((byte)ch0 | ((byte)ch1 << 8) | ((byte)ch2 << 16) | ((byte)ch3 << 24));
			}
		}

		public static byte[] GetDDSFile(Texture texture)
		{
			DXGI_FORMAT dXGIFormat = GetDXGIFormat(texture.Format);
			ImageStruct imageStruct = GetImageStruct(texture, dXGIFormat);
			Image[] mipmapImages = GetMipmapImages(imageStruct, dXGIFormat);
			TexMetadata imageMetadata = GetImageMetadata(imageStruct, dXGIFormat);
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			int mipMapLevels = imageStruct.MipMapLevels;
			if (!DXTex._EncodeDDSHeader(imageMetadata, 0u, binaryWriter, out var _))
			{
				throw new Exception("Couldn't make DDS header");
			}
			switch (imageMetadata.dimension)
			{
			case TEX_DIMENSION.TEX_DIMENSION_TEXTURE1D:
			case TEX_DIMENSION.TEX_DIMENSION_TEXTURE2D:
			{
				int num6 = 0;
				for (int k = 0; k < imageMetadata.arraySize; k++)
				{
					int num7 = 0;
					while (num7 < imageMetadata.mipLevels)
					{
						if (num6 >= mipMapLevels)
						{
							throw new Exception("Tried to write mip out of range");
						}
						if (mipmapImages[num6].rowPitch <= 0)
						{
							throw new Exception("Invalid row pitch.");
						}
						if (mipmapImages[num6].slicePitch <= 0)
						{
							throw new Exception("Invalid slice pitch.");
						}
						DXTex.ComputePitch(imageMetadata.format, mipmapImages[num6].width, mipmapImages[num6].height, out var rowPitch3, out var slicePitch2, 0u);
						if (mipmapImages[num6].slicePitch == slicePitch2)
						{
							int num8 = slicePitch2;
							if (mipmapImages[num6].pixels + slicePitch2 > imageStruct.Data.Length)
							{
								num8 = imageStruct.Data.Length - mipmapImages[num6].pixels;
								_ = 0;
							}
							if (num8 > 0)
							{
								binaryWriter.Write(imageStruct.Data, mipmapImages[num6].pixels, num8);
							}
						}
						else
						{
							int rowPitch4 = mipmapImages[num6].rowPitch;
							if (rowPitch4 < rowPitch3)
							{
								throw new Exception("Input pitch isn't a full line of data");
							}
							int num9 = mipmapImages[num6].pixels;
							int num10 = DXTex.ComputeScanlines(imageMetadata.format, mipmapImages[num6].height);
							for (int l = 0; l < num10; l++)
							{
								binaryWriter.Write(imageStruct.Data, num9, rowPitch3);
								num9 += rowPitch4;
							}
						}
						num7++;
						num6++;
					}
				}
				break;
			}
			case TEX_DIMENSION.TEX_DIMENSION_TEXTURE3D:
			{
				if (imageMetadata.arraySize != 1)
				{
					throw new Exception("Texture3D must have arraySize == 1");
				}
				int num = imageMetadata.depth;
				int num2 = 0;
				for (int i = 0; i < imageMetadata.mipLevels; i++)
				{
					int num3 = 0;
					while (num3 < num)
					{
						if (num2 >= mipMapLevels)
						{
							throw new Exception("Tried to write mip out of range");
						}
						if (mipmapImages[num2].rowPitch <= 0)
						{
							throw new Exception("Invalid row pitch.");
						}
						if (mipmapImages[num2].slicePitch <= 0)
						{
							throw new Exception("Invalid slice pitch.");
						}
						DXTex.ComputePitch(imageMetadata.format, mipmapImages[num2].width, mipmapImages[num2].height, out var rowPitch, out var slicePitch, 0u);
						if (mipmapImages[num2].slicePitch == slicePitch)
						{
							binaryWriter.Write(imageStruct.Data, mipmapImages[num2].pixels, slicePitch);
						}
						else
						{
							int rowPitch2 = mipmapImages[num2].rowPitch;
							if (rowPitch2 < rowPitch)
							{
								throw new Exception("Input pitch isn't a full line of data");
							}
							int num4 = mipmapImages[num2].pixels;
							int num5 = DXTex.ComputeScanlines(imageMetadata.format, mipmapImages[num2].height);
							for (int j = 0; j < num5; j++)
							{
								binaryWriter.Write(imageStruct.Data, num4, rowPitch);
								num4 += rowPitch2;
							}
						}
						num3++;
						num2++;
					}
					if (num > 1)
					{
						num >>= 1;
					}
				}
				break;
			}
			default:
				throw new Exception("Unsupported texture dimension");
			}
			byte[] buffer = memoryStream.GetBuffer();
			byte[] array = new byte[memoryStream.Length];
			Array.Copy(buffer, array, array.Length);
			return array;
		}

		public static DXGI_FORMAT GetDXGIFormat(TextureFormat f)
		{
			DXGI_FORMAT result = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
			switch (f)
			{
			case TextureFormat.D3DFMT_DXT1:
				result = DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
				break;
			case TextureFormat.D3DFMT_DXT3:
				result = DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM;
				break;
			case TextureFormat.D3DFMT_DXT5:
				result = DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
				break;
			case TextureFormat.D3DFMT_ATI1:
				result = DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM;
				break;
			case TextureFormat.D3DFMT_ATI2:
				result = DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM;
				break;
			case TextureFormat.D3DFMT_BC7:
				result = DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
				break;
			case TextureFormat.D3DFMT_A1R5G5B5:
				result = DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM;
				break;
			case TextureFormat.D3DFMT_A8:
				result = DXGI_FORMAT.DXGI_FORMAT_A8_UNORM;
				break;
			case TextureFormat.D3DFMT_A8B8G8R8:
				result = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
				break;
			case TextureFormat.D3DFMT_L8:
				result = DXGI_FORMAT.DXGI_FORMAT_R8_UNORM;
				break;
			case TextureFormat.D3DFMT_A8R8G8B8:
				result = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
				break;
			case TextureFormat.D3DFMT_X8R8G8B8:
				result = DXGI_FORMAT.DXGI_FORMAT_B8G8R8X8_UNORM;
				break;
			}
			return result;
		}

		private static ImageStruct GetImageStruct(Texture texture, DXGI_FORMAT format)
		{
			return new ImageStruct
			{
				Data = texture.Data.FullData,
				Width = texture.Width,
				Height = texture.Height,
				MipMapLevels = texture.Levels,
				Format = (int)format
			};
		}

		private static Image[] GetMipmapImages(ImageStruct img, DXGI_FORMAT format)
		{
			Image[] array = new Image[img.MipMapLevels];
			int num = 0;
			int num2 = 1;
			int num3 = 0;
			for (int i = 0; i < img.MipMapLevels; i++)
			{
				array[i].width = img.Width / num2;
				array[i].height = img.Height / num2;
				array[i].format = format;
				array[i].pixels = num + num3;
				DXTex.ComputePitch(array[i].format, array[i].width, array[i].height, out array[i].rowPitch, out array[i].slicePitch, 0u);
				num3 += array[i].slicePitch;
				num2 *= 2;
			}
			return array;
		}

		private static TexMetadata GetImageMetadata(ImageStruct img, DXGI_FORMAT format)
		{
			return new TexMetadata
			{
				width = img.Width,
				height = img.Height,
				depth = 1,
				arraySize = 1,
				mipLevels = img.MipMapLevels,
				miscFlags = 0u,
				miscFlags2 = 0u,
				format = format,
				dimension = TEX_DIMENSION.TEX_DIMENSION_TEXTURE2D
			};
		}
	}
}
