using System;
using InvertedTomato.Compression.Integers.Wave2;
using Xunit;

namespace InvertedTomato.Compression.Integers.Wave2 {
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