using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Yaggi.Core.Memory
{
	/// <summary>
	/// Provides pooled <see cref="List{T}"/>
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	public static class ListPool<T>
	{
		private const int DefaultCapacity = 512;
		private static readonly ConcurrentQueue<List<T>> Pool = new();

		/// <summary>
		/// Provides a pooled <see cref="List{T}"/>
		/// </summary>
		/// <returns><see cref="List{T}"/> from the pool</returns>
		public static List<T> Rent()
		{
			return Pool.TryDequeue(out List<T> list) ? list : new List<T>(DefaultCapacity);
		}

		/// <summary>
		/// Provides a pooled <see cref="List{T}"/> with provided capacity
		/// </summary>
		/// <param name="capacity">Requested minimum capacity</param>
		/// <returns><see cref="List{T}"/> from the pool</returns>
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

		/// <summary>
		/// Provides a pooled <see cref="List{T}"/> with specified initial content
		/// </summary>
		/// <param name="enumerable">Initial content</param>
		/// <returns><see cref="List{T}"/> from the pool</returns>
		public static List<T> Rent(IEnumerable<T> enumerable)
		{
			if (Pool.TryDequeue(out List<T> list))
			{
				list.AddRange(enumerable);
				return list;
			}

			list = new List<T>(enumerable);
			if (list.Capacity < DefaultCapacity)
				list.Capacity = DefaultCapacity;
			return list;
		}

		/// <summary>
		/// Returns a <see cref="List{T}"/> to the pool
		/// </summary>
		/// <param name="list">Returned <see cref="List{T}"/></param>
		public static void Return(List<T> list)
		{
			list.Clear();
			Pool.Enqueue(list);
		}
	}
}
