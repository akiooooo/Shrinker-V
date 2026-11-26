using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameMath;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct BVHNode_s
	{
		[CompilerGenerated]
		private short _003CMinX_003Ek__BackingField;

		[CompilerGenerated]
		private short _003CMinY_003Ek__BackingField;

		[CompilerGenerated]
		private short _003CMinZ_003Ek__BackingField;

		[CompilerGenerated]
		private short _003CMaxX_003Ek__BackingField;

		[CompilerGenerated]
		private short _003CMaxY_003Ek__BackingField;

		[CompilerGenerated]
		private short _003CMaxZ_003Ek__BackingField;

		public short MinX
		{
			[CompilerGenerated]
			set
			{
				_003CMinX_003Ek__BackingField = value;
			}
		}

		public short MinY
		{
			[CompilerGenerated]
			set
			{
				_003CMinY_003Ek__BackingField = value;
			}
		}

		public short MinZ
		{
			[CompilerGenerated]
			set
			{
				_003CMinZ_003Ek__BackingField = value;
			}
		}

		public short MaxX
		{
			[CompilerGenerated]
			set
			{
				_003CMaxX_003Ek__BackingField = value;
			}
		}

		public short MaxY
		{
			[CompilerGenerated]
			set
			{
				_003CMaxY_003Ek__BackingField = value;
			}
		}

		public short MaxZ
		{
			[CompilerGenerated]
			set
			{
				_003CMaxZ_003Ek__BackingField = value;
			}
		}

		public short ItemId { get; set; }

		public short ItemCount { get; set; }

		public Vector3 Min
		{
			set
			{
				MinX = (short)value.X;
				MinY = (short)value.Y;
				MinZ = (short)value.Z;
			}
		}

		public Vector3 Max
		{
			set
			{
				MaxX = (short)value.X;
				MaxY = (short)value.Y;
				MaxZ = (short)value.Z;
			}
		}

		public override string ToString()
		{
			return ItemId + ": " + ItemCount;
		}
	}
}
