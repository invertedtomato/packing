using NUnit.Framework;

namespace InvertedTomato.Tests {
	[TestFixture]
	public class GetBytesExtensionsTests {
		/// <summary>
		/// Tests that bools convert to bytes in the expected manner
		/// </summary>
		[Test]
		public void BooleanGetBytes() {
			Assert.AreEqual(new byte[] { 1 }, true.GetBytes());
			Assert.AreEqual(new byte[] { 0 }, false.GetBytes());
		}
	}
}