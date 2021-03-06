using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	[Flags]
	public enum GitCheckoutNotify : uint
	{
		None = 0,
		Conflict = 1u << 0,
		Dirty = 1u << 1,
		Updated = 1u << 2,
		Untracked = 1u << 3,
		Ignored = 1u << 4,

		All = 0x0FFFFu
	}
}
