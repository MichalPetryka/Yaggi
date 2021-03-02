using System;
using System.Buffers;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Yaggi.Core.Cryptography;
using Yaggi.Core.Cryptography.Crc32C;
using Yaggi.Core.Memory;
#if !DEBUG
using Yaggi.Core.Security;
#endif

namespace Yaggi.Askpass
{
	internal class Program
	{
		private static int Main()
		{
#if !DEBUG
			ProcessDescriptors.SecureProcess();
#endif
			string[] args = Environment.GetCommandLineArgs();
			if (args.Length < 2)
				return 1;
			string pipeName = Environment.GetEnvironmentVariable("YAGGI_PIPE");
			if (string.IsNullOrEmpty(pipeName))
				return 2;
			string k = Environment.GetEnvironmentVariable("YAGGI_KEY");
			if (string.IsNullOrEmpty(pipeName))
				return 4;
			byte[] key;
			byte[] keyEncrypted = k.FromHex();
			try
			{
				byte[] keyKey = new byte[32];
				ulong time = ((ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds() & ~0b111UL) >> 3;
				Span<ulong> keyKeyData = MemoryMarshal.Cast<byte, ulong>(keyKey);
				keyKeyData[0] = time;
				keyKeyData[1] = time;
				keyKeyData[2] = ((ulong)Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes("YAGGI ASKPASS DIALOG")) << 32)
							| Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(
									Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
				keyKeyData[3] = ((ulong)Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory)) << 32) |
								Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(pipeName));
				key = AesGcmHelper.Decrypt(keyEncrypted, keyKey);
				keyKey.AsSpan().Clear();
			}
			finally
			{
				keyEncrypted.AsSpan().Clear();
			}

			try
			{
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
			finally
			{
				key.AsSpan().Clear();
			}
		}
	}
}
