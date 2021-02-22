using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Git.LibGit.Bindings.Enums
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct GitStrArray
	{
		public IntPtr* strings;
		public nuint count;
	}
}
