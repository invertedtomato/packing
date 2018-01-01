using InvertedTomato.IO.Buffers;
using InvertedTomato.IO.Bits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace InvertedTomato.Compression.Integers {
    [TestClass]
    public class RawCodecTests {
        private readonly Codec Codec = new RawCodec();

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
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(0));
        }
        [TestMethod]
        public void Compress_1() {
            Assert.AreEqual("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(1));
        }
        [TestMethod]
        public void Compress_2() {
            Assert.AreEqual("00000010 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(2));
        }
        [TestMethod]
        public void Compress_3() {
            Assert.AreEqual("00000011 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressOne(3));
        }
        [TestMethod]
        public void Compress_Max() {
            Assert.AreEqual("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111", CompressOne(UInt64.MaxValue));
        }
        [TestMethod]
        public void Compress_1_1_1() {
            Assert.AreEqual("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressMany(new UInt64[] { 1, 1, 1 }, 3 * 8));
        }


        private UInt64[] DecompressMany(String value, Int32 count) {
            var input = new MemoryStream(BitOperation.ParseToBytes(value));
            var codec = new RawCodec();
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
            Assert.AreEqual((UInt64)0, DecompressOne("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((UInt64)1, DecompressOne("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((UInt64)2, DecompressOne("00000010 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((UInt64)3, DecompressOne("00000011 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_Max() {
            Assert.AreEqual(RawCodec.MaxValue, DecompressOne("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111"));
        }
        [TestMethod]
        public void Decompress_1_1_1() {
            var set = DecompressMany("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", 3);
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
