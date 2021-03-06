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
			StringBuilder stringBuilder = StringBuilderPool.Rent();
			Assert.NotNull(stringBuilder);
			Assert.Equal(0, stringBuilder.Length);
			StringBuilderPool.Return(stringBuilder);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(256)]
		[InlineData(512)]
		[InlineData(1024)]
		public void CapacityTest(int capacity)
		{
			StringBuilder stringBuilder = StringBuilderPool.Rent(capacity);
			Assert.NotNull(stringBuilder);
			Assert.Equal(0, stringBuilder.Length);
			Assert.True(stringBuilder.Capacity >= capacity);
			StringBuilderPool.Return(stringBuilder);
		}

		[Theory]
		[InlineData("")]
		[InlineData("test 1")]
		[InlineData("test \n \0 \r")]
		public void TextTest(string input)
		{
			StringBuilder stringBuilder = StringBuilderPool.Rent(input);
			Assert.Equal(input, stringBuilder.ToString());
			StringBuilderPool.Return(stringBuilder);
		}

		[Theory]
		[InlineData("")]
		[InlineData("test 1")]
		[InlineData("test \n \0 \r")]
		public void ToStringReturnTest(string input)
		{
			StringBuilder stringBuilder = StringBuilderPool.Rent(input);
			Assert.Equal(input, StringBuilderPool.ToStringReturn(stringBuilder));
		}
	}
}
