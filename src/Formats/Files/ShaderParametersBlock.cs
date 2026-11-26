using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ShaderParametersBlock : ResourceSystemBlock
	{
		private ResourceSystemStructBlock<Vector4>[] ParameterDataBlocks;

		public override long BlockLength
		{
			get
			{
				long baseSize = BaseSize;
				ushort parametersDataSize = ParametersDataSize;
				return baseSize + parametersDataSize * 4;
			}
		}

		public override long BlockLength_Gen9 => G9_DataSize;

		public long BaseSize
		{
			get
			{
				long num = 32L;
				if (Parameters != null)
				{
					ShaderParameter[] parameters = Parameters;
					foreach (ShaderParameter shaderParameter in parameters)
					{
						num += 16;
						num += 16 * shaderParameter.DataType;
					}
					num += Parameters.Length * 4;
				}
				return num;
			}
		}

		public ushort ParametersDataSize
		{
			get
			{
				long num = BaseSize;
				if (num % 16 != 0L)
				{
					num += 16 - num % 16;
				}
				return (ushort)num;
			}
		}

		public ShaderParameter[] Parameters { get; set; }

		public MetaName[] Hashes { get; set; }

		public int Count { get; set; }

		public ShaderFX Owner { get; set; }

		public long G9_DataSize { get; set; }

		public ShaderParamInfosG9 G9_ParamInfos { get; set; }

		public ulong[] G9_BufferPtrs { get; set; }

		public uint[] G9_BufferSizes { get; set; }

		public uint G9_BuffersDataSize { get; set; }

		public uint G9_TexturesOffset { get; set; }

		public uint G9_UnknownsOffset { get; set; }

		public ulong[] G9_TexturePtrs { get; set; }

		public ulong[] G9_UnknownData { get; set; }

		public byte[] G9_Samplers { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Count = Convert.ToInt32(parameters[0]);
			Owner = parameters[1] as ShaderFX;
			if (reader.IsGen9)
			{
				GameFileCache.EnsureShadersGen9ConversionData();
				GameFileCache.ShadersGen9ConversionData.TryGetValue(Owner.Name, out var value);
				Dictionary<MetaHash, MetaHash> dictionary = value?.ParamsMapGen9ToLegacy;
				G9_ParamInfos = Owner.G9_ParamInfos;
				int unknown = G9_ParamInfos.Unknown2;
				uint num = (uint)unknown;
				byte numBuffers = G9_ParamInfos.NumBuffers;
				long num2 = reader.Position;
				G9_BufferPtrs = reader.ReadStructs<ulong>(numBuffers * num);
				G9_BufferSizes = new uint[numBuffers];
				for (int i = 0; i < numBuffers; i++)
				{
					G9_BufferSizes[i] = (uint)(G9_BufferPtrs[i + 1] - G9_BufferPtrs[i]);
				}
				ulong num3 = 0uL;
				ulong num4 = 0uL;
				if (G9_BufferPtrs != null && G9_BufferPtrs.Length > numBuffers)
				{
					num3 = G9_BufferPtrs[0];
					num4 = G9_BufferPtrs[numBuffers];
				}
				int num5 = numBuffers * 8 * unknown;
				int num6 = (int)(num4 - num3) * unknown;
				int num7 = G9_ParamInfos.NumTextures * 8 * unknown;
				int num8 = G9_ParamInfos.NumUnknowns * 8 * unknown;
				byte numSamplers = G9_ParamInfos.NumSamplers;
				int num9 = num5 + num6 + num7 + num8 + numSamplers;
				G9_BuffersDataSize = (uint)num6;
				G9_TexturesOffset = (uint)(num5 + num6);
				G9_UnknownsOffset = (uint)(num5 + num6 + num7);
				G9_DataSize = num9;
				if (Owner.G9_TextureRefsPointer != 0L)
				{
					G9_TexturePtrs = reader.ReadUlongsAt(Owner.G9_TextureRefsPointer, G9_ParamInfos.NumTextures * num, cache: false);
				}
				if (Owner.G9_UnknownParamsPointer != 0L)
				{
					G9_UnknownData = reader.ReadUlongsAt(Owner.G9_UnknownParamsPointer, G9_ParamInfos.NumUnknowns * num, cache: false);
				}
				if (G9_ParamInfos.NumSamplers > 0)
				{
					G9_Samplers = reader.ReadBytesAt((ulong)(num2 + (num5 + num6 + num7 + num8)), G9_ParamInfos.NumSamplers, cache: false);
				}
				List<ShaderParameter> list = new List<ShaderParameter>();
				List<MetaName> list2 = new List<MetaName>();
				ShaderParamInfoG9[] array = G9_ParamInfos.Params;
				for (int j = 0; j < array.Length; j++)
				{
					ShaderParamInfoG9 shaderParamInfoG = array[j];
					uint num10 = shaderParamInfoG.Name.Hash;
					if (dictionary != null && dictionary.TryGetValue(num10, out var value2))
					{
						num10 = value2;
					}
					if (shaderParamInfoG.Type == ShaderParamTypeG9.Texture)
					{
						ShaderParameter shaderParameter = new ShaderParameter();
						shaderParameter.DataType = 0;
						shaderParameter.DataPointer = G9_TexturePtrs[shaderParamInfoG.TextureIndex];
						shaderParameter.Data = reader.ReadBlockAt<TextureBase>(shaderParameter.DataPointer, Array.Empty<object>());
						list.Add(shaderParameter);
						list2.Add((MetaName)num10);
						if (shaderParameter.Data is TextureBase textureBase && textureBase.G9_SRV == null)
						{
						}
					}
					else
					{
						if (shaderParamInfoG.Type != ShaderParamTypeG9.CBuffer)
						{
							continue;
						}
						uint num11 = (uint)shaderParamInfoG.ParamLength / 4u;
						uint num12 = (uint)shaderParamInfoG.ParamLength / 16u;
						ShaderParameter shaderParameter2 = new ShaderParameter();
						shaderParameter2.DataType = (byte)Math.Max(num12, 1u);
						_ = shaderParamInfoG.ParamLength % 4;
						byte cBufferIndex = shaderParamInfoG.CBufferIndex;
						long num13 = (long)((G9_BufferPtrs != null && G9_BufferPtrs.Length > cBufferIndex) ? G9_BufferPtrs[cBufferIndex] : 0);
						if (num13 != 0L)
						{
							long num14 = num13 + shaderParamInfoG.ParamOffset;
							switch (num11)
							{
							case 1u:
								shaderParameter2.Data = new Vector4(reader.ReadStructAt<float>(num14), 0f, 0f, 0f);
								break;
							case 2u:
								shaderParameter2.Data = new Vector4(reader.ReadStructAt<Vector2>(num14), 0f, 0f);
								break;
							case 3u:
								shaderParameter2.Data = new Vector4(reader.ReadStructAt<Vector3>(num14), 0f);
								break;
							case 4u:
								shaderParameter2.Data = reader.ReadStructAt<Vector4>(num14);
								break;
							default:
								shaderParameter2.Data = reader.ReadStructsAt<Vector4>((ulong)num14, num12, cache: false);
								break;
							case 0u:
								break;
							}
						}
						list.Add(shaderParameter2);
						list2.Add((MetaName)num10);
					}
				}
				Parameters = list.ToArray();
				Hashes = list2.ToArray();
				Count = list.Count;
				return;
			}
			List<ShaderParameter> list3 = new List<ShaderParameter>();
			for (int k = 0; k < Count; k++)
			{
				ShaderParameter shaderParameter3 = new ShaderParameter();
				shaderParameter3.Read(reader);
				list3.Add(shaderParameter3);
			}
			int num15 = 0;
			for (int l = 0; l < Count; l++)
			{
				ShaderParameter shaderParameter4 = list3[l];
				switch (shaderParameter4.DataType)
				{
				case 0:
					num15 = num15;
					shaderParameter4.Data = reader.ReadBlockAt<TextureBase>(shaderParameter4.DataPointer, Array.Empty<object>());
					break;
				case 1:
					num15 += 16;
					shaderParameter4.Data = reader.ReadStructAt<Vector4>((long)shaderParameter4.DataPointer);
					break;
				default:
					num15 += 16 * shaderParameter4.DataType;
					shaderParameter4.Data = reader.ReadStructsAt<Vector4>(shaderParameter4.DataPointer, shaderParameter4.DataType, cache: false);
					break;
				}
			}
			reader.Position += num15;
			List<MetaName> list4 = new List<MetaName>();
			for (int m = 0; m < Count; m++)
			{
				list4.Add((MetaName)reader.ReadUInt32());
			}
			Parameters = list3.ToArray();
			Hashes = list4.ToArray();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			byte[] buf0;
			if (writer.IsGen9)
			{
				GameFileCache.EnsureShadersGen9ConversionData();
				GameFileCache.ShadersGen9ConversionData.TryGetValue(Owner.Name, out var value);
				Dictionary<MetaHash, MetaHash> dictionary = value?.ParamsMapLegacyToGen9;
				if (G9_ParamInfos == null)
				{
					G9_ParamInfos = Owner.G9_ParamInfos;
				}
				int unknown = G9_ParamInfos.Unknown2;
				uint num = (uint)unknown;
				byte numBuffers = G9_ParamInfos.NumBuffers;
				byte numTextures = G9_ParamInfos.NumTextures;
				byte numUnknowns = G9_ParamInfos.NumUnknowns;
				long filePosition = FilePosition;
				uint[] g9_BufferSizes = G9_BufferSizes;
				uint[] array = new uint[(g9_BufferSizes != null) ? g9_BufferSizes.Length : 0];
				int num2 = numBuffers * 8 * unknown;
				ulong[] array2 = new ulong[numBuffers * num];
				ulong num3 = (ulong)(filePosition + num2);
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < numBuffers; j++)
					{
						array2[i * numBuffers + j] = num3;
						num3 += g9_BufferSizes[j];
					}
				}
				uint num4 = 0u;
				for (int k = 0; k < g9_BufferSizes.Length; k++)
				{
					array[k] = num4;
					num4 += g9_BufferSizes[k];
				}
				_ = G9_BufferPtrs;
				G9_BufferPtrs = array2;
				writer.WriteUlongs(array2);
				int num5 = (int)(G9_BuffersDataSize / num);
				buf0 = new byte[num5];
				ulong[] array3 = new ulong[numTextures * num];
				if (Parameters != null && dictionary != null)
				{
					Dictionary<uint, ShaderParameter> dictionary2 = new Dictionary<uint, ShaderParameter>();
					int val = Parameters.Length;
					MetaName[] hashes = Hashes;
					int num6 = Math.Min(val, (hashes != null) ? hashes.Length : 0);
					for (int l = 0; l < num6; l++)
					{
						uint num7 = (uint)Hashes[l];
						if (!dictionary.TryGetValue(num7, out var value2))
						{
							value2 = num7;
						}
						if ((uint)value2 != 0)
						{
							dictionary2[value2] = Parameters[l];
						}
					}
					ShaderParamInfoG9[] array4 = G9_ParamInfos.Params;
					for (int m = 0; m < array4.Length; m++)
					{
						ShaderParamInfoG9 shaderParamInfoG = array4[m];
						dictionary2.TryGetValue(shaderParamInfoG.Name, out var value3);
						if (shaderParamInfoG.Type == ShaderParamTypeG9.Texture)
						{
							object obj = value3?.Data;
							array3[shaderParamInfoG.TextureIndex] = (ulong)((obj as TextureBase)?.FilePosition ?? 0);
						}
						else if (shaderParamInfoG.Type == ShaderParamTypeG9.CBuffer)
						{
							object obj2 = value3?.Data;
							uint num8 = (uint)shaderParamInfoG.ParamLength / 4u;
							uint num9 = (uint)shaderParamInfoG.ParamLength / 16u;
							byte cBufferIndex = shaderParamInfoG.CBufferIndex;
							int o = (int)(((array != null && array.Length > cBufferIndex) ? array[cBufferIndex] : 0) + shaderParamInfoG.ParamOffset);
							Vector4 val2 = ((obj2 is Vector4) ? ((Vector4)obj2) : Vector4.Zero);
							switch (num8)
							{
							case 1u:
								writeStruct<float>(o, val2.X);
								break;
							case 2u:
								writeStruct<Vector2>(o, new Vector2(val2.X, val2.Y));
								break;
							case 3u:
								writeStruct<Vector3>(o, new Vector3(val2.X, val2.Y, val2.Z));
								break;
							case 4u:
								writeStruct<Vector4>(o, val2);
								break;
							default:
								writeStructs<Vector4>(o, obj2 as Vector4[]);
								break;
							case 0u:
								break;
							}
						}
					}
				}
				for (int n = 0; n < num; n++)
				{
					writer.Write(buf0);
				}
				_ = G9_TexturePtrs;
				G9_TexturePtrs = ((array3.Length != 0) ? array3 : null);
				if (G9_TexturePtrs != null)
				{
					writer.WriteUlongs(G9_TexturePtrs);
				}
				_ = G9_UnknownData;
				G9_UnknownData = ((numUnknowns > 0) ? new ulong[numUnknowns * num] : null);
				if (G9_UnknownData != null)
				{
					writer.WriteUlongs(G9_UnknownData);
				}
				if (G9_Samplers != null)
				{
					writer.Write(G9_Samplers);
				}
				return;
			}
			for (int num10 = 0; num10 < Parameters.Length; num10++)
			{
				ShaderParameter shaderParameter = Parameters[num10];
				if (shaderParameter.DataType == 0)
				{
					shaderParameter.DataPointer = (ulong)((shaderParameter.Data as TextureBase)?.FilePosition ?? 0);
					continue;
				}
				ResourceSystemStructBlock<Vector4> resourceSystemStructBlock = ((num10 < ParameterDataBlocks?.Length) ? ParameterDataBlocks[num10] : null);
				if (resourceSystemStructBlock != null)
				{
					shaderParameter.DataPointer = (ulong)resourceSystemStructBlock.FilePosition;
				}
				else
				{
					shaderParameter.DataPointer = 0uL;
				}
			}
			ShaderParameter[] parameters2 = Parameters;
			for (int m = 0; m < parameters2.Length; m++)
			{
				parameters2[m].Write(writer);
			}
			for (int num11 = 0; num11 < Parameters.Length; num11++)
			{
				if (Parameters[num11].DataType != 0)
				{
					ResourceSystemStructBlock<Vector4> resourceSystemStructBlock2 = ((num11 < ParameterDataBlocks?.Length) ? ParameterDataBlocks[num11] : null);
					if (resourceSystemStructBlock2 != null)
					{
						writer.WriteBlock(resourceSystemStructBlock2);
					}
				}
			}
			MetaName[] hashes2 = Hashes;
			foreach (MetaName value4 in hashes2)
			{
				writer.Write((uint)value4);
			}
			ushort parametersDataSize = ParametersDataSize;
			writer.Write(new byte[32 + parametersDataSize * 4]);
			void writeStruct<T>(int startIndex, T structure) where T : struct
			{
				int num12 = Marshal.SizeOf(typeof(T));
				IntPtr intPtr = Marshal.AllocHGlobal(num12);
				Marshal.StructureToPtr(structure, intPtr, fDeleteOld: true);
				Marshal.Copy(intPtr, buf0, startIndex, num12);
				Marshal.FreeHGlobal(intPtr);
			}
			void writeStructs<T>(int num14, T[] array5) where T : struct
			{
				if (array5 != null)
				{
					int num12 = Marshal.SizeOf(typeof(T));
					foreach (T val3 in array5)
					{
						writeStruct<T>(num14, val3);
						num14 += num12;
					}
				}
			}
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			list.AddRange(base.GetReferences());
			ShaderParameter[] parameters = Parameters;
			foreach (ShaderParameter shaderParameter in parameters)
			{
				if (shaderParameter.DataType == 0)
				{
					list.Add(shaderParameter.Data as TextureBase);
				}
			}
			return list.ToArray();
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			List<Tuple<long, IResourceBlock>> list = new List<Tuple<long, IResourceBlock>>();
			list.AddRange(base.GetParts());
			long num = Parameters.Length * 16;
			List<ResourceSystemStructBlock<Vector4>> list2 = new List<ResourceSystemStructBlock<Vector4>>();
			ShaderParameter[] parameters = Parameters;
			foreach (ShaderParameter shaderParameter in parameters)
			{
				if (shaderParameter.DataType != 0)
				{
					Vector4[] array = shaderParameter.Data as Vector4[];
					if (array == null)
					{
						array = new Vector4[1] { (Vector4)shaderParameter.Data };
					}
					ResourceSystemStructBlock<Vector4> resourceSystemStructBlock = new ResourceSystemStructBlock<Vector4>(array);
					list.Add(new Tuple<long, IResourceBlock>(num, resourceSystemStructBlock));
					list2.Add(resourceSystemStructBlock);
				}
				else
				{
					list2.Add(null);
				}
				num += 16 * shaderParameter.DataType;
			}
			ParameterDataBlocks = list2.ToArray();
			return list.ToArray();
		}
	}
}
