using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct ShaderParamInfoG9
	{
		public MetaHash Name { get; set; }

		public uint Data { get; set; }

		public ShaderParamTypeG9 Type
		{
			get
			{
				return (ShaderParamTypeG9)(Data & 3);
			}
			set
			{
				Data = (Data & 0xFFFFFFF8u) + (uint)(value & ShaderParamTypeG9.CBuffer);
			}
		}

		public byte TextureIndex
		{
			get
			{
				return (byte)((Data >> 2) & 0xFF);
			}
			set
			{
				Data = (Data & 0xFFFFFC03u) + (uint)((value & 0xFF) << 2);
			}
		}

		public byte SamplerIndex
		{
			get
			{
				return (byte)((Data >> 2) & 0xFF);
			}
			set
			{
				Data = (Data & 0xFFFFFC03u) + (uint)((value & 0xFF) << 2);
			}
		}

		public byte CBufferIndex
		{
			get
			{
				return (byte)((Data >> 2) & 0x3F);
			}
			set
			{
				Data = (Data & 0xFFFFFF03u) + (uint)((value & 0x3F) << 2);
			}
		}

		public ushort ParamOffset
		{
			get
			{
				return (ushort)((Data >> 8) & 0xFFF);
			}
			set
			{
				Data = (Data & 0xFFF000FFu) + (uint)((value & 0xFFF) << 8);
			}
		}

		public ushort ParamLength
		{
			get
			{
				return (ushort)((Data >> 20) & 0xFFF);
			}
			set
			{
				Data = (Data & 0xFFFFF) + (uint)((value & 0xFFF) << 20);
			}
		}

		public override string ToString()
		{
			return $"{Name}: {Type}, {TextureIndex}, {ParamOffset}, {ParamLength}";
		}
	}
}
