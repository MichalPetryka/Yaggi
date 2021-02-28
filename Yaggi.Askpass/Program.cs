using System;
using System.Buffers;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Yaggi.Core.Cryptography;
using Yaggi.Core.Memory;

namespace Yaggi.Askpass
{
	internal class Program
	{
		private static int Main()
		{
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length < 2)
				return 1;
			string pipeName = Environment.GetEnvironmentVariable("YAGGI_PIPE");
			if (string.IsNullOrEmpty(pipeName))
				return 2;
			using (Mutex mutex = new(false, pipeName))
			{
				mutex.WaitOne();
				try
				{
					using (NamedPipeClientStream pipe = new(".", pipeName, PipeDirection.InOut))
					{
						pipe.Connect();
						Span<byte> intBuffer = stackalloc byte[sizeof(int)];
						byte[] prompt = Encoding.UTF8.GetBytes(args[1]);
						int l = prompt.Length;
						MemoryMarshal.Write(intBuffer, ref l);
						pipe.Write(intBuffer);
						pipe.Write(prompt);
						pipe.Read(intBuffer);
						int length = MemoryMarshal.Read<int>(intBuffer);
						if (length < 0)
							return 3;
						byte[] buffer = ArrayPool<byte>.Shared.Rent(length);
						Span<byte> data = buffer.AsSpan(0, length);
						try
						{
							pipe.Read(data);
							string k = Environment.GetEnvironmentVariable("YAGGI_KEY");
							if (string.IsNullOrEmpty(pipeName))
								return 4;
							byte[] key = k.FromHex();
							try
							{
								byte[] result = AesGcmHelper.Decrypt(data, key);
								try
								{
									Console.Write(Encoding.UTF8.GetString(result));
									return 0;
								}
								finally
								{
									result.AsSpan().Clear();
								}
							}
							finally
							{
								key.AsSpan().Clear();
							}
						}
						finally
						{
							data.Clear();
							ArrayPool<byte>.Shared.Return(buffer);
						}
					}
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}
	}
}
