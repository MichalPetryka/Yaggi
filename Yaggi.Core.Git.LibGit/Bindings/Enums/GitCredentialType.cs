using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	[Flags]
	internal enum GitCredentialType : uint
	{
		UserpassPlaintext = 1u << 0,
		SshKey = 1u << 1,
		SshCustom = 1u << 2,
		Default = 1u << 3,
		SshInteractive = 1u << 4,
		Username = 1u << 5,
		SshMemory = 1u << 6
	}
}
