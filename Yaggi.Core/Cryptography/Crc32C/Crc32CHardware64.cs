using System;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed unsafe class Crc32CHardware64 : Crc
	{
		public static bool Supported => CrcNative64.Supported && CrcNative64.HardwareAccelerated;

		internal Crc32CHardware64() : base(0x82F63B78)
		{
			if (!Supported)
				throw new PlatformNotSupportedException();
		}

		protected override uint Append(uint crc, ReadOnlySpan<byte> data)
		{
			fixed (byte* bytes = data)
				return CrcNative64.AppendHardware(crc ^ uint.MaxValue, bytes, (nuint)data.Length) ^ uint.MaxValue;
		}
	}
}
