using System;

namespace Yaggi.Core.Git
{
	/// <summary>
	/// Represents a Git remote, that is a separate copy of the repository linked to it
	/// </summary>
	public abstract class GitRemote
	{
		/// <summary>
		/// Remote name
		/// </summary>
		public abstract string Name { get; set; }
		/// <summary>
		/// Remote Url used to access it
		/// </summary>
		public abstract string Url { get; set; }
		// public GitBranch DefaultBranch { get; } TODO
		/// <summary>
		/// Repository the remote belongs to
		/// </summary>
		public virtual GitRepository Repository { get; }

		/// <summary>
		/// Creates a new remote
		/// </summary>
		/// <param name="repository">Parent repository</param>
		protected GitRemote(GitRepository repository)
		{
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		/// <summary>
		/// Must be called on rename to ensure proper remote caching
		/// </summary>
		/// <param name="oldName">Old remote name</param>
		/// <param name="newName">New remote name</param>
		protected void RenameRemote(string oldName, string newName)
		{
			Repository.RenameRemote(oldName, newName);
		}
	}
}
