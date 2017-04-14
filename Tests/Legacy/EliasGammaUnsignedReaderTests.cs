
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class EliasGammaUnsignedReaderTests {
        private ulong Read(string input) {
            return EliasGammaUnsignedReader.ReadOneDefault(BitOperation.ParseToBytes(input));
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
            Assert.AreEqual((ulong)2, Read("01100000"));
        }
        [TestMethod]
        public void Read_3() {
            Assert.AreEqual((ulong)3, Read("00100000"));
        }
        [TestMethod]
        public void Read_4() {
            Assert.AreEqual((ulong)4, Read("00101000"));
        }
        [TestMethod]
        public void Read_5() {
            Assert.AreEqual((ulong)5, Read("00110000"));
        }
        [TestMethod]
        public void Read_6() {
            Assert.AreEqual((ulong)6, Read("00111000"));
        }
        [TestMethod]
        public void Read_7() {
            Assert.AreEqual((ulong)7, Read("00010000"));
        }
        [TestMethod]
        public void Read_8() {
            Assert.AreEqual((ulong)8, Read("00010010"));
        }
        [TestMethod]
        public void Read_9() {
            Assert.AreEqual((ulong)9, Read("00010100"));
        }
        [TestMethod]
        public void Read_10() {
            Assert.AreEqual((ulong)10, Read("00010110"));
        }
        [TestMethod]
        public void Read_11() {
            Assert.AreEqual((ulong)11, Read("00011000"));
        }
        [TestMethod]
        public void Read_12() {
            Assert.AreEqual((ulong)12, Read("00011010"));
        }
        [TestMethod]
        public void Read_13() {
            Assert.AreEqual((ulong)13, Read("00011100"));
        }
        [TestMethod]
        public void Read_14() {
            Assert.AreEqual((ulong)14, Read("00011110"));
        }
        [TestMethod]
        public void Read_15() {
            Assert.AreEqual((ulong)15, Read("00001000 00000000"));
        }
        [TestMethod]
        public void Read_16() {
            Assert.AreEqual((ulong)16, Read("00001000 10000000"));
        }

        [TestMethod]
        public void Read_2_2_2() {
            using (var stream = new MemoryStream(BitOperation.ParseToBytes("01101101 10000000"))) {
                using (var reader = new EliasGammaUnsignedReader(stream)) {
                    Assert.AreEqual((ulong)2, reader.Read());
                    Assert.AreEqual((ulong)2, reader.Read());
                    Assert.AreEqual((ulong)2, reader.Read());
                }
            }
        }
        [TestMethod]
        public void Read_15_15_15() {
            using (var stream = new MemoryStream(BitOperation.ParseToBytes("00001000 00000100 00000010 00000000"))) {
                using (var reader = new EliasGammaUnsignedReader(stream)) {
                    Assert.AreEqual((ulong)15, reader.Read());
                    Assert.AreEqual((ulong)15, reader.Read());
                    Assert.AreEqual((ulong)15, reader.Read());
                }
            }
        }
    }
}
