namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	public enum GitErrorCode
	{
		Ok = 0,

		Error = -1,
		NotFound = -3,
		Exists = -4,
		Ambiguous = -5,
		Bufs = -6,
		User = -7,
		BareRepo = -8,
		UnbornBranch = -9,
		Unmerged = -10,
		NonFastForward = -11,
		InvalidSpec = -12,
		Conflict = -13,
		Locked = -14,
		Modified = -15,
		Auth = -16,
		Certificate = -17,
		Applied = -18,
		Peel = -19,
		Eof = -20,
		Invalid = -21,
		Uncommitted = -22,
		Directory = -23,
		MergeConflict = -24,
		Passthrough = -30,
		Iterover = -31,
		Retry = -32,
		Mismatch = -33,
		IndexDirty = -34,
		ApplyFail = -35
	}
}
