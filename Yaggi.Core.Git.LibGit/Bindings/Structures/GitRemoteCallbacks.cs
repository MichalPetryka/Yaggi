using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct GitRemoteCallbacks
	{
		public uint version;
		public IntPtr sidebandProgress;
		public IntPtr completion;
		public IntPtr credentials;
		public IntPtr certificateCheck;
		public IntPtr transferProgress;
		public IntPtr updateTips;
		public IntPtr packProgress;
		public IntPtr pushTransferProgress;
		public IntPtr pushUpdateReference;
		public IntPtr pushNegotiation;
		public IntPtr transport;
		public IntPtr payload;
		public IntPtr resolveUrl;
	}
}
