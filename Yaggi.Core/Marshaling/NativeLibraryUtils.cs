using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Yaggi.Core.Memory;

namespace Yaggi.Core.Marshaling
{
	public static class NativeLibraryUtils
	{
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
