using System;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed unsafe class Crc32CHardware : Crc
	{
		public static bool Supported => Crc32CNative.Supported && Crc32CNative.HardwareAccelerated;

		internal Crc32CHardware() : base(0x82F63B78)
		{
			if (!Supported)
				throw new PlatformNotSupportedException();
		}

		protected override uint Append(uint crc, ReadOnlySpan<byte> data)
		{
			fixed (byte* bytes = data)
				return Crc32CNative.AppendHardware(crc ^ uint.MaxValue, bytes, (nuint)data.Length) ^ uint.MaxValue;
		}
	}
}
