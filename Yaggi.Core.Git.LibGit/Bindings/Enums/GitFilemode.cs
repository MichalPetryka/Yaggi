namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	internal enum GitFilemode
	{
		Unreadable = 0000000,
		Tree = 0040000,
		Blob = 0100644,
		BlobExecutable = 0100755,
		Link = 0120000,
		Commit = 0160000
	}
}
