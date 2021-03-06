using System;

namespace Yaggi.Core.Interop
{
	/// <summary>
	/// Provides utility methods for working on strings
	/// </summary>
	public static unsafe class StringUtils
	{
		/// <summary>
		/// Gets the length of a string pointed to by the pointer.
		/// </summary>
		/// <param name="ptr">Pointer pointing to a null terminates string saved with a single byte encoding</param>
		/// <returns>String length</returns>
		public static int Strlen(byte* ptr)
		{
			return new ReadOnlySpan<byte>(ptr, int.MaxValue).IndexOf((byte)0);
		}

		/// <summary>
		/// Gets the length of a string pointed to by the pointer.
		/// </summary>
		/// <param name="ptr">Pointer pointing to a null terminates string saved with a double byte encoding</param>
		/// <returns>String length</returns>
		public static int Strlen(char* ptr)
		{
			return new ReadOnlySpan<char>(ptr, int.MaxValue).IndexOf('\0');
		}
	}
}
