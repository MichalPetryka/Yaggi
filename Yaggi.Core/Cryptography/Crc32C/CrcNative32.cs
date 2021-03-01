using System.Runtime.InteropServices;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal static unsafe class CrcNative32
	{
		private const string Library = "crc32c32";
		private const CallingConvention Convention = CallingConvention.Cdecl;

		public static readonly bool Supported;
		public static readonly bool HardwareAccelerated;

		static CrcNative32()
		{
			try
			{
				HardwareAccelerated = HardwareAvailable() != 0;
				Supported = true;
			}
			catch
			{
				Supported = false;
			}
		}

		[DllImport(Library, EntryPoint = "crc32c_append_sw", CallingConvention = Convention)]
		public static extern uint AppendSoftware(uint crc, byte* input, nuint length);

		[DllImport(Library, EntryPoint = "crc32c_append_hw", CallingConvention = Convention)]
		public static extern uint AppendHardware(uint crc, byte* input, nuint length);

		[DllImport(Library, EntryPoint = "crc32c_hw_available", CallingConvention = Convention)]
		private static extern int HardwareAvailable();
	}
}
