
using InvertedTomato.IO.Bits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class EliasDeltaUnsignedReaderTests {
        private ulong Read(string input) {
            return EliasDeltaUnsignedReader.ReadOneDefault(BitOperation.ParseToBytes(input));
        }

        [TestMethod]
        public void Read_0() {
            Assert.AreEqual((ulong)0, Read("10000000"));
        }
        [TestMethod]
        public void Read_1() {
            Assert.AreEqual((ulong)1, Read("01000000"));
        }
        [TestMethod]
        public void Read_2() {
            Assert.AreEqual((ulong)2, Read("01010000"));
        }
        [TestMethod]
        public void Read_3() {
            Assert.AreEqual((ulong)3, Read("01100000"));
        }
        [TestMethod]
        public void Read_4() {
            Assert.AreEqual((ulong)4, Read("01101000"));
        }
        [TestMethod]
        public void Read_5() {
            Assert.AreEqual((ulong)5, Read("01110000"));
        }
        [TestMethod]
        public void Read_6() {
            Assert.AreEqual((ulong)6, Read("01111000"));
        }
        [TestMethod]
        public void Read_7() {
            Assert.AreEqual((ulong)7, Read("00100000"));
        }
        [TestMethod]
        public void Read_8() {
            Assert.AreEqual((ulong)8, Read("00100001"));
        }
        [TestMethod]
        public void Read_9() {
            Assert.AreEqual((ulong)9, Read("00100010"));
        }
        [TestMethod]
        public void Read_10() {
            Assert.AreEqual((ulong)10, Read("00100011"));
        }
        [TestMethod]
        public void Read_11() {
            Assert.AreEqual((ulong)11, Read("00100100"));
        }
        [TestMethod]
        public void Read_12() {
            Assert.AreEqual((ulong)12, Read("00100101"));
        }
        [TestMethod]
        public void Read_13() {
            Assert.AreEqual((ulong)13, Read("00100110"));
        }
        [TestMethod]
        public void Read_14() {
            Assert.AreEqual((ulong)14, Read("00100111"));
        }
        [TestMethod]
        public void Read_15() {
            Assert.AreEqual((ulong)15, Read("00101000 00000000"));
        }
        [TestMethod]
        public void Read_16() {
            Assert.AreEqual((ulong)16, Read("00101000 10000000"));
        }

        [TestMethod]
        public void Read_2_2_2() {
            using (var stream = new MemoryStream(BitOperation.ParseToBytes("01010101 01010000"))) {
                using (var reader = new EliasDeltaUnsignedReader(stream)) {
                    Assert.AreEqual((ulong)2, reader.Read());
                    Assert.AreEqual((ulong)2, reader.Read());
                    Assert.AreEqual((ulong)2, reader.Read());
                }
            }
        }
        [TestMethod]
        public void Read_15_15_15() {
            using (var stream = new MemoryStream(BitOperation.ParseToBytes("00101000 00010100 00001010 00000000"))) {
                using (var reader = new EliasDeltaUnsignedReader(stream)) {
                    Assert.AreEqual((ulong)15, reader.Read());
                    Assert.AreEqual((ulong)15, reader.Read());
                    Assert.AreEqual((ulong)15, reader.Read());
                }
            }
        }
    }
}
