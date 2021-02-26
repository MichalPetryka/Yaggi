using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings;
using Yaggi.Core.Git.LibGit.Bindings.Enums;
using Yaggi.Core.Git.LibGit.Bindings.Structures;
using Yaggi.Core.IO;
using Yaggi.Core.Marshaling;
using Yaggi.Core.Memory;

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
			Directory.CreateDirectory(path);
			path = PathUtils.NormalizeDirectoryPath(path);
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

		public override GitRepository CloneRepository(string path, string url, Action<string, double> progress = null, Func<string, (string, string, bool)[], (bool, string[])> authenticationProvider = null)
		{
			Directory.CreateDirectory(path);
			path = PathUtils.NormalizeDirectoryPath(path);
			GitCloneOptions options = new();
			ThrowHelper.ThrowOnError(GitNative.InitializeCloneOptions(&options, 1));
			List<Action> freeCallbacks = ListPool<Action>.Rent();
			try
			{
				// ReSharper disable ConvertToLocalFunction
				if (authenticationProvider != null)
				{
					GitCredentialType type = 0;
					int tries = 0;
					GitCallbacks.CredentialAcquireCallback acquireCredentials = (data, ptr, fromUrl, types, _) =>
					{
						if (type != types)
						{
							tries = 0;
							type = types;
						}

						const int maxTries = 3;
						if (++tries > maxTries)
							return GitErrorCode.User;

						// ReSharper disable HeapView.BoxingAllocation
						string prompt = "Provide your authentication data";
						string remoteUrl = Marshal.PtrToStringUTF8(ptr);
						if (!string.IsNullOrEmpty(remoteUrl))
							prompt += $" for \"{remoteUrl}\"";
						prompt += $" (try {tries} out of {maxTries})";
						string usernameFromUrl = Marshal.PtrToStringUTF8(fromUrl);

						if (types.HasFlag(GitCredentialType.UserpassPlaintext))
						{
							(bool, string[]) input = authenticationProvider(prompt, new[]
							{
								("Username", usernameFromUrl, false),
								("Password", (string)null, true)
							});
							if (!input.Item1)
								return GitErrorCode.User;
							NativeString userName = new(input.Item2[0], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => userName.Dispose());
							NativeString password = new(input.Item2[1], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => password.Dispose());
							return GitNative.CredentialUserpassPlaintextNew(data, userName.Data, password.Data);
						}

						if (types.HasFlag(GitCredentialType.Default))
							return GitNative.CredentialDefaultNew(data);

						if (types.HasFlag(GitCredentialType.Username))
						{
							(bool, string[]) input3 =
								authenticationProvider(prompt, new[] { ("Username", usernameFromUrl, false) });
							if (!input3.Item1)
								return GitErrorCode.User;
							NativeString userName3 = new(input3.Item2[0], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => userName3.Dispose());
							return GitNative.CredentialUsernameNew(data, userName3.Data);
						}

						if (types.HasFlag(GitCredentialType.SshKey))
						{
							string pubPath =
								Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh",
									"id_rsa.pub");
							string privPath =
								Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh",
									"id_rsa");
							(bool, string[]) input2 = authenticationProvider(prompt, new[]
							{
								("Username", usernameFromUrl, false),
								("Public key path", File.Exists(pubPath) ? pubPath : null, false),
								("Private key path", File.Exists(privPath) ? privPath : null, false),
								("Passphrase", (string)null, true)
							});
							if (!input2.Item1)
								return GitErrorCode.User;
							NativeString userName2 = new(input2.Item2[0], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => userName2.Dispose());
							NativeString publickey = new(input2.Item2[1], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => publickey.Dispose());
							NativeString privatekey = new(input2.Item2[2], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => privatekey.Dispose());
							NativeString passphrase = new(input2.Item2[3], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => passphrase.Dispose());
							return GitNative.CredentialSshKeyNew(data, userName2.Data, publickey.Data, privatekey.Data,
								passphrase.Data);
						}

						if (types.HasFlag(GitCredentialType.SshMemory))
						{
							string pubPath2 =
								Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh",
									"id_rsa.pub");
							string privPath2 =
								Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh",
									"id_rsa");
							(bool, string[]) input4 = authenticationProvider(prompt, new[]
							{
								("Username", usernameFromUrl, false),
								("Public key path", File.Exists(pubPath2) ? File.ReadAllText(pubPath2) : null, false),
								("Private key path", File.Exists(privPath2) ? File.ReadAllText(privPath2) : null,
									false),
								("Passphrase", (string)null, true)
							});
							if (!input4.Item1)
								return GitErrorCode.User;
							NativeString userName4 = new(input4.Item2[0], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => userName4.Dispose());
							NativeString publickey2 = new(input4.Item2[1], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => publickey2.Dispose());
							NativeString privatekey2 = new(input4.Item2[2], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => privatekey2.Dispose());
							NativeString passphrase2 = new(input4.Item2[3], StringEncoding.UTF8, true);
							freeCallbacks.Add(() => passphrase2.Dispose());
							return GitNative.CredentialSshKeyMemoryNew(data, userName4.Data, publickey2.Data,
								privatekey2.Data, passphrase2.Data);
						}
						// ReSharper restore HeapView.BoxingAllocation

						return GitErrorCode.User;
					};
					GCHandle handle1 = GCHandle.Alloc(acquireCredentials, GCHandleType.Normal);
					freeCallbacks.Add(() => handle1.Free());
					options.fetchOpts.callbacks.credentials =
						Marshal.GetFunctionPointerForDelegate(acquireCredentials);
				}
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
					GCHandle handle1 = GCHandle.Alloc(indexer, GCHandleType.Normal);
					freeCallbacks.Add(() => handle1.Free());
					options.fetchOpts.callbacks.transferProgress =
						Marshal.GetFunctionPointerForDelegate(indexer);
					GitCallbacks.CheckoutProgressCallback checkout = (_, steps, totalSteps, _) =>
					{
						progress("Checking out", (double)steps / totalSteps);
					};
					GCHandle handle2 = GCHandle.Alloc(checkout, GCHandleType.Normal);
					freeCallbacks.Add(() => handle2.Free());
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
				foreach (Action callback in freeCallbacks)
					callback();
				ListPool<Action>.Return(freeCallbacks);
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
