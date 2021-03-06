using System.Runtime.InteropServices;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	internal readonly struct GitSignature
	{
		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public readonly string Name;
		[MarshalAs(UnmanagedType.LPUTF8Str)]
		public readonly string Email;
		public readonly GitTime When;
	}
}
