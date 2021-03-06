using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Interop
{
	/// <summary>
	/// Manages the lifetime of a native representation of a string
	/// </summary>
	public sealed unsafe class NativeString : IDisposable
	{
		/// <summary>
		/// Pointer pointing to the native representation of the string, null terminated.
		/// Can be null.
		/// </summary>
		public IntPtr Data { get; }

		private readonly object _disposeLock = new();
		private readonly StringEncoding _encoding;
		private readonly bool _clearOnFree;

		private bool _valid;

		/// <summary>
		/// Creates a native representation of the provided text using the provided encoding
		/// </summary>
		/// <param name="text">Provided text</param>
		/// <param name="encoding">Used encoding</param>
		/// <param name="clearOnFree">Controls whether the native buffer should be zeroed on free, use true for confidential data</param>
		public NativeString(string text, StringEncoding encoding, bool clearOnFree = false)
		{
			_encoding = encoding;
			Data = encoding switch
			{
				StringEncoding.ANSI => Marshal.StringToCoTaskMemAnsi(text),
				StringEncoding.Unicode => Marshal.StringToCoTaskMemUni(text),
				StringEncoding.UTF8 => Marshal.StringToCoTaskMemUTF8(text),
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

		/// <inheritdoc/>
		public void Dispose()
		{
			Free();
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		~NativeString()
		{
			Free();
		}
	}
}
