using System;
using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	public struct GitRepositoryInitOptions
	{
		public uint version;
		public GitRepositoryInitFlag flags;
		public GitRepositoryInitMode mode;
		public IntPtr workdir_path;
		public IntPtr description;
		public IntPtr template_path;
		public IntPtr initial_head;
		public IntPtr origin_url;
	}
}
