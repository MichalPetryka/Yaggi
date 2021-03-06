using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings;
using Yaggi.Core.Git.LibGit.Bindings.Structures;

namespace Yaggi.Core.Git.LibGit
{
	/// <summary>
	/// LibGit2 repository implementation
	/// </summary>
	public unsafe class LibGitRepository : GitRepository
	{
		internal readonly Bindings.Structures.GitRepository* Handle;
		private readonly object _freeLock = new();
		private bool _valid = true;

		/// <inheritdoc/>
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
						remotes[i] = TryGetRemote(Marshal.PtrToStringUTF8(strArray.strings[i]), name => new LibGitRemote(name, this));
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

		/// <inheritdoc/>
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
