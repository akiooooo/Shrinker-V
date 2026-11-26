using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct Matrix3_s
	{
		[CompilerGenerated]
		private Vector4 _003CRow1_003Ek__BackingField;

		[CompilerGenerated]
		private Vector4 _003CRow2_003Ek__BackingField;

		[CompilerGenerated]
		private Vector4 _003CRow3_003Ek__BackingField;
	}
}
