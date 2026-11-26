using System;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class VertexData : ResourceSystemBlock
	{
		public override long BlockLength
		{
			get
			{
				byte[] vertexBytes = VertexBytes;
				return (vertexBytes != null) ? vertexBytes.Length : 0;
			}
		}

		public int VertexStride { get; set; }

		public int VertexCount { get; set; }

		public VertexDeclaration Info { get; set; }

		public VertexType VertexType { get; set; }

		public byte[] VertexBytes { get; set; }

		public byte[] G9_VertexBytes { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VertexStride = Convert.ToInt32(parameters[0]);
			VertexCount = Convert.ToInt32(parameters[1]);
			Info = (VertexDeclaration)parameters[2];
			VertexType = (VertexType)Info.Flags;
			VertexBytes = reader.ReadBytes(VertexCount * VertexStride);
			switch (Info.Types)
			{
			default:
				_ = 8598872888530528662L;
				break;
			case VertexDeclarationTypes.GTAV2:
				if (Info.Flags == 16473)
				{
					VertexType = VertexType.PCCH2H4;
				}
				break;
			case VertexDeclarationTypes.GTAV3:
				if (Info.Flags == 89)
				{
					VertexType = VertexType.PNCH2;
				}
				break;
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			if (writer.IsGen9)
			{
				if (G9_VertexBytes != null)
				{
					writer.Write(G9_VertexBytes);
				}
			}
			else if (VertexBytes != null)
			{
				writer.Write(VertexBytes);
			}
		}

		public override string ToString()
		{
			return "Type: " + VertexType.ToString() + ", Count: " + VertexCount;
		}
	}
}
