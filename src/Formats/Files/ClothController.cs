using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ClothController : ResourceSystemBlock
	{
		public uint Unknown_4h = 1u;

		public ulong Unknown_8h;

		public ulong Unknown_38h;

		public ulong Unknown_40h;

		public ulong Unknown_48h;

		public uint Unknown_54h;

		public uint Unknown_7Ch;

		public override long BlockLength => 128L;

		public uint VFT { get; set; }

		public ulong BridgeSimGfxPointer { get; set; }

		public ulong MorphControllerPointer { get; set; }

		public ulong VerletCloth1Pointer { get; set; }

		public ulong VerletCloth2Pointer { get; set; }

		public ulong VerletCloth3Pointer { get; set; }

		public uint Type { get; set; }

		public PsoChar32 Name { get; set; }

		public float Unknown_78h { get; set; }

		public ClothBridgeSimGfx BridgeSimGfx { get; set; }

		public MorphController MorphController { get; set; }

		public VerletCloth VerletCloth1 { get; set; }

		public VerletCloth VerletCloth2 { get; set; }

		public VerletCloth VerletCloth3 { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_4h = reader.ReadUInt32();
			Unknown_8h = reader.ReadUInt64();
			BridgeSimGfxPointer = reader.ReadUInt64();
			MorphControllerPointer = reader.ReadUInt64();
			VerletCloth1Pointer = reader.ReadUInt64();
			VerletCloth2Pointer = reader.ReadUInt64();
			VerletCloth3Pointer = reader.ReadUInt64();
			Unknown_38h = reader.ReadUInt64();
			Unknown_40h = reader.ReadUInt64();
			Unknown_48h = reader.ReadUInt64();
			Type = reader.ReadUInt32();
			Unknown_54h = reader.ReadUInt32();
			Name = reader.ReadStruct<PsoChar32>();
			Unknown_78h = reader.ReadSingle();
			Unknown_7Ch = reader.ReadUInt32();
			BridgeSimGfx = reader.ReadBlockAt<ClothBridgeSimGfx>(BridgeSimGfxPointer, Array.Empty<object>());
			MorphController = reader.ReadBlockAt<MorphController>(MorphControllerPointer, Array.Empty<object>());
			VerletCloth1 = reader.ReadBlockAt<VerletCloth>(VerletCloth1Pointer, Array.Empty<object>());
			VerletCloth2 = reader.ReadBlockAt<VerletCloth>(VerletCloth2Pointer, Array.Empty<object>());
			VerletCloth3 = reader.ReadBlockAt<VerletCloth>(VerletCloth3Pointer, Array.Empty<object>());
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			BridgeSimGfxPointer = (ulong)((BridgeSimGfx != null) ? BridgeSimGfx.FilePosition : 0);
			MorphControllerPointer = (ulong)((MorphController != null) ? MorphController.FilePosition : 0);
			VerletCloth1Pointer = (ulong)((VerletCloth1 != null) ? VerletCloth1.FilePosition : 0);
			VerletCloth2Pointer = (ulong)((VerletCloth2 != null) ? VerletCloth2.FilePosition : 0);
			VerletCloth3Pointer = (ulong)((VerletCloth3 != null) ? VerletCloth3.FilePosition : 0);
			writer.Write(VFT);
			writer.Write(Unknown_4h);
			writer.Write(Unknown_8h);
			writer.Write(BridgeSimGfxPointer);
			writer.Write(MorphControllerPointer);
			writer.Write(VerletCloth1Pointer);
			writer.Write(VerletCloth2Pointer);
			writer.Write(VerletCloth3Pointer);
			writer.Write(Unknown_38h);
			writer.Write(Unknown_40h);
			writer.Write(Unknown_48h);
			writer.Write(Type);
			writer.Write(Unknown_54h);
			writer.WriteStruct(Name);
			writer.Write(Unknown_78h);
			writer.Write(Unknown_7Ch);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (BridgeSimGfx != null)
			{
				list.Add(BridgeSimGfx);
			}
			if (MorphController != null)
			{
				list.Add(MorphController);
			}
			if (VerletCloth1 != null)
			{
				list.Add(VerletCloth1);
			}
			if (VerletCloth2 != null)
			{
				list.Add(VerletCloth2);
			}
			if (VerletCloth3 != null)
			{
				list.Add(VerletCloth3);
			}
			return list.ToArray();
		}
	}
}
