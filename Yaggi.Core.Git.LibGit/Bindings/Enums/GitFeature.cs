using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	[Flags]
	internal enum GitFeature
	{
		Threads = 1 << 0,
		Https = 1 << 1,
		Ssh = 1 << 2,
		Nsec = 1 << 3
	}
}
