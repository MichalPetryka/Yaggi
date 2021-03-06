using System;
using System.Collections.Generic;
using System.IO;
using Yaggi.Core.IO;

namespace Yaggi.Core.Git
{
	/// <summary>
	/// Representation of a local git repository
	/// </summary>
	public abstract class GitRepository : IDisposable, IEquatable<GitRepository>
	{
		/// <summary>
		/// Repository path
		/// </summary>
		public string Path { get; }

		/// <summary>
		/// Repository remotes
		/// </summary>
		public abstract GitRemote[] Remotes { get; }

		private readonly Dictionary<string, GitRemote> _remoteTracker = new();
		private readonly object _trackerLock = new();

		/// <summary>
		/// Creates a new repository
		/// </summary>
		/// <param name="path">Repository path</param>
		protected GitRepository(string path)
		{
			path = PathUtils.NormalizeDirectoryPath(path);
			if (!Directory.Exists(System.IO.Path.Combine(path, ".git")))
				throw new ArgumentException($"Directory \"{path}\" is not a git repository");
			Path = path;
		}

		/// <summary>
		/// Returns a cached remote instance, used to provide a single instance for every remote
		/// </summary>
		/// <param name="name">Remote name</param>
		/// <param name="factory">Factory for a new remote instance when one does not already exist</param>
		/// <returns>Cached or created remote</returns>
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

		/// <summary>
		/// Releases resources held by the repository
		/// </summary>
		/// <param name="disposing">Whether <see cref="Dispose()"/> was called or the object is finalized</param>
		protected abstract void Dispose(bool disposing);

		private void Free(bool disposing)
		{
			Dispose(disposing);

			lock (_trackerLock)
				_remoteTracker.Clear();
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Free(true);
			GC.SuppressFinalize(this);
		}

		/// <inheritdoc/>
		~GitRepository()
		{
			Free(false);
		}

#pragma warning disable IDE0041
		/// <inheritdoc/>
		public bool Equals(GitRepository other)
		{
			return this == other;
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == GetType() && this == (GitRepository)obj;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return Path.GetHashCode();
		}

		/// <inheritdoc/>
		public static bool operator ==(GitRepository left, GitRepository right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Path == right.Path;
		}

		/// <inheritdoc/>
		public static bool operator !=(GitRepository left, GitRepository right)
		{
			return !(left == right);
		}
#pragma warning restore IDE0041
	}
}
