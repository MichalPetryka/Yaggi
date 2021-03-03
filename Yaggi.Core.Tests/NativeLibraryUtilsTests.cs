using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using Yaggi.Core.Marshaling;

namespace Yaggi.Core.Tests
{
	public class NativeLibraryUtilsTests
	{
		public static IEnumerable<object[]> ValidCases =>
			new List<object[]>
			{
				new object[] { null, null, "Kernel32.dll", "libc.so", "libc.dylib", "libc" },
				new object[] { "Kernel32.dll", "libc.so", "libc.dylib", "libc", null, null },
				new object[] { "Kernel32.dll", "ntdll.dll", "libc.so", "libdl.so", "libc.dylib", "libdl.dylib", "libc", "libdl" },
			};

		[Theory]
		[MemberData(nameof(ValidCases))]
		public void LoadTest(params string[] names)
		{
			IntPtr module = NativeLibraryUtils.LoadAny(names);
			Assert.NotEqual(IntPtr.Zero, module);
			NativeLibrary.Free(module);
		}

		[Theory]
		[InlineData("invalidname.txt", "", null, "invalidname.zip", "invalidlibrary")]
		[InlineData(null, null, null)]
		public void InvalidNamesTest(params string[] names)
		{
			Assert.Equal(names.Length, Assert.Throws<AggregateException>(() => { NativeLibraryUtils.LoadAny(names); }).InnerExceptions.Count);
		}

		[Fact]
		public void NullNamesTest()
		{
			Assert.Throws<ArgumentNullException>(() => { NativeLibraryUtils.LoadAny(null); });
		}

		[Fact]
		public void EmptyNamesTest()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => { NativeLibraryUtils.LoadAny(); });
		}
	}
}
