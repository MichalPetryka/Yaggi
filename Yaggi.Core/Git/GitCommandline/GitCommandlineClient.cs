using System.IO;
using System.Text;

namespace Yaggi.Core.Git.GitCommandline
{
	public class GitCommandlineClient : GitClient
	{
		public override GitRepository InitializeRepository(string path, string branchName)
		{
			Directory.CreateDirectory(path);
			CommandlineUtils.CreateProcess("git", $"init --initial-branch=\"{branchName}\"", Encoding.UTF8, path);
			return new GitCommandlineRepository(path);
		}

		public override GitRepository CloneRepository(string path, string url)
		{
			Directory.CreateDirectory(path);
			CommandlineUtils.CreateProcess("git", $"clone -- \"{url}\" \"{path}\"", Encoding.UTF8, path);
			return new GitCommandlineRepository(path);
		}

		protected override void Dispose(bool disposing) { }
	}
}
