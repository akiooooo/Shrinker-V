using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysTypeChild : ResourceSystemBlock
	{
		public uint Unknown_04h = 1u;

		public uint Unknown_14h;

		public ulong Unknown_18h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public ulong Unknown_30h;

		public ulong Unknown_38h;

		public ulong Unknown_40h;

		public ulong Unknown_48h;

		public ulong Unknown_50h;

		public ulong Unknown_58h;

		public ulong Unknown_60h;

		public ulong Unknown_68h;

		public ulong Unknown_70h;

		public ulong Unknown_78h;

		public ulong Unknown_80h;

		public ulong Unknown_88h;

		public ulong Unknown_90h;

		public ulong Unknown_98h;

		public ulong Unknown_B8h;

		public ulong Unknown_C0h;

		public ulong Unknown_C8h;

		public ulong Unknown_D0h;

		public ulong Unknown_D8h;

		public ulong Unknown_E0h;

		public ulong Unknown_E8h;

		public ulong Unknown_F0h;

		public ulong Unknown_F8h;

		[CompilerGenerated]
		private float _003CUnkFloatFromParent_003Ek__BackingField;

		[CompilerGenerated]
		private Vector4 _003CUnkVecFromParent_003Ek__BackingField;

		[CompilerGenerated]
		private Vector4 _003CInertiaTensorFromParent_003Ek__BackingField;

		[CompilerGenerated]
		private FragPhysicsLOD _003COwnerFragPhysLod_003Ek__BackingField;

		[CompilerGenerated]
		private int _003COwnerFragPhysIndex_003Ek__BackingField;

		public override long BlockLength => 256L;

		public uint VFT { get; set; } = 1080061712u;

		public float PristineMass { get; set; }

		public float DamagedMass { get; set; }

		public ushort GroupIndex { get; set; }

		public ushort BoneTag { get; set; }

		public ulong Drawable1Pointer { get; set; }

		public ulong Drawable2Pointer { get; set; }

		public ulong EvtSetPointer { get; set; }

		public FragDrawable Drawable1 { get; set; }

		public FragDrawable Drawable2 { get; set; }

		public FragPhysEvtSet EvtSet { get; set; }

		public float UnkFloatFromParent
		{
			[CompilerGenerated]
			set
			{
				_003CUnkFloatFromParent_003Ek__BackingField = value;
			}
		}

		public Vector4 UnkVecFromParent
		{
			[CompilerGenerated]
			set
			{
				_003CUnkVecFromParent_003Ek__BackingField = value;
			}
		}

		public Vector4 InertiaTensorFromParent
		{
			[CompilerGenerated]
			set
			{
				_003CInertiaTensorFromParent_003Ek__BackingField = value;
			}
		}

		public FragPhysicsLOD OwnerFragPhysLod
		{
			[CompilerGenerated]
			set
			{
				_003COwnerFragPhysLod_003Ek__BackingField = value;
			}
		}

		public int OwnerFragPhysIndex
		{
			[CompilerGenerated]
			set
			{
				_003COwnerFragPhysIndex_003Ek__BackingField = value;
			}
		}

		public FragPhysTypeGroup Group { get; set; }

		public string GroupName => Group?.ToString();

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			PristineMass = reader.ReadSingle();
			DamagedMass = reader.ReadSingle();
			GroupIndex = reader.ReadUInt16();
			BoneTag = reader.ReadUInt16();
			Unknown_14h = reader.ReadUInt32();
			Unknown_18h = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			Unknown_30h = reader.ReadUInt64();
			Unknown_38h = reader.ReadUInt64();
			Unknown_40h = reader.ReadUInt64();
			Unknown_48h = reader.ReadUInt64();
			Unknown_50h = reader.ReadUInt64();
			Unknown_58h = reader.ReadUInt64();
			Unknown_60h = reader.ReadUInt64();
			Unknown_68h = reader.ReadUInt64();
			Unknown_70h = reader.ReadUInt64();
			Unknown_78h = reader.ReadUInt64();
			Unknown_80h = reader.ReadUInt64();
			Unknown_88h = reader.ReadUInt64();
			Unknown_90h = reader.ReadUInt64();
			Unknown_98h = reader.ReadUInt64();
			Drawable1Pointer = reader.ReadUInt64();
			Drawable2Pointer = reader.ReadUInt64();
			EvtSetPointer = reader.ReadUInt64();
			Unknown_B8h = reader.ReadUInt64();
			Unknown_C0h = reader.ReadUInt64();
			Unknown_C8h = reader.ReadUInt64();
			Unknown_D0h = reader.ReadUInt64();
			Unknown_D8h = reader.ReadUInt64();
			Unknown_E0h = reader.ReadUInt64();
			Unknown_E8h = reader.ReadUInt64();
			Unknown_F0h = reader.ReadUInt64();
			Unknown_F8h = reader.ReadUInt64();
			Drawable1 = reader.ReadBlockAt<FragDrawable>(Drawable1Pointer, Array.Empty<object>());
			Drawable2 = reader.ReadBlockAt<FragDrawable>(Drawable2Pointer, Array.Empty<object>());
			EvtSet = reader.ReadBlockAt<FragPhysEvtSet>(EvtSetPointer, Array.Empty<object>());
			if (Drawable1 != null)
			{
				Drawable1.OwnerFragmentPhys = this;
			}
			if (Drawable2 != null)
			{
				Drawable2.OwnerFragmentPhys = this;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			Drawable1Pointer = (ulong)((Drawable1 != null) ? Drawable1.FilePosition : 0);
			Drawable2Pointer = (ulong)((Drawable2 != null) ? Drawable2.FilePosition : 0);
			EvtSetPointer = (ulong)((EvtSet != null) ? EvtSet.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(PristineMass);
			writer.Write(DamagedMass);
			writer.Write(GroupIndex);
			writer.Write(BoneTag);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
			writer.Write(Unknown_30h);
			writer.Write(Unknown_38h);
			writer.Write(Unknown_40h);
			writer.Write(Unknown_48h);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_58h);
			writer.Write(Unknown_60h);
			writer.Write(Unknown_68h);
			writer.Write(Unknown_70h);
			writer.Write(Unknown_78h);
			writer.Write(Unknown_80h);
			writer.Write(Unknown_88h);
			writer.Write(Unknown_90h);
			writer.Write(Unknown_98h);
			writer.Write(Drawable1Pointer);
			writer.Write(Drawable2Pointer);
			writer.Write(EvtSetPointer);
			writer.Write(Unknown_B8h);
			writer.Write(Unknown_C0h);
			writer.Write(Unknown_C8h);
			writer.Write(Unknown_D0h);
			writer.Write(Unknown_D8h);
			writer.Write(Unknown_E0h);
			writer.Write(Unknown_E8h);
			writer.Write(Unknown_F0h);
			writer.Write(Unknown_F8h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Drawable1 != null)
			{
				list.Add(Drawable1);
			}
			if (Drawable2 != null)
			{
				list.Add(Drawable2);
			}
			if (EvtSet != null)
			{
				list.Add(EvtSet);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return GroupName ?? "(FragPhysTypeChild: no group)";
		}
	}
}
