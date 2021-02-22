using System;

namespace Yaggi.Core.Git
{
	public abstract class GitClient : IDisposable
	{
		public abstract GitRepository InitializeRepository(string path, string branchName);

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
