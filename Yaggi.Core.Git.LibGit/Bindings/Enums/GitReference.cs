namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	internal enum GitReference
	{
		Invalid = 0,
		Direct = 1,
		Symbolic = 2,
		All = Direct | Symbolic
	}
}
