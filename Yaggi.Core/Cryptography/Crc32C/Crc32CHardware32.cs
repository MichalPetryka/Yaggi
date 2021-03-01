using System;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed unsafe class Crc32CHardware32 : Crc
	{
		public static bool Supported => CrcNative32.Supported && CrcNative32.HardwareAccelerated;

		internal Crc32CHardware32() : base(0x82F63B78)
		{
			if (!Supported)
				throw new PlatformNotSupportedException();
		}

		protected override uint Append(uint crc, ReadOnlySpan<byte> data)
		{
			fixed (byte* bytes = data)
				return CrcNative32.AppendHardware(crc ^ uint.MaxValue, bytes, (nuint)data.Length) ^ uint.MaxValue;
		}
	}
}
