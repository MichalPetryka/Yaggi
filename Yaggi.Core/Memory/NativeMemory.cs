using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Memory
{
	/// <summary>
	/// Provides an abstraction over native memory allocations that prevents memory leaks.
	/// </summary>
	public sealed class NativeMemory : IDisposable
	{
		/// <summary>
		/// Allocated memory block
		/// </summary>
		public IntPtr Data { get; }
		/// <summary>
		/// Usable memory size in bytes
		/// </summary>
		public int Length { get; }

		private readonly object _disposeLock = new();
		private bool _valid;

		/// <summary>
		/// Allocates a memory block of the specified size
		/// </summary>
		/// <param name="length">Requested size</param>
		public NativeMemory(int length)
		{
			if (length < 0)
				throw new ArgumentOutOfRangeException(nameof(length));

			Length = length;
			Data = Marshal.AllocCoTaskMem(length);
			if (length > 0)
				GC.AddMemoryPressure(length);
			_valid = Data != IntPtr.Zero;
		}

		private void Free()
		{
			lock (_disposeLock)
			{
				if (!_valid)
					return;

				Marshal.FreeCoTaskMem(Data);
				if (Length > 0)
					GC.RemoveMemoryPressure(Length);
				_valid = false;
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Free();
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		~NativeMemory()
		{
			Free();
		}
	}
}
