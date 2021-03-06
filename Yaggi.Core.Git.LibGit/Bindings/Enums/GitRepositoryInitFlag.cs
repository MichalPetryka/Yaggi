using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	[Flags]
	public enum GitRepositoryInitFlag : uint
	{
		Bare = 1u << 0,
		NoReinit = 1u << 1,
		NoDotgitDir = 1u << 2,
		Mkdir = 1u << 3,
		Mkpath = 1u << 4,
		ExternalTemplate = 1u << 5,
		RelativeGitlink = 1u << 6
	}
}
