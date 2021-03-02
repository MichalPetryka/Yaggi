using System;
using System.Buffers;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Yaggi.Core.IO
{
	public static unsafe class PathUtils
	{
		private static readonly IntPtr InvalidHandle = new(-1);
		private const int MaxPath = 4096;

		public static string NormalizeFilePath(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path), "Path is null");
			if (!File.Exists(path))
				throw new FileNotFoundException("File does not exist", path);
			path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			path = GetRealPath(path, false);
			path = Path.GetFullPath(path);
			if (path.Length > MaxPath)
				throw new Exception();
			return path;
		}

		public static string NormalizeDirectoryPath(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path), "Path is null");
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException("Directory does not exist");
			path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			path = path.TrimEnd(Path.DirectorySeparatorChar);
			path = GetRealPath(path, true);
			path = Path.GetFullPath(path);
			if (path.Length > MaxPath)
				throw new Exception();
			return path;
		}

		private static string GetRealPath(string path, bool isDirectory)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IntPtr handle = CreateFile(path, 0x80000000, 0x00000001 | 0x00000002, IntPtr.Zero, 3,
					0x80u | (isDirectory ? 0x02000000u : 0u), IntPtr.Zero);
				if (handle == InvalidHandle)
					throw new Win32Exception();
				char[] buffer = ArrayPool<char>.Shared.Rent(MaxPath + 5);
				try
				{
					uint length;
					fixed (char* ptr = buffer)
						length = GetFinalPathNameByHandle(handle, ptr, (uint)buffer.Length, 0);
					if (length == 0)
						throw new Win32Exception();
					if (length >= buffer.Length)
						throw new Exception();
					ReadOnlySpan<char> normalizedPath = new(buffer, 0, (int)length);
					if (normalizedPath.StartsWith(@"\\?\"))
						normalizedPath = normalizedPath.Slice(4);
					return normalizedPath.ToString();
				}
				finally
				{
					ArrayPool<char>.Shared.Return(buffer);
#pragma warning disable CA2219
					if (CloseHandle(handle) == 0)
						throw new Win32Exception();
#pragma warning restore CA2219
				}
			}

			string realPath = RealPath(path, IntPtr.Zero);
			path = realPath ?? throw new Win32Exception();
			path = Path.GetFullPath(path);

			string directoryName = Path.GetDirectoryName(path);
			if (directoryName == null)
				return path;
			string basePath = GetRealPath(directoryName, true);
			string name = Path.GetFileName(path);

			if (isDirectory)
			{
				if (!Directory.Exists(Path.Combine(basePath, name)))
					throw new DirectoryNotFoundException();
				name = Directory.EnumerateDirectories(basePath, name).FirstOrDefault() ?? Directory
				.EnumerateDirectories(basePath).First(s =>
						string.Equals(name, Path.GetFileName(s), StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				if (!File.Exists(Path.Combine(basePath, name)))
					throw new FileNotFoundException();
				name = Directory.EnumerateFiles(basePath, name).FirstOrDefault() ??
						Directory.EnumerateFiles(basePath).First(s =>
							string.Equals(name, Path.GetFileName(s), StringComparison.OrdinalIgnoreCase));
			}

			return Path.Combine(basePath, name);
		}

		[DllImport("Kernel32", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr CreateFile(string path, uint access, uint sharemode, IntPtr security, uint disposition, uint flags, IntPtr template);

		[DllImport("Kernel32", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern uint GetFinalPathNameByHandle(IntPtr handle, char* path, uint length, uint flags);

		[DllImport("Kernel32", EntryPoint = "CloseHandle", ExactSpelling = true, SetLastError = true)]
		private static extern int CloseHandle(IntPtr handle);

		[DllImport("libc", EntryPoint = "realpath", ExactSpelling = true, BestFitMapping = false, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.LPUTF8Str)]
		private static extern string RealPath([MarshalAs(UnmanagedType.LPUTF8Str)] string path, IntPtr resolvedPath);
	}
}
