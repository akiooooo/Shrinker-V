using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct BoundMaterial_s
	{
		public uint Data1;

		public uint Data2;

		public BoundsMaterialType Type => (byte)(Data1 & 0xFF);

		public byte ProceduralId => (byte)((Data1 >> 8) & 0xFF);

		public byte RoomId => (byte)((Data1 >> 16) & 0x1F);

		public byte PedDensity => (byte)((Data1 >> 21) & 7);

		public EBoundMaterialFlags Flags => (EBoundMaterialFlags)(((Data1 >> 24) & 0xFF) | ((Data2 & 0xFF) << 8));

		public byte MaterialColorIndex => (byte)((Data2 >> 8) & 0xFF);

		public ushort Unk4 => (ushort)((Data2 >> 16) & 0xFFFF);

		public override string ToString()
		{
			return Data1 + ", " + Data2 + ", " + Type.ToString() + ", " + ProceduralId + ", " + RoomId + ", " + PedDensity + ", " + Flags.ToString() + ", " + MaterialColorIndex + ", " + Unk4;
		}
	}
}
