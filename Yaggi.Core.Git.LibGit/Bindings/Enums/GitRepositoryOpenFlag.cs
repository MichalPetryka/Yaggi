using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	/// <summary>
	/// Option flags for `git_repository_open_ext`.
	/// </summary>
	[Flags]
	public enum GitRepositoryOpenFlag : uint
	{
		/// <summary>
		/// Only open the repository if it can be immediately found in the
		/// start_path. Do not walk up from the start_path looking at parent
		/// directories.
		/// </summary>
		NoSearch = 1 << 0,
		/// <summary>
		/// Unless this flag is set, open will not continue searching across
		/// filesystem boundaries (i.e. when `st_dev` changes from the `stat`
		/// system call).  For example, searching in a user's home directory at
		/// "/home/user/source/" will not return "/.git/" as the found repo if
		/// "/" is a different filesystem than "/home".
		/// </summary>
		CrossFs = 1 << 1,
		/// <summary>
		/// Open repository as a bare repo regardless of core.bare config, and
		/// defer loading config file for faster setup.
		/// Unlike `git_repository_open_bare`, this can follow gitlinks.
		/// </summary>
		Bare = 1 << 2,
		/// <summary>
		/// Do not check for a repository by appending /.git to the start_path;
		/// only open the repository if start_path itself points to the git
		/// directory.
		/// </summary>
		NoDotgit = 1 << 3,
		/// <summary>
		/// Find and open a git repository, respecting the environment variables
		/// used by the git command-line tools.
		/// If set, `git_repository_open_ext` will ignore the other flags and
		/// the `ceiling_dirs` argument, and will allow a NULL `path` to use
		/// `GIT_DIR` or search from the current directory.
		/// The search for a repository will respect $GIT_CEILING_DIRECTORIES and
		/// $GIT_DISCOVERY_ACROSS_FILESYSTEM.  The opened repository will
		/// respect $GIT_INDEX_FILE, $GIT_NAMESPACE, $GIT_OBJECT_DIRECTORY, and
		/// $GIT_ALTERNATE_OBJECT_DIRECTORIES.
		/// In the future, this flag will also cause `git_repository_open_ext`
		/// to respect $GIT_WORK_TREE and $GIT_COMMON_DIR; currently,
		/// `git_repository_open_ext` with this flag will error out if either
		/// $GIT_WORK_TREE or $GIT_COMMON_DIR is set.
		/// </summary>
		FromEnv = 1 << 4
	}
}
