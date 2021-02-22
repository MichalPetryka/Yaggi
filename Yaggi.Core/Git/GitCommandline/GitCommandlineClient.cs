using System.IO;

namespace Yaggi.Core.Git.GitCommandline
{
	public class GitCommandlineClient : GitClient
	{
		public override GitRepository InitializeRepository(string path, string branchName)
		{
			Directory.CreateDirectory(path);
			CommandlineUtils.CreateProcess("git", $"init --initial-branch=\"{branchName}\"", path);
			return new GitCommandlineRepository(path);
		}

		protected override void Dispose(bool disposing) { }
	}
}
