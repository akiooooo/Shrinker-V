using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BoundGeomOctants : ResourceSystemBlock
	{
		public uint[] Counts { get; } = new uint[8];

		public uint[][] Items { get; } = new uint[8][];

		public override long BlockLength
		{
			get
			{
				long num = 128L;
				for (int i = 0; i < 8; i++)
				{
					num += Counts[i] * 4;
				}
				return num;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			if (((parameters != null) ? parameters.Length : 0) >= 1)
			{
				ulong num = (ulong)parameters[0];
				for (int i = 0; i < 8; i++)
				{
					Counts[i] = reader.ReadUInt32();
				}
				ulong[] array = reader.ReadUlongsAt(num, 8u, cache: false);
				for (int j = 0; j < 8; j++)
				{
					Items[j] = reader.ReadUintsAt(array[j], Counts[j], cache: false);
				}
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			long num = writer.Position + 96;
			for (int i = 0; i < 8; i++)
			{
				writer.Write(Counts[i]);
			}
			for (int j = 0; j < 8; j++)
			{
				writer.Write((ulong)num);
				num += Counts[j] * 4;
			}
			for (int k = 0; k < 8; k++)
			{
				uint[] array = ((k < Items.Length) ? Items[k] : null);
				if (array != null)
				{
					uint num2 = Counts[k];
					for (int l = 0; l < num2; l++)
					{
						uint value = ((l < array.Length) ? array[l] : 0u);
						writer.Write(value);
					}
				}
			}
			writer.Write(new byte[32]);
		}
	}
}
