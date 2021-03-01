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
		public void CalculateCrc32CTest(string data, uint expected)
		{
			const uint poly = 0x82F63B78;
			byte[] bytes = Encoding.UTF8.GetBytes(data);

			CrcManaged crcManaged = new(poly);
			Assert.Equal(expected, crcManaged.Calculate(bytes));
			Assert.Equal(poly, crcManaged.Polynomial);

			if (Crc32CHardware64.Supported)
			{
				Crc32CHardware64 crc32CHardware64 = new();
				Assert.Equal(expected, crc32CHardware64.Calculate(bytes));
				Assert.Equal(poly, crc32CHardware64.Polynomial);
			}

			if (Crc32CHardware32.Supported)
			{
				Crc32CHardware32 crc32CHardware32 = new();
				Assert.Equal(expected, crc32CHardware32.Calculate(bytes));
				Assert.Equal(poly, crc32CHardware32.Polynomial);
			}

			if (Crc32CSoftware64.Supported)
			{
				Crc32CSoftware64 crc32CSoftware64 = new();
				Assert.Equal(expected, crc32CSoftware64.Calculate(bytes));
				Assert.Equal(poly, crc32CSoftware64.Polynomial);
			}

			if (Crc32CSoftware32.Supported)
			{
				Crc32CSoftware32 crc32CSoftware32 = new();
				Assert.Equal(expected, crc32CSoftware32.Calculate(bytes));
				Assert.Equal(poly, crc32CSoftware32.Polynomial);
			}

			if (Crc32CSse64.Supported)
			{
				Crc32CSse64 crc32CSse64 = new();
				Assert.Equal(expected, crc32CSse64.Calculate(bytes));
				Assert.Equal(poly, crc32CSse64.Polynomial);
			}

			if (Crc32CSse32.Supported)
			{
				Crc32CSse32 crc32CSse32 = new();
				Assert.Equal(expected, crc32CSse32.Calculate(bytes));
				Assert.Equal(poly, crc32CSse32.Polynomial);
			}

			if (Crc32CArm64.Supported)
			{
				Crc32CArm64 crc32CArm64 = new();
				Assert.Equal(expected, crc32CArm64.Calculate(bytes));
				Assert.Equal(poly, crc32CArm64.Polynomial);
			}

			if (Crc32CArm32.Supported)
			{
				Crc32CArm32 crc32CArm32 = new();
				Assert.Equal(expected, crc32CArm32.Calculate(bytes));
				Assert.Equal(poly, crc32CArm32.Polynomial);
			}

			Assert.NotNull(Crc.Crc32C);
			Assert.Equal(expected, Crc.Crc32C.Calculate(bytes));
			Assert.Equal(poly, Crc.Crc32C.Polynomial);
		}

		[Theory]
		[InlineData("", 0)]
		[InlineData("0", 0xF4DBDF21)]
		[InlineData("aaaaaaaaaaaaaaaaaaa", 0xB9D11277)]
		[InlineData("999999999999999999999999999999999999999999999999999999999999999999999999999", 0x39A59BA4)]
		public void CalculateCrc32Test(string data, uint expected)
		{
			const uint poly = 0xEDB88320;
			byte[] bytes = Encoding.UTF8.GetBytes(data);

			CrcManaged crcManaged = new(poly);
			Assert.Equal(expected, crcManaged.Calculate(bytes));
			Assert.Equal(poly, crcManaged.Polynomial);

			if (Crc32Arm64.Supported)
			{
				Crc32Arm64 crc32Arm64 = new();
				Assert.Equal(expected, crc32Arm64.Calculate(bytes));
				Assert.Equal(poly, crc32Arm64.Polynomial);
			}

			if (Crc32Arm32.Supported)
			{
				Crc32Arm32 crc32Arm32 = new();
				Assert.Equal(expected, crc32Arm32.Calculate(bytes));
				Assert.Equal(poly, crc32Arm32.Polynomial);
			}

			Assert.NotNull(Crc.Crc32);
			Assert.Equal(expected, Crc.Crc32.Calculate(bytes));
			Assert.Equal(poly, Crc.Crc32.Polynomial);
		}
	}
}
