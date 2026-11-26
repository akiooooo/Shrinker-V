using System;
using System.Collections.Generic;
using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BVH : ResourceSystemBlock
	{
		public override long BlockLength => 128L;

		public ResourceSimpleList64b_s<BVHNode_s> Nodes { get; set; }

		public uint Unknown_10h { get; set; }

		public uint Unknown_14h { get; set; }

		public uint Unknown_18h { get; set; }

		public uint Unknown_1Ch { get; set; }

		public Vector4 BoundingBoxMin { get; set; }

		public Vector4 BoundingBoxMax { get; set; }

		public Vector4 BoundingBoxCenter { get; set; }

		public Vector4 QuantumInverse { get; set; }

		public Vector4 Quantum { get; set; }

		public ResourceSimpleList64_s<BVHTreeInfo_s> Trees { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			Nodes = reader.ReadBlock<ResourceSimpleList64b_s<BVHNode_s>>(Array.Empty<object>());
			Unknown_10h = reader.ReadUInt32();
			Unknown_14h = reader.ReadUInt32();
			Unknown_18h = reader.ReadUInt32();
			Unknown_1Ch = reader.ReadUInt32();
			BoundingBoxMin = reader.ReadVector4();
			BoundingBoxMax = reader.ReadVector4();
			BoundingBoxCenter = reader.ReadVector4();
			QuantumInverse = reader.ReadVector4();
			Quantum = reader.ReadVector4();
			Trees = reader.ReadBlock<ResourceSimpleList64_s<BVHTreeInfo_s>>(Array.Empty<object>());
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.WriteBlock(Nodes);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.Write(Unknown_1Ch);
			writer.Write(BoundingBoxMin);
			writer.Write(BoundingBoxMax);
			writer.Write(BoundingBoxCenter);
			writer.Write(QuantumInverse);
			writer.Write(Quantum);
			writer.WriteBlock(Trees);
		}

		public override IResourceBlock[] GetReferences()
		{
			return new List<IResourceBlock>().ToArray();
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[2]
			{
				new Tuple<long, IResourceBlock>(0L, Nodes),
				new Tuple<long, IResourceBlock>(112L, Trees)
			};
		}
	}
}
