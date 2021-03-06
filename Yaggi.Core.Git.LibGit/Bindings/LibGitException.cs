using System;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Yaggi.Core.Git.LibGit.Bindings
{
	public sealed class LibGitException : Exception
	{
		public GitErrorCode Code { get; }
		public GitErrorClass ErrorClass { get; }

		internal LibGitException(GitErrorCode code, GitErrorClass errorClass, string message) : base(message ?? "Unknown Git error.")
		{
			Code = code;
			ErrorClass = errorClass;
		}
	}
}
