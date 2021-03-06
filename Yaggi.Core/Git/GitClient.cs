using System;

namespace Yaggi.Core.Git
{
	public abstract class GitClient : IDisposable
	{
		public delegate (bool successful, string[] responses) AuthenticationProviderCallback(string title, params (string prompt, string defaultValue, bool confidential)[] inputs);

		public abstract GitRepository InitializeRepository(string path, string branchName);
		public abstract GitRepository CloneRepository(string path, string url, Action<string, double> progress = null, AuthenticationProviderCallback authenticationProvider = null);

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
