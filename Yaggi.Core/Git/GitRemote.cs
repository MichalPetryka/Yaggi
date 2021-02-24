using System;

namespace Yaggi.Core.Git
{
	public abstract class GitRemote : IDisposable, IEquatable<GitRemote>
	{
		public abstract string Name { get; set; }
		public abstract string Url { get; set; }
		// public GitBranch DefaultBranch { get; } TODO
		public virtual GitRepository Repository { get; }

		protected GitRemote(GitRepository repository)
		{
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		internal abstract void Update(string name);

		protected abstract void Dispose(bool disposing);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GitRemote()
		{
			Dispose(false);
		}

#pragma warning disable IDE0041
		public bool Equals(GitRemote other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && this == (GitRemote)obj;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Repository);
		}

		public static bool operator ==(GitRemote left, GitRemote right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Name == right.Name && left.Repository == right.Repository;
		}

		public static bool operator !=(GitRemote left, GitRemote right)
		{
			return !(left == right);
		}
#pragma warning restore IDE0041
	}
}
