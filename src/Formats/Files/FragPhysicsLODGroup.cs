using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysicsLODGroup : ResourceSystemBlock
	{
		public uint Unknown_04h = 1u;

		public ulong Unknown_08h;

		public ulong Unknown_28h;

		public override long BlockLength => 48L;

		public uint VFT { get; set; } = 1080055472u;

		public ulong PhysicsLOD1Pointer { get; set; }

		public ulong PhysicsLOD2Pointer { get; set; }

		public ulong PhysicsLOD3Pointer { get; set; }

		public FragPhysicsLOD PhysicsLOD1 { get; set; }

		public FragPhysicsLOD PhysicsLOD2 { get; set; }

		public FragPhysicsLOD PhysicsLOD3 { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			Unknown_08h = reader.ReadUInt64();
			PhysicsLOD1Pointer = reader.ReadUInt64();
			PhysicsLOD2Pointer = reader.ReadUInt64();
			PhysicsLOD3Pointer = reader.ReadUInt64();
			Unknown_28h = reader.ReadUInt64();
			PhysicsLOD1 = reader.ReadBlockAt<FragPhysicsLOD>(PhysicsLOD1Pointer, Array.Empty<object>());
			PhysicsLOD2 = reader.ReadBlockAt<FragPhysicsLOD>(PhysicsLOD2Pointer, Array.Empty<object>());
			PhysicsLOD3 = reader.ReadBlockAt<FragPhysicsLOD>(PhysicsLOD3Pointer, Array.Empty<object>());
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			PhysicsLOD1Pointer = (ulong)((PhysicsLOD1 != null) ? PhysicsLOD1.FilePosition : 0);
			PhysicsLOD2Pointer = (ulong)((PhysicsLOD2 != null) ? PhysicsLOD2.FilePosition : 0);
			PhysicsLOD3Pointer = (ulong)((PhysicsLOD3 != null) ? PhysicsLOD3.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.Write(PhysicsLOD1Pointer);
			writer.Write(PhysicsLOD2Pointer);
			writer.Write(PhysicsLOD3Pointer);
			writer.Write(Unknown_28h);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (PhysicsLOD1 != null)
			{
				list.Add(PhysicsLOD1);
			}
			if (PhysicsLOD2 != null)
			{
				list.Add(PhysicsLOD2);
			}
			if (PhysicsLOD3 != null)
			{
				list.Add(PhysicsLOD3);
			}
			return list.ToArray();
		}
	}
}
