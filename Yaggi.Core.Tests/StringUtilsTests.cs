using System;
using System.Runtime.InteropServices;
using Xunit;
using Yaggi.Core.Marshaling;

namespace Yaggi.Core.Tests
{
	public unsafe class StringUtilsTests
	{
		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(8)]
		[InlineData(10)]
		[InlineData(16)]
		[InlineData(256)]
		[InlineData(1024)]
		public void SinglebyteStrlenTest(int size)
		{
			IntPtr ptr = Marshal.AllocCoTaskMem(size + 1);
			if (ptr == IntPtr.Zero)
				throw new NullReferenceException();
			try
			{
				new Span<byte>(ptr.ToPointer(), size).Fill(61);
				// ReSharper disable once PossibleNullReferenceException
				((byte*)ptr)[size] = 0;
				Assert.Equal(size, StringUtils.Strlen((byte*)ptr));
			}
			finally
			{
				Marshal.FreeCoTaskMem(ptr);
			}
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(8)]
		[InlineData(10)]
		[InlineData(16)]
		[InlineData(256)]
		[InlineData(1024)]
		public void DualbyteStrlenTest(int size)
		{
			IntPtr ptr = Marshal.AllocCoTaskMem((size + 1) * sizeof(char));
			if (ptr == IntPtr.Zero)
				throw new NullReferenceException();
			try
			{
				new Span<char>(ptr.ToPointer(), size).Fill('a');
				// ReSharper disable once PossibleNullReferenceException
				((char*)ptr)[size] = '\0';
				Assert.Equal(size, StringUtils.Strlen((char*)ptr));
			}
			finally
			{
				Marshal.FreeCoTaskMem(ptr);
			}
		}
	}
}
