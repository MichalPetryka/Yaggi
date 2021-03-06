using System;

namespace Yaggi.Core.Git
{
	public abstract class GitRemote
	{
		public abstract string Name { get; set; }
		public abstract string Url { get; set; }
		// public GitBranch DefaultBranch { get; } TODO
		public virtual GitRepository Repository { get; }

		protected GitRemote(GitRepository repository)
		{
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		protected void RenameRemote(string oldName, string newName)
		{
			Repository.RenameRemote(oldName, newName);
		}
	}
}
