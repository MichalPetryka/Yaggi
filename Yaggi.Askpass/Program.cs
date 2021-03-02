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
			// GIT/SSH always invoke with one parameter containing the prompt
			if (args.Length < 2)
				return 1;

			string pipeName = Environment.GetEnvironmentVariable("YAGGI_PIPE");
			if (string.IsNullOrEmpty(pipeName))
				return 2;
			string k = Environment.GetEnvironmentVariable("YAGGI_KEY");
			if (string.IsNullOrEmpty(pipeName))
				return 4;

			byte[] key = DecryptKey(k, pipeName);

			try
			{
				using (Mutex mutex = new(false, pipeName))
				{
					mutex.WaitOne();
					try
					{
						using (NamedPipeClientStream pipe = new(".", pipeName, PipeDirection.InOut))
						{
							return ProcessPipe(pipe, args[1], key);
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

		private static byte[] DecryptKey(string k, string pipeName)
		{
			byte[] keyEncrypted = k.FromHex();
			Span<byte> keyKey = stackalloc byte[32];
			try
			{
				ulong time = ((ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds() & ~0b111UL) >> 3;
				Span<ulong> keyKeyUlongs = MemoryMarshal.Cast<byte, ulong>(keyKey.Slice(0, sizeof(ulong) * 2));
				keyKeyUlongs[0] = time;
				keyKeyUlongs[1] = time;
				Span<uint> keyKeyUInts = MemoryMarshal.Cast<byte, uint>(keyKey.Slice(sizeof(ulong) * 2, sizeof(uint) * 4));
				keyKeyUInts[0] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes("YAGGI ASKPASS DIALOG"));
				keyKeyUInts[1] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
				keyKeyUInts[2] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory));
				keyKeyUInts[3] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(pipeName));

				return AesGcmHelper.Decrypt(keyEncrypted, keyKey);
			}
			finally
			{
				keyKey.Clear();
				keyEncrypted.AsSpan().Clear();
			}
		}

		private static int ProcessPipe(NamedPipeClientStream pipe, string prompt, byte[] key)
		{
			pipe.Connect();

			Span<byte> intBuffer = stackalloc byte[sizeof(int)];
			byte[] promptBytes = Encoding.UTF8.GetBytes(prompt);
			int l = promptBytes.Length;
			MemoryMarshal.Write(intBuffer, ref l);
			pipe.Write(intBuffer);
			pipe.Write(promptBytes);
			pipe.Read(intBuffer);
			int length = MemoryMarshal.Read<int>(intBuffer);
			// negative length means that the dialog was cancelled
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
}
