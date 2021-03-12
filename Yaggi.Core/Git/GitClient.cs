using System;

namespace Yaggi.Core.Git
{
	/// <summary>
	/// Base for Git Client implementations
	/// </summary>
	public abstract class GitClient : IDisposable
	{
		/// <summary>
		/// Callaback for credential requests
		/// </summary>
		/// <param name="title">Request description</param>
		/// <param name="inputs">Requested credentials</param>
		/// <returns>Request result</returns>
		public delegate (bool successful, string[] responses) AuthenticationProviderCallback(string title, params (string prompt, string defaultValue, bool confidential)[] inputs);

		/// <summary>
		/// Initializes a new empty repository
		/// </summary>
		/// <param name="path">Repository path</param>
		/// <param name="branchName">Initial branch name</param>
		/// <returns>Created repository</returns>
		public abstract GitRepository InitializeRepository(string path, string branchName);
		/// <summary>
		/// Clones a remote repository
		/// </summary>
		/// <param name="path">Repository path</param>
		/// <param name="url">Remote location</param>
		/// <param name="progress">Callback for cloning progress</param>
		/// <param name="authenticationProvider">Credential provider</param>
		/// <returns>Cloned repository</returns>
		public abstract GitRepository CloneRepository(string path, string url, Action<string, double> progress = null, AuthenticationProviderCallback authenticationProvider = null);
		/// <summary>
		/// Opens an existing repository
		/// </summary>
		/// <param name="path">Repository path</param>
		/// <returns>Opened repository</returns>
		public abstract GitRepository OpenRepository(string path);

		/// <summary>
		/// Releases resources held by the client
		/// </summary>
		/// <param name="disposing">Whether <see cref="Dispose()"/> was called or the object is finalized</param>
		protected abstract void Dispose(bool disposing);

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		~GitClient()
		{
			Dispose(false);
		}
	}
}
