using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvertedTomato.IO.Bits;
using Xunit;

namespace InvertedTomato.Compression.Integers.Legacy {
	public class FibonacciCodecTests {
		// TODO: Check Compress and Decompress's "used" value is correct
		public readonly Codec Codec = new FibonacciCodec();

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
			Assert.Equal("11000000", CompressOne(0));
		}

		[Fact]
		public void Compress_1() {
			Assert.Equal("01100000", CompressOne(1));
		}

		[Fact]
		public void Compress_2() {
			Assert.Equal("00110000", CompressOne(2));
		}

		[Fact]
		public void Compress_3() {
			Assert.Equal("10110000", CompressOne(3));
		}

		[Fact]
		public void Compress_4() {
			Assert.Equal("00011000", CompressOne(4));
		}

		[Fact]
		public void Compress_5() {
			Assert.Equal("10011000", CompressOne(5));
		}

		[Fact]
		public void Compress_6() {
			Assert.Equal("01011000", CompressOne(6));
		}

		[Fact]
		public void Compress_7() {
			Assert.Equal("00001100", CompressOne(7));
		}

		[Fact]
		public void Compress_8() {
			Assert.Equal("10001100", CompressOne(8));
		}

		[Fact]
		public void Compress_9() {
			Assert.Equal("01001100", CompressOne(9));
		}

		[Fact]
		public void Compress_10() {
			Assert.Equal("00101100", CompressOne(10));
		}

		[Fact]
		public void Compress_11() {
			Assert.Equal("10101100", CompressOne(11));
		}

		[Fact]
		public void Compress_12() {
			Assert.Equal("00000110", CompressOne(12));
		}

		[Fact]
		public void Compress_13() {
			Assert.Equal("10000110", CompressOne(13));
		}


		[Fact]
		public void Compress_20() {
			Assert.Equal("00000011", CompressOne(20)); // Exactly one byte
		}

		[Fact]
		public void Compress_33() {
			Assert.Equal("00000001 10000000", CompressOne(33)); // Termination bit is on next byte
		}

		[Fact]
		public void Compress_54() {
			Assert.Equal("00000000 11000000", CompressOne(54)); // Final and termination bits on next byte
		}


		[Fact]
		public void Compress_986() {
			Assert.Equal("00000000 00000011", CompressOne(986)); // Exactly one byte
		}

		[Fact]
		public void Compress_1596() {
			Assert.Equal("00000000 00000001 10000000", CompressOne(1596)); // Termination bit is on next byte
		}

		[Fact]
		public void Compress_2583() {
			Assert.Equal("00000000 00000000 11000000", CompressOne(2583)); // Final and termination bits on next byte
		}


		[Fact]
		public void Compress_20_0_1() {
			Assert.Equal("00000011 11011000", CompressMany(new UInt64[] {20, 0, 1})); // Exactly one byte
		}

		[Fact]
		public void Compress_33_0_1() {
			Assert.Equal("00000001 11101100", CompressMany(new UInt64[] {33, 0, 1})); // Termination bit is on next byte
		}

		[Fact]
		public void Compress_54_0_1() {
			Assert.Equal("00000000 11110110", CompressMany(new UInt64[] {54, 0, 1})); // Final and termination bits on next byte
		}


		[Fact]
		public void Compress_986_0_1() {
			Assert.Equal("00000000 00000011 11011000", CompressMany(new UInt64[] {986, 0, 1})); // Exactly one byte
		}

		[Fact]
		public void Compress_1596_0_1() {
			Assert.Equal("00000000 00000001 11101100", CompressMany(new UInt64[] {1596, 0, 1})); // Termination bit is on next byte
		}

		[Fact]
		public void Compress_2583_0_1() {
			Assert.Equal("00000000 00000000 11110110", CompressMany(new UInt64[] {2583, 0, 1})); // Final and termination bits on next byte
		}


		[Fact]
		public void Compress_Max() {
			Assert.Equal("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011000", CompressOne(FibonacciCodec.MaxValue, 32)); // Not completely sure about this value
		}

		[Fact]
		public void Compress_0_1_2() {
			Assert.Equal("11011001 10000000", CompressMany(new UInt64[] {0, 1, 2}));
		}

		[Fact]
		public void Compress_0_1_2_3_4_5_6() {
			Assert.Equal("11011001 11011000 11100110 10110000", CompressMany(new UInt64[] {0, 1, 2, 3, 4, 5, 6}));
		}
		
		
		[Fact]
		public void Compress_1_2_3_4_5_6() {
			Assert.Equal("01100111 01100011 10011010 11000000", CompressMany(new UInt64[] {1, 2, 3, 4, 5, 6}));
		}

		[Fact]
		public void Compress_10x1() {
			Assert.Equal("11111111 11111111 11110000", CompressMany(new UInt64[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0}));
		}

		[Fact]
		public void Compress_OutputPerfectSize() {
			var input = new UInt64[] {0, 1, 2};
			var output = new MemoryStream(2);
			var codec = new FibonacciCodec();
			codec.EncodeMany(output, input);
			Assert.Equal("11011001 10000000", output.ToArray().ToBinaryString());
			Assert.Equal(3, input.Length);
		}


		private UInt64[] DecompressMany(String value, Int32 count) {
			var input = new MemoryStream(BitOperation.ParseToBytes(value));
			var output = new UInt64[count];
			var codec = new FibonacciCodec();
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
			Assert.Equal((UInt64) 0, DecompressOne("11 000000"));
		}

		[Fact]
		public void Decompress_1() {
			Assert.Equal((UInt64) 1, DecompressOne("011 00000"));
		}

		[Fact]
		public void Decompress_2() {
			Assert.Equal((UInt64) 2, DecompressOne("0011 0000"));
		}

		[Fact]
		public void Decompress_3() {
			Assert.Equal((UInt64) 3, DecompressOne("1011 0000"));
		}

		[Fact]
		public void Decompress_4() {
			Assert.Equal((UInt64) 4, DecompressOne("00011 000"));
		}

		[Fact]
		public void Decompress_5() {
			Assert.Equal((UInt64) 5, DecompressOne("10011 000"));
		}

		[Fact]
		public void Decompress_6() {
			Assert.Equal((UInt64) 6, DecompressOne("01011 000"));
		}

		[Fact]
		public void Decompress_7() {
			Assert.Equal((UInt64) 7, DecompressOne("000011 00"));
		}

		[Fact]
		public void Decompress_8() {
			Assert.Equal((UInt64) 8, DecompressOne("100011 00"));
		}

		[Fact]
		public void Decompress_9() {
			Assert.Equal((UInt64) 9, DecompressOne("010011 00"));
		}

		[Fact]
		public void Decompress_10() {
			Assert.Equal((UInt64) 10, DecompressOne("001011 00"));
		}

		[Fact]
		public void Decompress_11() {
			Assert.Equal((UInt64) 11, DecompressOne("101011 00"));
		}


		[Fact]
		public void Decompress_20() {
			Assert.Equal((UInt64) 20, DecompressOne("00000011")); // Exactly one byte
		}

		[Fact]
		public void Decompress_33() {
			Assert.Equal((UInt64) 33, DecompressOne("000000011 0000000")); // Termination bit is on next byte
		}

		[Fact]
		public void Decompress_54() {
			Assert.Equal((UInt64) 54, DecompressOne("0000000011 000000")); // Final and termination bits on next byte
		}


		[Fact]
		public void Decompress_986() {
			Assert.Equal((UInt64) 986, DecompressOne("00000000 00000011")); // Exactly two bytes
		}

		[Fact]
		public void Decompress_1596() {
			Assert.Equal((UInt64) 1596, DecompressOne("00000000 000000011 0000000")); // Termination bit is on next byte
		}

		[Fact]
		public void Decompress_2583() {
			Assert.Equal((UInt64) 2583, DecompressOne("00000000 0000000011 000000")); // Final and termination bits on next byte
		}

		
		
		[Fact]
		public void Decompress_20_0_1() {
			var symbols = DecompressMany("00000011 11011000", 3); 
			Assert.Equal((UInt64) 20, symbols[0]);
			Assert.Equal((UInt64) 0, symbols[1]);
			Assert.Equal((UInt64) 1, symbols[2]);
		}
		
		[Fact]
		public void Decompress_33_0_1() {
			var symbols = DecompressMany("00000001 11101100", 3); 
			Assert.Equal((UInt64) 33, symbols[0]);
			Assert.Equal((UInt64) 0, symbols[1]);
			Assert.Equal((UInt64) 1, symbols[2]);
		}

		[Fact]
		public void Decompress_54_0_1() {
			var symbols = DecompressMany("00000000 11110110", 3); 
			Assert.Equal((UInt64) 54, symbols[0]);
			Assert.Equal((UInt64) 0, symbols[1]);
			Assert.Equal((UInt64) 1, symbols[2]);
		}
		

		[Fact]
		public void Decompress_986_0_1() {
			var symbols = DecompressMany("00000000 00000011 11011000", 3); 
			Assert.Equal((UInt64) 986, symbols[0]);
			Assert.Equal((UInt64) 0, symbols[1]);
			Assert.Equal((UInt64) 1, symbols[2]);
		}

		[Fact]
		public void Decompress_1596_0_1() {
			var symbols = DecompressMany("00000000 00000001 11101100", 3); 
			Assert.Equal((UInt64) 1596, symbols[0]);
			Assert.Equal((UInt64) 0, symbols[1]);
			Assert.Equal((UInt64) 1, symbols[2]);
		}
		
		[Fact]
		public void Decompress_2583_0_1() {
			var symbols = DecompressMany("00000000 00000000 11110110", 3); 
			Assert.Equal((UInt64) 2583, symbols[0]);
			Assert.Equal((UInt64) 0, symbols[1]);
			Assert.Equal((UInt64) 1, symbols[2]);
		}


		[Fact]
		public void Decompress_Max() {
			Assert.Equal(FibonacciCodec.MaxValue, DecompressOne("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011 000"));
		}

		[Fact]
		public void Decompress_Overflow1() {
			// Symbol too large
			Assert.Throws<OverflowException>(() => { DecompressOne("01010100 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011 000"); });
		}

		[Fact]
		public void Decompress_Overflow2() {
			// Symbol too large and too many bits
			Assert.Throws<OverflowException>(() => { DecompressOne("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 010011 00"); });
		}

		[Fact]
		public void Decompress_0_1_2() {
			var symbols = DecompressMany("11 011 0011 0000000", 3); // 0 1 2
			Assert.Equal((UInt64) 0, symbols[0]);
			Assert.Equal((UInt64) 1, symbols[1]);
			Assert.Equal((UInt64) 2, symbols[2]);
		}
		
		[Fact]
		public void Decompress_0_1_2_4_5_6() {
			var symbols = DecompressMany("11 011 0011 1011 00011 10011 01011 0000", 7); // 0 1 2 3 4 5 6
			Assert.Equal((UInt64) 0, symbols[0]);
			Assert.Equal((UInt64) 1, symbols[1]);
			Assert.Equal((UInt64) 2, symbols[2]);
			Assert.Equal((UInt64) 3, symbols[3]);
			Assert.Equal((UInt64) 4, symbols[4]);
			Assert.Equal((UInt64) 5, symbols[5]);
			Assert.Equal((UInt64) 6, symbols[6]);
		}

		[Fact]
		public void Decompress_1_2_3() {
			var symbols = DecompressMany("011 0011 1011 00000", 3); // 1 2 3 
			Assert.Equal((UInt64) 1, symbols[0]);
			Assert.Equal((UInt64) 2, symbols[1]);
			Assert.Equal((UInt64) 3, symbols[2]);
		}

		[Fact]
		public void Decompress_0_0_0_0() {
			// Complete byte
			var symbols = DecompressMany("11 11 11 11", 4); // 0 0 0 0
			Assert.Equal((UInt64) 0, symbols[0]);
			Assert.Equal((UInt64) 0, symbols[1]);
			Assert.Equal((UInt64) 0, symbols[2]);
			Assert.Equal((UInt64) 0, symbols[3]);
		}

		[Fact]
		public void Decompress_OutputPerfectSize() {
			var input = new MemoryStream(BitOperation.ParseToBytes("11 000000"));
			var output = Codec.DecodeSingle(input);
			Assert.Equal((UInt64) 0, output);
		}

		[Fact]
		public void Decompress_InputClipped() {
			Assert.Throws<EndOfStreamException>(() => {
				var input = new MemoryStream(BitOperation.ParseToBytes("11011001"));
				var output = new UInt64[3];
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