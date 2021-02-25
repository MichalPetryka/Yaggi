using System;
using Yaggi.Core.Git.LibGit.Bindings;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit
{
	public unsafe class LibGitRepository : GitRepository
	{
		internal readonly Bindings.Structures.GitRepository* Handle;
		private readonly object _freeLock = new();
		private bool _valid = true;

		public override GitRemote[] Remotes
		{
			get
			{
				GitStrArray strArray = new();
				ThrowHelper.ThrowOnError(GitNative.ListRemotes(&strArray, Handle));
				try
				{
					GitRemote[] remotes = new GitRemote[strArray.count];
					for (nuint i = 0; i < strArray.count; i++)
					{
						Bindings.Structures.GitRemote* remote = null;
						ThrowHelper.ThrowOnError(GitNative.LookupRemote(&remote, Handle, strArray.strings[i]));
						TrackRemote(remotes[i] = new LibGitRemote(remote, this));
					}

					return remotes;
				}
				finally
				{
					GitNative.FreeStrArray(&strArray);
				}
			}
		}

		internal LibGitRepository(Bindings.Structures.GitRepository* handle, string path) : base(path)
		{
			if (handle == null)
				throw new ArgumentNullException(nameof(handle));
			Handle = handle;
		}

		protected override void Dispose(bool disposing)
		{
			lock (_freeLock)
			{
				if (!_valid)
					return;
				GitNative.FreeRepository(Handle);
				_valid = false;
			}
		}
	}
}
