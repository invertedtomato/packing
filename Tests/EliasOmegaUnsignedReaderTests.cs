using InvertedTomato.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class EliasOmegaUnsignedReaderTests {
        private ulong Read(string input) {
            return EliasOmegaUnsignedReader.ReadOneDefault(ByteArrayUtility.ParseBinaryString(input));
        }

        [TestMethod]
        public void Read_0() {
            Assert.AreEqual((ulong)0, Read("00000000"));
        }
        [TestMethod]
        public void Read_1() {
            Assert.AreEqual((ulong)1, Read("10000000"));
        }
        [TestMethod]
        public void Read_2() {
            Assert.AreEqual((ulong)2, Read("11000000"));
        }
        [TestMethod]
        public void Read_3() {
            Assert.AreEqual((ulong)3, Read("10100000"));
        }
        [TestMethod]
        public void Read_4() {
            Assert.AreEqual((ulong)4, Read("10101000"));
        }
        [TestMethod]
        public void Read_5() {
            Assert.AreEqual((ulong)5, Read("10110000"));
        }
        [TestMethod]
        public void Read_6() {
            Assert.AreEqual((ulong)6, Read("10111000"));
        }
        [TestMethod]
        public void Read_7() {
            Assert.AreEqual((ulong)7, Read("11100000"));
        }
        [TestMethod]
        public void Read_8() {
            Assert.AreEqual((ulong)8, Read("11100100"));
        }
        [TestMethod]
        public void Read_9() {
            Assert.AreEqual((ulong)9, Read("11101000"));
        }
        [TestMethod]
        public void Read_10() {
            Assert.AreEqual((ulong)10, Read("11101100"));
        }
        [TestMethod]
        public void Read_11() {
            Assert.AreEqual((ulong)11, Read("11110000"));
        }
        [TestMethod]
        public void Read_12() {
            Assert.AreEqual((ulong)12, Read("11110100"));
        }
        [TestMethod]
        public void Read_13() {
            Assert.AreEqual((ulong)13, Read("11111000"));
        }
        [TestMethod]
        public void Read_14() {
            Assert.AreEqual((ulong)14, Read("11111100"));
        }
        [TestMethod]
        public void Read_15() {
            Assert.AreEqual((ulong)15, Read("10100100 00000000"));
        }
        [TestMethod]
        public void Read_16() {
            Assert.AreEqual((ulong)16, Read("10100100 01000000"));
        }
        [TestMethod]
        public void Read_99() {
            Assert.AreEqual((ulong)99, Read("10110110 01000000"));
        }
        [TestMethod]
        public void Read_999() {
            Assert.AreEqual((ulong)999, Read("11100111 11101000 00000000"));
        }
        [TestMethod]
        public void Read_9999() {
            Assert.AreEqual((ulong)9999, Read("11110110 01110001 00000000"));
        }
        [TestMethod]
        public void Read_99999() {
            Assert.AreEqual((ulong)99999, Read("10100100 00110000 11010100 00000000"));
        }
        [TestMethod]
        public void Read_999999() {
            Assert.AreEqual((ulong)999999, Read("10100100 11111101 00001001 00000000"));
        }
        [TestMethod]
        public void Read_Max() {
            Assert.AreEqual(ulong.MaxValue-1, Read("10101111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11100000"));
        }

        [TestMethod]
        public void Read_2_2_2() {
            using (var stream = new MemoryStream(ByteArrayUtility.ParseBinaryString("11011011 00000000"))) {
                using (var reader = new EliasOmegaUnsignedReader(stream)) {
                    Assert.AreEqual((ulong)2, reader.Read());
                    Assert.AreEqual((ulong)2, reader.Read());
                    Assert.AreEqual((ulong)2, reader.Read());
                }
            }
        }
        [TestMethod]
        public void Read_15_15_15() {
            using (var stream = new MemoryStream(ByteArrayUtility.ParseBinaryString("10100100 00010100 10000010 10010000 00000000"))) {
                using (var reader = new EliasOmegaUnsignedReader(stream)) {
                    Assert.AreEqual((ulong)15, reader.Read());
                    Assert.AreEqual((ulong)15, reader.Read());
                    Assert.AreEqual((ulong)15, reader.Read());
                }
            }
        }
    }
}
