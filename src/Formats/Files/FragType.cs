using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragType : ResourceFileBase
	{
		public ulong Unknown_10h;

		public ulong Unknown_18h;

		public ulong Unknown_50h;

		public ulong Unknown_70h;

		public ulong Unknown_78h;

		public ulong Unknown_80h;

		public ulong Unknown_88h;

		public ulong Unknown_90h;

		public ulong Unknown_98h;

		public ulong Unknown_A0h;

		public int Unknown_B4h;

		public int Unknown_C8h = -1;

		public byte Unknown_D8h;

		public ushort Unknown_DAh;

		public uint Unknown_DCh;

		public ulong Unknown_E8h;

		public ulong Unknown_100h;

		public ulong Unknown_108h;

		public ulong Unknown_128h;

		private string_r NameBlock;

		[CompilerGenerated]
		private YftFile _003CYft_003Ek__BackingField;

		public override long BlockLength => 304L;

		public Vector3 BoundingSphereCenter { get; set; }

		public float BoundingSphereRadius { get; set; }

		public ulong DrawablePointer { get; set; }

		public ulong DrawableArrayPointer { get; set; }

		public ulong DrawableArrayNamesPointer { get; set; }

		public uint DrawableArrayCount { get; set; }

		public int DrawableArrayFlag { get; set; }

		public ulong NamePointer { get; set; }

		public ResourcePointerList64<EnvironmentCloth> Cloths { get; set; }

		public ulong BoneTransformsPointer { get; set; }

		public int Unknown_B0h { get; set; }

		public int Unknown_B8h { get; set; }

		public int Unknown_BCh { get; set; }

		public int Unknown_C0h { get; set; }

		public int Unknown_C4h { get; set; }

		public float Unknown_CCh { get; set; }

		public float GravityFactor { get; set; }

		public float BuoyancyFactor { get; set; }

		public byte GlassWindowsCount { get; set; }

		public ulong GlassWindowsPointer { get; set; }

		public ulong PhysicsLODGroupPointer { get; set; }

		public ulong DrawableClothPointer { get; set; }

		public ResourceSimpleList64<LightAttributes> LightAttributes { get; set; }

		public ulong VehicleGlassWindowsPointer { get; set; }

		public FragDrawable Drawable { get; set; }

		public ResourcePointerArray64<FragDrawable> DrawableArray { get; set; }

		public ResourcePointerArray64<string_r> DrawableArrayNames { get; set; }

		public string Name { get; set; }

		public FragBoneTransforms BoneTransforms { get; set; }

		public ResourcePointerArray64<FragGlassWindow> GlassWindows { get; set; }

		public FragPhysicsLODGroup PhysicsLODGroup { get; set; }

		public FragDrawable DrawableCloth { get; set; }

		public FragVehicleGlassWindows VehicleGlassWindows { get; set; }

		public YftFile Yft
		{
			[CompilerGenerated]
			set
			{
				_003CYft_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_10h = reader.ReadUInt64();
			Unknown_18h = reader.ReadUInt64();
			BoundingSphereCenter = reader.ReadVector3();
			BoundingSphereRadius = reader.ReadSingle();
			DrawablePointer = reader.ReadUInt64();
			DrawableArrayPointer = reader.ReadUInt64();
			DrawableArrayNamesPointer = reader.ReadUInt64();
			DrawableArrayCount = reader.ReadUInt32();
			DrawableArrayFlag = reader.ReadInt32();
			Unknown_50h = reader.ReadUInt64();
			NamePointer = reader.ReadUInt64();
			Cloths = reader.ReadBlock<ResourcePointerList64<EnvironmentCloth>>(Array.Empty<object>());
			Unknown_70h = reader.ReadUInt64();
			Unknown_78h = reader.ReadUInt64();
			Unknown_80h = reader.ReadUInt64();
			Unknown_88h = reader.ReadUInt64();
			Unknown_90h = reader.ReadUInt64();
			Unknown_98h = reader.ReadUInt64();
			Unknown_A0h = reader.ReadUInt64();
			BoneTransformsPointer = reader.ReadUInt64();
			Unknown_B0h = reader.ReadInt32();
			Unknown_B4h = reader.ReadInt32();
			Unknown_B8h = reader.ReadInt32();
			Unknown_BCh = reader.ReadInt32();
			Unknown_C0h = reader.ReadInt32();
			Unknown_C4h = reader.ReadInt32();
			Unknown_C8h = reader.ReadInt32();
			Unknown_CCh = reader.ReadSingle();
			GravityFactor = reader.ReadSingle();
			BuoyancyFactor = reader.ReadSingle();
			Unknown_D8h = reader.ReadByte();
			GlassWindowsCount = reader.ReadByte();
			Unknown_DAh = reader.ReadUInt16();
			Unknown_DCh = reader.ReadUInt32();
			GlassWindowsPointer = reader.ReadUInt64();
			Unknown_E8h = reader.ReadUInt64();
			PhysicsLODGroupPointer = reader.ReadUInt64();
			DrawableClothPointer = reader.ReadUInt64();
			Unknown_100h = reader.ReadUInt64();
			Unknown_108h = reader.ReadUInt64();
			LightAttributes = reader.ReadBlock<ResourceSimpleList64<LightAttributes>>(Array.Empty<object>());
			VehicleGlassWindowsPointer = reader.ReadUInt64();
			Unknown_128h = reader.ReadUInt64();
			Drawable = reader.ReadBlockAt<FragDrawable>(DrawablePointer, Array.Empty<object>());
			DrawableArray = reader.ReadBlockAt<ResourcePointerArray64<FragDrawable>>(DrawableArrayPointer, new object[1] { DrawableArrayCount });
			DrawableArrayNames = reader.ReadBlockAt<ResourcePointerArray64<string_r>>(DrawableArrayNamesPointer, new object[1] { DrawableArrayCount });
			Name = reader.ReadStringAt(NamePointer);
			BoneTransforms = reader.ReadBlockAt<FragBoneTransforms>(BoneTransformsPointer, Array.Empty<object>());
			GlassWindows = reader.ReadBlockAt<ResourcePointerArray64<FragGlassWindow>>(GlassWindowsPointer, new object[1] { GlassWindowsCount });
			PhysicsLODGroup = reader.ReadBlockAt<FragPhysicsLODGroup>(PhysicsLODGroupPointer, Array.Empty<object>());
			DrawableCloth = reader.ReadBlockAt<FragDrawable>(DrawableClothPointer, Array.Empty<object>());
			VehicleGlassWindows = reader.ReadBlockAt<FragVehicleGlassWindows>(VehicleGlassWindowsPointer, Array.Empty<object>());
			if (Drawable != null)
			{
				Drawable.OwnerFragment = this;
			}
			if (DrawableCloth != null)
			{
				DrawableCloth.OwnerFragment = this;
				_ = DrawableCloth.OwnerCloth;
			}
			if (DrawableArray?.data_items != null)
			{
				for (int i = 0; i < DrawableArray.data_items.Length; i++)
				{
					FragDrawable fragDrawable = DrawableArray.data_items[i];
					if (fragDrawable != null)
					{
						fragDrawable.OwnerFragment = this;
					}
				}
			}
			_ = GlassWindows;
			_ = VehicleGlassWindows;
			AssignChildrenShaders();
			AssignGlassWindowsGroups();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			DrawablePointer = (ulong)((Drawable != null) ? Drawable.FilePosition : 0);
			DrawableArrayPointer = (ulong)((DrawableArray != null) ? DrawableArray.FilePosition : 0);
			DrawableArrayNamesPointer = (ulong)((DrawableArrayNames != null) ? DrawableArrayNames.FilePosition : 0);
			DrawableArrayCount = ((DrawableArray != null) ? ((uint)DrawableArray.Count) : 0u);
			DrawableArrayFlag = (((DrawableArray?.data_items?.Length).GetValueOrDefault() <= 0) ? (-1) : 0);
			NamePointer = (ulong)((NameBlock != null) ? NameBlock.FilePosition : 0);
			BoneTransformsPointer = (ulong)((BoneTransforms != null) ? BoneTransforms.FilePosition : 0);
			GlassWindowsCount = (byte)((GlassWindows != null) ? ((uint)GlassWindows.Count) : 0u);
			GlassWindowsPointer = (ulong)((GlassWindows != null) ? GlassWindows.FilePosition : 0);
			PhysicsLODGroupPointer = (ulong)((PhysicsLODGroup != null) ? PhysicsLODGroup.FilePosition : 0);
			DrawableClothPointer = (ulong)((DrawableCloth != null) ? DrawableCloth.FilePosition : 0);
			VehicleGlassWindowsPointer = (ulong)((VehicleGlassWindows != null) ? VehicleGlassWindows.FilePosition : 0);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_18h);
			writer.Write(BoundingSphereCenter);
			writer.Write(BoundingSphereRadius);
			writer.Write(DrawablePointer);
			writer.Write(DrawableArrayPointer);
			writer.Write(DrawableArrayNamesPointer);
			writer.Write(DrawableArrayCount);
			writer.Write(DrawableArrayFlag);
			writer.Write(Unknown_50h);
			writer.Write(NamePointer);
			writer.WriteBlock(Cloths);
			writer.Write(Unknown_70h);
			writer.Write(Unknown_78h);
			writer.Write(Unknown_80h);
			writer.Write(Unknown_88h);
			writer.Write(Unknown_90h);
			writer.Write(Unknown_98h);
			writer.Write(Unknown_A0h);
			writer.Write(BoneTransformsPointer);
			writer.Write(Unknown_B0h);
			writer.Write(Unknown_B4h);
			writer.Write(Unknown_B8h);
			writer.Write(Unknown_BCh);
			writer.Write(Unknown_C0h);
			writer.Write(Unknown_C4h);
			writer.Write(Unknown_C8h);
			writer.Write(Unknown_CCh);
			writer.Write(GravityFactor);
			writer.Write(BuoyancyFactor);
			writer.Write(Unknown_D8h);
			writer.Write(GlassWindowsCount);
			writer.Write(Unknown_DAh);
			writer.Write(Unknown_DCh);
			writer.Write(GlassWindowsPointer);
			writer.Write(Unknown_E8h);
			writer.Write(PhysicsLODGroupPointer);
			writer.Write(DrawableClothPointer);
			writer.Write(Unknown_100h);
			writer.Write(Unknown_108h);
			writer.WriteBlock(LightAttributes);
			writer.Write(VehicleGlassWindowsPointer);
			writer.Write(Unknown_128h);
		}

		public void AssignChildrenShaders()
		{
			if (PhysicsLODGroup == null)
			{
				return;
			}
			FragDrawable pdrwbl = Drawable ?? DrawableCloth;
			_ = pdrwbl?.Skeleton;
			assign(PhysicsLODGroup.PhysicsLOD1);
			assign(PhysicsLODGroup.PhysicsLOD2);
			assign(PhysicsLODGroup.PhysicsLOD3);
			if (DrawableArray?.data_items != null)
			{
				FragDrawable[] data_items = DrawableArray.data_items;
				for (int i = 0; i < data_items.Length; i++)
				{
					assigndr(data_items[i], null, 0);
				}
			}
			void assign(FragPhysicsLOD lod)
			{
				FragPhysTypeChild[] array = lod?.Children?.data_items;
				BoundComposite pbcmp = (lod?.Archetype1?.Bound ?? pdrwbl?.Bound) as BoundComposite;
				BoundComposite pbcmp2 = (lod?.Archetype2?.Bound ?? pdrwbl?.Bound) as BoundComposite;
				if (array != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						FragPhysTypeChild obj = array[j];
						assigndr(obj?.Drawable1, pbcmp, j);
						assigndr(obj?.Drawable2, pbcmp2, j);
					}
				}
			}
			void assigndr(FragDrawable dr, BoundComposite pbcmp, int num)
			{
				if (dr != null && pdrwbl != null)
				{
					dr.OwnerDrawable = pdrwbl;
					dr.AssignGeometryShaders(pdrwbl.ShaderGroup);
				}
			}
		}

		public void AssignGlassWindowsGroups()
		{
			assign(PhysicsLODGroup?.PhysicsLOD1);
			assign(PhysicsLODGroup?.PhysicsLOD2);
			assign(PhysicsLODGroup?.PhysicsLOD3);
			void assign(FragPhysicsLOD lod)
			{
				if (lod?.Groups?.data_items != null)
				{
					FragPhysTypeGroup[] data_items = lod.Groups.data_items;
					foreach (FragPhysTypeGroup fragPhysTypeGroup in data_items)
					{
						byte glassWindowIndex = fragPhysTypeGroup.GlassWindowIndex;
						if ((fragPhysTypeGroup.GlassFlags & 2) > 0 && GlassWindows?.data_items != null && glassWindowIndex < GlassWindows.data_items.Length)
						{
							FragGlassWindow obj = GlassWindows.data_items[glassWindowIndex];
							obj.Group = fragPhysTypeGroup;
							obj.GroupLOD = lod;
						}
					}
				}
			}
		}

		public void EnsureGen9()
		{
			Drawable?.EnsureGen9();
			DrawableCloth?.EnsureGen9();
			if (DrawableArray?.data_items != null)
			{
				FragDrawable[] data_items = DrawableArray.data_items;
				for (int i = 0; i < data_items.Length; i++)
				{
					data_items[i]?.EnsureGen9();
				}
			}
			ensure(PhysicsLODGroup?.PhysicsLOD1);
			ensure(PhysicsLODGroup?.PhysicsLOD2);
			ensure(PhysicsLODGroup?.PhysicsLOD3);
			void ensure(FragPhysicsLOD lod)
			{
				FragPhysTypeChild[] array = lod?.Children?.data_items;
				if (array != null)
				{
					foreach (FragPhysTypeChild obj in array)
					{
						obj?.Drawable1?.EnsureGen9();
						obj?.Drawable2?.EnsureGen9();
					}
				}
			}
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (Drawable != null)
			{
				list.Add(Drawable);
			}
			if (DrawableArray != null)
			{
				list.Add(DrawableArray);
			}
			if (DrawableArrayNames != null)
			{
				list.Add(DrawableArrayNames);
			}
			if (Name != null)
			{
				NameBlock = (string_r)Name;
				list.Add(NameBlock);
			}
			if (BoneTransforms != null)
			{
				list.Add(BoneTransforms);
			}
			if (GlassWindows != null)
			{
				list.Add(GlassWindows);
			}
			if (PhysicsLODGroup != null)
			{
				list.Add(PhysicsLODGroup);
			}
			if (DrawableCloth != null)
			{
				list.Add(DrawableCloth);
			}
			if (VehicleGlassWindows != null)
			{
				list.Add(VehicleGlassWindows);
			}
			return list.ToArray();
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[2]
			{
				new Tuple<long, IResourceBlock>(96L, Cloths),
				new Tuple<long, IResourceBlock>(272L, LightAttributes)
			};
		}
	}
}
