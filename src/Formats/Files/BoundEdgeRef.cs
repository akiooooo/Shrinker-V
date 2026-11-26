using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public struct BoundEdgeRef
	{
		[CompilerGenerated]
		private int _003CVertex1_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CVertex2_003Ek__BackingField;

		public int Vertex1
		{
			[CompilerGenerated]
			set
			{
				_003CVertex1_003Ek__BackingField = value;
			}
		}

		public int Vertex2
		{
			[CompilerGenerated]
			set
			{
				_003CVertex2_003Ek__BackingField = value;
			}
		}

		public BoundEdgeRef(int i1, int i2)
		{
			Vertex1 = Math.Min(i1, i2);
			Vertex2 = Math.Max(i1, i2);
		}
	}
}
