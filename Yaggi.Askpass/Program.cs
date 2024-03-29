﻿using System;
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
	internal static class Program
	{
		private enum ExitCode
		{
			Ok = 0,
			NoParameters = 1,
			NoPipe = 2,
			NoKey = 3,
			NoSalt = 4,
			PromptCancelled = 5
		}

		private static int Main(string[] args)
		{
#if !DEBUG
			ProcessDescriptors.SecureProcess();
#endif
			// GIT/SSH always invoke with one parameter containing the prompt
			if (args.Length < 1)
				return (int)ExitCode.NoParameters;

			string pipeName = Environment.GetEnvironmentVariable("YAGGI_PIPE");
			if (string.IsNullOrEmpty(pipeName))
				return (int)ExitCode.NoPipe;

			string key = Environment.GetEnvironmentVariable("YAGGI_KEY");
			if (string.IsNullOrEmpty(pipeName))
				return (int)ExitCode.NoKey;

			string saltText = Environment.GetEnvironmentVariable("YAGGI_SALT");
			// ReSharper disable once ConvertIfStatementToReturnStatement
			if (string.IsNullOrEmpty(saltText) || !ulong.TryParse(saltText, out ulong salt))
				return (int)ExitCode.NoSalt;

			return (int)SendRequest(key, pipeName, args[0], salt);
		}

		private static ExitCode SendRequest(string k, string pipeName, string prompt, ulong salt)
		{
			byte[] key = DecryptKey(k, pipeName, salt);

			try
			{
				using (Mutex mutex = new(false, pipeName))
				{
					mutex.WaitOne();
					try
					{
						using (NamedPipeClientStream pipe = new(".", pipeName, PipeDirection.InOut))
						{
							return ProcessPipe(pipe, prompt, key);
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

		private static byte[] DecryptKey(string k, string pipeName, ulong salt)
		{
			byte[] keyEncrypted = k.FromHex();
			Span<byte> keyKey = stackalloc byte[32];
			try
			{
				GeneratePseudokey(pipeName, salt, keyKey);
				return AesGcmHelper.Decrypt(keyEncrypted, keyKey);
			}
			finally
			{
				keyKey.Clear();
				keyEncrypted.AsSpan().Clear();
			}
		}

		private static void GeneratePseudokey(string pipeName, ulong salt, Span<byte> keyKey)
		{
			// keep a 7 second window in case process start takes a lot of time, shift out 0 bits
			ulong time = ((ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds() & ~0b111UL) >> 3;
			Span<ulong> keyKeyUlongs = MemoryMarshal.Cast<byte, ulong>(keyKey.Slice(0, sizeof(ulong) * 2));
			keyKeyUlongs[0] = time;
			keyKeyUlongs[1] = salt;

			Span<uint> keyKeyUInts = MemoryMarshal.Cast<byte, uint>(keyKey.Slice(sizeof(ulong) * 2, sizeof(uint) * 4));
			keyKeyUInts[0] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes("YAGGI ASKPASS DIALOG"));
			keyKeyUInts[1] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));
			keyKeyUInts[2] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory));
			keyKeyUInts[3] = Crc.Crc32C.Calculate(Encoding.UTF8.GetBytes(pipeName));
		}

		private static ExitCode ProcessPipe(NamedPipeClientStream pipe, string prompt, byte[] key)
		{
			pipe.Connect();

			Span<byte> intBuffer = stackalloc byte[sizeof(int)];
			byte[] promptBytes = Encoding.UTF8.GetBytes(prompt);
			int promptBytesLength = promptBytes.Length;
			MemoryMarshal.Write(intBuffer, ref promptBytesLength);
			pipe.Write(intBuffer);
			pipe.Write(promptBytes);

			pipe.Read(intBuffer);
			int length = MemoryMarshal.Read<int>(intBuffer);
			// negative length means that the dialog was cancelled
			if (length < 0)
				return ExitCode.PromptCancelled;

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
						return ExitCode.Ok;
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
