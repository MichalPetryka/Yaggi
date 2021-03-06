using System.Runtime.CompilerServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings
{
	internal static class ThrowHelper
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowOnError(GitErrorCode error)
		{
			if (error != GitErrorCode.Ok)
				Throw(error);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ThrowOnError(int value)
		{
			if (value < 0)
				Throw((GitErrorCode)value);
			return value;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void Throw(GitErrorCode code)
		{
			GitNative.TryGetError(out string message, out GitErrorClass error);
			throw new LibGitException(code, error, message);
		}
	}
}
