using System;
using Xunit;
using Yaggi.Core.Interop;

namespace Yaggi.Core.Tests
{
	public class NativeModuleTests
	{
		[Theory]
		[InlineData(null, null, "Kernel32.dll", "libc.so", "libc.dylib", "libc")]
		[InlineData("Kernel32.dll", "libc.so", "libc.dylib", "libc", null, null)]
		[InlineData("Kernel32.dll", "ntdll.dll", "libc.so", "libdl.so", "libc.dylib", "libdl.dylib", "libc", "libdl")]
		public void LoadTest(params string[] names)
		{
			using (NativeModule nativeModule = new(names))
			{
				Assert.NotEqual(IntPtr.Zero, nativeModule.Module);
				Assert.False(string.IsNullOrEmpty(nativeModule.Name));
				Assert.Contains(nativeModule.Name, names);
			}
		}

		[Theory]
		[InlineData("invalidname.txt", "", null, "invalidname.zip", "invalidlibrary")]
		[InlineData(null, null, null)]
		public void InvalidNamesTest(params string[] names)
		{
			Assert.Equal(names.Length, Assert.Throws<AggregateException>(() => new NativeModule(names)).InnerExceptions.Count);
		}

		[Fact]
		public void NullNamesTest()
		{
			Assert.Throws<ArgumentNullException>(() => new NativeModule(null));
		}

		[Fact]
		public void EmptyNamesTest()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new NativeModule());
		}
	}
}
