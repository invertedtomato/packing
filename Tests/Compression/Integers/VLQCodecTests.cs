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
        public string CompressMany(ulong[] set, int outputBufferSize = 8) {
            var input = new Buffer<ulong>(set);
            var output = new Buffer<byte>(outputBufferSize);
            var codec = new VLQCodec();
            Assert.IsTrue(codec.Compress(input, output));

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
            var input = new Buffer<ulong>(new ulong[] { 128 });
            var output = new Buffer<byte>(2);
            var codec = new VLQCodec();
            Assert.IsTrue(codec.Compress(input, output));
            Assert.AreEqual("00000000 10000000", output.ToArray().ToBinaryString());
            Assert.AreEqual(1, input.Start);
            Assert.AreEqual(1, input.End);
        }
        [TestMethod]
        public void Compress_OutputTooSmall() {
            var input = new Buffer<ulong>(new ulong[] { 128 });
            var output = new Buffer<byte>(1);
            var codec = new VLQCodec();
            Assert.IsFalse(codec.Compress(input, output));
            Assert.AreEqual("", output.ToArray().ToBinaryString());
            Assert.AreEqual(0, input.Start);
            Assert.AreEqual(1, input.End);
        }



        private ulong[] DecompressMany(string value, int count) {
            var input = new Buffer<byte>(BitOperation.ParseToBytes(value));
            var output = new Buffer<ulong>(count);
            var codec = new VLQCodec();
            Assert.IsTrue(codec.Decompress(input, output));

            return output.ToArray();
        }
        private ulong DecompressOne(string value) {
            var set= DecompressMany(value, 1);
            return set[0];
        }

        [TestMethod]
        public void Decompress_Empty() {
            Assert.AreEqual(0, DecompressMany("", 0).Length);
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
            var set = DecompressMany("10000001 10000001 10000001", 3);
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual((ulong)1, set[0]);
            Assert.AreEqual((ulong)1, set[1]);
            Assert.AreEqual((ulong)1, set[2]);
        }
        [TestMethod]
        public void Decompress_OutputPerfectSize() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("00000000 10000000"));
            var output = new Buffer<ulong>(1);
            var codec = new VLQCodec();
            Assert.IsTrue(codec.Decompress(input, output));
            Assert.AreEqual(1, output.Used);
            Assert.AreEqual((ulong)128, output.Dequeue());
        }
        [TestMethod]
        public void Decompress_OutputTooSmall() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("00000000 10000000 10000000"));
            var output = new Buffer<ulong>(1);
            var codec = new VLQCodec();
            Assert.IsFalse(codec.Decompress(input, output));
            Assert.AreEqual(1, output.Used);
            Assert.AreEqual((ulong)128, output.Dequeue());
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Decompress_InputClipped() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("00000000"));
            var output = new Buffer<ulong>(1);
            var codec = new VLQCodec();
            codec.Decompress(input, output);
        }
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Decompress_Overflow() {
            DecompressOne("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000");
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
        public void CompressDecompress_First1000_Parallel() {
            var codec = new VLQCodec();

            // Create input
            var input = new Buffer<ulong>(1000);
            input.Seed(0, 999);
            Assert.AreEqual(0, input.Start);
            Assert.AreEqual(1000, input.End);

            // Create output
            var compressed = new Buffer<byte>(100);
            Assert.AreEqual(0, compressed.Start);
            Assert.AreEqual(0, compressed.End);

            // Attempt to compress with various output sizes - all will fail until the output is big enough
            Assert.IsFalse(codec.Compress(input, compressed));
            Assert.AreEqual(100, input.Start);
            Assert.AreEqual(1000, input.End);
            Assert.AreEqual(0, compressed.Start);
            Assert.AreEqual(100, compressed.End);

            compressed = compressed.Resize(2000);
            Assert.IsTrue(codec.Compress(input, compressed));
            Assert.AreEqual(1000, input.Start);
            Assert.AreEqual(1000, input.End);
            Assert.AreEqual(0, compressed.Start);
            //Assert.AreEqual(1872, compressed.End); // TODO - is this size correct?

            // Decompress in one batch - if it's in chunks the first value of each chunk will be messed up
            var decompressed = new Buffer<ulong>(1000);
            Assert.IsTrue(codec.Decompress(compressed, decompressed));

            // Validate
            for (ulong i = 0; i < 1000; i++) {
                Assert.AreEqual(i, decompressed.Dequeue());
            }
        }
        [TestMethod]
        public void CompressDecompress_First1000_Parallel_Basic() {
            var codec = new VLQCodec();

            // Create input
            var input = new long[1000];
            for(var i =0; i<input.Length; i++) {
                input[i] = i;
            }

            // Compress
            var compressed = codec.Compress(input);

            // Decompress
            var output = codec.Decompress(compressed);

            // Validate
            Assert.AreEqual(1000, output.Length);
            for (long i = 0; i < 1000; i++) {
                Assert.AreEqual(i, output[i]);
            }
        }
    }
}
