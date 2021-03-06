using System;
using System.Globalization;
using System.Text;

namespace Yaggi.Core.Memory
{
	/// <summary>
	/// Extension methods for working on Spans
	/// </summary>
	public static class SpanExtensions
	{
		/// <summary>
		/// Provides a hexadecimal text representation of provided data
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Hexadecimal text</returns>
		public static string ToHex(this byte[] data) => new ReadOnlySpan<byte>(data).ToHex();
		/// <summary>
		/// Provides a hexadecimal text representation of provided data
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Hexadecimal text</returns>
		public static string ToHex(this Span<byte> data) => ((ReadOnlySpan<byte>)data).ToHex();
		/// <summary>
		/// Provides a hexadecimal text representation of provided data
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Hexadecimal text</returns>
		public static string ToHex(this ReadOnlySpan<byte> data)
		{
			StringBuilder stringBuilder = new(data.Length * 2);
			for (int i = 0; i < data.Length; i++)
				stringBuilder.Append(data[i].ToString("X2"));

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Provides a binary representation of provided hexadecimal text
		/// </summary>
		/// <param name="data">Hexadecimal text</param>
		/// <returns>Binary representation</returns>
		public static byte[] FromHex(this string data) => data.AsSpan().FromHex();
		/// <summary>
		/// Provides a binary representation of provided hexadecimal text
		/// </summary>
		/// <param name="data">Hexadecimal text</param>
		/// <returns>Binary representation</returns>
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
