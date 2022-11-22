using System;

namespace Yaggi.Core.Memory.Disposables
{
	/// <summary>
	/// Manages lifetime of data in a pointer using a provided dispose callback
	/// </summary>
	/// <typeparam name="T">Data type</typeparam>
	public sealed unsafe class PointerDisposable<T> : IDisposable where T : unmanaged
	{
		/// <summary>
		/// Pointer to managed data
		/// </summary>
		public T* Value { get; }

		private readonly DisposeCallback _action;
		private readonly object _disposeLock = new();

		private bool _disposable;

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
		/// <remarks>Do NOT use stack pointers with this, use <see cref="StackPointerDisposable{T}"/> instead</remarks>
		public PointerDisposable(T* value, DisposeCallback callback)
		{
			Value = value;
			_action = callback ?? throw new ArgumentNullException(nameof(callback));
			_disposable = true;
		}

		private void Free()
		{
			lock (_disposeLock)
			{
				if (!_disposable)
					return;

				_action(Value);
				_disposable = false;
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Free();
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		~PointerDisposable()
		{
			Free();
		}
	}
}
