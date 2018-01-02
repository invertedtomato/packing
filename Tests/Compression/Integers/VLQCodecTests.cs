using InvertedTomato.IO.Buffers;
using InvertedTomato.IO.Bits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    [TestClass]
    public class VLQCodecTests {
        private readonly Codec Codec = new VLQCodec();

        public String CompressMany(UInt64[] set, Int32 outputBufferSize = 8) {
            var output = new MemoryStream(outputBufferSize);
            Codec.CompressUnsigned(output, set);

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
            Assert.AreEqual("01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", CompressOne(VLQCodec.MaxValue, 32));
        }
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Compress_Overflow() {
             CompressOne(UInt64.MaxValue, 32);
        }
        [TestMethod]
        public void Compress_1_1_1() {
            Assert.AreEqual("10000001 10000001 10000001", CompressMany(new UInt64[] { 1, 1, 1 }));
        }
        [TestMethod]
        public void Compress_128_128_128() {
            Assert.AreEqual("00000000 10000000 00000000 10000000 00000000 10000000", CompressMany(new UInt64[] { 128, 128, 128 }));
        }
        [TestMethod]
        public void Compress_OutputPerfectSize() {
            var input = new UInt64[] { 128 };
            var output = new MemoryStream();
            Codec.CompressUnsigned(output, input);
            Assert.AreEqual("00000000 10000000", output.ToArray().ToBinaryString());
        }




        private UInt64[] DecompressMany(String value, int count) {
            var input = new MemoryStream(BitOperation.ParseToBytes(value));
            var output = Codec.DecompressUnsigned(input, count);
            return output.ToArray();
        }
        private UInt64 DecompressOne(String value) {
            var output = DecompressMany(value, 1);
            Assert.AreEqual(1, output.Length);
            return output[0];
        }

        [TestMethod]
        public void Decompress_Empty() {
            Assert.AreEqual(0, DecompressMany("", 0).Length);
        }
        [TestMethod]
        public void Decompress_0() {
            Assert.AreEqual((UInt64)0, DecompressOne("10000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((UInt64)1, DecompressOne("10000001"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((UInt64)2, DecompressOne("10000010"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((UInt64)3, DecompressOne("10000011"));
        }
        [TestMethod]
        public void Decompress_127() {
            Assert.AreEqual((UInt64)127, DecompressOne("11111111"));
        }
        [TestMethod]
        public void Decompress_128() {
            Assert.AreEqual((UInt64)128, DecompressOne("00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_129() {
            Assert.AreEqual((UInt64)129, DecompressOne("00000001 10000000"));
        }
        [TestMethod]
        public void Decompress_16511() {
            Assert.AreEqual((UInt64)16511, DecompressOne("01111111 11111111"));
        }
        [TestMethod]
        public void Decompress_16512() {
            Assert.AreEqual((UInt64)16512, DecompressOne("00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_16513() {
            Assert.AreEqual((UInt64)16513, DecompressOne("00000001 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_2113663() {
            Assert.AreEqual((UInt64)2113663, DecompressOne("01111111 01111111 11111111"));
        }
        [TestMethod]
        public void Decompress_2113664() {
            Assert.AreEqual((UInt64)2113664, DecompressOne("00000000 00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Decompress_Max() {
            Assert.AreEqual(VLQCodec.MaxValue, DecompressOne("01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000"));
        }
        [TestMethod]
        public void Decompress_1_1_1() {
            var set = DecompressMany("10000001 10000001 10000001", 3);
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual((UInt64)1, set[0]);
            Assert.AreEqual((UInt64)1, set[1]);
            Assert.AreEqual((UInt64)1, set[2]);
        }
        [TestMethod]
        [ExpectedException(typeof(EndOfStreamException))]
        public void Decompress_InputClipped() {
            var input = new MemoryStream(BitOperation.ParseToBytes("00000000"));
            var output = Codec.DecompressUnsigned(input, 1).ToArray();
        }
        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Decompress_Overflow() {
            DecompressOne("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000");
        }

        [TestMethod]
        public void Decompress_1_X() {
            var input = new MemoryStream(BitOperation.ParseToBytes("10000001 00000011"));
            Assert.AreEqual((UInt64)1, Codec.DecompressUnsigned(input, 1).Single());
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
