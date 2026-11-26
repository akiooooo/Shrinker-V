using System;

namespace GameFormats.Files
{
	[Flags]
	public enum EBoneFlags : ushort
	{
		None = 0,
		RotX = 1,
		RotY = 2,
		RotZ = 4,
		LimitRotation = 8,
		TransX = 0x10,
		TransY = 0x20,
		TransZ = 0x40,
		LimitTranslation = 0x80,
		ScaleX = 0x100,
		ScaleY = 0x200,
		ScaleZ = 0x400,
		LimitScale = 0x800,
		Unk0 = 0x1000,
		Unk1 = 0x2000,
		Unk2 = 0x4000,
		Unk3 = 0x8000
	}
}
