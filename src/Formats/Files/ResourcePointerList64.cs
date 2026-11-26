using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace GameFormats.Files
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourcePointerList64<T> : ResourceSystemBlock, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : IResourceSystemBlock, new()
	{
		public bool ManualCountOverride;

		public bool ManualReferenceOverride;

		private ResourcePointerArray64<T> data_block;

		public override long BlockLength => 16L;

		public ulong EntriesPointer { get; private set; }

		public ushort EntriesCount { get; set; }

		public ushort EntriesCapacity { get; set; }

		public ulong[] data_pointers { get; private set; }

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

		public int Count => EntriesCount;

		public bool IsReadOnly => false;

		public override void Read(ResourceDataReader reader, params object[] parameters)
		{
			EntriesPointer = reader.ReadUInt64();
			EntriesCount = reader.ReadUInt16();
			EntriesCapacity = reader.ReadUInt16();
			reader.Position += 4L;
			data_pointers = reader.ReadUlongsAt(EntriesPointer, EntriesCapacity);
			data_items = new T[EntriesCount];
			for (int i = 0; i < EntriesCount; i++)
			{
				data_items[i] = reader.ReadBlockAt<T>(data_pointers[i], Array.Empty<object>());
			}
		}

		public override void Write(ResourceDataWriter writer, params object[] parameters)
		{
			EntriesPointer = (ulong)((data_block != null) ? data_block.FilePosition : 0);
			if (!ManualCountOverride)
			{
				EntriesCapacity = (ushort)((data_block != null) ? ((uint)data_block.Count) : 0u);
				EntriesCount = (ushort)((data_block != null) ? ((uint)data_block.Count) : 0u);
			}
			writer.Write(EntriesPointer);
			writer.Write(EntriesCount);
			writer.Write(EntriesCapacity);
			writer.Write(0u);
		}

		public override IResourceBlock[] GetReferences()
		{
			List<IResourceBlock> list = new List<IResourceBlock>();
			T[] array = data_items;
			if (array != null && array.Length != 0)
			{
				data_block = new ResourcePointerArray64<T>();
				data_block.data_items = data_items;
				data_block.ManualReferenceOverride = ManualReferenceOverride;
				list.Add(data_block);
			}
			else
			{
				data_block = null;
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
			return "(Count: " + EntriesCount + ")";
		}
	}
}
