using System;
using System.Collections.Generic;
using System.IO;
using Yaggi.Core.IO;

namespace Yaggi.Core.Git
{
	public abstract class GitRepository : IDisposable, IEquatable<GitRepository>
	{
		public string Path { get; }

		public abstract GitRemote[] Remotes { get; }

		private readonly Dictionary<string, GitRemote> _remoteTracker = new();
		private readonly object _trackerLock = new();

		protected GitRepository(string path)
		{
			path = PathUtils.NormalizeDirectoryPath(path);
			if (!Directory.Exists(System.IO.Path.Combine(path, ".git")))
				throw new ArgumentException($"Directory \"{path}\" is not a git repository");
			Path = path;
		}

		protected GitRemote TryGetRemote(string name, Func<string, GitRemote> factory)
		{
			lock (_trackerLock)
			{
				if (_remoteTracker.TryGetValue(name, out GitRemote remote))
					return remote;

				return _remoteTracker[name] = factory(name);
			}
		}

		internal void RenameRemote(string oldName, string newName)
		{
			lock (_trackerLock)
			{
				_remoteTracker[newName] = _remoteTracker[oldName];
				_remoteTracker.Remove(oldName);
			}
		}

		protected abstract void Dispose(bool disposing);

		private void Free(bool disposing)
		{
			Dispose(disposing);

			lock (_trackerLock)
				_remoteTracker.Clear();
		}

		public void Dispose()
		{
			Free(true);
			GC.SuppressFinalize(this);
		}

		~GitRepository()
		{
			Free(false);
		}

#pragma warning disable IDE0041
		public bool Equals(GitRepository other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && this == (GitRepository)obj;
		}

		public override int GetHashCode()
		{
			return Path.GetHashCode();
		}

		public static bool operator ==(GitRepository left, GitRepository right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Path == right.Path;
		}

		public static bool operator !=(GitRepository left, GitRepository right)
		{
			return !(left == right);
		}
#pragma warning restore IDE0041
	}
}
