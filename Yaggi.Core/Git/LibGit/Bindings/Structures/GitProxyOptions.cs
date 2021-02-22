using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	public struct GitProxyOptions
	{
		public uint version;
		public GitProxy type;
		public IntPtr url;
		public IntPtr credentials;
		public IntPtr certificate_check;
		public IntPtr payload;
	}
}
