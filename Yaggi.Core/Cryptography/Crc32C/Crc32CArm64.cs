﻿using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed class Crc32CArm64 : Crc
	{
		public static bool Supported => System.Runtime.Intrinsics.Arm.Crc32.IsSupported && System.Runtime.Intrinsics.Arm.Crc32.Arm64.IsSupported;

		internal Crc32CArm64() : base(0x82F63B78)
		{
			if (!Supported)
				throw new PlatformNotSupportedException();
		}

		protected override uint Append(uint crc, ReadOnlySpan<byte> data)
		{
			int processed = 0;
			if (data.Length > sizeof(ulong))
			{
				processed = data.Length / sizeof(ulong) * sizeof(ulong);
				ReadOnlySpan<ulong> ulongs = MemoryMarshal.Cast<byte, ulong>(data.Slice(0, processed));
				for (int i = 0; i < ulongs.Length; i++)
					crc = System.Runtime.Intrinsics.Arm.Crc32.Arm64.ComputeCrc32C(crc, ulongs[i]);
			}
			else if (data.Length > sizeof(uint))
			{
				processed = data.Length / sizeof(uint) * sizeof(uint);
				ReadOnlySpan<uint> uints = MemoryMarshal.Cast<byte, uint>(data.Slice(0, processed));
				for (int i = 0; i < uints.Length; i++)
					crc = System.Runtime.Intrinsics.Arm.Crc32.ComputeCrc32C(crc, uints[i]);
			}

			for (int i = processed; i < data.Length; i++)
				crc = System.Runtime.Intrinsics.Arm.Crc32.ComputeCrc32C(crc, data[i]);

			return crc;
		}
	}
}
