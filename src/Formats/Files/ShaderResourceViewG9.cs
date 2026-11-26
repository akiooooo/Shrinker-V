namespace GameFormats.Files
{
	public class ShaderResourceViewG9 : ResourceSystemBlock
	{
		public override long BlockLength => 32L;

		public ulong VFT { get; set; } = 5375752152uL;

		public ulong Unknown_08h { get; set; }

		public ShaderResourceViewDimensionG9 Dimension { get; set; }

		public ushort Unknown_12h { get; set; } = ushort.MaxValue;

		public uint Unknown_14h { get; set; } = uint.MaxValue;

		public ulong Unknown_18h { get; set; }

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			VFT = reader.ReadUInt64();
			Unknown_08h = reader.ReadUInt64();
			Dimension = (ShaderResourceViewDimensionG9)reader.ReadUInt16();
			Unknown_12h = reader.ReadUInt16();
			Unknown_14h = reader.ReadUInt32();
			Unknown_18h = reader.ReadUInt64();
			ulong vFT = VFT;
			if (vFT <= 5375759112L)
			{
				switch (vFT)
				{
				}
			}
			else if (vFT <= 5376067184L)
			{
				switch (vFT)
				{
				}
			}
			else if (vFT != 5376087792L && vFT != 5376112688L)
			{
				_ = 5376219992L;
			}
			switch (Dimension)
			{
			default:
				_ = 163;
				break;
			case (ShaderResourceViewDimensionG9)0:
			case (ShaderResourceViewDimensionG9)1:
			case (ShaderResourceViewDimensionG9)2:
			case (ShaderResourceViewDimensionG9)3:
			case (ShaderResourceViewDimensionG9)4:
			case (ShaderResourceViewDimensionG9)5:
			case (ShaderResourceViewDimensionG9)6:
			case (ShaderResourceViewDimensionG9)7:
			case (ShaderResourceViewDimensionG9)8:
			case (ShaderResourceViewDimensionG9)9:
			case (ShaderResourceViewDimensionG9)10:
			case (ShaderResourceViewDimensionG9)11:
			case (ShaderResourceViewDimensionG9)12:
			case (ShaderResourceViewDimensionG9)13:
			case (ShaderResourceViewDimensionG9)14:
			case (ShaderResourceViewDimensionG9)15:
			case (ShaderResourceViewDimensionG9)16:
			case (ShaderResourceViewDimensionG9)17:
			case (ShaderResourceViewDimensionG9)18:
			case (ShaderResourceViewDimensionG9)19:
			case ShaderResourceViewDimensionG9.Buffer:
			case (ShaderResourceViewDimensionG9)21:
			case (ShaderResourceViewDimensionG9)22:
			case (ShaderResourceViewDimensionG9)23:
			case (ShaderResourceViewDimensionG9)24:
			case (ShaderResourceViewDimensionG9)25:
			case (ShaderResourceViewDimensionG9)26:
			case (ShaderResourceViewDimensionG9)27:
			case (ShaderResourceViewDimensionG9)28:
			case (ShaderResourceViewDimensionG9)29:
			case (ShaderResourceViewDimensionG9)30:
			case (ShaderResourceViewDimensionG9)31:
			case (ShaderResourceViewDimensionG9)32:
			case (ShaderResourceViewDimensionG9)33:
			case (ShaderResourceViewDimensionG9)34:
			case (ShaderResourceViewDimensionG9)35:
			case (ShaderResourceViewDimensionG9)36:
			case (ShaderResourceViewDimensionG9)37:
			case (ShaderResourceViewDimensionG9)38:
			case (ShaderResourceViewDimensionG9)39:
			case (ShaderResourceViewDimensionG9)40:
			case (ShaderResourceViewDimensionG9)41:
			case (ShaderResourceViewDimensionG9)42:
			case (ShaderResourceViewDimensionG9)43:
			case (ShaderResourceViewDimensionG9)44:
			case (ShaderResourceViewDimensionG9)45:
			case (ShaderResourceViewDimensionG9)46:
			case (ShaderResourceViewDimensionG9)47:
			case (ShaderResourceViewDimensionG9)48:
			case (ShaderResourceViewDimensionG9)49:
			case (ShaderResourceViewDimensionG9)50:
			case (ShaderResourceViewDimensionG9)51:
			case (ShaderResourceViewDimensionG9)52:
			case (ShaderResourceViewDimensionG9)53:
			case (ShaderResourceViewDimensionG9)54:
			case (ShaderResourceViewDimensionG9)55:
			case (ShaderResourceViewDimensionG9)56:
			case (ShaderResourceViewDimensionG9)57:
			case (ShaderResourceViewDimensionG9)58:
			case (ShaderResourceViewDimensionG9)59:
			case (ShaderResourceViewDimensionG9)60:
			case (ShaderResourceViewDimensionG9)61:
			case (ShaderResourceViewDimensionG9)62:
			case (ShaderResourceViewDimensionG9)63:
			case (ShaderResourceViewDimensionG9)64:
			case ShaderResourceViewDimensionG9.Texture2D:
			case ShaderResourceViewDimensionG9.Texture2DArray:
				break;
			}
			_ = Unknown_08h;
			_ = Unknown_18h;
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			writer.Write(VFT);
			writer.Write(Unknown_08h);
			writer.Write((ushort)Dimension);
			writer.Write(Unknown_12h);
			writer.Write(Unknown_14h);
			writer.Write(Unknown_18h);
		}
	}
}
