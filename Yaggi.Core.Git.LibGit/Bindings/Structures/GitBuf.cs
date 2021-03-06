using System;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	internal struct GitBuf
	{
		public IntPtr ptr;
		public nuint asize;
		public nuint size;
	}
}
