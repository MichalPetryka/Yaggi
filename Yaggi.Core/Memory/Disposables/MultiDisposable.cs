using System;
using System.Collections.Generic;

namespace Yaggi.Core.Memory.Disposables
{
	/// <summary>
	/// Combines multiple <see cref="IDisposable"/> into one
	/// </summary>
	public sealed class MultiDisposable : IDisposable
	{
		private readonly List<IDisposable> _disposables;
		private readonly object _disposeLock = new();

		private bool _disposable = true;

		/// <summary>
		/// Combines multiple <see cref="IDisposable"/> into one
		/// </summary>
		public MultiDisposable()
		{
			_disposables = new List<IDisposable>();
		}

		/// <summary>
		/// Combines multiple <see cref="IDisposable"/> into one
		/// </summary>
		/// <param name="disposables">Combined <see cref="IDisposable"/>s</param>
		public MultiDisposable(params IDisposable[] disposables) : this((IEnumerable<IDisposable>)disposables) { }
		/// <summary>
		/// Combines multiple <see cref="IDisposable"/> into one
		/// </summary>
		/// <param name="disposables">Combined <see cref="IDisposable"/>s</param>
		public MultiDisposable(IEnumerable<IDisposable> disposables)
		{
			_disposables = new List<IDisposable>(disposables);
		}

		/// <summary>
		/// Adds an <see cref="IDisposable"/>
		/// </summary>
		/// <typeparam name="T">Type of used <see cref="IDisposable"/></typeparam>
		/// <param name="disposable">Added <see cref="IDisposable"/></param>
		/// <returns>Added <see cref="IDisposable"/></returns>
		public T Add<T>(T disposable) where T : IDisposable
		{
			// ReSharper disable once HeapView.PossibleBoxingAllocation
			IDisposable d = disposable;
			if (d == null)
				throw new ArgumentNullException(nameof(disposable));

			lock (_disposeLock)
			{
				if (!_disposable)
					throw new InvalidOperationException();

				_disposables.Add(d);
			}

			return disposable;
		}

		private void Free()
		{
			lock (_disposeLock)
			{
				if (!_disposable)
					return;

				foreach (IDisposable disposable in _disposables)
					disposable?.Dispose();
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
		~MultiDisposable()
		{
			Free();
		}
	}
}
