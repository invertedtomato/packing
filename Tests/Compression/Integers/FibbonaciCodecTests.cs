using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.IO.Buffers;
using System.Collections.Generic;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class FibonacciCodecTests {
        public string CompressSet(ulong[] set, bool includeHeader = false) {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = includeHeader;
            codec.DecompressedSet = new Buffer<ulong>(set);
            codec.Compress();
            return codec.CompressedSet.ToArray().ToBinaryString();
        }
        public string CompressSymbol(ulong value, bool includeHeader = false) {
            return CompressSet(new ulong[] { value }, includeHeader);
        }

        [TestMethod]
        public void Compress_Empty() {
            Assert.AreEqual("", CompressSet(new ulong[] { }));
        }
        [TestMethod]
        public void Compress_0() {
            Assert.AreEqual("11000000", CompressSymbol(0));
        }
        [TestMethod]
        public void Compress_1() {
            Assert.AreEqual("01100000", CompressSymbol(1));
        }
        [TestMethod]
        public void Compress_2() {
            Assert.AreEqual("00110000", CompressSymbol(2));
        }
        [TestMethod]
        public void Compress_3() {
            Assert.AreEqual("10110000", CompressSymbol(3));
        }
        [TestMethod]
        public void Compress_4() {
            Assert.AreEqual("00011000", CompressSymbol(4));
        }
        [TestMethod]
        public void Compress_5() {
            Assert.AreEqual("10011000", CompressSymbol(5));
        }
        [TestMethod]
        public void Compress_6() {
            Assert.AreEqual("01011000", CompressSymbol(6));
        }
        [TestMethod]
        public void Compress_7() {
            Assert.AreEqual("00001100", CompressSymbol(7));
        }
        [TestMethod]
        public void Compress_8() {
            Assert.AreEqual("10001100", CompressSymbol(8));
        }
        [TestMethod]
        public void Compress_9() {
            Assert.AreEqual("01001100", CompressSymbol(9));
        }
        [TestMethod]
        public void Compress_10() {
            Assert.AreEqual("00101100", CompressSymbol(10));
        }
        [TestMethod]
        public void Compress_11() {
            Assert.AreEqual("10101100", CompressSymbol(11));
        }
        [TestMethod]
        public void Compress_12() {
            Assert.AreEqual("00000110", CompressSymbol(12));
        }
        [TestMethod]
        public void Compress_13() {
            Assert.AreEqual("10000110", CompressSymbol(13));
        }
        [TestMethod]
        public void Compress_Max() {
            Assert.AreEqual("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011000", CompressSymbol(FibonacciCodec.MaxValue)); // Not completely sure about this value
        }
        [TestMethod]
        public void Compress_0_1_2() {
            Assert.AreEqual("11011001 10000000", CompressSet(new ulong[] { 0, 1, 2 }));
        }
        [TestMethod]
        public void Compress_10x1() {
            Assert.AreEqual("11111111 11111111 11110000", CompressSet(new ulong[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        }
        [TestMethod]
        public void Compress_10xSize() {
            var codec = new FibonacciCodec();
            codec.DecompressedSet = new Buffer<ulong>(new ulong[10]);
            codec.Compress();
            Assert.AreEqual(Math.Ceiling((float)(10 * 2) / 8), codec.CompressedSet.ToArray().Length);
        }

        [TestMethod]
        public void Compress_Empty_WithHeader() {
            Assert.AreEqual("11000000", CompressSet(new ulong[] { }, true));
        }
        [TestMethod]
        public void Compress_0_WithHeader() {
            Assert.AreEqual("01111000", CompressSymbol(0, true));
        }
        [TestMethod]
        public void Compress_1_WithHeader() {
            Assert.AreEqual("01101100", CompressSymbol(1, true));
        }
        [TestMethod]
        public void Compress_2_WithHeader() {
            Assert.AreEqual("01100110", CompressSymbol(2, true));
        }
        [TestMethod]
        public void Compress_0_1_2_WithHeader() {
            Assert.AreEqual("10111101 10011000", CompressSet(new ulong[] { 0, 1, 2 }, true));
        }



        


        private ulong[] DecompressSet(string value, bool includeHeader = false) {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = includeHeader;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes(value));
            Assert.AreEqual(0, codec.Decompress());
            return codec.DecompressedSet.ToArray();
        }

        private ulong DecompressSymbol(string value, bool includeHeader = false) {
            var set = DecompressSet(value, includeHeader);
            Assert.IsTrue(set.Length <= 2); // There's possibly a trailing value from unused bits
            return set[0];
        }



        [TestMethod]
        public void Decompress_Empty1() {
            Assert.AreEqual(0, DecompressSet("").Length);
        }
        [TestMethod]
        public void Decompress_Empty2() {
            Assert.AreEqual(0, DecompressSet("00000000").Length); // Trailing bits are ignored
        }
        [TestMethod]
        public void Decompress_0() {
            Assert.AreEqual((ulong)0, DecompressSymbol("11 000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((ulong)1, DecompressSymbol("011 00000"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((ulong)2, DecompressSymbol("0011 0000"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((ulong)3, DecompressSymbol("1011 0000"));
        }
        [TestMethod]
        public void Decompress_4() {
            Assert.AreEqual((ulong)4, DecompressSymbol("00011 000"));
        }
        [TestMethod]
        public void Decompress_5() {
            Assert.AreEqual((ulong)5, DecompressSymbol("10011 000"));
        }
        [TestMethod]
        public void Decompress_6() {
            Assert.AreEqual((ulong)6, DecompressSymbol("01011 000"));
        }
        [TestMethod]
        public void Decompress_7() {
            Assert.AreEqual((ulong)7, DecompressSymbol("000011 00"));
        }
        [TestMethod]
        public void Decompress_8() {
            Assert.AreEqual((ulong)8, DecompressSymbol("100011 00"));
        }
        [TestMethod]
        public void Decompress_9() {
            Assert.AreEqual((ulong)9, DecompressSymbol("010011 00"));
        }
        [TestMethod]
        public void Decompress_10() {
            Assert.AreEqual((ulong)10, DecompressSymbol("001011 00"));
        }
        [TestMethod]
        public void Decompress_11() {
            Assert.AreEqual((ulong)11, DecompressSymbol("101011 00"));
        }
        [TestMethod]
        public void Decompress_Max() {
            Assert.AreEqual((ulong)ulong.MaxValue - 1, DecompressSymbol("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011 000"));
        }


        [TestMethod]
        public void Decompress_0_1_2() {
            var symbols = DecompressSet("11 011 0011 0000000"); // 0 1 2
            Assert.AreEqual((ulong)0, symbols[0]);
            Assert.AreEqual((ulong)1, symbols[1]);
            Assert.AreEqual((ulong)2, symbols[2]);
        }

        [TestMethod]
        public void Decompress_0_0_0_0() { // Complete byte
            var symbols = DecompressSet("11 11 11 11"); // 0 0 0 0
            Assert.AreEqual(4, symbols.Length); // Important to check there's no trailing 0s which happens when it isn't a complete byte
            Assert.AreEqual((ulong)0, symbols[0]);
            Assert.AreEqual((ulong)0, symbols[1]);
            Assert.AreEqual((ulong)0, symbols[2]);
            Assert.AreEqual((ulong)0, symbols[3]);
        }

        [TestMethod]
        public void Decompress_Empty1_WithHeader() {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes(""));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_Empty2_WithHeader() {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("11 000000"));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_Empty3_WithHeader() {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("11 000000 00000000")); // Trailing bits are ignored
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_0_WithHeader() {
            Assert.AreEqual((ulong)0, DecompressSymbol("011 11 000", true)); // 1 0
        }
        [TestMethod]
        public void Decompress_1_WithHeader() {
            Assert.AreEqual((ulong)1, DecompressSymbol("011 011 00", true)); // 1 1
        }
        [TestMethod]
        public void Decompress_2_WithHeader() {
            Assert.AreEqual((ulong)2, DecompressSymbol("0110 011 0", true)); // 1 2
        }

        [TestMethod]
        public void Decompress_0_1_2_WithHeader() {
            var symbols = DecompressSet("1011 11 011 0011 000", true); // 0 1 2
            Assert.AreEqual((ulong)0, symbols[0]);
            Assert.AreEqual((ulong)1, symbols[1]);
            Assert.AreEqual((ulong)2, symbols[2]);
        }



        [TestMethod]
        public void Decompress_0_WithHeader_WithTrailingJunk() {
            var symbols = DecompressSet("011 11 11 11 0000000", true); // 1 0 0 0
            Assert.AreEqual(1, symbols.Length); // Important to check there's no trailing 0s which happens when it isn't a complete byte
            Assert.AreEqual((ulong)0, symbols[0]);
        }


        [TestMethod]
        public void Decompress_InsufficentData1_WithHeader() {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes(""));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_InsufficentData2_WithHeader() {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("00000000"));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_InsufficentData3_WithHeader() {
            var codec = new FibonacciCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("01100000"));
            Assert.AreEqual(1, codec.Decompress());
        }







        [TestMethod]
        public void CompressDecompress_First1000_Series_WithHeader() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = CompressSymbol(input, true);
                var output = DecompressSymbol(encoded, true);

                Assert.AreEqual(input, output);
            }
        }
        [TestMethod]
        public void CompressDecompress_First1000_Parallel_WithHeader() {
            var set = new ulong[1000];

            for (ulong i = 0; i < (ulong)set.LongLength; i++) {
                set[i] = i;
            }

            var encoded = CompressSet(set, true);
            var decoded = DecompressSet(encoded, true);

            Assert.AreEqual(set.Length, decoded.Length);
            for (ulong i = 0; i < (ulong)set.LongLength; i++) {
                Assert.AreEqual((ulong)i, decoded[i]);
            }
        }
    }
}
