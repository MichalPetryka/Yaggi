using System.Text;
using Xunit;
using Yaggi.Core.Cryptography.Crc32C;

namespace Yaggi.Core.Tests
{
	public class CrcTests
	{
		[Theory]
		[InlineData("", 0)]
		[InlineData("0", 0x629E1AE0)]
		[InlineData("aaaaaaaaaaaaaaaaaaa", 0xA7CD6940)]
		[InlineData("999999999999999999999999999999999999999999999999999999999999999999999999999", 0xF61D331E)]
		public void CalculateTest(string data, uint expected)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);

			Assert.Equal(expected, new Crc32CManaged().Calculate(bytes));
			if (Crc32CSse64.Supported)
				Assert.Equal(expected, new Crc32CSse64().Calculate(bytes));
			if (Crc32CSse32.Supported)
				Assert.Equal(expected, new Crc32CSse32().Calculate(bytes));
			if (Crc32CArm64.Supported)
				Assert.Equal(expected, new Crc32CArm64().Calculate(bytes));
			if (Crc32CArm32.Supported)
				Assert.Equal(expected, new Crc32CArm32().Calculate(bytes));

			Assert.NotNull(Crc32C.Shared);
			Assert.Equal(expected, Crc32C.Shared.Calculate(bytes));
		}
	}
}
