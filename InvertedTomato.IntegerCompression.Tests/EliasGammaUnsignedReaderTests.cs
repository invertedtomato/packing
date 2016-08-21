using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class EliasGammaUnsignedReaderTests {
        private ulong TestRead(string input) {
            return EliasGammaUnsignedReader.ReadAll(BinaryToByte(input)).First();
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
        public void Read_0() {
            Assert.AreEqual((ulong)0, TestRead("10000000"));
        }
        [TestMethod]
        public void Read_1() {
            Assert.AreEqual((ulong)1, TestRead("01000000"));
        }
        [TestMethod]
        public void Read_2() {
            Assert.AreEqual((ulong)2, TestRead("01100000"));
        }
        [TestMethod]
        public void Read_3() {
            Assert.AreEqual((ulong)3, TestRead("00100000"));
        }
        [TestMethod]
        public void Read_4() {
            Assert.AreEqual((ulong)4, TestRead("00101000"));
        }
        [TestMethod]
        public void Read_5() {
            Assert.AreEqual((ulong)5, TestRead("00110000"));
        }
        [TestMethod]
        public void Read_6() {
            Assert.AreEqual((ulong)6, TestRead("00111000"));
        }
        [TestMethod]
        public void Read_7() {
            Assert.AreEqual((ulong)7, TestRead("00010000"));
        }
        [TestMethod]
        public void Read_8() {
            Assert.AreEqual((ulong)8, TestRead("00010010"));
        }
        [TestMethod]
        public void Read_9() {
            Assert.AreEqual((ulong)9, TestRead("00010100"));
        }
        [TestMethod]
        public void Read_10() {
            Assert.AreEqual((ulong)10, TestRead("00010110"));
        }
        [TestMethod]
        public void Read_11() {
            Assert.AreEqual((ulong)11, TestRead("00011000"));
        }
        [TestMethod]
        public void Read_12() {
            Assert.AreEqual((ulong)12, TestRead("00011010"));
        }
        [TestMethod]
        public void Read_13() {
            Assert.AreEqual((ulong)13, TestRead("00011100"));
        }
        [TestMethod]
        public void Read_14() {
            Assert.AreEqual((ulong)14, TestRead("00011110"));
        }
        [TestMethod]
        public void Read_15() {
            Assert.AreEqual((ulong)15, TestRead("00001000 00000000"));
        }
        [TestMethod]
        public void Read_16() {
            Assert.AreEqual((ulong)16, TestRead("00001000 10000000"));
        }

        [TestMethod]
        public void Read_2_2_2() {
            var result = EliasGammaUnsignedReader.ReadAll(BinaryToByte("01101101 10000000"));
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual((ulong)2, result.ElementAt(0));
            Assert.AreEqual((ulong)2, result.ElementAt(1));
            Assert.AreEqual((ulong)2, result.ElementAt(2));
        }
        [TestMethod]
        public void Read_15_15_15() {
            var result = EliasGammaUnsignedReader.ReadAll(BinaryToByte("00001000 00000100 00000010 00000000"));
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual((ulong)15, result.ElementAt(0));
            Assert.AreEqual((ulong)15, result.ElementAt(1));
            Assert.AreEqual((ulong)15, result.ElementAt(2));
        }
    }
}
