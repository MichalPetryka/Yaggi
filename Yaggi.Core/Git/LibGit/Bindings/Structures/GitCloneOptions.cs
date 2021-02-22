using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	public struct GitCloneOptions
	{
		public uint version;
		public GitCheckoutOptions checkout_opts;
		public GitFetchOptions fetch_opts;
		public int bare;
		public GitCloneLocal local;
		public IntPtr checkout_branch;
		public IntPtr repository_cb;
		public IntPtr repository_cb_payload;
		public IntPtr remote_cb;
		public IntPtr remote_cb_payload;
	}
}
