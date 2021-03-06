using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	public struct GitBuf
	{
		public IntPtr ptr;
		public nuint asize;
		public nuint size;
	}
}
