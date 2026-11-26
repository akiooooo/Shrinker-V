using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DrawableDictionary : ResourceFileBase
	{
		public ulong Unknown_10h;

		public ulong Unknown_18h = 1uL;

		public uint Unknown_2Ch;

		public uint Unknown_3Ch;

		private ResourceSystemStructBlock<uint> HashesBlock;

		public override long BlockLength => 64L;

		public ulong HashesPointer { get; set; }

		public ushort HashesCount1 { get; set; }

		public ushort HashesCount2 { get; set; }

		public ulong DrawablesPointer { get; set; }

		public ushort DrawablesCount1 { get; set; }

		public ushort DrawablesCount2 { get; set; }

		public uint[] Hashes { get; set; }

		public ResourcePointerArray64<Drawable> Drawables { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			base.Read(reader, parameters);
			Unknown_10h = reader.ReadUInt64();
			Unknown_18h = reader.ReadUInt64();
			HashesPointer = reader.ReadUInt64();
			HashesCount1 = reader.ReadUInt16();
			HashesCount2 = reader.ReadUInt16();
			Unknown_2Ch = reader.ReadUInt32();
			DrawablesPointer = reader.ReadUInt64();
			DrawablesCount1 = reader.ReadUInt16();
			DrawablesCount2 = reader.ReadUInt16();
			Unknown_3Ch = reader.ReadUInt32();
			Hashes = reader.ReadUintsAt(HashesPointer, HashesCount1);
			Drawables = reader.ReadBlockAt<ResourcePointerArray64<Drawable>>(DrawablesPointer, new object[1] { DrawablesCount1 });
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			base.Write(writer, parameters);
			HashesPointer = (ulong)((HashesBlock != null) ? HashesBlock.FilePosition : 0);
			HashesCount1 = (ushort)((HashesBlock != null) ? ((uint)HashesBlock.ItemCount) : 0u);
			HashesCount2 = (ushort)((HashesBlock != null) ? ((uint)HashesBlock.ItemCount) : 0u);
			DrawablesPointer = (ulong)((Drawables != null) ? Drawables.FilePosition : 0);
			DrawablesCount1 = (ushort)((Drawables != null) ? ((uint)Drawables.Count) : 0u);
			DrawablesCount2 = (ushort)((Drawables != null) ? ((uint)Drawables.Count) : 0u);
			writer.Write(Unknown_10h);
			writer.Write(Unknown_18h);
			writer.Write(HashesPointer);
			writer.Write(HashesCount1);
			writer.Write(HashesCount2);
			writer.Write(Unknown_2Ch);
			writer.Write(DrawablesPointer);
			writer.Write(DrawablesCount1);
			writer.Write(DrawablesCount2);
			writer.Write(Unknown_3Ch);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>(base.GetReferences());
			if (Hashes != null)
			{
				HashesBlock = new ResourceSystemStructBlock<uint>(Hashes);
				list.Add(HashesBlock);
			}
			if (Drawables != null)
			{
				list.Add(Drawables);
			}
			return list.ToArray();
		}
	}
}
