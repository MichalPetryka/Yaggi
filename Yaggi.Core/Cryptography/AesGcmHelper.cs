using System;
using System.Security.Cryptography;

namespace Yaggi.Core.Cryptography
{
	public static class AesGcmHelper
	{
		private const int TagLength = 16;
		private const int NonceLength = 12;

		public static byte[] Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
		{
			byte[] result = new byte[TagLength + NonceLength + data.Length];

			Span<byte> nonce = result.AsSpan(TagLength, NonceLength);
			RandomNumberGenerator.Fill(nonce);

			using (AesGcm aes = new(key))
				aes.Encrypt(nonce, data, result.AsSpan(NonceLength + TagLength), result.AsSpan(0, TagLength));

			return result;
		}

		public static byte[] Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
		{
			ReadOnlySpan<byte> encrypted = data.Slice(NonceLength + TagLength);
			byte[] result = new byte[encrypted.Length];

			using (AesGcm aes = new(key))
				aes.Decrypt(data.Slice(TagLength, NonceLength), encrypted, data.Slice(0, TagLength), result);

			return result;
		}
	}
}
