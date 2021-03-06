using System;

namespace Yaggi.Core.Marshaling
{
	public static unsafe class StringUtils
	{
		public static int Strlen(byte* ptr)
		{
			return new ReadOnlySpan<byte>(ptr, int.MaxValue).IndexOf((byte)0);
		}

		public static int Strlen(char* ptr)
		{
			return new ReadOnlySpan<char>(ptr, int.MaxValue).IndexOf('\0');
		}
	}
}
