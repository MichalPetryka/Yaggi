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

		public override GitRepository CloneRepository(string path, string url, Action<string, double> progress = null, Func<string, (string, string, bool)[], (bool, string[])> authenticationProvider = null)
		{
			Directory.CreateDirectory(path);
			path = PathUtils.NormalizeDirectoryPath(path);
			object pipeLock = new();
			NamedPipeServerStream pipe = null;
			byte[] key = null;
			byte[] pipeId = null;
			string pipeKey = null;
			string pipeName = null;
			try
			{
				if (authenticationProvider != null)
				{
					key = new byte[32];
					RandomNumberGenerator.Fill(key);
					pipeKey = key.ToHex();
					pipeId = new byte[32];
					RandomNumberGenerator.Fill(pipeId);
					pipeName = $"yaggi-{pipeId.ToHex()}";
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
								(bool, string[]) r = authenticationProvider(null, new[] { (prompt, (string)null, true) });
								if (!r.Item1 || string.IsNullOrEmpty(r.Item2[0]))
								{
									int e = -1;
									MemoryMarshal.Write(intBuffer, ref e);
									pipe.Write(intBuffer);
									return;
								}

								byte[] bytes = Encoding.UTF8.GetBytes(r.Item2[0]);
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
							finally
							{
								pipe.EndWaitForConnection(ar);
							}
						}
					}, null);
					// ReSharper restore AccessToDisposedClosure
					// ReSharper restore AccessToModifiedClosure
				}

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
					if (pipe != null)
					{
						string askpass = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Yaggi.Askpass*").First(
							s =>
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
						process.StartInfo.Environment["YAGGI_KEY"] = pipeKey;
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

					object outputLock = new();
					StringBuilder outputBuilder = new();

					object errorLock = new();
					StringBuilder errorBuilder = new();

					process.OutputDataReceived += (_, args) =>
					{
						lock (outputLock)
						{
							outputBuilder.AppendLine(args.Data);
						}
					};

					long lastProgress = 0;
					process.ErrorDataReceived += (_, args) =>
					{
						lock (errorLock)
						{
							errorBuilder.AppendLine(args.Data);
							if (progress == null)
								return;

							string line = args.Data;

							// check if line contains progress data
							if (string.IsNullOrEmpty(line))
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
							bool currentParsed =
								long.TryParse(line.AsSpan(progressData.Index, progressSplitter - progressData.Index),
									out long currentProgress);
							if (!currentParsed || currentProgress == lastProgress)
								return;
							bool totalParsed =
								long.TryParse(
									line.AsSpan(progressSplitter + 1,
										progressData.Length + progressData.Index - progressSplitter - 1), out long total);
							if (!totalParsed)
								return;
							lastProgress = currentProgress;

							// call the progress callbacks
							progress(line.Substring(0, stepEnd), (double)currentProgress / total);
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
				return new GitCommandlineRepository(path);
			}
			finally
			{
				lock (pipeLock)
				{
					pipe?.Dispose();
					pipeId?.AsSpan().Clear();
					key?.AsSpan().Clear();
					pipe = null;
				}
			}
		}

		protected override void Dispose(bool disposing) { }
	}
}
