using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct AABB_s
	{
		[CompilerGenerated]
		private Vector4 _003CMin_003Ek__BackingField;

		[CompilerGenerated]
		private Vector4 _003CMax_003Ek__BackingField;

		public Vector4 Min
		{
			[CompilerGenerated]
			set
			{
				_003CMin_003Ek__BackingField = value;
			}
		}

		public Vector4 Max
		{
			[CompilerGenerated]
			set
			{
				_003CMax_003Ek__BackingField = value;
			}
		}
	}
}
