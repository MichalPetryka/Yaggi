using System;
using System.Globalization;
using System.Text;

namespace Yaggi.Core.Memory
{
	public static class SpanExtensions
	{
		public static string ToHex(this byte[] data) => new ReadOnlySpan<byte>(data).ToHex();
		public static string ToHex(this Span<byte> data) => ((ReadOnlySpan<byte>)data).ToHex();
		public static string ToHex(this ReadOnlySpan<byte> data)
		{
			StringBuilder stringBuilder = new(data.Length * 2);
			for (int i = 0; i < data.Length; i++)
				stringBuilder.Append(data[i].ToString("X2"));

			return stringBuilder.ToString();
		}

		public static byte[] FromHex(this string data) => data.AsSpan().FromHex();
		public static byte[] FromHex(this ReadOnlySpan<char> data)
		{
			byte[] result = new byte[data.Length / 2];
			for (int i = 0; i < result.Length; i++)
			{
				result[i] = byte.Parse(data.Slice(i * 2, 2), NumberStyles.HexNumber);
			}

			return result;
		}
	}
}
