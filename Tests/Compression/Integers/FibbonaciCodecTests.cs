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
            Assert.AreEqual(set.Length, codec.CompressMany(input, output));

            return output.ToArray().ToBinaryString();
        }
        public string CompressOne(ulong value, int outputBufferSize = 8) {
            var output = new Buffer<byte>(outputBufferSize);
            var codec = new FibonacciCodec();
            codec.CompressOne(value, output);

            return output.ToArray().ToBinaryString();
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
        public void Compress_10xSize() {
            var input = new Buffer<ulong>(new ulong[10]);
            var output = new Buffer<byte>(32);
            var codec = new FibonacciCodec();
            codec.CompressMany(input, output);
            Assert.AreEqual(Math.Ceiling((float)(10 * 2) / 8), output.ToArray().Length);
        }






        private ulong[] DecompressMany(string value, int count) {
            var input = new Buffer<byte>(BitOperation.ParseToBytes(value));
            var output = new Buffer<ulong>(count);
            var codec = new FibonacciCodec();
            Assert.AreEqual(count, codec.DecompressMany(input, output));

            return output.ToArray();
        }
        private ulong DecompressOne(string value) {
            var input = new Buffer<byte>(BitOperation.ParseToBytes(value));
            var codec = new FibonacciCodec();
            return codec.DecompressOne(input);
        }

        [TestMethod]
        public void Decompress_Empty1() {
            Assert.AreEqual(0, DecompressMany("", 0).Length);
        }
        [TestMethod]
        public void Decompress_Empty2() {
            Assert.AreEqual(0, DecompressMany("00000000", 0).Length); // Trailing bits are ignored
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
            Assert.AreEqual((ulong)ulong.MaxValue - 1, DecompressOne("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011 000"));
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
        public void Decompress_InsufficentData1() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes(""));
            var output = new Buffer<ulong>(1);
            var codec = new FibonacciCodec();
            Assert.AreEqual(0, codec.DecompressMany(input, output));
        }
        [TestMethod]
        public void Decompress_InsufficentData2() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("00000000"));
            var output = new Buffer<ulong>(1);
            var codec = new FibonacciCodec();
            Assert.AreEqual(0, codec.DecompressMany(input, output));
        }
        [TestMethod]
        public void Decompress_InsufficentData3() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("01100000"));
            var output = new Buffer<ulong>(2);
            var codec = new FibonacciCodec();
            Assert.AreEqual(1, codec.DecompressMany(input, output));
        }




        /// <summary>
        /// Simulate a packet split mid-symbol.
        /// </summary>
        [TestMethod]
        public void Decompress_11_11_11_SplitWithin() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("10101110 10111010"),3);
            var output = new Buffer<ulong>(3);
            var codec = new FibonacciCodec();

            Assert.AreEqual(2, codec.DecompressMany(input, output));
            Assert.AreEqual(2, output.Used);
            Assert.AreEqual((ulong)11, output.Dequeue());
            Assert.AreEqual((ulong)11, output.Dequeue());

            input.EnqueueArray(BitOperation.ParseToBytes("11000000"));
            Assert.AreEqual(1, codec.DecompressMany(input, output));
            Assert.AreEqual(1, output.Used);
            Assert.AreEqual((ulong)11, output.Dequeue());
        }

        /// <summary>
        /// Simulate a packet split between-symbols.
        /// </summary>
        [TestMethod]
        public void Decompress_0_0_0_0_0_SplitBetween() {
            var input = new Buffer<byte>(BitOperation.ParseToBytes("11111111"),3);
            var output = new Buffer<ulong>(3);
            var codec = new FibonacciCodec();

            Assert.AreEqual(4, codec.DecompressMany(input, output));
            Assert.AreEqual(4, output.Used);
            Assert.AreEqual((ulong)0, output.Dequeue());
            Assert.AreEqual((ulong)0, output.Dequeue());
            Assert.AreEqual((ulong)0, output.Dequeue());
            Assert.AreEqual((ulong)0, output.Dequeue());

            input.EnqueueArray(BitOperation.ParseToBytes("11000000"));
            Assert.AreEqual(1, codec.DecompressMany(input, output));
            Assert.AreEqual(1, output.Used);
            Assert.AreEqual((ulong)0, output.Dequeue());
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
            var max = 1000;

            var codec = new FibonacciCodec();

            // Create input
            var input = new Buffer<ulong>(max);
            for (ulong i = 0; i < (ulong)input.MaxCapacity; i++) {
                input.Enqueue(i);
            }

            // Compress in chunks
            var compressed = new Buffer<byte>(128);
            var cycles = 0;
            while (input.Used > 0 && cycles++ < 20) {
                var count = codec.CompressMany(input, compressed);
                compressed = compressed.Resize(compressed.Used * 2);
            }
            Assert.IsTrue(cycles < 20, "Aborted - was going to loop forever");

            // Decompress in chunks
            var decompressed = new Buffer<ulong>(100);
            cycles = 0;
            ulong index = 0;
            while (index < (ulong)max &&
                cycles++ < 20) {
                codec.DecompressMany(compressed, decompressed);

                while (decompressed.Used > 0) {
                    var a = decompressed.Dequeue();
                    Assert.AreEqual(index++, a);
                }

                decompressed = decompressed.Resize(decompressed.MaxCapacity * 2);
            }

            Assert.IsTrue(cycles < 20, "Aborted - was going to loop forever");
        }
    }
}
