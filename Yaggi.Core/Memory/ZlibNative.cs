using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Memory
{
	internal static unsafe class ZlibNative
	{
		private const string Library = "zlib1";
		private const CallingConvention Convention = CallingConvention.Cdecl;

		public static readonly bool Supported;
		public static readonly string Version;

		static ZlibNative()
		{
			try
			{
				Version = Marshal.PtrToStringUTF8(GetVersion()) ?? throw new NullReferenceException();
				Supported = true;
			}
			catch
			{
				Supported = false;
			}
		}

		[DllImport(Library, EntryPoint = "crc32", CallingConvention = Convention)]
		public static extern uint Append(uint crc, byte* input, uint length);

		[DllImport(Library, EntryPoint = "zlibVersion", CallingConvention = Convention)]
		private static extern IntPtr GetVersion();
	}
}
