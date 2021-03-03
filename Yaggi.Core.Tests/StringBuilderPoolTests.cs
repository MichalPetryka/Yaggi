using System.Text;
using Xunit;
using Yaggi.Core.Memory;

namespace Yaggi.Core.Tests
{
	public class StringBuilderPoolTests
	{
		[Fact]
		public void ValidTest()
		{
			StringBuilder sb = StringBuilderPool.Rent();
			Assert.NotNull(sb);
			Assert.Equal(0, sb.Length);
			StringBuilderPool.Return(sb);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(256)]
		[InlineData(512)]
		[InlineData(1024)]
		public void CapacityTest(int capacity)
		{
			StringBuilder sb = StringBuilderPool.Rent(capacity);
			Assert.NotNull(sb);
			Assert.Equal(0, sb.Length);
			Assert.True(sb.Capacity >= capacity);
			StringBuilderPool.Return(sb);
		}

		[Theory]
		[InlineData("")]
		[InlineData("test 1")]
		[InlineData("test \n \0 \r")]
		public void TextTest(string input)
		{
			StringBuilder sb = StringBuilderPool.Rent(input);
			Assert.Equal(input, sb.ToString());
			StringBuilderPool.Return(sb);
		}

		[Theory]
		[InlineData("")]
		[InlineData("test 1")]
		[InlineData("test \n \0 \r")]
		public void ToStringReturnTest(string input)
		{
			StringBuilder sb = StringBuilderPool.Rent(input);
			Assert.Equal(input, StringBuilderPool.ToStringReturn(sb));
		}
	}
}
