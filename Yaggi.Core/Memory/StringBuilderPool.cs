using System;
using System.Collections.Concurrent;
using System.Text;

namespace Yaggi.Core.Memory
{
	public static class StringBuilderPool
	{
		private const int DefaultCapacity = 512;
		private static readonly ConcurrentQueue<StringBuilder> Pool = new();

		public static StringBuilder Rent()
		{
			return Pool.TryDequeue(out StringBuilder stringBuilder) ? stringBuilder : new StringBuilder(DefaultCapacity);
		}

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

		public static StringBuilder Rent(string text)
		{
			if (Pool.TryDequeue(out StringBuilder stringBuilder))
			{
				stringBuilder.Append(text);
				return stringBuilder;
			}

			return new StringBuilder(text, DefaultCapacity);
		}

		public static void Return(StringBuilder stringBuilder)
		{
			stringBuilder.Clear();
			Pool.Enqueue(stringBuilder);
		}

		public static string ToStringReturn(StringBuilder stringBuilder)
		{
			string value = stringBuilder.ToString();
			stringBuilder.Clear();
			Pool.Enqueue(stringBuilder);
			return value;
		}
	}
}
