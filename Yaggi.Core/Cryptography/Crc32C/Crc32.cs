using System;
using System.Buffers;
using System.IO;

namespace Yaggi.Core.Cryptography.Crc32C
{
	public abstract class Crc
	{
		public uint Polynomial { get; }

		public static readonly Crc Crc32 = Create(0xEDB88320);
		public static readonly Crc Crc32C = Create(0x82F63B78);

		public static Crc Create(uint polynomial)
		{
			switch (polynomial)
			{
				case 0x82F63B78:
					if (Crc32CHardware.Supported)
						return new Crc32CHardware();

					if (Crc32CSse64.Supported)
						return new Crc32CSse64();

					if (Crc32CSse32.Supported)
						return new Crc32CSse32();

					if (Crc32CArm64.Supported)
						return new Crc32CArm64();

					if (Crc32CArm32.Supported)
						return new Crc32CArm32();

					if (Crc32CSoftware.Supported)
						return new Crc32CSoftware();
					break;
				case 0xEDB88320:
					if (Crc32Zlib.Supported)
						return new Crc32Zlib();

					if (Crc32Arm64.Supported)
						return new Crc32Arm64();

					if (Crc32Arm32.Supported)
						return new Crc32Arm32();
					break;
			}

			return new CrcManaged(polynomial);
		}

		protected Crc(uint polynomial)
		{
			Polynomial = polynomial;
		}

		public virtual uint Calculate(ReadOnlySpan<byte> data)
		{
			return Append(uint.MaxValue, data) ^ uint.MaxValue;
		}

		public virtual uint Calculate(Stream stream, int bufferSize = 4096)
		{
			byte[] buffer = null;
			try
			{
				buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
				// allign to ulongs for better speed
				Span<byte> data = buffer.AsSpan(0, buffer.Length / sizeof(ulong) * sizeof(ulong));
				uint crc = uint.MaxValue;
				int count;
				while ((count = stream.Read(data)) != 0)
					crc = Append(crc, data.Slice(0, count));

				return crc ^ uint.MaxValue;
			}
			finally
			{
				if (buffer != null)
					ArrayPool<byte>.Shared.Return(buffer);
			}
		}

		protected abstract uint Append(uint crc, ReadOnlySpan<byte> data);
	}
}
