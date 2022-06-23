using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvertedTomato.IO.Bits;
using Xunit;

namespace InvertedTomato.Compression.Integers.Legacy {
	public class RawCodecTests { // TODO: Check Compress and Decompress's "used" value is correct
		private readonly Codec Codec = new RawCodec();

		public String CompressMany(UInt64[] input, Int32 outputBufferSize = 8) {
			var output = new MemoryStream(outputBufferSize);
			Codec.EncodeMany(output, input);

			return output.ToArray().ToBinaryString();
		}

		public String CompressOne(UInt64 value, Int32 outputBufferSize = 8) {
			return CompressMany(new[] {value}, outputBufferSize);
		}

		[Fact]
		public void Compress_Empty() {
			Assert.Equal("", CompressMany(new UInt64[] { }));
		}

		[Fact]
		public void Compress_0() {
			Assert.Equal("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(0));
		}

		[Fact]
		public void Compress_1() {
			Assert.Equal("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(1));
		}

		[Fact]
		public void Compress_2() {
			Assert.Equal("00000010 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(2));
		}

		[Fact]
		public void Compress_3() {
			Assert.Equal("00000011 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(3));
		}

		[Fact]
		public void Compress_Max() {
			Assert.Equal("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111", CompressOne(UInt64.MaxValue));
		}

		[Fact]
		public void Compress_1_1_1() {
			Assert.Equal("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressMany(new UInt64[] {1, 1, 1}, 3 * 8));
		}


		private UInt64[] DecompressMany(String value, Int32 count) {
			var input = new MemoryStream(BitOperation.ParseToBytes(value));
			var codec = new RawCodec();
			var output = new UInt64[count];
			codec.DecodeMany(input, output);

			return output;
		}

		private UInt64 DecompressOne(String value) {
			var set = DecompressMany(value, 1);
			return set[0];
		}

		[Fact]
		public void Decompress_Empty() {
			Assert.Empty(DecompressMany("", 0));
		}

		[Fact]
		public void Decompress_0() {
			Assert.Equal((UInt64) 0, DecompressOne("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
		}

		[Fact]
		public void Decompress_1() {
			Assert.Equal((UInt64) 1, DecompressOne("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
		}

		[Fact]
		public void Decompress_2() {
			Assert.Equal((UInt64) 2, DecompressOne("00000010 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
		}

		[Fact]
		public void Decompress_3() {
			Assert.Equal((UInt64) 3, DecompressOne("00000011 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
		}

		[Fact]
		public void Decompress_Max() {
			Assert.Equal(RawCodec.MaxValue, DecompressOne("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111"));
		}

		[Fact]
		public void Decompress_1_1_1() {
			var set = DecompressMany("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", 3);
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
		public void CompressDecompress_First1000_Series() {
			for (UInt64 input = 0; input < 1000; input++) {
				var encoded = CompressOne(input);
				var output = DecompressOne(encoded);

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