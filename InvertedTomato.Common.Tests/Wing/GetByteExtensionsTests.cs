using NUnit.Framework;

namespace InvertedTomato.Feather.Tests {
	[TestFixture()]
	public class GetByteExtensionsTests {
		[Test()]
		public void GetBytesSByteTest() {
			Assert.AreEqual(new byte[] { 128 }, ((sbyte)0).GetBytes());
			Assert.AreEqual(new byte[] { 0x00 }, ((sbyte)-128).GetBytes());
			Assert.AreEqual(new byte[] { 0xFF }, ((sbyte)127).GetBytes());
		}
	}
}