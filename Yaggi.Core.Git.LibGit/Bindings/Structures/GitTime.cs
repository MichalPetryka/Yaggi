using System;
using System.Runtime.InteropServices;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	internal readonly struct GitTime
	{
		public readonly long Time;
		public readonly int Offset;
		public readonly byte Sign;

		public DateTimeOffset ToOffset() => DateTimeOffset.FromUnixTimeSeconds(Time);
	}
}
