using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;
using Yaggi.Core.Git.LibGit.Bindings.Structures;

namespace Yaggi.Core.Git.LibGit.Bindings
{
	internal static unsafe class GitNative
	{
		private const string Library = "libgit2";
		private const CallingConvention Convention = CallingConvention.Cdecl;
		private const UnmanagedType StringType = UnmanagedType.LPUTF8Str;

		[DllImport(Library, EntryPoint = "git_libgit2_init", CallingConvention = Convention)]
		public static extern int Initialize();

		[DllImport(Library, EntryPoint = "git_libgit2_shutdown", CallingConvention = Convention)]
		public static extern int Shutdown();

		[DllImport(Library, EntryPoint = "git_libgit2_version", CallingConvention = Convention)]
		public static extern GitErrorCode GetVersion(out int major, out int minor, out int rev);

		[DllImport(Library, EntryPoint = "git_libgit2_features", CallingConvention = Convention)]
		public static extern GitFeature GetFeatures();

		[DllImport(Library, EntryPoint = "git_libgit2_opts", CallingConvention = Convention)]
		public static extern GitErrorCode SetOption(GitLibGit2Opt option, int enabled);

		[DllImport(Library, EntryPoint = "git_error_last", CallingConvention = Convention)]
		private static extern GitError* GetLastError();

		[DllImport(Library, EntryPoint = "git_error_clear", CallingConvention = Convention)]
		private static extern void ClearError();

		[DllImport(Library, EntryPoint = "git_repository_init_options_init", CallingConvention = Convention)]
		public static extern GitErrorCode InitializeOptions(GitRepositoryInitOptions* options, uint version);

		[DllImport(Library, EntryPoint = "git_repository_init_ext", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode InitializeRepository(out Structures.GitRepository* repository, [MarshalAs(StringType)] string path, GitRepositoryInitOptions* options);

		[DllImport(Library, EntryPoint = "git_repository_free", CallingConvention = Convention)]
		public static extern void FreeRepository(Structures.GitRepository* repository);

		public static bool TryGetError(out string message, out GitErrorClass error)
		{
			GitError* err = GetLastError();
			if (err == null)
			{
				message = null;
				error = GitErrorClass.None;
				return false;
			}

			message = Marshal.PtrToStringUTF8(err->Message)?.Trim();
			if (!string.IsNullOrEmpty(message))
			{
				if (char.IsLower(message[0]))
					message = char.ToUpperInvariant(message[0]) + message.Substring(1);
				if (!message.EndsWith('.'))
					message += '.';
			}
			error = err->Klass;
			ClearError();
			return true;
		}
	}
}
