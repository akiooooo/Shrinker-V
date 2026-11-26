using System;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class ResourceSystemBlock : IResourceSystemBlock, IResourceBlock
	{
		private long position;

		public virtual long FilePosition
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
				Tuple<long, IResourceBlock>[] parts = GetParts();
				foreach (Tuple<long, IResourceBlock> tuple in parts)
				{
					tuple.Item2.FilePosition = value + tuple.Item1;
				}
			}
		}

		public abstract long BlockLength { get; }

		public virtual long BlockLength_Gen9 => BlockLength;

		public abstract void Read(ResourceDataReader reader, params object[] parameters);

		public abstract void Write(ResourceDataWriter writer, params object[] parameters);

		public virtual Tuple<long, IResourceBlock>[] GetParts()
		{
			return new Tuple<long, IResourceBlock>[0];
		}

		public virtual IResourceBlock[] GetReferences()
		{
			return new IResourceBlock[0];
		}
	}
}
