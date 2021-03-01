using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Memory
{
	internal static unsafe class ZlibNative
	{
		private static readonly IntPtr Module;

		public static readonly bool Supported;
		public static readonly string Version;
		public static readonly delegate* unmanaged[Cdecl]<uint, byte*, uint, uint> Append;

		static ZlibNative()
		{
			try
			{
				try
				{
					Module = NativeLibrary.Load("zlib1");
				}
				catch
				{
					Module = NativeLibrary.Load("zlib");
				}

				if (Module == IntPtr.Zero)
					throw new PlatformNotSupportedException();

				delegate* unmanaged[Cdecl]<IntPtr> export = (delegate* unmanaged[Cdecl]<IntPtr>)NativeLibrary.GetExport(Module, "zlibVersion");

				if (export == null)
					throw new PlatformNotSupportedException();

				Version = Marshal.PtrToStringUTF8(export()) ?? throw new NullReferenceException();

				Append = (delegate* unmanaged[Cdecl]<uint, byte*, uint, uint>)NativeLibrary.GetExport(Module, "crc32");

				if (Append == null)
					throw new PlatformNotSupportedException();

				Supported = true;
			}
			catch
			{
				Supported = false;
			}
		}
	}
}
