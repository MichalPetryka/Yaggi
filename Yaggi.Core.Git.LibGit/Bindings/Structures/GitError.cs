using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	public readonly struct GitError
	{
		public readonly IntPtr Message;
		public readonly GitErrorClass Klass;
	}
}
