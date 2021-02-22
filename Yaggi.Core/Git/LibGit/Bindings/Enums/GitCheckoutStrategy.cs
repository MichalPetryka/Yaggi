using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	[Flags]
	public enum GitCheckoutStrategy : uint
	{
		None = 0,
		Safe = 1u << 0,
		Force = 1u << 1,
		RecreateMissing = 1u << 2,
		AllowConflicts = 1u << 4,
		RemoveUntracked = 1u << 5,
		RemoveIgnored = 1u << 6,
		UpdateOnly = 1u << 7,
		DontUpdateIndex = 1u << 8,
		NoRefresh = 1u << 9,
		SkipUnmerged = 1u << 10,
		UseOurs = 1u << 11,
		UseTheirs = 1u << 12,
		DisablePathspecMatch = 1u << 13,
		SkipLockedDirectories = 1u << 18,
		DontOverwriteIgnored = 1u << 19,
		ConflictStyleMerge = 1u << 20,
		ConflictStyleDiff3 = 1u << 21,
		DontRemoveExisting = 1u << 22,
		DontWriteIndex = 1u << 23,
		UpdateSubmodules = 1u << 16,
		UpdateSubmodulesIfChanged = 1u << 17
	}
}
