using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Yaggi.Core.Memory;

namespace Yaggi.Core.Marshaling
{
	/// <summary>
	/// Provides utility methods for working with native libraries
	/// </summary>
	public static class NativeLibraryUtils
	{
		/// <summary>
		/// Loads any of provided library names
		/// </summary>
		/// <param name="names">Library names</param>
		/// <returns>Pointer to loaded module</returns>
		/// <exception cref="AggregateException">Thrown when none of names could be loaded</exception>
		public static IntPtr LoadAny(params string[] names)
		{
			if (names == null)
				throw new ArgumentNullException(nameof(names));
			if (names.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(names));

			IntPtr module = IntPtr.Zero;
			List<Exception> exceptions = ListPool<Exception>.Rent(names.Length);

			foreach (string name in names)
			{
				try
				{
					module = NativeLibrary.Load(name);
					break;
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
			}

			if (module == IntPtr.Zero)
				throw new AggregateException(exceptions);
			ListPool<Exception>.Return(exceptions);

			return module;
		}
	}
}
