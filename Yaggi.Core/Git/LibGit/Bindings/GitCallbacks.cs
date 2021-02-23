using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Structures;

namespace Yaggi.Core.Git.LibGit.Bindings
{
	internal static unsafe class GitCallbacks
	{
		private const CallingConvention Convention = CallingConvention.Cdecl;

		[UnmanagedFunctionPointer(Convention)]
		public delegate int IndexerProgressCallback(GitIndexerProgress* stats, IntPtr payload);

		[UnmanagedFunctionPointer(Convention)]
		public delegate void CheckoutProgress(IntPtr path, nuint completedSteps, nuint totalSteps, IntPtr payload);
	}
}
