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
    public class RawCodecTests {
        private readonly Codec Codec = new RawCodec();

        public string CompressMany(ulong[] set, int outputBufferSize = 8) {
            var input = new Buffer<ulong>(set);
            var output = new Buffer<byte>(outputBufferSize);
            Codec.CompressUnsignedBuffer(input, output);

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
            Assert.AreEqual("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111", CompressOne(ulong.MaxValue));
        }
        [TestMethod]
        public void Compress_1_1_1() {
            Assert.AreEqual("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", CompressMany(new ulong[] { 1, 1, 1 }, 3 * 8));
        }


        private ulong[] DecompressMany(string value, int count) {
            var input = new Buffer<byte>(BitOperation.ParseToBytes(value));
            var output = new Buffer<ulong>(count);
            var codec = new RawCodec();
            Assert.IsTrue(codec.DecompressUnsignedBuffer(input, output));

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
            Assert.AreEqual((ulong)0, DecompressOne("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_1() {
            Assert.AreEqual((ulong)1, DecompressOne("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_2() {
            Assert.AreEqual((ulong)2, DecompressOne("00000010 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_3() {
            Assert.AreEqual((ulong)3, DecompressOne("00000011 00000000 00000000 00000000 00000000 00000000 00000000 00000000"));
        }
        [TestMethod]
        public void Decompress_Max() {
            Assert.AreEqual(RawCodec.MaxValue, DecompressOne("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111"));
        }
        [TestMethod]
        public void Decompress_1_1_1() {
            var set = DecompressMany("00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 00000000 00000000 00000000 00000000 00000000 00000000 00000000", 3);
            Assert.AreEqual(3, set.Length);
            Assert.AreEqual((ulong)1, set[0]);
            Assert.AreEqual((ulong)1, set[1]);
            Assert.AreEqual((ulong)1, set[2]);
        }
        [TestMethod]
        [ExpectedException(typeof(InsufficentInputException))]
        public void Decompress_InputClipped() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("00000000"));
            var output = new Buffer<ulong>(1);
            var codec = new RawCodec();
            codec.DecompressUnsignedBuffer(input, output);
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
            // Create input
            var input = new Buffer<ulong>(1000);
            input.Seed(0, 999);
            
            // Compress
            var compressed = new Buffer<byte>(8000);
            Assert.IsTrue(Codec.CompressUnsignedBuffer(input, compressed));

            // Decompress
            var output = new Buffer<ulong>(1000);
            Assert.IsTrue(Codec.DecompressUnsignedBuffer(compressed, output));

            // Validate
            Assert.IsFalse(output.IsWritable);
            for (ulong i = 0; i < 1000; i++) {
                Assert.AreEqual(i, output.Dequeue());
            }
        }
    }
}
