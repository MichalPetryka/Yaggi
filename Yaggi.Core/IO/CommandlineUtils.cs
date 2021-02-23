using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Yaggi.Core.Git.GitCommandline;
using Yaggi.Core.Memory;

// ReSharper disable HeapView.BoxingAllocation

namespace Yaggi.Core.IO
{
	/// <summary>
	/// Utility class for launching console programs
	/// </summary>
	public static class CommandlineUtils
	{
		private const char Quote = '\"';
		private const char Backslash = '\\';

		/// <summary>
		/// Creates a process and waits for it to exit
		/// </summary>
		/// <param name="fileName">Executable name</param>
		/// <param name="arguments">Arguments</param>
		/// <param name="encoding">Encoding used to read process output</param>
		/// <param name="workingDirectory">Working directory, current if null</param>
		/// <param name="outData">Process output callback</param>
		/// <param name="errorData">Process error callback</param>
		/// <returns>Process exit code</returns>
		public static void CreateProcess(string fileName, string arguments, Encoding encoding, string workingDirectory = null, Action<string, Process> outData = null, Action<string, Process> errorData = null)
		{
			CreateProcess(fileName, arguments, encoding, out _, out _, workingDirectory, outData, errorData);
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
		/// <param name="outData">Process output callback</param>
		/// <param name="errorData">Process error callback</param>
		/// <returns>Process exit code</returns>
		public static void CreateProcess(string fileName, string arguments, Encoding encoding, out string output, out string error, string workingDirectory = null, Action<string, Process> outData = null, Action<string, Process> errorData = null)
		{
			using (Process process = new()
			{
				StartInfo = new ProcessStartInfo(fileName, arguments)
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					RedirectStandardInput = true,
					StandardOutputEncoding = encoding,
					StandardErrorEncoding = encoding,
					StandardInputEncoding = encoding,
					WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory()
				}
			})
			{
				object outputLock = new();
				StringBuilder outputBuilder = new();

				object errorLock = new();
				StringBuilder errorBuilder = new();

				// ReSharper disable AccessToDisposedClosure
				process.OutputDataReceived += (_, args) =>
				{
					lock (outputLock)
					{
						outputBuilder.AppendLine(args.Data);
						outData?.Invoke(args.Data, process);
					}
				};

				process.ErrorDataReceived += (_, args) =>
				{
					lock (errorLock)
					{
						errorBuilder.AppendLine(args.Data);
						errorData?.Invoke(args.Data, process);
					}
				};
				// ReSharper restore AccessToDisposedClosure

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

		public static string EscapeArgument(string argument)
		{
			if (string.IsNullOrEmpty(argument))
				return @"""";

			if (!ContainsWhitespaceOrQuote(argument))
				return argument;

			StringBuilder sb = StringBuilderPool.Rent(argument.Length * 2);

			sb.Append(Quote);
			int idx = 0;
			while (idx < argument.Length)
			{
				char c = argument[idx++];
				switch (c)
				{
					case Backslash:
						int numBackSlash = 1;
						while (idx < argument.Length && argument[idx] == Backslash)
						{
							idx++;
							numBackSlash++;
						}

						if (idx == argument.Length)
							sb.Append(Backslash, numBackSlash * 2);
						else if (argument[idx] == Quote)
						{
							sb.Append(Backslash, numBackSlash * 2 + 1);
							sb.Append(Quote);
							idx++;
						}
						else
							sb.Append(Backslash, numBackSlash);

						continue;
					case Quote:
						sb.Append(Backslash);
						sb.Append(Quote);
						continue;
					default:
						sb.Append(c);
						break;
				}
			}

			sb.Append(Quote);
			return StringBuilderPool.ToStringReturn(sb);
		}

		private static bool ContainsWhitespaceOrQuote(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (char.IsWhiteSpace(c) || c == Quote)
					return true;
			}

			return false;
		}
	}
}
