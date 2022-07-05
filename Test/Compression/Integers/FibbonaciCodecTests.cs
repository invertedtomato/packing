using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers
{
    public class FibonacciCodecTests
    {
        private Byte[] Encode(UInt64 value)
        {
            var codec = new FibonacciCodec();
            using var stream = new MemoryStream();
            using (var writer = new StreamBitWriter(stream))
            {
                codec.EncodeUInt64(value, writer);
            }

            return stream.ToArray();
        }

        private UInt64 Decode(Byte[] encoded)
        {
            var codec = new FibonacciCodec();
            using var stream = new MemoryStream(encoded);
            using var reader = new StreamBitReader(stream);

            return codec.DecodeUInt64(reader);
        }


        [Fact]
        public void Encode_0()
        {
            Assert.Equal(new Byte[] {0b11000000}, Encode(0));
        }

        [Fact]
        public void Encode_1()
        {
            Assert.Equal(new Byte[] {0b01100000}, Encode(1));
        }

        [Fact]
        public void Encode_2()
        {
            Assert.Equal(new Byte[] {0b00110000}, Encode(2));
        }

        [Fact]
        public void Encode_3()
        {
            Assert.Equal(new Byte[] {0b10110000}, Encode(3));
        }

        [Fact]
        public void Encode_4()
        {
            Assert.Equal(new Byte[] {0b00011000}, Encode(4));
        }

        [Fact]
        public void Encode_5()
        {
            Assert.Equal(new Byte[] {0b10011000}, Encode(5));
        }

        [Fact]
        public void Encode_6()
        {
            Assert.Equal(new Byte[] {0b01011000}, Encode(6));
        }

        [Fact]
        public void Encode_7()
        {
            Assert.Equal(new Byte[] {0b00001100}, Encode(7));
        }

        [Fact]
        public void Encode_8()
        {
            Assert.Equal(new Byte[] {0b10001100}, Encode(8));
        }

        [Fact]
        public void Encode_9()
        {
            Assert.Equal(new Byte[] {0b01001100}, Encode(9));
        }

        [Fact]
        public void Encode_10()
        {
            Assert.Equal(new Byte[] {0b00101100}, Encode(10));
        }

        [Fact]
        public void Encode_11()
        {
            Assert.Equal(new Byte[] {0b10101100}, Encode(11));
        }

        [Fact]
        public void Encode_12()
        {
            Assert.Equal(new Byte[] {0b00000110}, Encode(12));
        }

        [Fact]
        public void Encode_13()
        {
            Assert.Equal(new Byte[] {0b10000110}, Encode(13));
        }


        [Fact]
        public void Encode_20()
        {
            Assert.Equal(new Byte[] {0b00000011}, Encode(20)); // Exactly one byte
        }

        [Fact]
        public void Encode_33()
        {
            Assert.Equal(new Byte[] {0b00000001, 0b10000000}, Encode(33)); // Termination bit is on next byte
        }

        [Fact]
        public void Encode_54()
        {
            Assert.Equal(new Byte[] {0b00000000, 0b11000000}, Encode(54)); // Final and termination bits on next byte
        }


        [Fact]
        public void Encode_986()
        {
            Assert.Equal(new Byte[] {0b00000000, 0b00000011}, Encode(986)); // Exactly one byte
        }

        [Fact]
        public void Encode_1596()
        {
            Assert.Equal(new Byte[] {0b00000000, 0b00000001, 0b10000000}, Encode(1596)); // Termination bit is on next byte
        }

        [Fact]
        public void Encode_2583()
        {
            Assert.Equal(new Byte[] {0b00000000, 0b00000000, 0b11000000}, Encode(2583)); // Final and termination bits on next byte
        }

        [Fact]
        public void Encode_Max()
        {
            Assert.Equal(new Byte[] {0b01010000, 0b01010001, 0b01000001, 0b00010101, 0b00010010, 0b00100100, 0b00000010, 0b01000100, 0b10001000, 0b10100000, 0b10001010, 0b01011000}, Encode(new FibonacciCodec().MaxValue)); // Not completely sure about this value
        }

        [Fact]
        public void Decode_0()
        {
            Assert.Equal((UInt64) 0, Decode(new Byte[] {0b11_000000}));
        }

        [Fact]
        public void Decode_1()
        {
            Assert.Equal((UInt64) 1, Decode(new Byte[] {0b011_00000}));
        }

        [Fact]
        public void Decode_2()
        {
            Assert.Equal((UInt64) 2, Decode(new Byte[] {0b0011_0000}));
        }

        [Fact]
        public void Decode_3()
        {
            Assert.Equal((UInt64) 3, Decode(new Byte[] {0b1011_0000}));
        }

        [Fact]
        public void Decode_4()
        {
            Assert.Equal((UInt64) 4, Decode(new Byte[] {0b00011_000}));
        }

        [Fact]
        public void Decode_5()
        {
            Assert.Equal((UInt64) 5, Decode(new Byte[] {0b10011_000}));
        }

        [Fact]
        public void Decode_6()
        {
            Assert.Equal((UInt64) 6, Decode(new Byte[] {0b01011_000}));
        }

        [Fact]
        public void Decode_7()
        {
            Assert.Equal((UInt64) 7, Decode(new Byte[] {0b000011_00}));
        }

        [Fact]
        public void Decode_8()
        {
            Assert.Equal((UInt64) 8, Decode(new Byte[] {0b100011_00}));
        }

        [Fact]
        public void Decode_9()
        {
            Assert.Equal((UInt64) 9, Decode(new Byte[] {0b010011_00}));
        }

        [Fact]
        public void Decode_10()
        {
            Assert.Equal((UInt64) 10, Decode(new Byte[] {0b001011_00}));
        }

        [Fact]
        public void Decode_11()
        {
            Assert.Equal((UInt64) 11, Decode(new Byte[] {0b101011_00}));
        }


        [Fact]
        public void Decode_20()
        {
            Assert.Equal((UInt64) 20, Decode(new Byte[] {0b00000011})); // Exactly one byte
        }

        [Fact]
        public void Decode_33()
        {
            Assert.Equal((UInt64) 33, Decode(new Byte[] {0b00000001, 0b1_0000000})); // Termination bit is on next byte
        }

        [Fact]
        public void Decode_54()
        {
            Assert.Equal((UInt64) 54, Decode(new Byte[] {0b0000000011_000000})); // Final and termination bits on next byte
        }


        [Fact]
        public void Decode_986()
        {
            Assert.Equal((UInt64) 986, Decode(new Byte[] {0b00000000, 0b00000011})); // Exactly two bytes
        }

        [Fact]
        public void Decode_1596()
        {
            Assert.Equal((UInt64) 1596, Decode(new Byte[] {0b00000000, 0b00000001, 0b1_0000000})); // Termination bit is on next byte
        }

        [Fact]
        public void Decode_2583()
        {
            Assert.Equal((UInt64) 2583, Decode(new Byte[] {0b00000000, 0b0000000011_000000})); // Final and termination bits on next byte
        }

        [Fact]
        public void Decode_Max()
        {
            Assert.Equal(new FibonacciCodec().MaxValue, Decode(new Byte[] {0b01010000, 0b01010001, 0b01000001, 0b00010101, 0b00010010, 0b00100100, 0b00000010, 0b01000100, 0b10001000, 0b10100000, 0b10001010, 0b01011_000}));
        }

        [Fact]
        public void Decode_Overflow1()
        {
            // Symbol too large
            Assert.Throws<OverflowException>(() => { Decode(new Byte[] {0b01010100, 0b01010001, 0b01000001, 0b00010101, 0b00010010, 0b00100100, 0b00000010, 0b01000100, 0b10001000, 0b10100000, 0b10001010, 0b01011_000}); });
        }

        [Fact]
        public void Decode_Overflow2()
        {
            // Symbol too large and too many bits
            Assert.Throws<OverflowException>(() => { Decode(new Byte[] {0b01010000, 0b01010001, 0b01000001, 0b00010101, 0b00010010, 0b00100100, 0b00000010, 0b01000100, 0b10001000, 0b10100000, 0b10001010, 0b010011_00}); });
        }

        [Fact]
        public void EncodeDecode_1000()
        {
            var ta = new FibonacciCodec();
            using var stream = new MemoryStream();

            // Encode
            using (var writer = new StreamBitWriter(stream))
            {
                for (UInt64 symbol = 0; symbol < 1000; symbol++)
                {
                    ta.EncodeUInt64(symbol, writer);
                }
            }

            stream.Seek(0, SeekOrigin.Begin);

            // Decode
            using (var reader = new StreamBitReader(stream))
            {
                for (UInt64 symbol = 0; symbol < 1000; symbol++)
                {
                    Assert.Equal(symbol, ta.DecodeUInt64(reader));
                }
            }
        }
    }
}