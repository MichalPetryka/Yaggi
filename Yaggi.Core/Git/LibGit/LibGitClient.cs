using Yaggi.Core.Git.LibGit.Bindings;
using Yaggi.Core.Git.LibGit.Bindings.Enums;
using Yaggi.Core.Git.LibGit.Bindings.Structures;
using Yaggi.Core.Marshaling;

namespace Yaggi.Core.Git.LibGit
{
	public unsafe class LibGitClient : GitClient
	{
		private static readonly object LoadLock = new();
		private bool _valid;

		public LibGitClient()
		{
			lock (LoadLock)
			{
				ThrowHelper.ThrowOnError(GitNative.Initialize());
				_valid = true;
			}
		}

		public override GitRepository InitializeRepository(string path, string branchName)
		{
			GitRepositoryInitOptions options = new();
			ThrowHelper.ThrowOnError(GitNative.InitializeOptions(&options, 1));
			options.flags |= GitRepositoryInitFlag.NoReinit | GitRepositoryInitFlag.Mkdir | GitRepositoryInitFlag.Mkpath;
			using (NativeString branch = new(branchName, StringEncoding.UTF8))
			{
				options.initial_head = branch.Data;
				ThrowHelper.ThrowOnError(GitNative.InitializeRepository(out Bindings.Structures.GitRepository* repository, path, &options));
				return new LibGitRepository(repository, path);
			}
		}

		protected override void Dispose(bool disposing)
		{
			lock (LoadLock)
			{
				if (!_valid)
					return;
				ThrowHelper.ThrowOnError(GitNative.Shutdown());
				_valid = false;
			}
		}
	}
}
