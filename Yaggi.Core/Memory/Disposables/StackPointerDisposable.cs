using System;

namespace Yaggi.Core.Memory.Disposables
{
	/// <summary>
	/// Manages lifetime of data in a pointer using a provided dispose callback.
	/// Unlike <see cref="PointerDisposable{T}"/> it's not threadsafe and doesn't prevent multiple dispose calls
	/// </summary>
	/// <typeparam name="T">Data type</typeparam>
	public readonly unsafe struct StackPointerDisposable<T> : IDisposable where T : unmanaged
	{
		/// <summary>
		/// Pointer to managed data
		/// </summary>
		public T* Value { get; }

		private readonly DisposeCallback _action;

		/// <summary>
		/// Data dispose callback
		/// </summary>
		/// <param name="ptr">Data pointer</param>
		public delegate void DisposeCallback(T* ptr);

		/// <summary>
		/// Manages lifetime of data in a pointer using a provided dispose callback
		/// </summary>
		/// <param name="value">Managed data</param>
		/// <param name="callback">Dispose callback</param>
		public StackPointerDisposable(T* value, DisposeCallback callback)
		{
			Value = value;
			_action = callback ?? throw new ArgumentNullException(nameof(callback));
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			_action(Value);
		}
	}
}
