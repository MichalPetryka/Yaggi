using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Yaggi.Core.Cryptography;
using Yaggi.Core.Cryptography.Crc32C;
using Yaggi.Core.IO;
using Yaggi.Core.Memory;

namespace Yaggi.Core.Git.GitCommandline
{
	public class GitCommandlineClient : GitClient
	{
		private static readonly Regex ProgressRegex = new(@"(?<=\()([0-9]+)\/([0-9]+)(?=\))",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		public override GitRepository InitializeRepository(string path, string branchName)
		{
			Directory.CreateDirectory(path);
			path = PathUtils.NormalizeDirectoryPath(path);
			CommandlineUtils.CreateProcess("git",
				$"init --initial-branch={CommandlineUtils.EscapeArgument(branchName)}", Encoding.UTF8, path);
			return new GitCommandlineRepository(path);
		}

		public override GitRepository CloneRepository(string path, string url, Action<string, double> progress = null, AuthenticationProviderCallback authenticationProvider = null)
		{
			Directory.CreateDirectory(path);
			path = PathUtils.NormalizeDirectoryPath(path);
			object pipeLock = new();
			NamedPipeServerStream pipe = null;
			byte[] key = null;
			string pipeName = null;
			try
			{
				if (authenticationProvider != null)
				{
					key = new byte[32];
					RandomNumberGenerator.Fill(key);
					Span<byte> pipeId = stackalloc byte[32];
					RandomNumberGenerator.Fill(pipeId);
					pipeName = $"yaggi-{pipeId.ToHex()}";
					pipeId.Clear();

					pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
					// ReSharper disable AccessToDisposedClosure
					// ReSharper disable AccessToModifiedClosure
					pipe.BeginWaitForConnection(ar =>
					{
						lock (pipeLock)
						{
							if (pipe == null)
								return;
							try
							{
								ProcessPipeConnection(authenticationProvider, pipe, key);
							}
							finally
							{
								pipe.EndWaitForConnection(ar);
							}
						}
					}, null);
					// ReSharper restore AccessToDisposedClosure
					// ReSharper restore AccessToModifiedClosure
				}

				StartClone(path, url, progress, pipe, pipeName, key);
				return new GitCommandlineRepository(path);
			}
			finally
			{
				lock (pipeLock)
				{
					pipe?.Dispose();
					key?.AsSpan().Clear();
					pipe = null;
				}
			}
		}

		private static void StartClone(string path, string url, Action<string, double> progress, NamedPipeServerStream pipe, string pipeName, byte[] key)
		{
			const string git = "git";
			string arguments = $"clone{(progress != null ? " --progress" : "")} --verbose -- {CommandlineUtils.EscapeArgument(url)} {CommandlineUtils.EscapeArgument(path)}";
			using (Process process = new()
			{
				StartInfo = new ProcessStartInfo(git, arguments)
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					StandardOutputEncoding = Encoding.UTF8,
					StandardErrorEncoding = Encoding.UTF8,
					WorkingDirectory = path
				}
			})
			{
				SetupEnvironment(pipe, process, pipeName, key);

				object outputLock = new();
				StringBuilder outputBuilder = new();

				object errorLock = new();
				StringBuilder errorBuilder = new();

				process.OutputDataReceived += (_, args) =>
				{
					lock (outputLock)
						outputBuilder.AppendLine(args.Data);
				};

				long lastProgress = 0;
				process.ErrorDataReceived += (_, args) =>
				{
					lock (errorLock)
					{
						errorBuilder.AppendLine(args.Data);
						HandleProgress(progress, args.Data, ref lastProgress);
					}
				};

				process.Start();

				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				process.WaitForExit();

				string output;
				lock (outputLock)
					output = outputBuilder.ToString().Trim();
				string error;
				lock (errorLock)
					error = errorBuilder.ToString().Trim();
				if (process.ExitCode != 0)
					throw new ProcessException($"{git} {arguments}", process.ExitCode, output, error);
			}
		}

		private static void ProcessPipeConnection(AuthenticationProviderCallback authenticationProvider, NamedPipeServerStream pipe, byte[] key)
		{
			Span<byte> intBuffer = stackalloc byte[sizeof(int)];
			pipe.Read(intBuffer);
			int length = MemoryMarshal.Read<int>(intBuffer);
			byte[] buffer = ArrayPool<byte>.Shared.Rent(length);
			Span<byte> data = buffer.AsSpan(0, length);
			string prompt;
			try
			{
				pipe.Read(data);
				prompt = Encoding.UTF8.GetString(data);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer);
			}

			(bool successful, string[] responses) = authenticationProvider(null, (prompt, (string)null, true));
			if (!successful || string.IsNullOrEmpty(responses[0]))
			{
				int e = -1;
				MemoryMarshal.Write(intBuffer, ref e);
				pipe.Write(intBuffer);
				return;
			}

			byte[] bytes = Encoding.UTF8.GetBytes(responses[0]);
			try
			{
				byte[] encrypted = AesGcmHelper.Encrypt(bytes, key);
				try
				{
					int len = encrypted.Length;
					MemoryMarshal.Write(intBuffer, ref len);
					pipe.Write(intBuffer);
					pipe.Write(encrypted);
				}
				finally
				{
					encrypted.AsSpan().Clear();
				}
			}
			finally
			{
				bytes.AsSpan().Clear();
			}
		}

		private static void HandleProgress(Action<string, double> progress, string line, ref long lastProgress)
		{
			// check if line contains progress data
			if (progress == null || string.IsNullOrEmpty(line))
				return;
			int stepEnd = line.LastIndexOf(':');
			if (stepEnd == -1)
				return;
			Match progressData = ProgressRegex.Match(line, stepEnd);
			if (!progressData.Success)
				return;

			//parse current and total
			int progressSplitter = line.IndexOf('/', progressData.Index, progressData.Length);
			if (progressSplitter == -1)
				return;
			bool currentParsed = long.TryParse(line.AsSpan(progressData.Index, progressSplitter - progressData.Index), out long currentProgress);
			if (!currentParsed || currentProgress == lastProgress)
				return;
			bool totalParsed = long.TryParse(line.AsSpan(progressSplitter + 1, progressData.Length + progressData.Index - progressSplitter - 1), out long total);
			if (!totalParsed)
				return;
			lastProgress = currentProgress;

			// call the progress callbacks
			progress(line.Substring(0, stepEnd), (double)currentProgress / total);
		}

		private static void SetupEnvironment(NamedPipeServerStream pipe, Process process, string pipeName, byte[] key)
		{
			if (pipe != null)
			{
				string askpass = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Yaggi.Askpass*").First(s =>
				{
					return Path.GetExtension(s).ToLowerInvariant() switch
					{
						".dll" => false,
						".so" => false,
						".dylib" => false,
						".json" => false,
						".pdb" => false,
						_ => true
					};
				});
				process.StartInfo.Environment["YAGGI_PIPE"] = pipeName;

				Span<byte> keyKey = stackalloc byte[32];
				GeneratePseudokey(pipeName, keyKey);
				byte[] keyEncrypted = AesGcmHelper.Encrypt(key, keyKey);
				process.StartInfo.Environment["YAGGI_KEY"] = keyEncrypted.ToHex();
				keyEncrypted.AsSpan().Clear();
				keyKey.Clear();

				process.StartInfo.Environment["GIT_ASKPASS"] = askpass;
				process.StartInfo.Environment["SSH_ASKPASS"] = askpass;
				process.StartInfo.Environment["SSH_ASKPASS_REQUIRE"] = "force";
				if (!process.StartInfo.Environment.ContainsKey("DISPLAY") || string.IsNullOrEmpty(process.StartInfo.Environment["DISPLAY"]))
					process.StartInfo.Environment["DISPLAY"] = ":0.0";
			}

			process.StartInfo.Environment["GIT_TERMINAL_PROMPT"] = "0";
			process.StartInfo.Environment["GIT_FLUSH"] = "1";
			process.StartInfo.Environment["GCM_INTERACTIVE"] = "Always";
			process.StartInfo.Environment["GCM_MODAL_PROMPT"] = "true";
			process.StartInfo.Environment["GCM_VALIDATE"] = "true";
		}

		private static void GeneratePseudokey(string pipeName, Span<byte> keyKey)
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
		}

		protected override void Dispose(bool disposing) { }
	}
}
