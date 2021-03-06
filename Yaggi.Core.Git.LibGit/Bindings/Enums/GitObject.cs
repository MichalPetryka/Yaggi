namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	internal enum GitObject
	{
		Any = -2,
		Invalid = -1,
		Commit = 1,
		Tree = 2,
		Blob = 3,
		Tag = 4,
		OfsDelta = 6,
		RefDelta = 7
	}
}
