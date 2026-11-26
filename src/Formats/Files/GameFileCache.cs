using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GameFormats.Files
{
	public class GameFileCache
	{
		public class ShaderGen9XmlDataCollection
		{
			public MetaHash Name;

			public int[] BufferSizes;

			public byte[] SamplerValues;

			public ShaderParamInfoG9[] ParamInfos;

			public Dictionary<MetaHash, MetaHash> ParamsMapGen9ToLegacy;

			public Dictionary<MetaHash, MetaHash> ParamsMapLegacyToGen9;
		}

		public static Dictionary<MetaHash, ShaderGen9XmlDataCollection> ShadersGen9ConversionData;

		public static void EnsureShadersGen9ConversionData()
		{
			if (ShadersGen9ConversionData != null)
			{
				return;
			}
			string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ShadersGen9Conversion.xml");
			if (!File.Exists(path))
			{
				throw new Exception("Unable to load ShadersGen9Conversion.xml");
			}
			string xml = File.ReadAllText(path);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			XmlNodeList xmlNodeList = xmlDocument.SelectNodes("ShadersGen9Conversion/Item");
			Dictionary<MetaHash, ShaderGen9XmlDataCollection> dictionary = new Dictionary<MetaHash, ShaderGen9XmlDataCollection>();
			List<ShaderParamInfoG9> list = new List<ShaderParamInfoG9>();
			Dictionary<byte, byte> dictionary2 = new Dictionary<byte, byte>();
			foreach (XmlNode item2 in xmlNodeList)
			{
				list.Clear();
				dictionary2.Clear();
				string text = Xml.GetChildInnerText(item2, "Name")?.ToLowerInvariant();
				MetaHash metaHash = new MetaHash(JenkHash.GenHash(text));
				ShaderGen9XmlDataCollection shaderGen9XmlDataCollection = new ShaderGen9XmlDataCollection();
				shaderGen9XmlDataCollection.Name = metaHash;
				shaderGen9XmlDataCollection.BufferSizes = Xml.GetChildRawIntArray(item2, "BufferSizes");
				shaderGen9XmlDataCollection.ParamsMapLegacyToGen9 = new Dictionary<MetaHash, MetaHash>();
				shaderGen9XmlDataCollection.ParamsMapGen9ToLegacy = new Dictionary<MetaHash, MetaHash>();
				foreach (XmlNode item3 in item2.SelectNodes("Parameters/Item"))
				{
					string stringAttribute = Xml.GetStringAttribute(item3, "type");
					string text2 = Xml.GetStringAttribute(item3, "name")?.ToLowerInvariant();
					string text3 = Xml.GetStringAttribute(item3, "old")?.ToLowerInvariant();
					uint num = JenkHash.GenHash(text2);
					uint num2 = JenkHash.GenHash(text3);
					if (num != 0)
					{
						if (text2.StartsWith("hash_"))
						{
							num = (MetaHash)Convert.ToUInt32(text2.Substring(5), 16);
						}
						else
						{
							JenkIndex.Ensure(text2);
						}
					}
					Enum.TryParse<ShaderParamTypeG9>(stringAttribute, out var result);
					ShaderParamInfoG9 item = new ShaderParamInfoG9
					{
						Name = num,
						Type = result
					};
					switch (result)
					{
					case ShaderParamTypeG9.Texture:
						item.TextureIndex = (byte)Xml.GetIntAttribute(item3, "index");
						break;
					case ShaderParamTypeG9.Unknown:
						item.SamplerIndex = (byte)Xml.GetIntAttribute(item3, "index");
						break;
					case ShaderParamTypeG9.Sampler:
						item.SamplerIndex = (byte)Xml.GetIntAttribute(item3, "index");
						dictionary2[item.SamplerIndex] = (byte)Xml.GetIntAttribute(item3, "sampler");
						break;
					case ShaderParamTypeG9.CBuffer:
						item.CBufferIndex = (byte)Xml.GetIntAttribute(item3, "buffer");
						item.ParamLength = (ushort)Xml.GetUIntAttribute(item3, "length");
						item.ParamOffset = (ushort)Xml.GetUIntAttribute(item3, "offset");
						break;
					}
					list.Add(item);
					if (num != 0 && num2 != 0)
					{
						shaderGen9XmlDataCollection.ParamsMapLegacyToGen9[num2] = num;
						shaderGen9XmlDataCollection.ParamsMapGen9ToLegacy[num] = num2;
					}
				}
				shaderGen9XmlDataCollection.ParamInfos = list.ToArray();
				int num3 = 0;
				foreach (KeyValuePair<byte, byte> item4 in dictionary2)
				{
					if (item4.Key >= num3)
					{
						num3 = item4.Key + 1;
					}
				}
				byte[] array = new byte[num3];
				foreach (KeyValuePair<byte, byte> item5 in dictionary2)
				{
					array[item5.Key] = item5.Value;
				}
				shaderGen9XmlDataCollection.SamplerValues = array;
				dictionary[metaHash] = shaderGen9XmlDataCollection;
			}
			ShadersGen9ConversionData = dictionary;
		}
	}
}
