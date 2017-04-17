using InvertedTomato.IO.Bits;
using InvertedTomato.IO.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class FibonacciCodecTests {
        public string CompressMany(ulong[] set, int outputBufferSize = 8) {
            var input = new Buffer<ulong>(set);
            var output = new Buffer<byte>(outputBufferSize);
            var codec = new FibonacciCodec();
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
            Assert.AreEqual("11000000", CompressOne(0));
        }
        [TestMethod]
        public void Compress_1() {
            Assert.AreEqual("01100000", CompressOne(1));
        }
        [TestMethod]
        public void Compress_2() {
            Assert.AreEqual("00110000", CompressOne(2));
        }
        [TestMethod]
        public void Compress_3() {
            Assert.AreEqual("10110000", CompressOne(3));
        }
        [TestMethod]
        public void Compress_4() {
            Assert.AreEqual("00011000", CompressOne(4));
        }
        [TestMethod]
        public void Compress_5() {
            Assert.AreEqual("10011000", CompressOne(5));
        }
        [TestMethod]
        public void Compress_6() {
            Assert.AreEqual("01011000", CompressOne(6));
        }
        [TestMethod]
        public void Compress_7() {
            Assert.AreEqual("00001100", CompressOne(7));
        }
        [TestMethod]
        public void Compress_8() {
            Assert.AreEqual("10001100", CompressOne(8));
        }
        [TestMethod]
        public void Compress_9() {
            Assert.AreEqual("01001100", CompressOne(9));
        }
        [TestMethod]
        public void Compress_10() {
            Assert.AreEqual("00101100", CompressOne(10));
        }
        [TestMethod]
        public void Compress_11() {
            Assert.AreEqual("10101100", CompressOne(11));
        }
        [TestMethod]
        public void Compress_12() {
            Assert.AreEqual("00000110", CompressOne(12));
        }
        [TestMethod]
        public void Compress_13() {
            Assert.AreEqual("10000110", CompressOne(13));
        }
        [TestMethod]
        public void Compress_Max() {
            Assert.AreEqual("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011000", CompressOne(FibonacciCodec.MaxValue, 32)); // Not completely sure about this value
        }
        [TestMethod]
        public void Compress_0_1_2() {
            Assert.AreEqual("11011001 10000000", CompressMany(new ulong[] { 0, 1, 2 }));
        }
        [TestMethod]
        public void Compress_10x1() {
            Assert.AreEqual("11111111 11111111 11110000", CompressMany(new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }
        [TestMethod]
        public void Compress_OutputPerfectSize() {
            var input = new Buffer<ulong>(new ulong[] { 0, 1, 2 });
            var output = new Buffer<byte>(2);
            var codec = new FibonacciCodec();
            Assert.IsTrue(codec.Compress(input, output));
            Assert.AreEqual("11011001 10000000", output.ToArray().ToBinaryString());
            Assert.AreEqual(3, input.Start);
            Assert.AreEqual(3, input.End);
        }
        [TestMethod]
        public void Compress_OutputTooSmall() {
            var input = new Buffer<ulong>(new ulong[] { 0, 1, 2 });
            var output = new Buffer<byte>(1);
            var codec = new FibonacciCodec();
            Assert.IsFalse(codec.Compress(input, output));
            Assert.AreEqual("", output.ToArray().ToBinaryString());
            Assert.AreEqual(0, input.Start);
            Assert.AreEqual(3, input.End);
        }




        private ulong[] DecompressMany(string value, int count) {
            var input = new Buffer<byte>(BitOperation.ParseToBytes(value));
            var output = new Buffer<ulong>(count);
            var codec = new FibonacciCodec();
            Assert.IsTrue(codec.Decompress(input, output));

            return output.ToArray();
        }
        private ulong DecompressOne(string value) {
            var set = DecompressMany(value, 1);
            return set[0];
        }

        [TestMethod]
        public void Decompress_Empty() {
            Assert.AreEqual(0, DecompressMany("", 0).Length);
        }
        [TestMethod]
        public void Decompress_0() {
            Assert.AreEqual((ulong)0, DecompressOne("11 000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((ulong)1, DecompressOne("011 00000"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((ulong)2, DecompressOne("0011 0000"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((ulong)3, DecompressOne("1011 0000"));
        }
        [TestMethod]
        public void Decompress_4() {
            Assert.AreEqual((ulong)4, DecompressOne("00011 000"));
        }
        [TestMethod]
        public void Decompress_5() {
            Assert.AreEqual((ulong)5, DecompressOne("10011 000"));
        }
        [TestMethod]
        public void Decompress_6() {
            Assert.AreEqual((ulong)6, DecompressOne("01011 000"));
        }
        [TestMethod]
        public void Decompress_7() {
            Assert.AreEqual((ulong)7, DecompressOne("000011 00"));
        }
        [TestMethod]
        public void Decompress_8() {
            Assert.AreEqual((ulong)8, DecompressOne("100011 00"));
        }
        [TestMethod]
        public void Decompress_9() {
            Assert.AreEqual((ulong)9, DecompressOne("010011 00"));
        }
        [TestMethod]
        public void Decompress_10() {
            Assert.AreEqual((ulong)10, DecompressOne("001011 00"));
        }
        [TestMethod]
        public void Decompress_11() {
            Assert.AreEqual((ulong)11, DecompressOne("101011 00"));
        }
        [TestMethod]
        public void Decompress_Max() {
            Assert.AreEqual(FibonacciCodec.MaxValue, DecompressOne("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011 000"));
        }
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Decompress_Overflow1() { // Symbol too large
            DecompressOne("01010100 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011 000");
        }
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Decompress_Overflow2() { // Symbol too large and too many bits
            DecompressOne("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 010011 00");
        }
        [TestMethod]
        public void Decompress_0_1_2() {
            var symbols = DecompressMany("11 011 0011 0000000", 3); // 0 1 2
            Assert.AreEqual((ulong)0, symbols[0]);
            Assert.AreEqual((ulong)1, symbols[1]);
            Assert.AreEqual((ulong)2, symbols[2]);
        }
        [TestMethod]
        public void Decompress_0_0_0_0() { // Complete byte
            var symbols = DecompressMany("11 11 11 11", 4); // 0 0 0 0
            Assert.AreEqual((ulong)0, symbols[0]);
            Assert.AreEqual((ulong)0, symbols[1]);
            Assert.AreEqual((ulong)0, symbols[2]);
            Assert.AreEqual((ulong)0, symbols[3]);
        }
        [TestMethod]
        public void Decompress_0_0_0_0_OversizedOutput() {
            var symbols = DecompressMany("11 11 11 11", 5); // 0 0 0 0
            Assert.AreEqual(4, symbols.Length);
            Assert.AreEqual((ulong)0, symbols[0]);
            Assert.AreEqual((ulong)0, symbols[1]);
            Assert.AreEqual((ulong)0, symbols[2]);
            Assert.AreEqual((ulong)0, symbols[3]);
        }
        [TestMethod]
        public void Decompress_OutputPerfectSize() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("11 000000"));
            var output = new Buffer<ulong>(1);
            var codec = new FibonacciCodec();
            Assert.IsTrue(codec.Decompress(input, output));
            Assert.AreEqual(1, output.Used);
            Assert.AreEqual((ulong)0, output.Dequeue());
        }
        [TestMethod]
        public void Decompress_OutputTooSmall() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("11 011 0011 0000000"));
            var output = new Buffer<ulong>(1);
            var codec = new FibonacciCodec();
            Assert.IsFalse(codec.Decompress(input, output));
            Assert.AreEqual(1, output.Used);
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Decompress_InputClipped() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("11011001"));
            var output = new Buffer<ulong>(3);
            var codec = new FibonacciCodec();
            codec.Decompress(input, output);
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
            var codec = new FibonacciCodec();

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
            Assert.AreEqual(0, input.Start);
            Assert.AreEqual(1000, input.End);
            Assert.AreEqual(0, compressed.Start);
            Assert.AreEqual(0, compressed.End);

            compressed = compressed.Resize(2000);
            Assert.IsTrue(codec.Compress(input, compressed));
            Assert.AreEqual(1000, input.Start);
            Assert.AreEqual(1000, input.End);
            Assert.AreEqual(0, compressed.Start);
            //Assert.AreEqual(1680, compressed.End); // TODO - is this size correct?

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
            var codec = new FibonacciCodec();

            // Create input
            var input = new long[1000];
            for (var i = 0; i < input.Length; i++) {
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
