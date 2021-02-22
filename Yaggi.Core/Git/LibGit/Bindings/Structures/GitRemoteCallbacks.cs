using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	public struct GitRemoteCallbacks
	{
		private uint version;
		private IntPtr completion;
		private IntPtr credentials;
		private IntPtr certificate_check;
		private IntPtr transfer_progress;
		private IntPtr update_tips;
		private IntPtr pack_progress;
		private IntPtr push_transfer_progress;
		private IntPtr push_update_reference;
		private IntPtr push_negotiation;
		private IntPtr transport;
		private IntPtr payload;
		private IntPtr resolve_url;
	}
}
