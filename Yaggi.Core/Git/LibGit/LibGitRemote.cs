using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit
{
	public unsafe class LibGitRemote : GitRemote
	{
		private string _name;

		public override string Name
		{
			get => _name;
			set
			{
				GitStrArray strArray = new();
				ThrowHelper.ThrowOnError(GitNative.SetRemoteName(&strArray, Repository.Handle, _name, value));
				try
				{
					if (strArray.count == 0)
					{
						Repository.RenameRemote(_name, value);
						_name = value;
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
			get
			{
				Bindings.Structures.GitRemote* remote = null;
				ThrowHelper.ThrowOnError(GitNative.LookupRemote(&remote, Repository.Handle, _name));
				try
				{
					return Marshal.PtrToStringUTF8(GitNative.GetRemoteUrl(remote));
				}
				finally
				{
					GitNative.FreeRemote(remote);
				}
			}
			set => ThrowHelper.ThrowOnError(GitNative.SetRemoteUrl(Repository.Handle, _name, value));
		}

		public override LibGitRepository Repository { get; }

		internal LibGitRemote(string name, LibGitRepository repository) : base(repository)
		{
			_name = name ?? throw new ArgumentNullException(nameof(name));
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}
	}
}
