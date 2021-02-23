using System;
using System.Runtime.InteropServices;
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
			ThrowHelper.ThrowOnError(GitNative.InitializeInitOptions(&options, 1));
			options.flags |= GitRepositoryInitFlag.NoReinit | GitRepositoryInitFlag.Mkdir | GitRepositoryInitFlag.Mkpath;
			using (NativeString branch = new(branchName, StringEncoding.UTF8))
			{
				options.initialHead = branch.Data;
				ThrowHelper.ThrowOnError(GitNative.InitializeRepository(out Bindings.Structures.GitRepository* repository, path, &options));
				return new LibGitRepository(repository, path);
			}
		}

		public override GitRepository CloneRepository(string path, string url, Action<string, double> progress = null)
		{
			GitCloneOptions options = new();
			ThrowHelper.ThrowOnError(GitNative.InitializeCloneOptions(&options, 1));
			GCHandle? handle1 = null;
			GCHandle? handle2 = null;
			try
			{
				// ReSharper disable ConvertToLocalFunction
				if (progress != null)
				{
					long lastProgress = 0;
					GitCallbacks.IndexerProgressCallback indexer = (stats, _) =>
					{
						long p = Math.Min((long)stats->receivedObjects, stats->indexedObjects);
						if (lastProgress != p)
						{
							lastProgress = p;
							progress("Receiving", (double)p / stats->totalObjects);
						}
						return 0;
					};
					handle1 = GCHandle.Alloc(indexer, GCHandleType.Normal);
					options.fetchOpts.callbacks.transferProgress =
						Marshal.GetFunctionPointerForDelegate(indexer);
					GitCallbacks.CheckoutProgress checkout = (_, steps, totalSteps, _) =>
					{
						progress("Checking out", (double)steps / totalSteps);
					};
					handle2 = GCHandle.Alloc(checkout, GCHandleType.Normal);
					options.fetchOpts.callbacks.transferProgress =
						Marshal.GetFunctionPointerForDelegate(indexer);
					options.checkoutOpts.progressCb =
						Marshal.GetFunctionPointerForDelegate(checkout);
				}
				// ReSharper restore ConvertToLocalFunction
				ThrowHelper.ThrowOnError(GitNative.CloneRepository(out Bindings.Structures.GitRepository* repository, url, path, &options));
				return new LibGitRepository(repository, path);
			}
			finally
			{
				handle1?.Free();
				handle2?.Free();
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
