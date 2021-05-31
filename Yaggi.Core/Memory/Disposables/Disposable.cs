using System;

namespace Yaggi.Core.Memory.Disposables
{
	/// <summary>
	/// Wraps an <see cref="Action"/> into <see cref="IDisposable"/>
	/// </summary>
	public sealed unsafe class Disposable : IDisposable
	{
		private readonly Action _action;
		private readonly object _disposeLock = new();

		private bool _disposable;

		/// <summary>
		/// Wraps an <see cref="Action"/> into <see cref="IDisposable"/>
		/// </summary>
		/// <param name="callback">Wrapped <see cref="Action"/></param>
		public Disposable(Action callback)
		{
			_action = callback ?? throw new ArgumentNullException(nameof(callback));
			_disposable = true;
		}

		/// <summary>
		/// Wraps an <see cref="Action"/> into <see cref="IDisposable"/>
		/// </summary>
		/// <param name="callback">Wrapped <see cref="Action"/></param>
		/// <returns>Wrapping <see cref="IDisposable"/></returns>
		public static Disposable Create(Action callback) => new(callback);
		/// <summary>
		/// Combines multiple <see cref="IDisposable"/> into one
		/// </summary>
		/// <param name="disposables">Combined <see cref="IDisposable"/>s</param>
		/// <returns>Combined <see cref="IDisposable"/></returns>
		public static MultiDisposable Create(params IDisposable[] disposables) => new(disposables);
		/// <summary>
		/// Manages lifetime of data using a provided dispose callback
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="value">Managed data</param>
		/// <param name="callback">Dispose callback</param>
		/// <returns>Created <see cref="IDisposable"/></returns>
		public static Disposable<T> Create<T>(T value, Action<T> callback) => new(value, callback);
		/// <summary>
		/// Manages lifetime of data in a pointer using a provided dispose callback
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="value">Managed data</param>
		/// <param name="callback">Dispose callback</param>
		/// <returns>Created <see cref="IDisposable"/></returns>
		public static PointerDisposable<T> Create<T>(T* value, PointerDisposable<T>.DisposeCallback callback) where T : unmanaged
			=> new(value, callback);

		private void Free()
		{
			lock (_disposeLock)
			{
				if (!_disposable)
					return;

				_action();
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
		~Disposable()
		{
			Free();
		}
	}

	/// <summary>
	/// Manages lifetime of data using a provided dispose callback
	/// </summary>
	/// <typeparam name="T">Managed data</typeparam>
	public sealed class Disposable<T> : IDisposable
	{
		/// <summary>
		/// Managed data
		/// </summary>
		public T Value { get; }

		private readonly Action<T> _action;
		private readonly object _disposeLock = new();

		private bool _disposable;

		/// <summary>
		/// Manages lifetime of data using a provided dispose callback
		/// </summary>
		/// <param name="value">Data</param>
		/// <param name="callback">Dispose callback</param>
		public Disposable(T value, Action<T> callback)
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
		~Disposable()
		{
			Free();
		}
	}
}
