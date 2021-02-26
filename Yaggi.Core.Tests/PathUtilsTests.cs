using System;
using System.IO;
using Xunit;
using Yaggi.Core.IO;

namespace Yaggi.Core.Tests
{
	public class PathUtilsTests
	{
		private readonly bool _insensitiveCasing;

		public PathUtilsTests()
		{
			string name = Guid.NewGuid() + "abcde";
			File.Create(name.ToLowerInvariant()).Close();
			_insensitiveCasing = File.Exists(name.ToUpperInvariant());
			File.Delete(name.ToLowerInvariant());
		}

		[Fact]
		public void ValidateFilePathTest()
		{
			foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories))
			{
				string path = PathUtils.NormalizeFilePath(file);
				Assert.True(File.Exists(path), $"\"{file}\" - \"{path}\"");
				if (_insensitiveCasing)
					Assert.Equal(PathUtils.NormalizeFilePath(file.ToLowerInvariant()), PathUtils.NormalizeFilePath(file.ToUpperInvariant()));
			}
		}

		[Fact]
		public void ValidateDirectoryPathTest()
		{
			foreach (string directory in Directory.EnumerateDirectories(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories))
			{
				string path = PathUtils.NormalizeDirectoryPath(directory);
				Assert.True(Directory.Exists(path), $"\"{directory}\" - \"{path}\"");
				string path2 = PathUtils.NormalizeDirectoryPath(directory + Path.DirectorySeparatorChar);
				Assert.True(Directory.Exists(path2), $"\"{directory}\" - \"{path2}\"");
				Assert.Equal(path, path2);
				if (_insensitiveCasing)
					Assert.Equal(PathUtils.NormalizeDirectoryPath(directory.ToLowerInvariant()), PathUtils.NormalizeDirectoryPath(directory.ToUpperInvariant()));
			}
		}
	}
}
