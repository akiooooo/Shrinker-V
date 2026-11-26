using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class VerletCloth : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public ulong Unknown_10h;

		public ulong Unknown_20h;

		public ulong Unknown_28h;

		public uint Unknown_54h = 1u;

		public ulong Unknown_58h;

		public ulong Unknown_60h;

		public ulong Unknown_68h;

		public ulong Unknown_90h;

		public ulong Unknown_98h;

		public ulong Unknown_A0h;

		public ulong Unknown_B0h;

		public ulong Unknown_B8h;

		public ulong Unknown_C0h;

		public ulong Unknown_C8h;

		public ulong Unknown_D0h;

		public ulong Unknown_D8h;

		public ulong Unknown_E0h;

		public uint Unknown_F4h;

		public ushort Unknown_F8h = 3;

		public uint Unknown_FCh;

		public ulong Unknown_120h;

		public ulong Unknown_128h;

		public ulong Unknown_138h = 1048576uL;

		public uint Unknown_14Ch;

		public ulong Unknown_150h;

		public uint Unknown_15Ch;

		public ulong Unknown_160h;

		public ulong Unknown_168h;

		public ulong Unknown_170h;

		public ulong Unknown_178h;

		public override long BlockLength => 384L;

		public uint VFT { get; set; }

		public ulong BoundPointer { get; set; }

		public Vector3 BBMin { get; set; }

		public uint Unknown_3Ch { get; set; }

		public Vector3 BBMax { get; set; }

		public uint Unknown_4Ch { get; set; }

		public float Unknown_50h { get; set; }

		public ResourceSimpleList64_s<Vector4> Vertices2 { get; set; }

		public ResourceSimpleList64_s<Vector4> Vertices { get; set; }

		public float Unknown_A8h { get; set; }

		public float Unknown_ACh { get; set; }

		public uint Unknown_E8h { get; set; }

		public uint ConstraintCount { get; set; }

		public uint VertexCount { get; set; }

		public ushort Unknown_FAh { get; set; }

		public ResourceSimpleList64_s<Unknown_C_004> Constraints2 { get; set; }

		public ResourceSimpleList64_s<Unknown_C_004> Constraints { get; set; }

		public ulong BehaviorPointer { get; set; }

		public ulong Unknown_140h_Pointer { get; set; }

		public ushort Unknown_148h { get; set; }

		public ushort VertexCount2 { get; set; }

		public float Unknown_158h { get; set; }

		public Bounds Bound { get; set; }

		public EnvClothVerletBehavior Behavior { get; set; }

		public Unknown_C_007 Unknown_140h_Data { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			Unknown_10h = reader.ReadUInt64();
			BoundPointer = reader.ReadUInt64();
			Unknown_20h = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			BBMin = reader.ReadVector3();
			Unknown_3Ch = reader.ReadUInt32();
			BBMax = reader.ReadVector3();
			Unknown_4Ch = reader.ReadUInt32();
			Unknown_50h = reader.ReadSingle();
			Unknown_54h = reader.ReadUInt32();
			Unknown_58h = reader.ReadUInt64();
			Unknown_60h = reader.ReadUInt64();
			Unknown_68h = reader.ReadUInt64();
			Vertices2 = reader.ReadBlock<ResourceSimpleList64_s<Vector4>>(Array.Empty<object>());
			Vertices = reader.ReadBlock<ResourceSimpleList64_s<Vector4>>(Array.Empty<object>());
			Unknown_90h = reader.ReadUInt64();
			Unknown_98h = reader.ReadUInt64();
			Unknown_A0h = reader.ReadUInt64();
			Unknown_A8h = reader.ReadSingle();
			Unknown_ACh = reader.ReadSingle();
			Unknown_B0h = reader.ReadUInt64();
			Unknown_B8h = reader.ReadUInt64();
			Unknown_C0h = reader.ReadUInt64();
			Unknown_C8h = reader.ReadUInt64();
			Unknown_D0h = reader.ReadUInt64();
			Unknown_D8h = reader.ReadUInt64();
			Unknown_E0h = reader.ReadUInt64();
			Unknown_E8h = reader.ReadUInt32();
			ConstraintCount = reader.ReadUInt32();
			VertexCount = reader.ReadUInt32();
			Unknown_F4h = reader.ReadUInt32();
			Unknown_F8h = reader.ReadUInt16();
			Unknown_FAh = reader.ReadUInt16();
			Unknown_FCh = reader.ReadUInt32();
			Constraints2 = reader.ReadBlock<ResourceSimpleList64_s<Unknown_C_004>>(Array.Empty<object>());
			Constraints = reader.ReadBlock<ResourceSimpleList64_s<Unknown_C_004>>(Array.Empty<object>());
			Unknown_120h = reader.ReadUInt64();
			Unknown_128h = reader.ReadUInt64();
			BehaviorPointer = reader.ReadUInt64();
			Unknown_138h = reader.ReadUInt64();
			Unknown_140h_Pointer = reader.ReadUInt64();
			Unknown_148h = reader.ReadUInt16();
			VertexCount2 = reader.ReadUInt16();
			Unknown_14Ch = reader.ReadUInt32();
			Unknown_150h = reader.ReadUInt64();
			Unknown_158h = reader.ReadSingle();
			Unknown_15Ch = reader.ReadUInt32();
			Unknown_160h = reader.ReadUInt64();
			Unknown_168h = reader.ReadUInt64();
			Unknown_170h = reader.ReadUInt64();
			Unknown_178h = reader.ReadUInt64();
			Bound = reader.ReadBlockAt<Bounds>(BoundPointer, Array.Empty<object>());
			Behavior = reader.ReadBlockAt<EnvClothVerletBehavior>(BehaviorPointer, Array.Empty<object>());
			Unknown_140h_Data = reader.ReadBlockAt<Unknown_C_007>(Unknown_140h_Pointer, Array.Empty<object>());
			if (Bound != null)
			{
				Bound.Owner = this;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			BoundPointer = (ulong)((Bound != null) ? Bound.FilePosition : 0);
			BehaviorPointer = (ulong)((Behavior != null) ? Behavior.FilePosition : 0);
			Unknown_140h_Pointer = (ulong)((Unknown_140h_Data != null) ? Unknown_140h_Data.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(Unknown_10h);
			writer.Write(BoundPointer);
			writer.Write(Unknown_20h);
			writer.Write(Unknown_28h);
			writer.Write(BBMin);
			writer.Write(Unknown_3Ch);
			writer.Write(BBMax);
			writer.Write(Unknown_4Ch);
			writer.Write(Unknown_50h);
			writer.Write(Unknown_54h);
			writer.Write(Unknown_58h);
			writer.Write(Unknown_60h);
			writer.Write(Unknown_68h);
			writer.WriteBlock(Vertices2);
			writer.WriteBlock(Vertices);
			writer.Write(Unknown_90h);
			writer.Write(Unknown_98h);
			writer.Write(Unknown_A0h);
			writer.Write(Unknown_A8h);
			writer.Write(Unknown_ACh);
			writer.Write(Unknown_B0h);
			writer.Write(Unknown_B8h);
			writer.Write(Unknown_C0h);
			writer.Write(Unknown_C8h);
			writer.Write(Unknown_D0h);
			writer.Write(Unknown_D8h);
			writer.Write(Unknown_E0h);
			writer.Write(Unknown_E8h);
			writer.Write(ConstraintCount);
			writer.Write(VertexCount);
			writer.Write(Unknown_F4h);
			writer.Write(Unknown_F8h);
			writer.Write(Unknown_FAh);
			writer.Write(Unknown_FCh);
			writer.WriteBlock(Constraints2);
			writer.WriteBlock(Constraints);
			writer.Write(Unknown_120h);
			writer.Write(Unknown_128h);
			writer.Write(BehaviorPointer);
			writer.Write(Unknown_138h);
			writer.Write(Unknown_140h_Pointer);
			writer.Write(Unknown_148h);
			writer.Write(VertexCount2);
			writer.Write(Unknown_14Ch);
			writer.Write(Unknown_150h);
			writer.Write(Unknown_158h);
			writer.Write(Unknown_15Ch);
			writer.Write(Unknown_160h);
			writer.Write(Unknown_168h);
			writer.Write(Unknown_170h);
			writer.Write(Unknown_178h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (Bound != null)
			{
				list.Add(Bound);
			}
			if (Behavior != null)
			{
				list.Add(Behavior);
			}
			if (Unknown_140h_Data != null)
			{
				list.Add(Unknown_140h_Data);
			}
			return list.ToArray();
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[4]
			{
				new Tuple<long, IResourceBlock>(112L, Vertices2),
				new Tuple<long, IResourceBlock>(128L, Vertices),
				new Tuple<long, IResourceBlock>(256L, Constraints2),
				new Tuple<long, IResourceBlock>(272L, Constraints)
			};
		}
	}
}
