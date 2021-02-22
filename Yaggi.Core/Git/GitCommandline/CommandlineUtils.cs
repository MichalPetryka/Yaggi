using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

// ReSharper disable HeapView.BoxingAllocation

namespace Yaggi.Core.Git.GitCommandline
{
	/// <summary>
	/// Utility class for launching console programs
	/// </summary>
	public static class CommandlineUtils
	{
		private static readonly object ConsoleEncodingLock = new();
		private static Encoding _encodingCache;

		public static Encoding SystemConsoleEncoding
		{
			get
			{
				lock (ConsoleEncodingLock)
				{
					if (_encodingCache != null)
						return _encodingCache;

					if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					{
						try
						{
							// ReSharper disable AssignNullToNotNullAttribute
							// ReSharper disable PossibleNullReferenceException
							int cp;
							using (Process process = Process.Start(new ProcessStartInfo("chcp")
							{
								UseShellExecute = false,
								CreateNoWindow = true,
								RedirectStandardOutput = true
							}))
							{
								using (StreamReader standardOutput = process.StandardOutput)
									// it doesn't even return just the cp, also some text
									cp = int.Parse(string.Concat(standardOutput.ReadLine().Where(char.IsDigit)));
							}

							if (cp == 0)
								throw new InvalidOperationException();
							_encodingCache = Encoding.GetEncoding(cp);
							return _encodingCache;
							// ReSharper restore PossibleNullReferenceException
							// ReSharper restore AssignNullToNotNullAttribute
						}
						catch (Exception ex)
						{
							// TODO: handle
						}

						try
						{
							uint codePage = GetConsoleCodePage();
							if (codePage == 0)
								throw new Win32Exception();
							_encodingCache = Encoding.GetEncoding((int)codePage);
							return _encodingCache;
						}
						catch (Exception ex)
						{
							// TODO: handle
						}
					}
					else
						try
						{
							// ReSharper disable once AssignNullToNotNullAttribute
							_encodingCache = Encoding.GetEncoding(Environment.GetEnvironmentVariable("LC_CTYPE"));
							return _encodingCache;
						}
						catch (Exception ex)
						{
							// TODO: handle
						}

					_encodingCache = Console.OutputEncoding;
					return _encodingCache;
				}
			}
		}

		[DllImport("Kernel32", EntryPoint = "GetConsoleOutputCP", SetLastError = true)]
		private static extern uint GetConsoleCodePage();

		/// <summary>
		/// Creates a process and waits for it to exit
		/// </summary>
		/// <param name="fileName">Executable name</param>
		/// <param name="arguments">Arguments</param>
		/// <param name="encoding">Encoding used to read process output</param>
		/// <param name="workingDirectory">Working directory, current if null</param>
		/// <returns>Process exit code</returns>
		public static void CreateProcess(string fileName, string arguments, Encoding encoding, string workingDirectory = null)
		{
			CreateProcess(fileName, arguments, encoding, out _, out _, workingDirectory);
		}

		/// <summary>
		/// Creates a process and waits for it to exit
		/// </summary>
		/// <param name="fileName">Executable name</param>
		/// <param name="arguments">Arguments</param>
		/// <param name="encoding">Encoding used to read process output</param>
		/// <param name="output">Process output</param>
		/// <param name="error">Process error</param>
		/// <param name="workingDirectory">Working directory, current if null</param>
		/// <returns>Process exit code</returns>
		public static void CreateProcess(string fileName, string arguments, Encoding encoding, out string output, out string error, string workingDirectory = null)
		{
			using (Process process = new()
			{
				StartInfo = new ProcessStartInfo(fileName, arguments)
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					StandardOutputEncoding = encoding,
					StandardErrorEncoding = encoding,
					WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory()
				}
			})
			{
				object outputLock = new();
				StringBuilder outputBuilder = new();

				object errorLock = new();
				StringBuilder errorBuilder = new();

				process.OutputDataReceived += (_, args) =>
				{
					lock (outputLock)
						outputBuilder.AppendLine(args.Data);
				};

				process.ErrorDataReceived += (_, args) =>
				{
					lock (errorLock)
						errorBuilder.AppendLine(args.Data);
				};

				process.Start();

				process.BeginOutputReadLine();
				process.BeginErrorReadLine();

				process.WaitForExit();

				lock (outputLock)
					output = outputBuilder.ToString().Trim();
				lock (errorLock)
					error = errorBuilder.ToString().Trim();
				if (process.ExitCode != 0)
					throw new ProcessException($"{fileName} {arguments}", process.ExitCode, output, error);
			}
		}
	}
}
