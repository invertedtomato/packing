using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvertedTomato.IO.Bits;
using Xunit;

namespace InvertedTomato.Compression.Integers.Legacy {
	public class InvertedVlqCodecTests {
		private readonly Codec Codec = new InvertedVlqCodec();

		public String CompressMany(UInt64[] set, Int32 expectedCount) {
			var output = new MemoryStream(expectedCount);
			Codec.EncodeMany(output, set);

			return output.ToArray().ToBinaryString();
		}

		public String CompressOne(UInt64 value, Int32 expectedCount) {
			return CompressMany(new[] {value}, expectedCount);
		}

		[Fact]
		public void Compress_Empty() {
			Assert.Equal("", CompressMany(new UInt64[] { }, 0));
		}

		[Fact]
		public void Compress_0() {
			Assert.Equal("10000000", CompressOne(0, 1));
		}

		[Fact]
		public void Compress_1() {
			Assert.Equal("10000001", CompressOne(1, 1));
		}

		[Fact]
		public void Compress_2() {
			Assert.Equal("10000010", CompressOne(2, 1));
		}

		[Fact]
		public void Compress_3() {
			Assert.Equal("10000011", CompressOne(3, 1));
		}

		[Fact]
		public void Compress_127() {
			Assert.Equal("11111111", CompressOne(127, 1));
		}

		[Fact]
		public void Compress_128() {
			Assert.Equal("00000000 10000000", CompressOne(128, 2));
		}

		[Fact]
		public void Compress_129() {
			Assert.Equal("00000001 10000000", CompressOne(129, 2));
		}

		[Fact]
		public void Compress_16511() {
			Assert.Equal("01111111 11111111", CompressOne(16511, 2));
		}

		[Fact]
		public void Compress_16512() {
			Assert.Equal("00000000 00000000 10000000", CompressOne(16512, 3));
		}

		[Fact]
		public void Compress_2113663() {
			Assert.Equal("01111111 01111111 11111111", CompressOne(2113663, 3));
		}

		[Fact]
		public void Compress_2113664() {
			Assert.Equal("00000000 00000000 00000000 10000000", CompressOne(2113664, 4));
		}

		[Fact]
		public void Compress_Max() {
			Assert.Equal("01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", CompressOne(InvertedVlqCodec.MaxValue, 10));
		}

		[Fact]
		public void Compress_Overflow() {
			Assert.Throws<OverflowException>(() => { CompressOne(UInt64.MaxValue, 32); });
		}

		[Fact]
		public void Compress_1_1_1() {
			Assert.Equal("10000001 10000001 10000001", CompressMany(new UInt64[] {1, 1, 1}, 3));
		}

		[Fact]
		public void Compress_128_128_128() {
			Assert.Equal("00000000 10000000 00000000 10000000 00000000 10000000", CompressMany(new UInt64[] {128, 128, 128}, 6));
		}

		[Fact]
		public void Compress_OutputPerfectSize() {
			var input = new UInt64[] {128};
			var output = new MemoryStream();
			Codec.EncodeMany(output, input);
			Assert.Equal("00000000 10000000", output.ToArray().ToBinaryString());
		}


		private UInt64[] DecompressMany(String value, Int32 count, Int32 expectedUsed) {
			var input = new MemoryStream(BitOperation.ParseToBytes(value));
			var output = new UInt64[count];
			 Codec.DecodeMany(input,  output);
			return output.ToArray();
		}

		private UInt64 DecompressOne(String value, Int32 expectedUsed) {
			var output = DecompressMany(value, 1, expectedUsed);
			Assert.Single(output);
			return output[0];
		}

		[Fact]
		public void Decompress_Empty() {
			Assert.Empty(DecompressMany("", 0, 0));
		}

		[Fact]
		public void Decompress_0() {
			Assert.Equal((UInt64) 0, DecompressOne("10000000", 1));
		}

		[Fact]
		public void Decompress_1() {
			Assert.Equal((UInt64) 1, DecompressOne("10000001", 1));
		}

		[Fact]
		public void Decompress_2() {
			Assert.Equal((UInt64) 2, DecompressOne("10000010", 1));
		}

		[Fact]
		public void Decompress_3() {
			Assert.Equal((UInt64) 3, DecompressOne("10000011", 1));
		}

		[Fact]
		public void Decompress_127() {
			Assert.Equal((UInt64) 127, DecompressOne("11111111", 1));
		}

		[Fact]
		public void Decompress_128() {
			Assert.Equal((UInt64) 128, DecompressOne("00000000 10000000", 2));
		}

		[Fact]
		public void Decompress_129() {
			Assert.Equal((UInt64) 129, DecompressOne("00000001 10000000", 2));
		}

		[Fact]
		public void Decompress_16511() {
			Assert.Equal((UInt64) 16511, DecompressOne("01111111 11111111", 2));
		}

		[Fact]
		public void Decompress_16512() {
			Assert.Equal((UInt64) 16512, DecompressOne("00000000 00000000 10000000", 3));
		}

		[Fact]
		public void Decompress_16513() {
			Assert.Equal((UInt64) 16513, DecompressOne("00000001 00000000 10000000", 3));
		}

		[Fact]
		public void Decompress_2113663() {
			Assert.Equal((UInt64) 2113663, DecompressOne("01111111 01111111 11111111", 3));
		}

		[Fact]
		public void Decompress_2113664() {
			Assert.Equal((UInt64) 2113664, DecompressOne("00000000 00000000 00000000 10000000", 4));
		}

		[Fact]
		public void Decompress_Max() {
			Assert.Equal(InvertedVlqCodec.MaxValue, DecompressOne("01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", 10));
		}

		[Fact]
		public void Decompress_1_1_1() {
			var set = DecompressMany("10000001 10000001 10000001", 3, 3);
			Assert.Equal(3, set.Length);
			Assert.Equal((UInt64) 1, set[0]);
			Assert.Equal((UInt64) 1, set[1]);
			Assert.Equal((UInt64) 1, set[2]);
		}

		[Fact]
		public void Decompress_InputClipped() {
			Assert.Throws<EndOfStreamException>(() => {
				var input = new MemoryStream(BitOperation.ParseToBytes("00000000"));
				var output = new UInt64[1];
				Codec.DecodeMany(input, output);
			});
		}

		[Fact]
		public void Decompress_Overflow() {
			Assert.Throws<OverflowException>(() => { DecompressOne("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", 11); });
		}

		[Fact]
		public void Decompress_1_X() {
			var input = new MemoryStream(BitOperation.ParseToBytes("10000001 00000011"));
			Assert.Equal((UInt64) 1, Codec.DecodeSingle(input));
		}


		[Fact]
		public void CompressDecompress_First1000_Series() {
			for (UInt64 input = 0; input <= 127; input++) {
				var encoded = CompressOne(input, 1);
				var output = DecompressOne(encoded, 1);

				Assert.Equal(input, output);
			}

			for (UInt64 input = 128; input < 1000; input++) {
				var encoded = CompressOne(input, 2);
				var output = DecompressOne(encoded, 2);

				Assert.Equal(input, output);
			}
		}

		[Fact]
		public void CompressDecompress_First1000_Parallel() {
			// Create input
			var input = new List<UInt64>(1000);
			input.Seed(0, 999);

			// Compress
			var compressed = new MemoryStream();
			Codec.EncodeMany(compressed, input.ToArray());

			// Rewind stream
			compressed.Seek(0, SeekOrigin.Begin);

			// Decompress
			var output = new UInt64[input.Count];
			Codec.DecodeMany(compressed, output);

			// Validate
			for (var i = 0; i < 1000; i++) {
				Assert.Equal((UInt64) i, output[i]);
			}
		}
	}
}