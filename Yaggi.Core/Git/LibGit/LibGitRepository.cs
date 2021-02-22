using System;
using Yaggi.Core.Git.LibGit.Bindings;

namespace Yaggi.Core.Git.LibGit
{
	public unsafe class LibGitRepository : GitRepository
	{
		private readonly Bindings.Structures.GitRepository* _handle;

		internal LibGitRepository(Bindings.Structures.GitRepository* handle, string path) : base(path)
		{
			if (_handle == null)
				throw new ArgumentNullException(nameof(handle));
			_handle = handle;
		}

		protected override void Dispose(bool disposing)
		{
			GitNative.FreeRepository(_handle);
		}
	}
}
