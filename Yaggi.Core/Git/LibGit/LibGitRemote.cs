using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit
{
	public unsafe class LibGitRemote : GitRemote
	{
		private Bindings.Structures.GitRemote* _handle;

		private readonly object _freeLock = new();
		private bool _valid = true;

		public override string Name
		{
			get => Marshal.PtrToStringUTF8(GitNative.GetRemoteName(_handle));
			set
			{
				string oldName = Marshal.PtrToStringUTF8(GitNative.GetRemoteName(_handle));
				GitStrArray strArray = new();
				ThrowHelper.ThrowOnError(GitNative.SetRemoteName(&strArray, Repository.Handle, oldName, value));
				try
				{
					if (strArray.count == 0)
					{
						Repository.UpdateRemotes(oldName, value);
						return;
					}

					Exception[] exceptions = new Exception[strArray.count];
					for (nuint i = 0; i < strArray.count; i++)
						exceptions[i] = new Exception($"Rename error: {Marshal.PtrToStringUTF8(strArray.strings[i])}");

					throw new AggregateException(exceptions);
				}
				finally
				{
					GitNative.FreeStrArray(&strArray);
				}
			}
		}

		public override string Url
		{
			get => Marshal.PtrToStringUTF8(GitNative.GetRemoteUrl(_handle));
			set
			{
				string remoteName = Name;
				ThrowHelper.ThrowOnError(GitNative.SetRemoteUrl(Repository.Handle, remoteName, value));
				Repository.UpdateRemotes(remoteName, remoteName);
			}
		}

		public override LibGitRepository Repository { get; }

		internal LibGitRemote(Bindings.Structures.GitRemote* handle, LibGitRepository repository) : base(repository)
		{
			if (handle == null)
				throw new ArgumentNullException(nameof(handle));
			_handle = handle;
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		internal override void Update(string name)
		{
			GitNative.FreeRemote(_handle);
			fixed (Bindings.Structures.GitRemote** ptr = &_handle)
				ThrowHelper.ThrowOnError(GitNative.LookupRemote(ptr, Repository.Handle, name));
		}

		protected override void Dispose(bool disposing)
		{
			lock (_freeLock)
			{
				if (!_valid)
					return;

				Repository.UntrackRemote(this);
				GitNative.FreeRemote(_handle);
				_valid = false;
			}
		}
	}
}
