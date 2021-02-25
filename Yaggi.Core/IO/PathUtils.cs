using System;
using System.IO;

namespace Yaggi.Core.IO
{
	public static class PathUtils
	{
		public static string ValidateFilePath(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path), "Path is null");
			if (!File.Exists(path))
				throw new FileNotFoundException("File does not exist", path);
			path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			return Path.GetFullPath(path);
		}

		public static string ValidateDirectoryPath(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path), "Path is null");
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException("Directory does not exist");
			path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			return Path.GetFullPath(path);
		}
	}
}
