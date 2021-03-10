using System;

namespace Yaggi.Core.Memory.Disposables
{
	/// <summary>
	/// Wraps an <see cref="Action"/> into <see cref="IDisposable"/>.
	/// Unlike <see cref="Disposable"/> it's not threadsafe and doesn't prevent multiple dispose calls
	/// </summary>
	public readonly unsafe struct StackDisposable : IDisposable
	{
		private readonly Action _action;

		/// <summary>
		/// Wraps an <see cref="Action"/> into <see cref="IDisposable"/>
		/// </summary>
		/// <param name="callback">Wrapped <see cref="Action"/></param>
		public StackDisposable(Action callback)
		{
			_action = callback ?? throw new ArgumentNullException(nameof(callback));
		}

		/// <summary>
		/// Wraps an <see cref="Action"/> into <see cref="IDisposable"/>.
		/// Unlike <see cref="Disposable"/> it's not threadsafe and doesn't prevent multiple dispose calls
		/// </summary>
		/// <param name="callback">Wrapped <see cref="Action"/></param>
		/// <returns>Wrapping <see cref="IDisposable"/></returns>
		public static StackDisposable Create(Action callback) => new(callback);
		/// <summary>
		/// Manages lifetime of data using a provided dispose callback.
		/// Unlike <see cref="Disposable{T}"/> it's not threadsafe and doesn't prevent multiple dispose calls
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="value">Managed data</param>
		/// <param name="callback">Dispose callback</param>
		/// <returns>Created <see cref="IDisposable"/></returns>
		public static StackDisposable<T> Create<T>(T value, Action<T> callback) => new(value, callback);
		/// <summary>
		/// Manages lifetime of data in a pointer using a provided dispose callback.
		/// Unlike <see cref="PointerDisposable{T}"/> it's not threadsafe and doesn't prevent multiple dispose calls
		/// </summary>
		/// <typeparam name="T">Data type</typeparam>
		/// <param name="value">Managed data</param>
		/// <param name="callback">Dispose callback</param>
		/// <returns>Created <see cref="IDisposable"/></returns>
		public static StackPointerDisposable<T> Create<T>(T* value, StackPointerDisposable<T>.DisposeCallback callback) where T : unmanaged
			=> new(value, callback);

		/// <inheritdoc/>
		public void Dispose()
		{
			_action();
		}
	}

	/// <summary>
	/// Manages lifetime of data using a provided dispose callback.
	/// Unlike <see cref="Disposable{T}"/> it's not threadsafe and doesn't prevent multiple dispose calls
	/// </summary>
	/// <typeparam name="T">Managed data</typeparam>
	public readonly struct StackDisposable<T> : IDisposable
	{
		/// <summary>
		/// Managed data
		/// </summary>
		public T Value { get; }

		private readonly Action<T> _action;

		/// <summary>
		/// Manages lifetime of data using a provided dispose callback
		/// </summary>
		/// <param name="value">Data</param>
		/// <param name="callback">Dispose callback</param>
		public StackDisposable(T value, Action<T> callback)
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
