using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed class Crc32CSse32 : Crc
	{
		public static bool Supported => Sse42.IsSupported;

		internal Crc32CSse32() : base(Crc32CPolynomial)
		{
			if (!Supported)
				throw new PlatformNotSupportedException();
		}

		protected override uint Append(uint crc, ReadOnlySpan<byte> data)
		{
			int processed = 0;
			if (data.Length > sizeof(uint))
			{
				processed = data.Length / sizeof(uint) * sizeof(uint);
				ReadOnlySpan<uint> uints = MemoryMarshal.Cast<byte, uint>(data.Slice(0, processed));
				for (int i = 0; i < uints.Length; i++)
					crc = Sse42.Crc32(crc, uints[i]);
			}

			for (int i = processed; i < data.Length; i++)
				crc = Sse42.Crc32(crc, data[i]);

			return crc;
		}
	}
}
