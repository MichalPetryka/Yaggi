using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Yaggi.Core.Memory;

namespace Yaggi.Core.Interop
{
	/// <summary>
	/// Managed the lifetime of a native module
	/// </summary>
	public sealed class NativeModule : IDisposable
	{
		/// <summary>
		/// Loaded module handle
		/// </summary>
		public IntPtr Module { get; }
		/// <summary>
		/// Loaded module name
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Module usability, false when module was freed.
		/// </summary>
		public bool Valid { get; private set; }

		private readonly object _moduleLock = new();
		private readonly bool _freeOnDispose;

		/// <summary>
		/// Loads the provided library name
		/// </summary>
		/// <param name="name">Library name</param>
		/// <param name="freeOnDispose">Whether the library should be freed on dispose</param>
		public NativeModule(string name, bool freeOnDispose = true)
		{
			lock (_moduleLock)
			{
				switch (name)
				{
					case null:
						throw new ArgumentNullException(nameof(name));
					case "":
						throw new ArgumentException("Module name can't be empty", nameof(name));
				}

				Module = NativeLibrary.Load(name);
				if (Module == IntPtr.Zero)
					throw new DllNotFoundException();
				Name = name;
				Valid = Module != IntPtr.Zero;
				_freeOnDispose = freeOnDispose;
			}
		}

		/// <summary>
		/// Loads any of provided library names
		/// </summary>
		/// <param name="names">Library names</param>
		/// <exception cref="AggregateException">Thrown when none of names could be loaded</exception>
		public NativeModule(params string[] names) : this(true, names) { }

		/// <summary>
		/// Loads any of provided library names
		/// </summary>
		/// <param name="freeOnDispose">Whether the library should be freed on dispose</param>
		/// <param name="names">Library names</param>
		/// <exception cref="AggregateException">Thrown when none of names could be loaded</exception>
		public NativeModule(bool freeOnDispose = true, params string[] names)
		{
			lock (_moduleLock)
			{
				if (names == null)
					throw new ArgumentNullException(nameof(names));
				if (names.Length == 0)
					throw new ArgumentOutOfRangeException(nameof(names));

				List<Exception> exceptions = ListPool<Exception>.Rent(names.Length);

				foreach (string name in names)
				{
					try
					{
						Module = NativeLibrary.Load(name);
						Name = name;
						break;
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
				}

				if (Module == IntPtr.Zero)
					throw new AggregateException(exceptions);
				ListPool<Exception>.Return(exceptions);

				Valid = Module != IntPtr.Zero;
				_freeOnDispose = freeOnDispose;
			}
		}

		/// <summary>
		/// Returns a module export
		/// </summary>
		/// <param name="symbol">Export name</param>
		/// <returns>Exported pointer</returns>
		public IntPtr GetSymbol(string symbol)
		{
			if (!Valid)
				throw new ObjectDisposedException(nameof(NativeModule));
			return NativeLibrary.GetExport(Module, symbol);
		}

		/// <summary>
		/// Creates a delegate to a module export
		/// </summary>
		/// <typeparam name="T">Delegate type</typeparam>
		/// <param name="symbol">Export name</param>
		/// <returns>Created delegate</returns>
		public T GetDelegate<T>(string symbol) where T : Delegate
		{
			if (!Valid)
				throw new ObjectDisposedException(nameof(NativeModule));
			return Marshal.GetDelegateForFunctionPointer<T>(NativeLibrary.GetExport(Module, symbol));
		}

		/// <summary>
		/// Creates a delegate to a module export
		/// </summary>
		/// <param name="symbol">Export name</param>
		/// <param name="type">Delegate type</param>
		/// <returns>Created delegate</returns>
		public object GetDelegate(string symbol, Type type)
		{
			if (!Valid)
				throw new ObjectDisposedException(nameof(NativeModule));
			return Marshal.GetDelegateForFunctionPointer(NativeLibrary.GetExport(Module, symbol), type);
		}

		private void Free()
		{
			lock (_moduleLock)
			{
				if (!Valid || !_freeOnDispose)
					return;
				NativeLibrary.Free(Module);
				Valid = false;
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Free();
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		~NativeModule()
		{
			Free();
		}
	}
}
