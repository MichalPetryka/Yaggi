namespace Yaggi.Core.Git.GitCommandline
{
	public class GitCommandlineRepository : GitRepository
	{
		internal GitCommandlineRepository(string path) : base(path) { }

		protected override void Dispose(bool disposing) { }
	}
}
