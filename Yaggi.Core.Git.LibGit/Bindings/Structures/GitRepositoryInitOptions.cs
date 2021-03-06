using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct GitRepositoryInitOptions
	{
		public uint version;
		public GitRepositoryInitFlag flags;
		public GitRepositoryInitMode mode;
		public IntPtr workdirPath;
		public IntPtr description;
		public IntPtr templatePath;
		public IntPtr initialHead;
		public IntPtr originUrl;
	}
}
