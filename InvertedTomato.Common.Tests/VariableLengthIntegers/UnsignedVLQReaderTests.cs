using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VariableLengthIntegers;
using System.Linq;

namespace InvertedTomato.Common.Tests {
    [TestClass]
    public class UnsignedVLQReaderTests {
        private ulong TestRead(int minBytes, string input) {
            return UnsignedVLQReader.ReadAll(minBytes, BinaryToByte(input)).First();
        }

        private byte[] BinaryToByte(string input) {
            input = input.Replace(" ", "");

            var inputBytes = Enumerable.Range(0, input.Length / 8).Select(i => input.Substring(i * 8, 8));
            var output = new byte[inputBytes.Count()];

            for (var i = 0; i < output.Length; i++) {
                output[i] = Convert.ToByte(inputBytes.ElementAt(i), 2);
            }

            return output;
        }

        [TestMethod]
        public void Read_Min1_Min() {
            Assert.AreEqual(ulong.MinValue, TestRead(1, "10000000"));
        }
        [TestMethod]
        public void Read_Min1_1() {
            Assert.AreEqual((ulong)1, TestRead(1, "10000001"));
        }
        [TestMethod]
        public void Read_Min1_127() {
            Assert.AreEqual((ulong)127, TestRead(1, "11111111"));
        }
        [TestMethod]
        public void Read_Min1_128() {
            Assert.AreEqual((ulong)128, TestRead(1, "00000000 10000000"));
        }
        [TestMethod]
        public void Read_Min1_129() {
            Assert.AreEqual((ulong)129, TestRead(1, "00000001 10000000"));
        }
        [TestMethod]
        public void Read_Min1_16511() {
            Assert.AreEqual((ulong)16511, TestRead(1, "01111111 11111111"));
        }
        [TestMethod]
        public void Read_Min1_16512() {
            Assert.AreEqual((ulong)16512, TestRead(1, "00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Read_Min1_2113663() {
            Assert.AreEqual((ulong)2113663, TestRead(1, "01111111 01111111 11111111"));
        }
        [TestMethod]
        public void Read_Min1_2113664() {
            Assert.AreEqual((ulong)2113664, TestRead(1, "00000000 00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Read_Min1_Max() {
            Assert.AreEqual(ulong.MaxValue, TestRead(1, "01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000"));
        }

        [TestMethod]
        public void Read_Min2_1() {
            Assert.AreEqual((ulong)1, TestRead(2, "00000001 10000000"));
        }
        [TestMethod]
        public void Read_Min2_255() {
            Assert.AreEqual((ulong)255, TestRead(2, "11111111 10000000"));
        }
        [TestMethod]
        public void Read_Min2_256() {
            Assert.AreEqual((ulong)256, TestRead(2, "00000000 10000001"));
        }
        [TestMethod]
        public void Read_Min2_32767() {
            Assert.AreEqual((ulong)32767, TestRead(2, "11111111 11111111"));
        }
        [TestMethod]
        public void Read_Min2_32768() {
            Assert.AreEqual((ulong)32768, TestRead(2, "00000000 00000000 10000000"));
        }

        [TestMethod]
        public void Read_Min4_1() {
            Assert.AreEqual((ulong)1, TestRead(4, "00000001 00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Read_Min4_255() {
            Assert.AreEqual((ulong)255, TestRead(4, "11111111 00000000 00000000 10000000"));
        }
        [TestMethod]
        public void Read_Min4_256() {
            Assert.AreEqual((ulong)256, TestRead(4, "00000000 00000001 00000000 10000000"));
        }

        [TestMethod]
        public void Read_Min1_UnneededBytes() {
            Assert.AreEqual((ulong)1, TestRead(1, "10000001 10000000 10000000"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Read_Min1_InsufficentBytes1() {
            Assert.AreEqual((ulong)1, TestRead(1, ""));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Read_Min1_InsufficentBytes2() {
            Assert.AreEqual((ulong)1, TestRead(1, "00000000"));
        }
    }
}
