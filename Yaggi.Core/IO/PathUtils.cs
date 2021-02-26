using System;
using System.Buffers;
using System.ComponentModel;
using System.IO;
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
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IntPtr handle = CreateFile(path, 0x80000000, 0x00000001 | 0x00000002, IntPtr.Zero, 3, 0x80, IntPtr.Zero);
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
					path = normalizedPath.ToString();
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
			else
			{
				string realPath = RealPath(path, IntPtr.Zero);
				if (realPath != null)
					path = realPath;
			}
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
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				IntPtr handle = CreateFile(path, 0x80000000, 0x00000001 | 0x00000002, IntPtr.Zero, 3, 0x80 | 0x02000000, IntPtr.Zero);
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
					path = normalizedPath.ToString();
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
			else
			{
				string realPath = RealPath(path, IntPtr.Zero);
				if (realPath != null)
					path = realPath;
			}
			path = Path.GetFullPath(path);
			if (path.Length > MaxPath)
				throw new Exception();
			return path;
		}

		[DllImport("Kernel32", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr CreateFile(string path, uint access, uint sharemode, IntPtr security, uint disposition, uint flags, IntPtr template);

		[DllImport("Kernel32", EntryPoint = "GetFinalPathNameByHandleW", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern uint GetFinalPathNameByHandle(IntPtr handle, char* path, uint length, uint flags);

		[DllImport("Kernel32", EntryPoint = "CloseHandle", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		private static extern int CloseHandle(IntPtr handle);

		[DllImport("libc", EntryPoint = "realpath", ExactSpelling = true, BestFitMapping = false)]
		[return: MarshalAs(UnmanagedType.LPUTF8Str)]
		private static extern string RealPath([MarshalAs(UnmanagedType.LPUTF8Str)] string path, IntPtr resolvedPath);
	}
}
