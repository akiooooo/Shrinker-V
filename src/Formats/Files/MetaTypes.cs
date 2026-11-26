using System;
using System.Runtime.InteropServices;

namespace GameFormats.Files
{
	public static class MetaTypes
	{
		public static byte[] ConvertArrayToBytes<T>(params T[] items) where T : struct
		{
			if (items == null)
			{
				return null;
			}
			int num = Marshal.SizeOf(typeof(T)) * items.Length;
			byte[] array = new byte[num];
			GCHandle gCHandle = GCHandle.Alloc(items, GCHandleType.Pinned);
			Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, 0, num);
			gCHandle.Free();
			return array;
		}

		public static T[] ConvertDataArray<T>(byte[] data, int offset, int count) where T : struct
		{
			T[] array = new T[count];
			int num = Marshal.SizeOf(typeof(T));
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			IntPtr destination = gCHandle.AddrOfPinnedObject();
			Marshal.Copy(data, offset, destination, num * count);
			gCHandle.Free();
			return array;
		}
	}
}
