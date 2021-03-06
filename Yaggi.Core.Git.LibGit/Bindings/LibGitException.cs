using System;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Yaggi.Core.Git.LibGit.Bindings
{
	/// <summary>
	/// LibGit2 error
	/// </summary>
	public sealed class LibGitException : Exception
	{
		/// <summary>
		/// Error code
		/// </summary>
		public GitErrorCode Code { get; }
		/// <summary>
		/// Error class
		/// </summary>
		public GitErrorClass ErrorClass { get; }

		internal LibGitException(GitErrorCode code, GitErrorClass errorClass, string message) : base(message ?? "Unknown Git error.")
		{
			Code = code;
			ErrorClass = errorClass;
		}
	}
}
