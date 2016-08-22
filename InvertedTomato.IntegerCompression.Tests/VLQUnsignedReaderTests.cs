using InvertedTomato.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class VLQUnsignedReaderTests {
        private ulong Read(string input) {
            return VLQUnsignedReader.ReadAll(ByteArrayUtility.ParseBinaryString(input)).First();
        }

        [TestMethod]
        public void Read_0() {
            Assert.AreEqual(ulong.MinValue, Read("10000000"));
        }
        [TestMethod]
        public void Read_1() {
            Assert.AreEqual((ulong)1, Read("10000001"));
        }
        [TestMethod]
        public void Read_127() {
            Assert.AreEqual((ulong)127, Read("11111111"));
        }
        [TestMethod]
        public void Read_128() {
            Assert.AreEqual((ulong)128, Read("00000000 10000000"));
        }
        [TestMethod]
        public void Read_129() {
            Assert.AreEqual((ulong)129, Read("00000001 10000000"));
        }
        [TestMethod]
        public void Read_16511() {
            Assert.AreEqual((ulong)16511, Read("01111111 11111111"));
        }
        [TestMethod]
        public void Read_16512() {
            Assert.AreEqual((ulong)16512, Read("00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Read_16513() {
            Assert.AreEqual((ulong)16513, Read("00000001 00000000 10000000"));
        }
        [TestMethod]
        public void Read_2113663() {
            Assert.AreEqual((ulong)2113663, Read("01111111 01111111 11111111"));
        }
        [TestMethod]
        public void Read_2113664() {
            Assert.AreEqual((ulong)2113664, Read("00000000 00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Read_Max() {
            Assert.AreEqual(ulong.MaxValue, Read("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000"));
        }

        [TestMethod]
        public void Read_UnneededBytes() {
            Assert.AreEqual((ulong)1, Read("10000001 10000000 10000000"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Read_InsufficentBytes1() {
            Assert.AreEqual((ulong)1, Read(""));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Read_InsufficentBytes2() {
            Assert.AreEqual((ulong)1, Read("00000000"));
        }
    }
}
