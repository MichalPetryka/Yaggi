using Xunit;
using Yaggi.Core.IO;

namespace Yaggi.Core.Tests
{
	public class CommandlineUtilsTests
	{
		[Theory]
		[InlineData("", @"""")]
		[InlineData(" ", "\" \"")]
		[InlineData("\" \"", "\"\\\" \\\"\"")]
		[InlineData(@"C:\test directory", "\"C:\\test directory\"")]
		[InlineData(@"C:\test directory\", "\"C:\\test directory\\\\\"")]
		[InlineData("a", "a")]
		public void EscapeTest(string text, string expected)
		{
			Assert.Equal(expected, CommandlineUtils.EscapeArgument(text));
		}
	}
}
