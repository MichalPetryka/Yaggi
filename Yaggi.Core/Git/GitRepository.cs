using System;
using System.Collections.Generic;
using System.IO;

namespace Yaggi.Core.Git
{
	public abstract class GitRepository : IDisposable, IEquatable<GitRepository>
	{
		public string Path { get; }

		public abstract GitRemote[] Remotes { get; }

		private readonly Dictionary<string, List<GitRemote>> _remoteTracker = new();
		private readonly object _trackerLock = new();

		protected GitRepository(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			path = System.IO.Path.GetFullPath(path);
			if (!Directory.Exists(path))
				throw new ArgumentException($"Directory \"{path}\" does not exist");
			if (!Directory.Exists(System.IO.Path.Combine(path, ".git")))
				throw new ArgumentException($"Directory \"{path}\" is not a git repository");
			Path = path;
		}

		protected void TrackRemote(GitRemote remote)
		{
			lock (_trackerLock)
			{
				if (_remoteTracker.TryGetValue(remote.Name, out List<GitRemote> remotes))
					remotes.Add(remote);

				_remoteTracker[remote.Name] = new List<GitRemote> { remote };
			}
		}

		internal void UntrackRemote(GitRemote remote)
		{
			lock (_trackerLock)
			{
				List<GitRemote> remotes = _remoteTracker[remote.Name];
				for (int i = remotes.Count - 1; i >= 0; i--)
				{
					if (!ReferenceEquals(remotes[i], remote))
						continue;
					remotes.RemoveAt(i);
					break;
				}

				if (remotes.Count == 0)
					_remoteTracker.Remove(remote.Name);
			}
		}

		internal void UpdateRemotes(string oldName, string newName)
		{
			lock (_trackerLock)
			{
				if (oldName != newName)
				{
					_remoteTracker[newName] = _remoteTracker[oldName];
					_remoteTracker.Remove(oldName);
				}

				foreach (GitRemote gitRemote in _remoteTracker[newName])
					gitRemote.Update(newName);
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
