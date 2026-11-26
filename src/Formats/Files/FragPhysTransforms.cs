using System.ComponentModel;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragPhysTransforms : ResourceSystemBlock
	{
		public uint Unknown_04h = 1u;

		public ulong Unknown_08h;

		public uint Unknown_14h;

		public ulong Unknown_18h;

		public override long BlockLength
		{
			get
			{
				Matrix[] matrices = Matrices;
				return 32 + ((matrices != null) ? matrices.Length : 0) * 64;
			}
		}

		public uint VFT { get; set; } = 1080043536u;

		public uint MatricesCount { get; set; }

		public Matrix[] Matrices { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt32();
			Unknown_04h = reader.ReadUInt32();
			Unknown_08h = reader.ReadUInt64();
			MatricesCount = reader.ReadUInt32();
			Unknown_14h = reader.ReadUInt32();
			Unknown_18h = reader.ReadUInt64();
			Matrices = reader.ReadStructsAt<Matrix>((ulong)reader.Position, MatricesCount);
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			Matrix[] matrices = Matrices;
			MatricesCount = ((matrices != null) ? ((uint)matrices.Length) : 0u);
			writer.Write(VFT);
			writer.Write(Unknown_04h);
			writer.Write(Unknown_08h);
			writer.Write(MatricesCount);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
			writer.WriteStructs(Matrices);
		}
	}
}
