using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Yaggi.Core.IO;

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
			long lastProgress = 0;
			CommandlineUtils.CreateProcess("git",
				$"clone{(progress != null ? " --progress" : "")} -- {CommandlineUtils.EscapeArgument(url)} {CommandlineUtils.EscapeArgument(path)}",
				Encoding.UTF8, path,
				(output, process) =>
				{
					if (string.IsNullOrEmpty(output))
						return;

					if (output.StartsWith("Username for", StringComparison.OrdinalIgnoreCase))
					{
						if (authenticationProvider == null)
							throw new InvalidOperationException("Authentication provider required");
						(bool, string[]) input = authenticationProvider("Provide your authentication data", new[] { ("Username", (string)null, false) });
						if (!input.Item1)
							throw new InvalidOperationException("User cancelled the operation");
						process.StandardInput.WriteLine(input.Item2[0]);
						return;
					}

					if (output.StartsWith("Password for", StringComparison.OrdinalIgnoreCase))
					{
						if (authenticationProvider == null)
							throw new InvalidOperationException("Authentication provider required");
						(bool, string[]) input = authenticationProvider("Provide your authentication data", new[] { ("Password", (string)null, true) });
						if (!input.Item1)
							throw new InvalidOperationException("User cancelled the operation");
						process.StandardInput.WriteLine(input.Item2[0]);
						return;
					}

					if (output.StartsWith("Enter passphrase for key", StringComparison.OrdinalIgnoreCase))
					{
						if (authenticationProvider == null)
							throw new InvalidOperationException("Authentication provider required");
						(bool, string[]) input = authenticationProvider("Provide your authentication data", new[] { ("Passpharase", (string)null, true) });
						if (!input.Item1)
							throw new InvalidOperationException("User cancelled the operation");
						process.StandardInput.WriteLine(input.Item2[0]);
					}
				}, progress == null ? null : (line, _) =>
				{
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
				});
			return new GitCommandlineRepository(path);
		}

		protected override void Dispose(bool disposing) { }
	}
}
