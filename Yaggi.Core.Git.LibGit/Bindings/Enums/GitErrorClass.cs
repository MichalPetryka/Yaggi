namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	/// <summary>
	/// Git error type
	/// </summary>
	public enum GitErrorClass
	{
		/// <summary>
		/// No error
		/// </summary>
		None = 0,
		/// <summary>
		/// System out of memory
		/// </summary>
		NoMemory,
		/// <summary>
		/// System error
		/// </summary>
		Os,
		/// <summary>
		/// Invalid error
		/// </summary>
		Invalid,
		/// <summary>
		/// Reference error
		/// </summary>
		Reference,
		/// <summary>
		/// Zlib error
		/// </summary>
		Zlib,
		/// <summary>
		/// Repository error
		/// </summary>
		Repository,
		/// <summary>
		/// Configuration error
		/// </summary>
		Config,
		/// <summary>
		/// Regular expression error
		/// </summary>
		Regex,
		/// <summary>
		/// Object database error
		/// </summary>
		Odb,
		/// <summary>
		/// Inded error
		/// </summary>
		Index,
		/// <summary>
		/// Object error
		/// </summary>
		Object,
		/// <summary>
		/// Internet error
		/// </summary>
		Net,
		/// <summary>
		/// Tag error
		/// </summary>
		Tag,
		/// <summary>
		/// Tree error
		/// </summary>
		Tree,
		/// <summary>
		/// Indexer error
		/// </summary>
		Indexer,
		/// <summary>
		/// Ssl error
		/// </summary>
		Ssl,
		/// <summary>
		/// Submodule error
		/// </summary>
		Submodule,
		/// <summary>
		/// Thread error
		/// </summary>
		Thread,
		/// <summary>
		/// Stash error
		/// </summary>
		Stash,
		/// <summary>
		/// Checkout error
		/// </summary>
		Checkout,
		/// <summary>
		/// Fetch head error
		/// </summary>
		Fetchhead,
		/// <summary>
		/// Merge error
		/// </summary>
		Merge,
		/// <summary>
		/// Ssh error
		/// </summary>
		Ssh,
		/// <summary>
		/// Filter error
		/// </summary>
		Filter,
		/// <summary>
		/// Revert error
		/// </summary>
		Revert,
		/// <summary>
		/// Callback error
		/// </summary>
		Callback,
		/// <summary>
		/// Cherrypick error
		/// </summary>
		Cherrypick,
		/// <summary>
		/// Describe error
		/// </summary>
		Describe,
		/// <summary>
		/// Rebase error
		/// </summary>
		Rebase,
		/// <summary>
		/// Filesystem error
		/// </summary>
		Filesystem,
		/// <summary>
		/// Patch error
		/// </summary>
		Patch,
		/// <summary>
		/// Worktree error
		/// </summary>
		Worktree,
		/// <summary>
		/// Sha1 error
		/// </summary>
		Sha1,
		/// <summary>
		/// Http error
		/// </summary>
		Http,
		/// <summary>
		/// Internal error
		/// </summary>
		Internal
	}
}
