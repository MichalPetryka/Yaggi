using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	public struct GitCloneOptions
	{
		public uint version;
		public GitCheckoutOptions checkoutOpts;
		public GitFetchOptions fetchOpts;
		public int bare;
		public GitCloneLocal local;
		public IntPtr checkoutBranch;
		public IntPtr repositoryCb;
		public IntPtr repositoryCbPayload;
		public IntPtr remoteCb;
		public IntPtr remoteCbPayload;
	}
}
