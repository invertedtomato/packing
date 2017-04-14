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
        public string CompressSet(ulong[] set, bool includeHeader = false) {
            var codec = new VLQCodec();
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
            Assert.AreEqual("10000000", CompressSymbol(0));
        }
        [TestMethod]
        public void Compress_1() {
            Assert.AreEqual("10000001", CompressSymbol(1));
        }
        [TestMethod]
        public void Compress_2() {
            Assert.AreEqual("10000010", CompressSymbol(2));
        }
        [TestMethod]
        public void Compress_3() {
            Assert.AreEqual("10000011", CompressSymbol(3));
        }
        [TestMethod]
        public void Compress_127() {
            Assert.AreEqual("11111111", CompressSymbol(127));
        }
        [TestMethod]
        public void Compress_128() {
            Assert.AreEqual("00000000 10000000", CompressSymbol(128));
        }
        [TestMethod]
        public void Compress_129() {
            Assert.AreEqual("00000001 10000000", CompressSymbol(129));
        }
        [TestMethod]
        public void Compress_16511() {
            Assert.AreEqual("01111111 11111111", CompressSymbol(16511));
        }
        [TestMethod]
        public void Compress_16512() {
            Assert.AreEqual("00000000 00000000 10000000", CompressSymbol(16512));
        }
        [TestMethod]
        public void Compress_2113663() {
            Assert.AreEqual("01111111 01111111 11111111", CompressSymbol(2113663));
        }
        [TestMethod]
        public void Compress_2113664() {
            Assert.AreEqual("00000000 00000000 00000000 10000000", CompressSymbol(2113664));
        }
        [TestMethod]
        public void Compress_Max() {
            Assert.AreEqual("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", CompressSymbol(ulong.MaxValue));
        }
        [TestMethod]
        public void Compress_1_1_1() {
            Assert.AreEqual("10000001 10000001 10000001", CompressSet(new ulong[] { 1, 1, 1 }));
        }
        [TestMethod]
        public void Compress_128_128_128() {
            Assert.AreEqual("00000000 10000000 00000000 10000000 00000000 10000000", CompressSet(new ulong[] { 128, 128, 128 }));
        }

        [TestMethod]
        public void Compress_Empty_WithHeader() {
            Assert.AreEqual("10000000", CompressSet(new ulong[] { }, true));
        }
        [TestMethod]
        public void Compress_0_WithHeader() {
            Assert.AreEqual("10000001 10000000", CompressSymbol(0, true));
        }
        [TestMethod]
        public void Compress_1_WithHeader() {
            Assert.AreEqual("10000001 10000001", CompressSymbol(1, true));
        }
        [TestMethod]
        public void Compress_127_WithHeader() {
            Assert.AreEqual("10000001 11111111", CompressSymbol(127, true));
        }
        [TestMethod]
        public void Compress_128_WithHeader() {
            Assert.AreEqual("10000001 00000000 10000000", CompressSymbol(128, true));
        }




        [TestMethod]
        public void CompressDecompress_First1000_Series() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = CompressSymbol(input);
                var output = DecompressSymbol(encoded);

                Assert.AreEqual(input, output);
            }
        }
        [TestMethod]
        public void CompressDecompress_First1000_Parallel() {
            var set = new ulong[1000];

            for (ulong i = 0; i < (ulong)set.LongLength; i++) {
                set[i] = (ulong)i;
            }

            var encoded = CompressSet(set);
            var decoded = DecompressSet(encoded);

            Assert.AreEqual(set.Length, decoded.Length);
            for (ulong i = 0; i < (ulong)set.LongLength; i++) {
                Assert.AreEqual(i, decoded[i]);
            }
        }









        private ulong[] DecompressSet(string value, bool includeHeader = false) {
            var codec = new VLQCodec();
            codec.IncludeHeader = includeHeader;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes(value));
            Assert.AreEqual(0, codec.Decompress());
            return codec.DecompressedSet.ToArray();
        }

        private ulong DecompressSymbol(string value, bool includeHeader = false) {
            var set = DecompressSet(value, includeHeader);
            Assert.AreEqual(1, set.Length);
            return set[0];
        }

        [TestMethod]
        public void Decompress_Empty() {
            Assert.AreEqual(0, DecompressSet("").Length);
        }
        [TestMethod]
        public void Decompress_0() {
            Assert.AreEqual((ulong)0, DecompressSymbol("10000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((ulong)1, DecompressSymbol("10000001"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((ulong)2, DecompressSymbol("10000010"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((ulong)3, DecompressSymbol("10000011"));
        }
        [TestMethod]
        public void Decompress_127() {
            Assert.AreEqual((ulong)127, DecompressSymbol("11111111"));
        }
        [TestMethod]
        public void Decompress_128() {
            Assert.AreEqual((ulong)128, DecompressSymbol("00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_129() {
            Assert.AreEqual((ulong)129, DecompressSymbol("00000001 10000000"));
        }
        [TestMethod]
        public void Decompress_16511() {
            Assert.AreEqual((ulong)16511, DecompressSymbol("01111111 11111111"));
        }
        [TestMethod]
        public void Decompress_16512() {
            Assert.AreEqual((ulong)16512, DecompressSymbol("00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_16513() {
            Assert.AreEqual((ulong)16513, DecompressSymbol("00000001 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_2113663() {
            Assert.AreEqual((ulong)2113663, DecompressSymbol("01111111 01111111 11111111"));
        }
        [TestMethod]
        public void Decompress_2113664() {
            Assert.AreEqual((ulong)2113664, DecompressSymbol("00000000 00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_Max() {
            Assert.AreEqual(ulong.MaxValue, DecompressSymbol("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000"));
        }

        [TestMethod]
        public void Decompress_1_1_1() {
            var set = DecompressSet("10000001 10000001 10000001");
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual((ulong)1, set[0]);
            Assert.AreEqual((ulong)1, set[1]);
            Assert.AreEqual((ulong)1, set[2]);
        }

        [TestMethod]
        public void Decompress_InsufficentBytes1() {
            var codec = new VLQCodec();
            codec.IncludeHeader = false;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes(""));
            Assert.AreEqual(0, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_InsufficentBytes2() {
            var codec = new VLQCodec();
            codec.IncludeHeader = false;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("00000000"));
            Assert.AreEqual(0, codec.Decompress());
        }

        [TestMethod]
        public void Decompress_Empty1_WithHeader() {
            var codec = new VLQCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes(""));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_Empty2_WithHeader() {
            var codec = new VLQCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("10000000"));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_Empty3_WithHeader() {
            var codec = new VLQCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("10000000 00000000")); // Trailing bits are ignored
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_0_WithHeader() {
            Assert.AreEqual((ulong)0, DecompressSymbol("10000001 10000000", true));
        }
        [TestMethod]
        public void Decompress_1_WithHeader() {
            Assert.AreEqual((ulong)1, DecompressSymbol("10000001 10000001", true));
        }
        [TestMethod]
        public void Decompress_127_WithHeader() {
            Assert.AreEqual((ulong)127, DecompressSymbol("10000001 11111111", true));
        }
        [TestMethod]
        public void Decompress_128_WithHeader() {
            Assert.AreEqual((ulong)128, DecompressSymbol("10000001 00000000 10000000", true));
        }
        [TestMethod]
        public void Decompress_UnneededBytes_WithHeader() {
            Assert.AreEqual((ulong)1, DecompressSymbol("10000001 10000001 10000000", true)); // Ignore trailing bytes
        }
        [TestMethod]
        public void Decompress_InsufficentBytes1_WithHeader() {
            var codec = new VLQCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes(""));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_InsufficentBytes2_WithHeader() {
            var codec = new VLQCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("00000000"));
            Assert.AreEqual(1, codec.Decompress());
        }
        [TestMethod]
        public void Decompress_InsufficentBytes3_WithHeader() {
            var codec = new VLQCodec();
            codec.IncludeHeader = true;
            codec.CompressedSet = new Buffer<byte>(BitOperation.ParseToBytes("10000001"));
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

            for (var i = 0; i < set.Length; i++) {
                set[i] = (ulong)i;
            }

            var encoded = CompressSet(set, true);
            var decoded = DecompressSet(encoded, true);

            Assert.AreEqual(set.Length, decoded.Length);
            for (var i = 0; i < set.Length; i++) {
                Assert.AreEqual((ulong)i, decoded[i]);
            }
        }
    }
}
