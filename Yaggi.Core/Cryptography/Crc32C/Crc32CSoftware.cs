using System;

namespace Yaggi.Core.Cryptography.Crc32C
{
	internal sealed unsafe class Crc32CSoftware : Crc
	{
		public static bool Supported => Crc32CNative.Supported;

		internal Crc32CSoftware() : base(Crc32CPolynomial)
		{
			if (!Supported)
				throw new PlatformNotSupportedException();
		}

		protected override uint Append(uint crc, ReadOnlySpan<byte> data)
		{
			fixed (byte* bytes = data)
				return Crc32CNative.AppendSoftware(crc ^ uint.MaxValue, bytes, (nuint)data.Length) ^ uint.MaxValue;
		}
	}
}
