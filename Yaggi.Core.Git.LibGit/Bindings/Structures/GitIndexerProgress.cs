using System.Runtime.InteropServices;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct GitIndexerProgress
	{
		public uint totalObjects;
		public uint indexedObjects;
		public uint receivedObjects;
		public uint localObjects;
		public uint totalDeltas;
		public uint indexedDeltas;
		public nuint receivedBytes;
	}
}
