using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers.Wave3 {
	public class VlqCodecTests {
		private readonly Codec Vlq = new VlqCodec();
		
		public Byte[] Encode(UInt64 value) {
			using (var stream = new MemoryStream()) {
				Vlq.EncodeSingle(stream, value);
				stream.Seek(0, SeekOrigin.Begin);
				return stream.ToArray();
			}
		}
		
		[Fact]
		public void Encode0() {
			Assert.Equal(new Byte[] {0b00000000}, Encode(0));
		}

		[Fact]
		public void Encode1() {
			Assert.Equal(new Byte[] {0b00000001}, Encode(1));
		}

		[Fact]
		public void Encode2() {
			Assert.Equal(new Byte[] {0b00000010}, Encode(2));
		}

		[Fact]
		public void Encode3() {
			Assert.Equal(new Byte[] {0b00000011}, Encode(3));
		}

		[Fact]
		public void Encode127() {
			Assert.Equal(new Byte[] {0b01111111}, Encode(127));
		}

		[Fact]
		public void Encode128() {
			Assert.Equal(new Byte[] {0b10000000, 0b00000000}, Encode(128));
		}

		[Fact]
		public void Encode129() {
			Assert.Equal(new Byte[] {0b10000001, 0b00000000}, Encode(129));
		}

		[Fact]
		public void Encode16511() {
			Assert.Equal(new Byte[] {0b11111111, 0b01111111}, Encode(16511));
		}

		[Fact]
		public void Encode16512() {
			Assert.Equal(new Byte[] {0b10000000, 0b10000000, 0b00000000}, Encode(16512));
		}

		[Fact]
		public void Encode2113663() {
			Assert.Equal(new Byte[] {0b11111111, 0b11111111, 0b01111111}, Encode(2113663));
		}

		[Fact]
		public void Encode2113664() {
			Assert.Equal(new Byte[] {0b10000000, 0b10000000, 0b10000000, 0b00000000}, Encode(2113664));
		}

		[Fact]
		public void EncodeMax() {
			Assert.Equal(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}, Encode(VlqCodec.MaxValue));
		}

		[Fact]
		public void EncodeOverflow() {
			Assert.Throws<OverflowException>(() => { Encode(UInt64.MaxValue); });
		}

		public UInt64 Decode(Byte[] encoded) {
			using (var stream = new MemoryStream(encoded)) {
				return Vlq.DecodeSingle(stream);
			}
		}
		
		[Fact]
		public void Decode0() {
			Assert.Equal((UInt64) 0, Decode(new Byte[] {0b00000000}));
		}

		[Fact]
		public void Decode1() {
			Assert.Equal((UInt64) 1, Decode(new Byte[] {0b00000001}));
		}

		[Fact]
		public void Decode2() {
			Assert.Equal((UInt64) 2, Decode(new Byte[] {0b00000010}));
		}

		[Fact]
		public void Decode3() {
			Assert.Equal((UInt64) 3, Decode(new Byte[] {0b00000011}));
		}

		[Fact]
		public void Decode127() {
			Assert.Equal((UInt64) 127, Decode(new Byte[] {0b01111111}));
		}

		[Fact]
		public void Decode128() {
			Assert.Equal((UInt64) 128, Decode(new Byte[] {0b10000000,0b00000000}));
		}

		[Fact]
		public void Decode129() {
			Assert.Equal((UInt64) 129, Decode(new Byte[] {0b10000001,0b00000000}));
		}

		[Fact]
		public void Decode16511() {
			Assert.Equal((UInt64) 16511, Decode(new Byte[] {0b11111111,0b01111111}));
		}

		[Fact]
		public void Decode16512() {
			Assert.Equal((UInt64) 16512, Decode(new Byte[] {0b10000000,0b10000000,0b00000000}));
		}

		[Fact]
		public void Decode16513() {
			Assert.Equal((UInt64) 16513, Decode(new Byte[] {0b10000001,0b10000000,0b00000000}));
		}

		[Fact]
		public void Decode2113663() {
			Assert.Equal((UInt64) 2113663, Decode(new Byte[] {0b11111111,0b11111111,0b01111111}));
		}

		[Fact]
		public void Decode2113664() {
			Assert.Equal((UInt64) 2113664, Decode(new Byte[] {0b10000000,0b10000000,0b10000000,0b00000000}));
		}

		[Fact]
		public void DecodeMax() {
			Assert.Equal(VlqCodec.MaxValue, Decode(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}));
		}

		[Fact]
		public void Decode1_1_1() {
			using(var stream = new MemoryStream(new Byte[]{0b00000001,0b00000001,0b00000001})) {
				Assert.Equal((UInt64)1, Vlq.DecodeSingle(stream));
				Assert.Equal((UInt64)1, Vlq.DecodeSingle(stream));
				Assert.Equal((UInt64)1, Vlq.DecodeSingle(stream));
				Assert.Throws<EndOfStreamException>(() => {
					return Vlq.DecodeSingle(stream);
				});
			}
		}

		[Fact]
		public void DecodeInputClipped() {
			Assert.Throws<EndOfStreamException>(() => { Decode(new Byte[] {0b10000000}); });
		}

		[Fact]
		public void DecodeOverflow() {
			Assert.Throws<OverflowException>(() => { Decode(new Byte[] {0b11111111, 0b11111110 , 0b11111110 , 0b11111110 , 0b11111110 , 0b11111110 , 0b11111110 , 0b11111110 , 0b11111110 , 0b11111110 , 0b00000000}); });
		}

		[Fact]
		public void Decode1_X() {
			Assert.Equal((UInt64) 1, Decode(new Byte[]{0b00000001,0b10000011}));
		}
	}
}