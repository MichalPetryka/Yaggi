using System;
using System.Runtime.InteropServices;
using Xunit;
using Yaggi.Core.Interop;

namespace Yaggi.Core.Tests
{
	public unsafe class NativeModuleTests
	{
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate int GetProcessId();

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void LoadTest(bool free)
		{
			string name = OperatingSystem.IsWindows() ? "Kernel32.dll" : "libc.so";
			NativeModule nativeModule;
			using (nativeModule = new NativeModule(name, free))
			{
				Assert.NotEqual(IntPtr.Zero, nativeModule.Module);
				Assert.True(nativeModule.Valid);
				Assert.False(string.IsNullOrEmpty(nativeModule.Name));
				Assert.Equal(name, nativeModule.Name);
				string symbolName = nativeModule.Name == "Kernel32.dll" ? "GetCurrentProcessId" : "getpid";
				Assert.Equal(Environment.ProcessId, ((delegate* unmanaged<int>)nativeModule.GetSymbol(symbolName))());
				Assert.Equal(Environment.ProcessId, nativeModule.GetDelegate<GetProcessId>(symbolName)());
				Assert.Equal(Environment.ProcessId, ((GetProcessId)nativeModule.GetDelegate(symbolName, typeof(GetProcessId)))());
			}
			Assert.Equal(!free, nativeModule.Valid);
		}

		[Theory]
		[InlineData(null, null, "Kernel32.dll", "libc.so", "libc.dylib", "libc")]
		[InlineData("Kernel32.dll", "libc.so", "libc.dylib", "libc", null, null)]
		[InlineData("Kernel32.dll", "libc.so", "libc.dylib", "libc")]
		public void LoadAnyTest(params string[] names)
		{
			NativeModule nativeModule;
			using (nativeModule = new NativeModule(names))
			{
				Assert.NotEqual(IntPtr.Zero, nativeModule.Module);
				Assert.True(nativeModule.Valid);
				Assert.False(string.IsNullOrEmpty(nativeModule.Name));
				Assert.Contains(nativeModule.Name, names);
				string symbolName = nativeModule.Name == "Kernel32.dll" ? "GetCurrentProcessId" : "getpid";
				Assert.Equal(Environment.ProcessId, ((delegate* unmanaged<int>)nativeModule.GetSymbol(symbolName))());
				Assert.Equal(Environment.ProcessId, nativeModule.GetDelegate<GetProcessId>(symbolName)());
				Assert.Equal(Environment.ProcessId, ((GetProcessId)nativeModule.GetDelegate(symbolName, typeof(GetProcessId)))());
			}
			Assert.False(nativeModule.Valid);
		}

		[Theory]
		[InlineData(null, null, "Kernel32.dll", "libc.so", "libc.dylib", "libc")]
		[InlineData("Kernel32.dll", "libc.so", "libc.dylib", "libc", null, null)]
		[InlineData("Kernel32.dll", "libc.so", "libc.dylib", "libc")]
		public void LoadAnyNoFreeTest(params string[] names)
		{
			NativeModule nativeModule;
			using (nativeModule = new NativeModule(false, names))
			{
				Assert.NotEqual(IntPtr.Zero, nativeModule.Module);
				Assert.True(nativeModule.Valid);
				Assert.False(string.IsNullOrEmpty(nativeModule.Name));
				Assert.Contains(nativeModule.Name, names);
				string symbolName = nativeModule.Name == "Kernel32.dll" ? "GetCurrentProcessId" : "getpid";
				Assert.Equal(Environment.ProcessId, ((delegate* unmanaged<int>)nativeModule.GetSymbol(symbolName))());
				Assert.Equal(Environment.ProcessId, nativeModule.GetDelegate<GetProcessId>(symbolName)());
				Assert.Equal(Environment.ProcessId, ((GetProcessId)nativeModule.GetDelegate(symbolName, typeof(GetProcessId)))());
			}
			Assert.True(nativeModule.Valid);
		}

		[Theory]
		[InlineData("invalidname.txt", "", null, "invalidname.zip", "invalidlibrary")]
		[InlineData(null, null, null)]
		public void InvalidNamesTest(params string[] names)
		{
			Assert.Equal(names.Length, Assert.Throws<AggregateException>(() => new NativeModule(names)).InnerExceptions.Count);
		}

		[Fact]
		public void NullNameTest()
		{
			Assert.Throws<ArgumentNullException>(() => new NativeModule((string)null));
		}

		[Fact]
		public void EmptyNameTest()
		{
			Assert.Throws<ArgumentException>(() => new NativeModule(""));
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
