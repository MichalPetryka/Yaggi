using System;
using System.Collections.Concurrent;
using System.Text;

namespace Yaggi.Core.Memory
{
	/// <summary>
	/// Provides pooled <see cref="StringBuilder"/>
	/// </summary>
	public static class StringBuilderPool
	{
		private const int DefaultCapacity = 512;
		private static readonly ConcurrentQueue<StringBuilder> Pool = new();

		/// <summary>
		/// Provides a pooled <see cref="StringBuilder"/>
		/// </summary>
		/// <returns><see cref="StringBuilder"/> from the pool</returns>
		public static StringBuilder Rent()
		{
			return Pool.TryDequeue(out StringBuilder stringBuilder) ? stringBuilder : new StringBuilder(DefaultCapacity);
		}

		/// <summary>
		/// Provides a pooled <see cref="StringBuilder"/> with provided capacity
		/// </summary>
		/// <param name="capacity">Requested minimum capacity</param>
		/// <returns><see cref="StringBuilder"/> from the pool</returns>
		public static StringBuilder Rent(int capacity)
		{
			if (Pool.TryDequeue(out StringBuilder stringBuilder))
			{
				if (stringBuilder.Capacity < capacity)
					stringBuilder.Capacity = capacity;
				return stringBuilder;
			}

			return new StringBuilder(Math.Max(capacity, DefaultCapacity));
		}

		/// <summary>
		/// Provides a pooled <see cref="StringBuilder"/> with initial content
		/// </summary>
		/// <param name="text">Initial content</param>
		/// <returns><see cref="StringBuilder"/> from the pool</returns>
		public static StringBuilder Rent(string text)
		{
			if (Pool.TryDequeue(out StringBuilder stringBuilder))
			{
				stringBuilder.Append(text);
				return stringBuilder;
			}

			return new StringBuilder(text, DefaultCapacity);
		}

		/// <summary>
		/// Returns a <see cref="StringBuilder"/> to the pool
		/// </summary>
		/// <param name="stringBuilder">Returned <see cref="StringBuilder"/></param>
		public static void Return(StringBuilder stringBuilder)
		{
			stringBuilder.Clear();
			Pool.Enqueue(stringBuilder);
		}

		/// <summary>
		/// Provides the content of a <see cref="StringBuilder"/> and returns it to the pool
		/// </summary>
		/// <param name="stringBuilder">Returned <see cref="StringBuilder"/></param>
		/// <returns>The content of the <see cref="StringBuilder"/></returns>
		public static string ToStringReturn(StringBuilder stringBuilder)
		{
			string value = stringBuilder.ToString();
			stringBuilder.Clear();
			Pool.Enqueue(stringBuilder);
			return value;
		}
	}
}
