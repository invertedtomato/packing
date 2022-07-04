using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers
{
    public class ThompsonAlphaTests
    {
        private Byte[] Encode(UInt64 value)
        {
            var codec = new ThompsonAlphaCodec();
            using var stream = new MemoryStream();
            using (var writer = new StreamBitWriter(stream))
            {
                codec.EncodeUInt64(value, writer);
            }

            return stream.ToArray();
        }

        private UInt64 Decode(Byte[] encoded)
        {
            var codec = new ThompsonAlphaCodec();
            using var stream = new MemoryStream(encoded);
            using var reader = new StreamBitReader(stream);

            return codec.DecodeUInt64(reader);
        }

        [Fact]
        public void Encode0()
        {
            Assert.Equal(new Byte[] {0b000000_0}, Encode(0)); // Len=0, Val=(1)
        }

        [Fact]
        public void Encode1()
        {
            Assert.Equal(new Byte[] {0b000001_1}, Encode(1)); // Len=1, Val=(1)1
        }

        [Fact]
        public void Encode2()
        {
            Assert.Equal(new Byte[] {0b000010_10}, Encode(2)); // Len=10, val=(1)10
        }
        // TODO: Max value test


        [Fact]
        public void Decode0()
        {
            Assert.Equal((UInt64) 0, Decode(new Byte[] {0b000000_00}));
        }

        [Fact]
        public void Decode1()
        {
            Assert.Equal((UInt64) 1, Decode(new Byte[] {0b000001_1_0})); // (len)_(val)_(padding)
        }

        [Fact]
        public void Decode2()
        {
            Assert.Equal((UInt64) 2, Decode(new Byte[] {0b000010_10}));
        }

        [Fact]
        public void EncodeDecode_1000()
        {
            var ta = new ThompsonAlphaCodec();
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