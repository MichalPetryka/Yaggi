using System;
using System.IO;

namespace Yaggi.Core.Git
{
	public abstract class GitRepository : IDisposable
	{
		public string Path { get; }

		protected GitRepository(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			path = System.IO.Path.GetFullPath(path);
			if (!Directory.Exists(path))
				throw new ArgumentException($"Directory \"{path}\" does not exist");
			if (!Directory.Exists(System.IO.Path.Combine(path, ".git")))
				throw new ArgumentException($"Directory \"{path}\" is not a git repository");
			Path = path;
		}

		protected abstract void Dispose(bool disposing);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GitRepository()
		{
			Dispose(false);
		}
	}
}
