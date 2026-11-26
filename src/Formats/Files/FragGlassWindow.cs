using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class FragGlassWindow : ResourceSystemBlock
	{
		public uint UnkUint1 = 2139095041u;

		public uint UnkUint2 = 2139095041u;

		public uint UnkUint3 = 2139095041u;

		public ushort UnkUshort1 = 2;

		public uint UnkUint4 = 2139095041u;

		[CompilerGenerated]
		private FragPhysTypeGroup _003CGroup_003Ek__BackingField;

		[CompilerGenerated]
		private FragPhysicsLOD _003CGroupLOD_003Ek__BackingField;

		public override long BlockLength => 112L;

		public override long BlockLength_Gen9 => 416L;

		public Vector3 ProjectionRow1 { get; set; }

		public Vector3 ProjectionRow2 { get; set; }

		public Vector3 ProjectionRow3 { get; set; }

		public float UnkFloat13 { get; set; }

		public float UnkFloat14 { get; set; }

		public float UnkFloat15 { get; set; }

		public float UnkFloat16 { get; set; }

		public VertexDeclaration VertexDeclaration { get; set; }

		public VertexDeclarationG9 VertexDeclarationG9 { get; set; }

		public float Thickness { get; set; }

		public ushort Flags { get; set; }

		public float UnkFloat18 { get; set; }

		public float UnkFloat19 { get; set; }

		public Vector3 Tangent { get; set; }

		public FragPhysTypeGroup Group
		{
			[CompilerGenerated]
			set
			{
				_003CGroup_003Ek__BackingField = value;
			}
		}

		public FragPhysicsLOD GroupLOD
		{
			[CompilerGenerated]
			set
			{
				_003CGroupLOD_003Ek__BackingField = value;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			ProjectionRow1 = reader.ReadVector3();
			UnkUint1 = reader.ReadUInt32();
			ProjectionRow2 = reader.ReadVector3();
			UnkUint2 = reader.ReadUInt32();
			ProjectionRow3 = reader.ReadVector3();
			UnkUint3 = reader.ReadUInt32();
			UnkFloat13 = reader.ReadSingle();
			UnkFloat14 = reader.ReadSingle();
			UnkFloat15 = reader.ReadSingle();
			UnkFloat16 = reader.ReadSingle();
			if (reader.IsGen9)
			{
				VertexDeclarationG9 = new VertexDeclarationG9();
				VertexDeclarationG9.Read(reader);
				VertexDeclaration = VertexDeclarationG9.GetLegacyDeclaration();
			}
			else
			{
				VertexDeclaration = new VertexDeclaration();
				VertexDeclaration.Read(reader);
			}
			Thickness = reader.ReadSingle();
			UnkUshort1 = reader.ReadUInt16();
			Flags = reader.ReadUInt16();
			UnkFloat18 = reader.ReadSingle();
			UnkFloat19 = reader.ReadSingle();
			Tangent = reader.ReadVector3();
			UnkUint4 = reader.ReadUInt32();
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(ProjectionRow1);
			writer.Write(UnkUint1);
			writer.Write(ProjectionRow2);
			writer.Write(UnkUint2);
			writer.Write(ProjectionRow3);
			writer.Write(UnkUint3);
			writer.Write(UnkFloat13);
			writer.Write(UnkFloat14);
			writer.Write(UnkFloat15);
			writer.Write(UnkFloat16);
			if (VertexDeclaration == null)
			{
				VertexDeclaration = CreateVertexDeclaration();
			}
			if (writer.IsGen9)
			{
				if (VertexDeclarationG9 == null)
				{
					VertexDeclarationG9 = VertexDeclarationG9.FromLegacyDeclaration(VertexDeclaration);
				}
				VertexDeclarationG9.Write(writer);
			}
			else
			{
				VertexDeclaration.Write(writer);
			}
			writer.Write(Thickness);
			writer.Write(UnkUshort1);
			writer.Write(Flags);
			writer.Write(UnkFloat18);
			writer.Write(UnkFloat19);
			writer.Write(Tangent);
			writer.Write(UnkUint4);
		}

		private VertexDeclaration CreateVertexDeclaration()
		{
			VertexDeclaration vertexDeclaration = new VertexDeclaration();
			vertexDeclaration.Types = VertexDeclarationTypes.GTAV4;
			vertexDeclaration.Flags = 217u;
			vertexDeclaration.UpdateCountAndStride();
			return vertexDeclaration;
		}
	}
}
