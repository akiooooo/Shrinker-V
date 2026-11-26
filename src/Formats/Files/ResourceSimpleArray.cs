using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourceSimpleArray<T> : ListBase<T>, IResourceNoCacheBlock, IResourceBlock where T : IResourceSystemBlock, new()
	{
		public override long BlockLength
		{
			get
			{
				long num = 0L;
				if (base.Data != null)
				{
					foreach (T datum in base.Data)
					{
						num += datum.BlockLength;
					}
				}
				return num;
			}
		}

		public override long BlockLength_Gen9
		{
			get
			{
				long num = 0L;
				if (base.Data != null)
				{
					foreach (T datum in base.Data)
					{
						num += datum.BlockLength_Gen9;
					}
				}
				return num;
			}
		}

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			int num = Convert.ToInt32(parameters[0]);
			base.Data = new List<T>(num);
			for (int i = 0; i < num; i++)
			{
				T item = reader.ReadBlock<T>(Array.Empty<object>());
				base.Data.Add(item);
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			foreach (T datum in base.Data)
			{
				datum.Write(writer);
			}
		}

		public override Tuple<long, IResourceBlock>[] GetParts()
		{
			List<Tuple<long, IResourceBlock>> list = new List<Tuple<long, IResourceBlock>>();
			bool isGen = RpfManager.IsGen9;
			long num = 0L;
			if (base.Data != null)
			{
				foreach (T datum in base.Data)
				{
					list.Add(new Tuple<long, IResourceBlock>(num, datum));
					num += (isGen ? datum.BlockLength_Gen9 : datum.BlockLength);
				}
			}
			return list.ToArray();
		}

		public override string ToString()
		{
			return "(Count: " + base.Count + ")";
		}
	}
}
