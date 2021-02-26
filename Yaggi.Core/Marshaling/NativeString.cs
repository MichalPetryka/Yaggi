using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Marshaling
{
	public sealed class NativeString : IDisposable
	{
		public IntPtr Data { get; }

		private readonly object _disposeLock = new();
		private bool _valid;

		public NativeString(string s, StringEncoding encoding)
		{
			Data = encoding switch
			{
				StringEncoding.ANSI => Marshal.StringToCoTaskMemAnsi(s),
				StringEncoding.Unicode => Marshal.StringToCoTaskMemUni(s),
				StringEncoding.UTF8 => Marshal.StringToCoTaskMemUTF8(s),
				_ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null)
			};

			_valid = Data != IntPtr.Zero;
		}

		private void Free()
		{
			lock (_disposeLock)
			{
				if (_valid)
					Marshal.FreeCoTaskMem(Data);
				_valid = false;
			}
		}

		public void Dispose()
		{
			Free();
			GC.SuppressFinalize(this);
		}

		~NativeString()
		{
			Free();
		}
	}
}
