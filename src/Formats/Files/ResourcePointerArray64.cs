using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourcePointerArray64<T> : ResourceSystemBlock, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : IResourceSystemBlock, new()
	{
		public bool ManualReferenceOverride;

		public override long BlockLength => (data_items != null) ? (8 * data_items.Length) : 0;

		public ulong[] data_pointers { get; set; }

		public T[] data_items { get; set; }

		public T this[int index]
		{
			get
			{
				return data_items[index];
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int Count
		{
			get
			{
				if (data_items == null)
				{
					return 0;
				}
				return data_items.Length;
			}
		}

		public bool IsReadOnly => false;

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			int num = Convert.ToInt32(parameters[0]);
			data_pointers = reader.ReadUlongsAt((ulong)reader.Position, (uint)num, cache: false);
			data_items = new T[num];
			for (int i = 0; i < num; i++)
			{
				data_items[i] = reader.ReadBlockAt<T>(data_pointers[i], Array.Empty<object>());
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			List<ulong> list = new List<ulong>();
			T[] array = data_items;
			for (int i = 0; i < array.Length; i++)
			{
				T val = array[i];
				if (val != null)
				{
					list.Add((uint)val.FilePosition);
				}
				else
				{
					list.Add(0uL);
				}
			}
			data_pointers = list.ToArray();
			ulong[] array2 = data_pointers;
			foreach (ulong value in array2)
			{
				writer.Write(value);
			}
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			if (!ManualReferenceOverride)
			{
				T[] array = data_items;
				foreach (T val in array)
				{
					list.Add(val);
				}
			}
			return list.ToArray();
		}

		public int IndexOf(T item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return "(Count: " + ((data_items != null) ? data_items.Length : 0) + ")";
		}
	}
}
