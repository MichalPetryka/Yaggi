using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Marshaling
{
	public sealed unsafe class NativeString : IDisposable
	{
		public IntPtr Data { get; }

		private readonly object _disposeLock = new();
		private readonly StringEncoding _encoding;
		private readonly bool _clearOnFree;

		private bool _valid;

		public NativeString(string s, StringEncoding encoding, bool clearOnFree = false)
		{
			_encoding = encoding;
			Data = encoding switch
			{
				StringEncoding.ANSI => Marshal.StringToCoTaskMemAnsi(s),
				StringEncoding.Unicode => Marshal.StringToCoTaskMemUni(s),
				StringEncoding.UTF8 => Marshal.StringToCoTaskMemUTF8(s),
				_ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null)
			};

			_valid = Data != IntPtr.Zero;
			_clearOnFree = clearOnFree;
		}

		private void Free()
		{
			lock (_disposeLock)
			{
				if (!_valid)
					return;

				if (_clearOnFree)
				{
					switch (_encoding)
					{
						case StringEncoding.Unicode:
							new Span<char>(Data.ToPointer(), StringUtils.Strlen((char*)Data)).Clear();
							break;
						case StringEncoding.ANSI:
						case StringEncoding.UTF8:
							new Span<byte>(Data.ToPointer(), StringUtils.Strlen((byte*)Data)).Clear();
							break;
					}
				}

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
