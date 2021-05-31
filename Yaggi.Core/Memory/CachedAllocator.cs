using System;
using System.Collections.Concurrent;

namespace Yaggi.Core.Memory
{
	/// <summary>
	/// Provides unmanaged memory, caches allocations with small size
	/// </summary>
	public static class CachedAllocator
	{
		private const int MaxPoolElements = 16;
		private const int CachedSize = 4096;

		private static readonly ConcurrentQueue<NativeMemory> Cache = new();

		/// <summary>
		/// Allocates a memory block with the size gueranteed to be at least the same as specified.
		/// Returns previously freed memory when possible.
		/// </summary>
		/// <param name="size">Minimum size</param>
		/// <returns>Usable memory</returns>
		public static NativeMemory Allocate(int size)
		{
			if (size <= CachedSize && Cache.TryDequeue(out NativeMemory result))
				return result;

			return new NativeMemory(Math.Max(CachedSize, size));
		}

		/// <summary>
		/// Frees a previously allocated memory block or returns it to the pool when possible.
		/// </summary>
		/// <param name="memory">Freed memory</param>
		public static void Free(NativeMemory memory)
		{
			if (memory.Length == CachedSize && Cache.Count <= MaxPoolElements)
			{
				Cache.Enqueue(memory);
				return;
			}

			memory.Dispose();
		}
	}
}
