using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DrawableBase : ResourceFileBase
	{
		[CompilerGenerated]
		private Dictionary<ulong, VertexDeclaration> _003CVertexDecls_003Ek__BackingField;

		[CompilerGenerated]
		private object _003COwner_003Ek__BackingField;

		public override long BlockLength => 168L;

		public ulong ShaderGroupPointer { get; set; }

		public ulong SkeletonPointer { get; set; }

		public Vector3 BoundingCenter { get; set; }

		public float BoundingSphereRadius { get; set; }

		public Vector3 BoundingBoxMin { get; set; }

		public uint Unknown_3Ch { get; set; } = 2139095041u;

		public Vector3 BoundingBoxMax { get; set; }

		public uint Unknown_4Ch { get; set; } = 2139095041u;

		public ulong DrawableModelsHighPointer { get; set; }

		public ulong DrawableModelsMediumPointer { get; set; }

		public ulong DrawableModelsLowPointer { get; set; }

		public ulong DrawableModelsVeryLowPointer { get; set; }

		public float LodDistHigh { get; set; }

		public float LodDistMed { get; set; }

		public float LodDistLow { get; set; }

		public float LodDistVlow { get; set; }

		public uint RenderMaskFlagsHigh { get; set; }

		public uint RenderMaskFlagsMed { get; set; }

		public uint RenderMaskFlagsLow { get; set; }

		public uint RenderMaskFlagsVlow { get; set; }

		public ulong JointsPointer { get; set; }

		public ushort Unknown_98h { get; set; }

		public ushort DrawableModelsBlocksSize { get; set; }

		public uint Unknown_9Ch { get; set; }

		public ulong DrawableModelsPointer { get; set; }

		public ShaderGroup ShaderGroup { get; set; }

		public Skeleton Skeleton { get; set; }

		public Joints Joints { get; set; }

		public DrawableModelsBlock DrawableModels { get; set; }

		public DrawableModel[] AllModels { get; set; }

		public Dictionary<ulong, VertexDeclaration> VertexDecls
		{
			[CompilerGenerated]
			set
			{
				_003CVertexDecls_003Ek__BackingField = value;
			}
		}

		public object Owner
		{
			[CompilerGenerated]
			set
			{
				_003COwner_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			ShaderGroupPointer = reader.ReadUInt64();
			SkeletonPointer = reader.ReadUInt64();
			BoundingCenter = reader.ReadVector3();
			BoundingSphereRadius = reader.ReadSingle();
			BoundingBoxMin = reader.ReadVector3();
			Unknown_3Ch = reader.ReadUInt32();
			BoundingBoxMax = reader.ReadVector3();
			Unknown_4Ch = reader.ReadUInt32();
			DrawableModelsHighPointer = reader.ReadUInt64();
			DrawableModelsMediumPointer = reader.ReadUInt64();
			DrawableModelsLowPointer = reader.ReadUInt64();
			DrawableModelsVeryLowPointer = reader.ReadUInt64();
			LodDistHigh = reader.ReadSingle();
			LodDistMed = reader.ReadSingle();
			LodDistLow = reader.ReadSingle();
			LodDistVlow = reader.ReadSingle();
			RenderMaskFlagsHigh = reader.ReadUInt32();
			RenderMaskFlagsMed = reader.ReadUInt32();
			RenderMaskFlagsLow = reader.ReadUInt32();
			RenderMaskFlagsVlow = reader.ReadUInt32();
			JointsPointer = reader.ReadUInt64();
			Unknown_98h = reader.ReadUInt16();
			DrawableModelsBlocksSize = reader.ReadUInt16();
			Unknown_9Ch = reader.ReadUInt32();
			DrawableModelsPointer = reader.ReadUInt64();
			ShaderGroup = reader.ReadBlockAt<ShaderGroup>(ShaderGroupPointer, Array.Empty<object>());
			Skeleton = reader.ReadBlockAt<Skeleton>(SkeletonPointer, Array.Empty<object>());
			Joints = reader.ReadBlockAt<Joints>(JointsPointer, Array.Empty<object>());
			DrawableModels = reader.ReadBlockAt<DrawableModelsBlock>((DrawableModelsPointer == 0L) ? DrawableModelsHighPointer : DrawableModelsPointer, new object[1] { this });
			BuildAllModels();
			BuildVertexDecls();
			AssignGeometryShaders(ShaderGroup);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			ShaderGroupPointer = (ulong)((ShaderGroup != null) ? ShaderGroup.FilePosition : 0);
			SkeletonPointer = (ulong)((Skeleton != null) ? Skeleton.FilePosition : 0);
			DrawableModelsHighPointer = (ulong)(DrawableModels?.GetHighPointer() ?? 0);
			DrawableModelsMediumPointer = (ulong)(DrawableModels?.GetMedPointer() ?? 0);
			DrawableModelsLowPointer = (ulong)(DrawableModels?.GetLowPointer() ?? 0);
			DrawableModelsVeryLowPointer = (ulong)(DrawableModels?.GetVLowPointer() ?? 0);
			JointsPointer = (ulong)((Joints != null) ? Joints.FilePosition : 0);
			DrawableModelsPointer = (ulong)(DrawableModels?.FilePosition ?? 0);
			DrawableModelsBlocksSize = (ushort)Math.Ceiling((double)(DrawableModels?.BlockLength ?? 0) / 16.0);
			writer.Write(ShaderGroupPointer);
			writer.Write(SkeletonPointer);
			writer.Write(BoundingCenter);
			writer.Write(BoundingSphereRadius);
			writer.Write(BoundingBoxMin);
			writer.Write(Unknown_3Ch);
			writer.Write(BoundingBoxMax);
			writer.Write(Unknown_4Ch);
			writer.Write(DrawableModelsHighPointer);
			writer.Write(DrawableModelsMediumPointer);
			writer.Write(DrawableModelsLowPointer);
			writer.Write(DrawableModelsVeryLowPointer);
			writer.Write(LodDistHigh);
			writer.Write(LodDistMed);
			writer.Write(LodDistLow);
			writer.Write(LodDistVlow);
			writer.Write(RenderMaskFlagsHigh);
			writer.Write(RenderMaskFlagsMed);
			writer.Write(RenderMaskFlagsLow);
			writer.Write(RenderMaskFlagsVlow);
			writer.Write(JointsPointer);
			writer.Write(Unknown_98h);
			writer.Write(DrawableModelsBlocksSize);
			writer.Write(Unknown_9Ch);
			writer.Write(DrawableModelsPointer);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (ShaderGroup != null)
			{
				list.Add(ShaderGroup);
			}
			if (Skeleton != null)
			{
				list.Add(Skeleton);
			}
			if (Joints != null)
			{
				list.Add(Joints);
			}
			if (DrawableModels != null)
			{
				list.Add(DrawableModels);
			}
			return list.ToArray();
		}

		public void AssignGeometryShaders(ShaderGroup shaderGrp)
		{
			if (shaderGrp?.Shaders?.data_items == null)
			{
				return;
			}
			ShaderFX[] data_items = shaderGrp.Shaders.data_items;
			DrawableModel[] allModels = AllModels;
			foreach (DrawableModel drawableModel in allModels)
			{
				if (drawableModel?.Geometries != null)
				{
					int num = drawableModel.Geometries.Length;
					for (int j = 0; j < num; j++)
					{
						DrawableGeometry obj = drawableModel.Geometries[j];
						ushort shaderID = obj.ShaderID;
						obj.Shader = ((shaderID < data_items.Length) ? data_items[shaderID] : null);
					}
				}
			}
		}

		public void BuildAllModels()
		{
			List<DrawableModel> list = new List<DrawableModel>();
			if (DrawableModels?.High != null)
			{
				list.AddRange(DrawableModels.High);
			}
			if (DrawableModels?.Med != null)
			{
				list.AddRange(DrawableModels.Med);
			}
			if (DrawableModels?.Low != null)
			{
				list.AddRange(DrawableModels.Low);
			}
			if (DrawableModels?.VLow != null)
			{
				list.AddRange(DrawableModels.VLow);
			}
			if (DrawableModels?.Extra != null)
			{
				list.AddRange(DrawableModels.Extra);
			}
			AllModels = list.ToArray();
		}

		public void BuildVertexDecls()
		{
			Dictionary<ulong, VertexDeclaration> dictionary = new Dictionary<ulong, VertexDeclaration>();
			DrawableModel[] allModels = AllModels;
			foreach (DrawableModel drawableModel in allModels)
			{
				if (drawableModel.Geometries == null)
				{
					continue;
				}
				DrawableGeometry[] geometries = drawableModel.Geometries;
				for (int j = 0; j < geometries.Length; j++)
				{
					VertexDeclaration info = geometries[j].VertexBuffer.Info;
					if (info != null)
					{
						ulong declarationId = info.GetDeclarationId();
						if (!dictionary.ContainsKey(declarationId))
						{
							dictionary.Add(declarationId, info);
						}
					}
				}
			}
			VertexDecls = new Dictionary<ulong, VertexDeclaration>(dictionary);
		}

		public void EnsureGen9()
		{
			base.FileVFT = 1079456120u;
			base.FileUnknown = 1u;
			Unknown_3Ch = 2139095041u;
			Unknown_4Ch = 2139095041u;
			if (Skeleton != null)
			{
				Skeleton.VFT = 1080114336u;
				Skeleton.Unknown_4h = 1u;
				if (Skeleton.Bones != null)
				{
					Skeleton.Bones.Unk0 = 0u;
					Skeleton.Bones.Unk1 = 0u;
					Skeleton.Bones.Unk2 = 0u;
				}
			}
			if (Joints != null)
			{
				Joints.VFT = 1080130656u;
				Joints.Unknown_4h = 1u;
			}
			if (ShaderGroup != null)
			{
				ShaderGroup.VFT = 1080113136u;
				ShaderGroup.Unknown_4h = 1u;
				ShaderGroup.TextureDictionary?.EnsureGen9();
				ShaderFX[] array = ShaderGroup.Shaders?.data_items;
				if (array != null)
				{
					ShaderFX[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i]?.EnsureGen9();
					}
				}
			}
			if (AllModels != null)
			{
				DrawableModel[] allModels = AllModels;
				foreach (DrawableModel drawableModel in allModels)
				{
					if (drawableModel == null)
					{
						continue;
					}
					drawableModel.VFT = 1080101528u;
					drawableModel.Unknown_4h = 1u;
					DrawableGeometry[] geometries = drawableModel.Geometries;
					if (geometries == null)
					{
						continue;
					}
					DrawableGeometry[] array3 = geometries;
					foreach (DrawableGeometry drawableGeometry in array3)
					{
						if (drawableGeometry != null)
						{
							drawableGeometry.VFT = 1080133528u;
							drawableGeometry.Unknown_4h = 1u;
							drawableGeometry.VertexBuffer?.EnsureGen9();
							drawableGeometry.IndexBuffer?.EnsureGen9();
						}
					}
				}
			}
			if (this is Drawable drawable && drawable.LightAttributes?.data_items != null)
			{
				LightAttributes[] data_items = drawable.LightAttributes.data_items;
				foreach (LightAttributes obj in data_items)
				{
					obj.Unknown_0h = 0u;
					obj.Unknown_4h = 0u;
				}
			}
		}
	}
}
