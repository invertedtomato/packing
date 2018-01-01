using InvertedTomato.IO.Bits;
using InvertedTomato.IO.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class FibonacciCodecTests {
        public readonly Codec Codec = new FibonacciCodec();

        public String CompressMany(UInt64[] input, Int32 outputBufferSize = 8) {
            var output = new MemoryStream(outputBufferSize);
            Codec.CompressUnsigned(output, input);

            return output.ToArray().ToBinaryString();
        }
        public String CompressOne(UInt64 value, Int32 outputBufferSize = 8) {
            return CompressMany(new UInt64[] { value }, outputBufferSize);
        }

        [TestMethod]
        public void Compress_Empty() {
            Assert.AreEqual("", CompressMany(new UInt64[] { }));
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
            Assert.AreEqual("11011001 10000000", CompressMany(new UInt64[] { 0, 1, 2 }));
        }
        [TestMethod]
        public void Compress_10x1() {
            Assert.AreEqual("11111111 11111111 11110000", CompressMany(new UInt64[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }
        [TestMethod]
        public void Compress_OutputPerfectSize() {
            var input = new UInt64[] { 0, 1, 2 };
            var output = new MemoryStream(2);
            var codec = new FibonacciCodec();
            codec.CompressUnsigned(output, input);
            Assert.AreEqual("11011001 10000000", output.ToArray().ToBinaryString());
            Assert.AreEqual(3, input.Length);
        }




        private UInt64[] DecompressMany(String value, Int32 count) {
            var input = new MemoryStream(BitOperation.ParseToBytes(value));
            var codec = new FibonacciCodec();
            var output = codec.DecompressUnsigned(input, count);

            return output.ToArray();
        }
        private UInt64 DecompressOne(String value) {
            var set = DecompressMany(value, 1);
            return set[0];
        }

        [TestMethod]
        public void Decompress_Empty() {
            Assert.AreEqual(0, DecompressMany("", 0).Length);
        }
        [TestMethod]
        public void Decompress_0() {
            Assert.AreEqual((UInt64)0, DecompressOne("11 000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((UInt64)1, DecompressOne("011 00000"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((UInt64)2, DecompressOne("0011 0000"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((UInt64)3, DecompressOne("1011 0000"));
        }
        [TestMethod]
        public void Decompress_4() {
            Assert.AreEqual((UInt64)4, DecompressOne("00011 000"));
        }
        [TestMethod]
        public void Decompress_5() {
            Assert.AreEqual((UInt64)5, DecompressOne("10011 000"));
        }
        [TestMethod]
        public void Decompress_6() {
            Assert.AreEqual((UInt64)6, DecompressOne("01011 000"));
        }
        [TestMethod]
        public void Decompress_7() {
            Assert.AreEqual((UInt64)7, DecompressOne("000011 00"));
        }
        [TestMethod]
        public void Decompress_8() {
            Assert.AreEqual((UInt64)8, DecompressOne("100011 00"));
        }
        [TestMethod]
        public void Decompress_9() {
            Assert.AreEqual((UInt64)9, DecompressOne("010011 00"));
        }
        [TestMethod]
        public void Decompress_10() {
            Assert.AreEqual((UInt64)10, DecompressOne("001011 00"));
        }
        [TestMethod]
        public void Decompress_11() {
            Assert.AreEqual((UInt64)11, DecompressOne("101011 00"));
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
            Assert.AreEqual((UInt64)0, symbols[0]);
            Assert.AreEqual((UInt64)1, symbols[1]);
            Assert.AreEqual((UInt64)2, symbols[2]);
        }
        [TestMethod]
        public void Decompress_1_2_3() {
            var symbols = DecompressMany("011 0011 1011 00000", 3); // 1 2 3 
            Assert.AreEqual((UInt64)1, symbols[0]);
            Assert.AreEqual((UInt64)2, symbols[1]);
            Assert.AreEqual((UInt64)3, symbols[2]);
        }
        [TestMethod]
        public void Decompress_0_0_0_0() { // Complete byte
            var symbols = DecompressMany("11 11 11 11", 4); // 0 0 0 0
            Assert.AreEqual((UInt64)0, symbols[0]);
            Assert.AreEqual((UInt64)0, symbols[1]);
            Assert.AreEqual((UInt64)0, symbols[2]);
            Assert.AreEqual((UInt64)0, symbols[3]);
        }
        [TestMethod]
        public void Decompress_OutputPerfectSize() {
            var input = new MemoryStream(BitOperation.ParseToBytes("11 000000"));
            var output = Codec.DecompressUnsigned(input, 1).ToList();
            Assert.AreEqual(1, output.Count());
            Assert.AreEqual((UInt64)0, output.First());
        }
        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Decompress_InputClipped() {
            var input = new MemoryStream(BitOperation.ParseToBytes("11011001"));
            var output = Codec.DecompressUnsigned(input, 3).ToArray();
        }

        [TestMethod]
        public void CompressDecompress_First1000_Series() {
            for(UInt64 input = 0; input < 1000; input++) {
                var encoded = CompressOne(input);
                var output = DecompressOne(encoded);

                Assert.AreEqual(input, output);
            }
        }


        [TestMethod]
        public void CompressDecompress_First1000_Parallel() {
            // Create input
            var input = new List<UInt64>(1000);
            input.Seed(0, 999);

            // Compress
            var compressed = new MemoryStream();
            Codec.CompressUnsigned(compressed, input.ToArray());

            // Rewind stream
            compressed.Seek(0, SeekOrigin.Begin);

            // Decompress
            var output = Codec.DecompressUnsigned(compressed, input.Count).ToList();

            // Validate
            for(var i = 0; i < 1000; i++) {
                Assert.AreEqual((UInt64)i, output[i]);
            }
        }
    }
}
