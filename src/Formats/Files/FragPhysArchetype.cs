using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysArchetype : ResourceSystemBlock
	{
		public uint Unknown_04h = 1u;

		public ulong Unknown_08h;

		public ulong Unknown_10h = 2uL;

		public uint Unknown_28h = 1u;

		public uint Unknown_2Ch = uint.MaxValue;

		public uint Unknown_30h = 65536u;

		public uint Unknown_34h;

		public ulong Unknown_38h;

		public ulong Unknown_58h;

		public uint Unknown_6Ch = 2143289345u;

		public uint Unknown_7Ch = 2143289345u;

		public uint Unknown_8Ch = 2139095041u;

		public uint Unknown_9Ch = 2139095041u;

		public uint Unknown_ACh = 2139095041u;

		public uint Unknown_BCh = 2139095041u;

		public uint Unknown_CCh = 2139095041u;

		public uint Unknown_DCh = 2139095041u;

		private string_r NameBlock;

		public override long BlockLength => 224L;

		public uint VFT { get; set; } = 1080215944u;

		public ulong NamePointer { get; set; }

		public ulong BoundPointer { get; set; }

		public float Mass { get; set; }

		public float MassInv { get; set; }

		public float Unknown_48h { get; set; } = 1f;

		public float Unknown_4Ch { get; set; } = 150f;

		public float Unknown_50h { get; set; } = (float)Math.PI * 2f;

		public float Unknown_54h { get; set; } = 1f;

		public Vector3 InertiaTensor { get; set; }

		public Vector3 InertiaTensorInv { get; set; }

		public Vector3 Unknown_80h { get; set; } = Vector3.Zero;

		public Vector3 Unknown_90h { get; set; } = Vector3.Zero;

		public Vector3 Unknown_A0h { get; set; } = Vector3.Zero;

		public Vector3 Unknown_B0h { get; set; } = Vector3.Zero;

		public Vector3 Unknown_C0h { get; set; } = Vector3.Zero;

		public Vector3 Unknown_D0h { get; set; } = Vector3.Zero;

		public string Name { get; set; }

		public Bounds Bound { get; set; }

		public FragPhysicsLOD Owner { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			Unknown_08h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt64();
			NamePointer = reader.ReadUInt64();
			BoundPointer = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt32();
			Unknown_2Ch = reader.ReadUInt32();
			Unknown_30h = reader.ReadUInt32();
			Unknown_34h = reader.ReadUInt32();
			Unknown_38h = reader.ReadUInt64();
			Mass = reader.ReadSingle();
			MassInv = reader.ReadSingle();
			Unknown_48h = reader.ReadSingle();
			Unknown_4Ch = reader.ReadSingle();
			Unknown_50h = reader.ReadSingle();
			Unknown_54h = reader.ReadSingle();
			Unknown_58h = reader.ReadUInt64();
			InertiaTensor = reader.ReadVector3();
			Unknown_6Ch = reader.ReadUInt32();
			InertiaTensorInv = reader.ReadVector3();
			Unknown_7Ch = reader.ReadUInt32();
			Unknown_80h = reader.ReadVector3();
			Unknown_8Ch = reader.ReadUInt32();
			Unknown_90h = reader.ReadVector3();
			Unknown_9Ch = reader.ReadUInt32();
			Unknown_A0h = reader.ReadVector3();
			Unknown_ACh = reader.ReadUInt32();
			Unknown_B0h = reader.ReadVector3();
			Unknown_BCh = reader.ReadUInt32();
			Unknown_C0h = reader.ReadVector3();
			Unknown_CCh = reader.ReadUInt32();
			Unknown_D0h = reader.ReadVector3();
			Unknown_DCh = reader.ReadUInt32();
			Name = reader.ReadStringAt(NamePointer);
			Bound = reader.ReadBlockAt<Bounds>(BoundPointer, Array.Empty<object>());
			if (Bound != null)
			{
				Bound.Owner = this;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			NamePointer = (ulong)((NameBlock != null) ? NameBlock.FilePosition : 0);
			BoundPointer = (ulong)((Bound != null) ? Bound.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.Write(Unknown_10h);
			writer.Write(NamePointer);
			writer.Write(BoundPointer);
			writer.Write(Unknown_28h);
			writer.Write(Unknown_2Ch);
			writer.Write(Unknown_30h);
			writer.Write(Unknown_34h);
			writer.Write(Unknown_38h);
			writer.Write(Mass);
			writer.Write(MassInv);
			writer.Write(Unknown_48h);
			writer.Write(Unknown_4Ch);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_54h);
			writer.Write(Unknown_58h);
			writer.Write(InertiaTensor);
			writer.Write(Unknown_6Ch);
			writer.Write(InertiaTensorInv);
			writer.Write(Unknown_7Ch);
			writer.Write(Unknown_80h);
			writer.Write(Unknown_8Ch);
			writer.Write(Unknown_90h);
			writer.Write(Unknown_9Ch);
			writer.Write(Unknown_A0h);
			writer.Write(Unknown_ACh);
			writer.Write(Unknown_B0h);
			writer.Write(Unknown_BCh);
			writer.Write(Unknown_C0h);
			writer.Write(Unknown_CCh);
			writer.Write(Unknown_D0h);
			writer.Write(Unknown_DCh);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Name != null)
			{
				NameBlock = (string_r)Name;
				list.Add(NameBlock);
			}
			if (Bound != null)
			{
				list.Add(Bound);
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return Name ?? "(FragPhysArchetype: no name)";
		}
	}
}
