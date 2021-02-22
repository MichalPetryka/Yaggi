using System.Runtime.InteropServices;
using Yaggi.Core.Git.LibGit.Bindings.Enums;

namespace Yaggi.Core.Git.LibGit.Bindings.Structures
{
	[StructLayout(LayoutKind.Sequential)]
	public struct GitFetchOptions
	{
		public int version;
		public GitRemoteCallbacks callbacks;
		public GitFetchPrune prune;
		public int updateFetchhead;
		public GitRemoteAutotagOptions downloadTags;
		public GitProxyOptions proxyOptions;
		public GitStrArray customHeaders;
	}
}
