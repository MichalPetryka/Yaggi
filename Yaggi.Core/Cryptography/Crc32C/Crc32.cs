using System;
using System.Buffers;
using System.IO;

namespace Yaggi.Core.Cryptography.Crc32C
{
	/// <summary>
	/// CRC32 hash implementation using the provided polynomial.
	/// CRC32 is NOT a secure cryptographic hash.
	/// </summary>
	public abstract class Crc
	{
		internal const uint Crc32Polynomial = 0xEDB88320;
		internal const uint Crc32CPolynomial = 0x82F63B78;

		/// <summary>
		/// CRC32 Polynomial used in this instance, different polynomial generates different results for the same data
		/// </summary>
		public uint Polynomial { get; }

		/// <summary>
		/// Shared instance using the CRC-32 IEEE polynomial
		/// </summary>
		public static readonly Crc Crc32 = Create(Crc32Polynomial);
		/// <summary>
		/// Shared instance using the CRC-32 Castagnoli polynomial
		/// </summary>
		public static readonly Crc Crc32C = Create(Crc32CPolynomial);

		/// <summary>
		/// Creates a CRC32 implementation using the provided polynomial, uses accelerated implementations for supported polynomials
		/// </summary>
		/// <param name="polynomial">Used polynomial</param>
		/// <returns>Created CRC32 instance</returns>
		public static Crc Create(uint polynomial)
		{
			switch (polynomial)
			{
				case Crc32CPolynomial:
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
				case Crc32Polynomial:
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

		/// <summary>
		/// Sets the polynomial of the instance
		/// </summary>
		/// <param name="polynomial">Used polynomial</param>
		protected Crc(uint polynomial)
		{
			Polynomial = polynomial;
		}

		/// <summary>
		/// Calculates the CRC32 of the provided data
		/// </summary>
		/// <param name="data">Processed data</param>
		/// <returns>Calculated CRC32</returns>
		public virtual uint Calculate(ReadOnlySpan<byte> data)
		{
			return Append(uint.MaxValue, data) ^ uint.MaxValue;
		}

		/// <summary>
		/// Calculates the CRC32 of the provided stream
		/// </summary>
		/// <param name="stream">Processed stream</param>
		/// <param name="bufferSize">Read buffer size</param>
		/// <returns>Calculated CRC32</returns>
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

		/// <summary>
		/// Appends more data to an existing CRC32 hash.
		/// </summary>
		/// <param name="crc">Starting CRC32</param>
		/// <param name="data">Hashed data</param>
		/// <returns>Calculated CRC32</returns>
		protected abstract uint Append(uint crc, ReadOnlySpan<byte> data);
	}
}
