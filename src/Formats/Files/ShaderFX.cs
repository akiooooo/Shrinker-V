using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ShaderFX : ResourceSystemBlock
	{
		public uint Unknown_Ch;

		public uint Unknown_1Ch;

		public ushort Unknown_24h;

		public byte Unknown_26h;

		public ulong Unknown_28h;

		public ulong G9_Unknown_28h;

		public ulong G9_Unknown_30h;

		public byte G9_Unknown_38h;

		public override long BlockLength => 48L;

		public override long BlockLength_Gen9 => 64L;

		public ulong ParametersPointer { get; set; }

		public MetaHash Name { get; set; }

		public byte ParameterCount { get; set; }

		public byte RenderBucket { get; set; }

		public ushort Unknown_12h { get; set; } = 32768;

		public ushort ParameterSize { get; set; }

		public ushort ParameterDataSize { get; set; }

		public MetaHash FileName { get; set; }

		public uint RenderBucketMask { get; set; }

		public byte TextureParametersCount { get; set; }

		public ShaderParametersBlock ParametersList { get; set; }

		public MetaHash G9_Preset { get; set; } = 1835365473u;

		public ulong G9_TextureRefsPointer { get; set; }

		public ulong G9_UnknownParamsPointer { get; set; }

		public ulong G9_ParamInfosPointer { get; set; }

		public ShaderParamInfosG9 G9_ParamInfos { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			if (reader.IsGen9)
			{
				Name = new MetaHash(reader.ReadUInt32());
				G9_Preset = reader.ReadUInt32();
				ParametersPointer = reader.ReadUInt64();
				G9_TextureRefsPointer = reader.ReadUInt64();
				G9_UnknownParamsPointer = reader.ReadUInt64();
				G9_ParamInfosPointer = reader.ReadUInt64();
				G9_Unknown_28h = reader.ReadUInt64();
				G9_Unknown_30h = reader.ReadUInt64();
				G9_Unknown_38h = reader.ReadByte();
				RenderBucket = reader.ReadByte();
				ParameterDataSize = reader.ReadUInt16();
				RenderBucketMask = reader.ReadUInt32();
				G9_ParamInfos = reader.ReadBlockAt<ShaderParamInfosG9>(G9_ParamInfosPointer, Array.Empty<object>());
				ParametersList = reader.ReadBlockAt<ShaderParametersBlock>(ParametersPointer, new object[2] { 0, this });
				FileName = JenkHash.GenHash(Name.ToCleanString() + ".sps");
				_ = G9_UnknownParamsPointer;
				_ = G9_Unknown_28h;
				_ = G9_Unknown_30h;
				_ = G9_Unknown_38h;
				_ = (uint)G9_Preset;
				_ = 1835365473;
			}
			else
			{
				ParametersPointer = reader.ReadUInt64();
				Name = new MetaHash(reader.ReadUInt32());
				Unknown_Ch = reader.ReadUInt32();
				ParameterCount = reader.ReadByte();
				RenderBucket = reader.ReadByte();
				Unknown_12h = reader.ReadUInt16();
				ParameterSize = reader.ReadUInt16();
				ParameterDataSize = reader.ReadUInt16();
				FileName = new MetaHash(reader.ReadUInt32());
				Unknown_1Ch = reader.ReadUInt32();
				RenderBucketMask = reader.ReadUInt32();
				Unknown_24h = reader.ReadUInt16();
				Unknown_26h = reader.ReadByte();
				TextureParametersCount = reader.ReadByte();
				Unknown_28h = reader.ReadUInt64();
				ParametersList = reader.ReadBlockAt<ShaderParametersBlock>(ParametersPointer, new object[2] { ParameterCount, this });
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			if (writer.IsGen9)
			{
				ParameterCount = (byte)(ParametersList?.Count ?? 0);
				ParameterDataSize = (ushort)ParametersList.G9_DataSize;
				ParametersPointer = (ulong)(ParametersList?.FilePosition ?? 0);
				G9_ParamInfosPointer = (ulong)(G9_ParamInfos?.FilePosition ?? 0);
				G9_TextureRefsPointer = ((ParametersPointer != 0L) ? (ParametersPointer + ParametersList.G9_TexturesOffset) : 0);
				G9_UnknownParamsPointer = ((ParametersPointer != 0L) ? (ParametersPointer + ParametersList.G9_UnknownsOffset) : 0);
				writer.Write(Name);
				writer.Write(G9_Preset);
				writer.Write(ParametersPointer);
				writer.Write(G9_TextureRefsPointer);
				writer.Write(G9_UnknownParamsPointer);
				writer.Write(G9_ParamInfosPointer);
				writer.Write(G9_Unknown_28h);
				writer.Write(G9_Unknown_30h);
				writer.Write(G9_Unknown_38h);
				writer.Write(RenderBucket);
				writer.Write(ParameterDataSize);
				writer.Write(RenderBucketMask);
			}
			else
			{
				ParametersPointer = (ulong)((ParametersList != null) ? ParametersList.FilePosition : 0);
				ParameterCount = (byte)((ParametersList != null) ? ((uint)ParametersList.Count) : 0u);
				writer.Write(ParametersPointer);
				writer.Write(Name.Hash);
				writer.Write(Unknown_Ch);
				writer.Write(ParameterCount);
				writer.Write(RenderBucket);
				writer.Write(Unknown_12h);
				writer.Write(ParameterSize);
				writer.Write(ParameterDataSize);
				writer.Write(FileName.Hash);
				writer.Write(Unknown_1Ch);
				writer.Write(RenderBucketMask);
				writer.Write(Unknown_24h);
				writer.Write(Unknown_26h);
				writer.Write(TextureParametersCount);
				writer.Write(Unknown_28h);
			}
		}

		public void EnsureGen9()
		{
			if (ParametersList == null)
			{
				return;
			}
			GameFileCache.EnsureShadersGen9ConversionData();
			GameFileCache.ShadersGen9ConversionData.TryGetValue(Name, out var value);
			int[] array = value?.BufferSizes;
			ShaderParamInfoG9[] array2 = value?.ParamInfos;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 1;
			uint[] array3 = new uint[(array != null) ? array.Length : 0];
			if (array != null)
			{
				num5 = 3;
				for (int i = 0; i < array.Length; i++)
				{
					num4 += (int)(array3[i] = (uint)array[i]);
				}
			}
			if (array2 != null)
			{
				num5 <<= 2;
				ShaderParamInfoG9[] array4 = array2;
				for (int j = 0; j < array4.Length; j++)
				{
					ShaderParamInfoG9 shaderParamInfoG = array4[j];
					switch (shaderParamInfoG.Type)
					{
					case ShaderParamTypeG9.Texture:
						num++;
						break;
					case ShaderParamTypeG9.Unknown:
						num2++;
						break;
					case ShaderParamTypeG9.Sampler:
						num3++;
						break;
					}
				}
			}
			ShaderParamInfosG9 shaderParamInfosG = new ShaderParamInfosG9();
			shaderParamInfosG.Params = array2;
			shaderParamInfosG.NumBuffers = (byte)((array != null) ? ((uint)array.Length) : 0u);
			shaderParamInfosG.NumParams = (byte)((array2 != null) ? ((uint)array2.Length) : 0u);
			shaderParamInfosG.NumTextures = (byte)num;
			shaderParamInfosG.NumUnknowns = (byte)num2;
			shaderParamInfosG.NumSamplers = (byte)num3;
			shaderParamInfosG.Unknown0 = 0;
			shaderParamInfosG.Unknown1 = 1;
			shaderParamInfosG.Unknown2 = (byte)num5;
			int num6 = shaderParamInfosG.NumBuffers * 8 * num5;
			int num7 = num4 * num5;
			int num8 = num * 8 * num5;
			int num9 = num2 * 8 * num5;
			int num10 = num3;
			int num11 = num6 + num7 + num8 + num9 + num10;
			ParametersList.G9_BuffersDataSize = (uint)num7;
			ParametersList.G9_TexturesOffset = (uint)(num6 + num7);
			ParametersList.G9_UnknownsOffset = (uint)(num6 + num7 + num8);
			ParametersList.G9_DataSize = num11;
			ParameterDataSize = (ushort)num11;
			_ = G9_ParamInfos;
			G9_ParamInfos = shaderParamInfosG;
			ParametersList.G9_ParamInfos = shaderParamInfosG;
			_ = ParametersList.G9_Samplers;
			ParametersList.G9_Samplers = value?.SamplerValues;
			_ = ParametersList.G9_BufferSizes;
			ParametersList.G9_BufferSizes = array3;
			ShaderParameter[] parameters = ParametersList.Parameters;
			if (parameters == null)
			{
				return;
			}
			ShaderParameter[] array5 = parameters;
			foreach (ShaderParameter shaderParameter in array5)
			{
				if (shaderParameter.Data is Texture texture)
				{
					TextureBase textureBase = new TextureBase();
					textureBase.Name = texture.Name;
					textureBase.NameHash = texture.NameHash;
					textureBase.G9_Dimension = texture.G9_Dimension;
					textureBase.G9_Flags = 2490368u;
					shaderParameter.Data = textureBase;
				}
				else if (shaderParameter.Data is TextureBase textureBase2)
				{
					textureBase2.VFT = 0u;
					textureBase2.Unknown_4h = 1u;
					if (textureBase2.G9_Flags == 0)
					{
						textureBase2.G9_Flags = 2490368u;
					}
				}
			}
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (G9_ParamInfos != null)
			{
				list.Add(G9_ParamInfos);
			}
			if (ParametersList != null)
			{
				list.Add(ParametersList);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return Name.ToString() + " (" + FileName.ToString() + ")";
		}
	}
}
