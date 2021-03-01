using System;
using Yaggi.Core.Memory;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed unsafe class Crc32Zlib : Crc
	{
		public static bool Supported => ZlibNative.Supported;

		internal Crc32Zlib() : base(0xEDB88320)
		{
			if (!Supported)
				throw new PlatformNotSupportedException();
		}

		protected override uint Append(uint crc, ReadOnlySpan<byte> data)
		{
			fixed (byte* bytes = data)
				return ZlibNative.Append(crc ^ uint.MaxValue, bytes, (uint)data.Length) ^ uint.MaxValue;
		}
	}
}
