using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct GitCheckoutOptions
	{
		public uint version;
		public GitCheckoutStrategy checkoutStrategy;
		public int disableFilters;
		public uint dirMode;
		public uint fileMode;
		public int fileOpenFlags;
		public GitCheckoutNotify notifyFlags;
		public IntPtr notifyCb;
		public IntPtr notifyPayload;
		public IntPtr progressCb;
		public IntPtr progressPayload;
		public GitStrArray paths;
		public GitTree* baseline;
		public GitIndex* baselineIndex;
		public IntPtr targetDirectory;
		public IntPtr ancestorLabel;
		public IntPtr ourLabel;
		public IntPtr theirLabel;
		public IntPtr perfdataCb;
		public IntPtr perfdataPayload;
	}
}
