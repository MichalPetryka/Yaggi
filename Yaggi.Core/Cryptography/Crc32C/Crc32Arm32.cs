using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed class Crc32Arm32 : Crc
	{
		public static bool Supported => System.Runtime.Intrinsics.Arm.Crc32.IsSupported;

		internal Crc32Arm32() : base(Crc32Polynomial)
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
					crc = System.Runtime.Intrinsics.Arm.Crc32.ComputeCrc32(crc, uints[i]);
			}

			for (int i = processed; i < data.Length; i++)
				crc = System.Runtime.Intrinsics.Arm.Crc32.ComputeCrc32(crc, data[i]);

			return crc;
		}
	}
}
