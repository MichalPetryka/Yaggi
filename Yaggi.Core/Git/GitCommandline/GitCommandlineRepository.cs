using System;
using System.Text;

namespace Yaggi.Core.Git.GitCommandline
{
	public class GitCommandlineRepository : GitRepository
	{
		internal GitCommandlineRepository(string path) : base(path) { }

		public override GitRemote[] Remotes
		{
			get
			{
				CommandlineUtils.CreateProcess("git", "remote", Encoding.UTF8, out string output, out _, Path);
				string[] names = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				GitRemote[] remotes = new GitRemote[names.Length];
				for (int i = 0; i < names.Length; i++)
					TrackRemote(remotes[i] = new GitCommandlineRemote(names[i], this));

				return remotes;
			}
		}

		protected override void Dispose(bool disposing) { }
	}
}
