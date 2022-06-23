using System;
using Xunit;

namespace InvertedTomato.Compression.Integers.Legacy {
	public class ZigZagTests {
		[Fact]
		public void EncodeDecodeMax() {
			var encoded = ZigZag.Encode(Int64.MaxValue);
			Assert.Equal(Int64.MaxValue, ZigZag.Decode(encoded));
		}
		
		[Fact]
		public void EncodeDecodeMin() {
			var encoded = ZigZag.Encode(Int64.MinValue+1);
			Assert.Equal(Int64.MinValue+1, ZigZag.Decode(encoded));
		}
	}
}