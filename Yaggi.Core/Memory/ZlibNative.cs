using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Interop;

namespace Yaggi.Core.Memory
{
	internal static unsafe class ZlibNative
	{
		private static readonly NativeModule Module;

		public static readonly bool Supported;
		public static readonly string Version;
		public static readonly delegate* unmanaged[Cdecl]<uint, byte*, uint, uint> Append;

		static ZlibNative()
		{
			try
			{
				Module = new NativeModule("zlib1", "libzlib", "libz", "libz.so", "zlib");

				delegate* unmanaged[Cdecl]<IntPtr> export = (delegate* unmanaged[Cdecl]<IntPtr>)Module.GetSymbol("zlibVersion");

				if (export == null)
					throw new PlatformNotSupportedException();

				Version = Marshal.PtrToStringUTF8(export()) ?? throw new NullReferenceException();

				Append = (delegate* unmanaged[Cdecl]<uint, byte*, uint, uint>)Module.GetSymbol("crc32");

				if (Append == null)
					throw new PlatformNotSupportedException();

				Supported = true;
			}
			catch
			{
				Module?.Dispose();
				Module = null;
				Version = null;
				Append = null;
				Supported = false;
			}
		}
	}
}
