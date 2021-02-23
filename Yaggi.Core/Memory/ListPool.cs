using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Yaggi.Core.Memory
{
	public static class ListPool<T> where T : class
	{
		private const int DefaultCapacity = 512;
		private static readonly ConcurrentQueue<List<T>> Pool = new();

		public static List<T> Rent()
		{
			return Pool.TryDequeue(out List<T> list) ? list : new List<T>(DefaultCapacity);
		}

		public static List<T> Rent(int capacity)
		{
			if (Pool.TryDequeue(out List<T> list))
			{
				if (list.Capacity < capacity)
					list.Capacity = capacity;
				return list;
			}

			return new List<T>(Math.Max(capacity, DefaultCapacity));
		}

		public static List<T> Rent(IEnumerable<T> enumerable)
		{
			if (Pool.TryDequeue(out List<T> list))
			{
				list.AddRange(enumerable);
				return list;
			}

			return new List<T>(enumerable);
		}

		public static void Return(List<T> list)
		{
			list.Clear();
			Pool.Enqueue(list);
		}
	}
}
