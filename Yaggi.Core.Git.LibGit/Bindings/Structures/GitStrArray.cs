using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct GitStrArray
	{
		public IntPtr* strings;
		public nuint count;
	}
}
