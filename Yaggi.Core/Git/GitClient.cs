using System;

namespace Yaggi.Core.Git
{
	public abstract class GitClient : IDisposable
	{
		public abstract GitRepository InitializeRepository(string path, string branchName);
		public abstract GitRepository CloneRepository(string path, string url);

		protected abstract void Dispose(bool disposing);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GitClient()
		{
			Dispose(false);
		}
	}
}
