using InvertedTomato.IO.Buffers;
using InvertedTomato.IO.Bits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Compression.Integers {
    [TestClass]
    public class VLQCodecTests {
        private readonly VLQCodec Codec = new VLQCodec();

        public string CompressMany(ulong[] set, int outputBufferSize = 8) {
            var output = new Buffer<byte>(outputBufferSize);
            Codec.CompressUIntArray(set, output);

            return output.ToArray().ToBinaryString();
        }
        public string CompressOne(ulong value, int outputBufferSize = 8) {
            return CompressMany(new ulong[] { value }, outputBufferSize);
        }

        [TestMethod]
        public void Compress_Empty() {
            Assert.AreEqual("", CompressMany(new ulong[] { }));
        }
        [TestMethod]
        public void Compress_0() {
            Assert.AreEqual("10000000", CompressOne(0));
        }
        [TestMethod]
        public void Compress_1() {
            Assert.AreEqual("10000001", CompressOne(1));
        }
        [TestMethod]
        public void Compress_2() {
            Assert.AreEqual("10000010", CompressOne(2));
        }
        [TestMethod]
        public void Compress_3() {
            Assert.AreEqual("10000011", CompressOne(3));
        }
        [TestMethod]
        public void Compress_127() {
            Assert.AreEqual("11111111", CompressOne(127));
        }
        [TestMethod]
        public void Compress_128() {
            Assert.AreEqual("00000000 10000000", CompressOne(128));
        }
        [TestMethod]
        public void Compress_129() {
            Assert.AreEqual("00000001 10000000", CompressOne(129));
        }
        [TestMethod]
        public void Compress_16511() {
            Assert.AreEqual("01111111 11111111", CompressOne(16511));
        }
        [TestMethod]
        public void Compress_16512() {
            Assert.AreEqual("00000000 00000000 10000000", CompressOne(16512));
        }
        [TestMethod]
        public void Compress_2113663() {
            Assert.AreEqual("01111111 01111111 11111111", CompressOne(2113663));
        }
        [TestMethod]
        public void Compress_2113664() {
            Assert.AreEqual("00000000 00000000 00000000 10000000", CompressOne(2113664));
        }
        [TestMethod]
        public void Compress_Max() {
            Assert.AreEqual("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", CompressOne(ulong.MaxValue, 32));
        }
        [TestMethod]
        public void Compress_1_1_1() {
            Assert.AreEqual("10000001 10000001 10000001", CompressMany(new ulong[] { 1, 1, 1 }));
        }
        [TestMethod]
        public void Compress_128_128_128() {
            Assert.AreEqual("00000000 10000000 00000000 10000000 00000000 10000000", CompressMany(new ulong[] { 128, 128, 128 }));
        }
        [TestMethod]
        public void Compress_OutputPerfectSize() {
            var output = new Buffer<byte>(2);
            Codec.CompressUIntArray(new ulong[] { 128 }, output);
            Assert.AreEqual("00000000 10000000", output.ToArray().ToBinaryString());
        }
        [TestMethod]
        [ExpectedException(typeof(BufferOverflowException))]
        public void Compress_OutputTooSmall() {
            var output = new Buffer<byte>(1);
            Codec.CompressUIntArray(new ulong[] { 128 }, output);
        }



        private ulong[] DecompressMany(string value) {
            var input = new Buffer<byte>(BitOperation.ParseToBytes(value));
            return Codec.DecompressUIntArray(input);
        }
        private ulong DecompressOne(string value) {
            var output = DecompressMany(value);
            Assert.AreEqual(1, output.Length);
            return output[0];
        }

        [TestMethod]
        public void Decompress_Empty() {
            Assert.AreEqual(0, DecompressMany("").Length);
        }
        [TestMethod]
        public void Decompress_0() {
            Assert.AreEqual((ulong)0, DecompressOne("10000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((ulong)1, DecompressOne("10000001"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((ulong)2, DecompressOne("10000010"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((ulong)3, DecompressOne("10000011"));
        }
        [TestMethod]
        public void Decompress_127() {
            Assert.AreEqual((ulong)127, DecompressOne("11111111"));
        }
        [TestMethod]
        public void Decompress_128() {
            Assert.AreEqual((ulong)128, DecompressOne("00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_129() {
            Assert.AreEqual((ulong)129, DecompressOne("00000001 10000000"));
        }
        [TestMethod]
        public void Decompress_16511() {
            Assert.AreEqual((ulong)16511, DecompressOne("01111111 11111111"));
        }
        [TestMethod]
        public void Decompress_16512() {
            Assert.AreEqual((ulong)16512, DecompressOne("00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_16513() {
            Assert.AreEqual((ulong)16513, DecompressOne("00000001 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_2113663() {
            Assert.AreEqual((ulong)2113663, DecompressOne("01111111 01111111 11111111"));
        }
        [TestMethod]
        public void Decompress_2113664() {
            Assert.AreEqual((ulong)2113664, DecompressOne("00000000 00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_Max() {
            Assert.AreEqual(VLQCodec.MaxValue, DecompressOne("01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000"));
        }
        [TestMethod]
        public void Decompress_1_1_1() {
            var set = DecompressMany("10000001 10000001 10000001");
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual((ulong)1, set[0]);
            Assert.AreEqual((ulong)1, set[1]);
            Assert.AreEqual((ulong)1, set[2]);
        }
        [TestMethod]
        [ExpectedException(typeof(InsufficentInputException))]
        public void Decompress_InputClipped() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("00000000"));
            Codec.DecompressUInt(input);
        }
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Decompress_Overflow() {
            DecompressOne("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000");
        }

        [TestMethod]
        public void Decompress_1_X() {
            var codec = new VLQCodec();
            var input = new Buffer<byte>(BitOperation.ParseToBytes("10000001 00000011"));
            Assert.AreEqual((ulong)1, Codec.DecompressUInt(input));
        }


        [TestMethod]
        public void CompressDecompress_First1000_Series() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = CompressOne(input);
                var output = DecompressOne(encoded);

                Assert.AreEqual(input, output);
            }
        }
        [TestMethod]
        public void CompressDecompress_First1000_Parallel_Basic() {
            var codec = new VLQCodec();

            // Create input
            var input = new long[1000];
            for (var i = 0; i < input.Length; i++) {
                input[i] = i;
            }

            // Compress
            var compressed = Codec.CompressArray(input);

            // Decompress
            var output = Codec.DecompressArray(compressed);

            // Validate
            Assert.AreEqual(1000, output.Length);
            for (long i = 0; i < 1000; i++) {
                Assert.AreEqual(i, output[i]);
            }
        }

    }
}
