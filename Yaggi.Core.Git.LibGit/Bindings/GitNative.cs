using System;
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
		public static extern GitErrorCode InitializeInitOptions(GitRepositoryInitOptions* options, uint version);

		[DllImport(Library, EntryPoint = "git_repository_init_ext", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode InitializeRepository(out Structures.GitRepository* repository, [MarshalAs(StringType)] string path, GitRepositoryInitOptions* options);

		[DllImport(Library, EntryPoint = "git_clone_options_init", CallingConvention = Convention)]
		public static extern GitErrorCode InitializeCloneOptions(GitCloneOptions* options, uint version);

		[DllImport(Library, EntryPoint = "git_clone", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode CloneRepository(out Structures.GitRepository* repository, [MarshalAs(StringType)] string url, [MarshalAs(StringType)] string path, GitCloneOptions* options);

		[DllImport(Library, EntryPoint = "git_repository_open", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode OpenRepository(out Structures.GitRepository* repository, [MarshalAs(StringType)] string path);

		[DllImport(Library, EntryPoint = "git_repository_open_ext", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode OpenRepository(out Structures.GitRepository* repository, [MarshalAs(StringType)] string path, GitRepositoryOpenFlag flags, [MarshalAs(StringType)] string ceilingDirs);

		[DllImport(Library, EntryPoint = "git_repository_free", CallingConvention = Convention)]
		public static extern void FreeRepository(Structures.GitRepository* repository);

		[DllImport(Library, EntryPoint = "git_remote_init_callbacks", CallingConvention = Convention)]
		public static extern GitErrorCode InitializeRemoteOptions(GitRemoteCallbacks* options, uint version);

		[DllImport(Library, EntryPoint = "git_proxy_options_init", CallingConvention = Convention)]
		public static extern GitErrorCode InitializeProxyOptions(GitProxyOptions* options, uint version);

		[DllImport(Library, EntryPoint = "git_strarray_dispose", CallingConvention = Convention)]
		public static extern void FreeStrArray(GitStrArray* array);

		[DllImport(Library, EntryPoint = "git_buf_dispose", CallingConvention = Convention)]
		public static extern void FreeBuf(GitBuf* buffer);

		[DllImport(Library, EntryPoint = "git_buf_grow", CallingConvention = Convention)]
		public static extern GitErrorCode GrowBuf(GitBuf* buffer, nuint targetSize);

		[DllImport(Library, EntryPoint = "git_buf_set", CallingConvention = Convention)]
		public static extern GitErrorCode SetBuf(GitBuf* buffer, IntPtr data, nuint dataLength);

		[DllImport(Library, EntryPoint = "git_buf_is_binary", CallingConvention = Convention)]
		public static extern int BufIsBinary(GitBuf* buffer);

		[DllImport(Library, EntryPoint = "git_buf_contains_nul", CallingConvention = Convention)]
		public static extern int BufContainsNull(GitBuf* buffer);

		[DllImport(Library, EntryPoint = "git_remote_list", CallingConvention = Convention)]
		public static extern GitErrorCode ListRemotes(GitStrArray* array, Structures.GitRepository* repository);

		[DllImport(Library, EntryPoint = "git_remote_lookup", CallingConvention = Convention)]
		public static extern GitErrorCode LookupRemote(Structures.GitRemote** remote, Structures.GitRepository* repository, IntPtr name);

		[DllImport(Library, EntryPoint = "git_remote_lookup", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode LookupRemote(Structures.GitRemote** remote, Structures.GitRepository* repository, [MarshalAs(StringType)] string name);

		[DllImport(Library, EntryPoint = "git_remote_free", CallingConvention = Convention)]
		public static extern void FreeRemote(Structures.GitRemote* remote);

		[DllImport(Library, EntryPoint = "git_remote_name", CallingConvention = Convention)]
		public static extern IntPtr GetRemoteName(Structures.GitRemote* remote);

		[DllImport(Library, EntryPoint = "git_remote_rename", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode SetRemoteName(GitStrArray* problems, Structures.GitRepository* repository, [MarshalAs(StringType)] string name, [MarshalAs(StringType)] string newName);

		[DllImport(Library, EntryPoint = "git_remote_url", CallingConvention = Convention)]
		public static extern IntPtr GetRemoteUrl(Structures.GitRemote* remote);

		[DllImport(Library, EntryPoint = "git_remote_set_url", CallingConvention = Convention, BestFitMapping = false)]
		public static extern GitErrorCode SetRemoteUrl(Structures.GitRepository* repository, [MarshalAs(StringType)] string remote, [MarshalAs(StringType)] string url);

		[DllImport(Library, EntryPoint = "git_credential_free", CallingConvention = Convention)]
		public static extern void CredentialFree(GitCredential* cred);

		[DllImport(Library, EntryPoint = "git_credential_has_username", CallingConvention = Convention)]
		public static extern int CredentialHasUsername(GitCredential* cred);

		[DllImport(Library, EntryPoint = "git_credential_get_username", CallingConvention = Convention)]
		public static extern IntPtr CredentialGetUsername(GitCredential* cred);

		[DllImport(Library, EntryPoint = "git_credential_userpass_plaintext_new", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialUserpassPlaintextNew(GitCredential** cred, IntPtr username, IntPtr password);

		[DllImport(Library, EntryPoint = "git_credential_default_new", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialDefaultNew(GitCredential** cred);

		[DllImport(Library, EntryPoint = "git_credential_username_new", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialUsernameNew(GitCredential** cred, IntPtr username);

		[DllImport(Library, EntryPoint = "git_credential_ssh_key_new", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialSshKeyNew(GitCredential** cred, IntPtr username, IntPtr publickey, IntPtr privatekey, IntPtr passphrase);

		[DllImport(Library, EntryPoint = "git_credential_ssh_key_memory_new", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialSshKeyMemoryNew(GitCredential** cred, IntPtr username, IntPtr publickey, IntPtr privatekey, IntPtr passphrase);

		[DllImport(Library, EntryPoint = "git_credential_ssh_interactive_new", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialSshInteractiveNew(GitCredential** cred, IntPtr username, IntPtr promptCallback, IntPtr payload);

		[DllImport(Library, EntryPoint = "git_credential_ssh_key_from_agent", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialSshKeyFromAgent(GitCredential** cred, IntPtr username);

		[DllImport(Library, EntryPoint = "git_credential_ssh_custom_new", CallingConvention = Convention)]
		public static extern GitErrorCode CredentialSshCustomNew(GitCredential** cred, IntPtr username, byte* publickey, nuint publickeyLength, IntPtr signCallback, IntPtr payload);

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
