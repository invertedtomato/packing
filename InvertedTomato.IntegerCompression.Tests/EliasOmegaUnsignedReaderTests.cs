using InvertedTomato.IntegerCompression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class EliasOmegaUnsignedReaderTests {
        private ulong TestRead(string input) {
            return EliasOmegaUnsignedReader.ReadAll(BinaryToByte(input)).First();
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
        public void Read_1() {
            Assert.AreEqual((ulong)1, TestRead("00000000"));
        }
        [TestMethod]
        public void Read_2() {
            Assert.AreEqual((ulong)2, TestRead("10000000"));
        }
        [TestMethod]
        public void Read_3() {
            Assert.AreEqual((ulong)3, TestRead("11000000"));
        }
        [TestMethod]
        public void Read_4() {
            Assert.AreEqual((ulong)4, TestRead("10100000"));
        }
        [TestMethod]
        public void Read_5() {
            Assert.AreEqual((ulong)5, TestRead("10101000"));
        }
        [TestMethod]
        public void Read_6() {
            Assert.AreEqual((ulong)6, TestRead("10110000"));
        }
        [TestMethod]
        public void Read_7() {
            Assert.AreEqual((ulong)7, TestRead("10111000"));
        }
        [TestMethod]
        public void Read_8() {
            Assert.AreEqual((ulong)8, TestRead("11100000"));
        }
        [TestMethod]
        public void Read_9() {
            Assert.AreEqual((ulong)9, TestRead("11100100"));
        }
        [TestMethod]
        public void Read_10() {
            Assert.AreEqual((ulong)10, TestRead("11101000"));
        }
        [TestMethod]
        public void Read_11() {
            Assert.AreEqual((ulong)11, TestRead("11101100"));
        }
        [TestMethod]
        public void Read_12() {
            Assert.AreEqual((ulong)12, TestRead("11110000"));
        }
        [TestMethod]
        public void Read_13() {
            Assert.AreEqual((ulong)13, TestRead("11110100"));
        }
        [TestMethod]
        public void Read_14() {
            Assert.AreEqual((ulong)14, TestRead("11111000"));
        }
        [TestMethod]
        public void Read_15() {
            Assert.AreEqual((ulong)15, TestRead("11111100"));
        }
        [TestMethod]
        public void Read_16() {
            Assert.AreEqual((ulong)16, TestRead("10100100 00000000"));
        }
        [TestMethod]
        public void Read_17() {
            Assert.AreEqual((ulong)17, TestRead("10100100 01000000"));
        }
        [TestMethod]
        public void Read_100() {
            Assert.AreEqual((ulong)100, TestRead("10110110 01000000"));
        }
        [TestMethod]
        public void Read_1000() {
            Assert.AreEqual((ulong)1000, TestRead("11100111 11101000 00000000"));
        }
        [TestMethod]
        public void Read_10000() {
            Assert.AreEqual((ulong)10000, TestRead("11110110 01110001 00000000"));
        }
        [TestMethod]
        public void Read_100000() {
            Assert.AreEqual((ulong)100000, TestRead("10100100 00110000 11010100 00000000"));
        }
        [TestMethod]
        public void Read_1000000() {
            Assert.AreEqual((ulong)1000000, TestRead("10100100 11111101 00001001 00000000"));
        }
        [TestMethod]
        public void Read_Max() {
            Assert.AreEqual((ulong)ulong.MaxValue, TestRead("10101111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11100000"));
        }

        [TestMethod]
        public void Read_3_3_3() {
            var result = EliasOmegaUnsignedReader.ReadAll(BinaryToByte("11011011 00000000"));
            Assert.AreEqual(10, result.Count());
            Assert.AreEqual((ulong)3, result.ElementAt(0));
            Assert.AreEqual((ulong)3, result.ElementAt(1));
            Assert.AreEqual((ulong)3, result.ElementAt(2));
            Assert.AreEqual((ulong)1, result.ElementAt(3));
            Assert.AreEqual((ulong)1, result.ElementAt(4));
            Assert.AreEqual((ulong)1, result.ElementAt(5));
            Assert.AreEqual((ulong)1, result.ElementAt(6));
            Assert.AreEqual((ulong)1, result.ElementAt(7));
            Assert.AreEqual((ulong)1, result.ElementAt(8));
            Assert.AreEqual((ulong)1, result.ElementAt(9));
        }
        [TestMethod]
        public void Read_16_16_16() {
            var result = EliasOmegaUnsignedReader.ReadAll(BinaryToByte("10100100 00010100 10000010 10010000 00000000"));
            Assert.AreEqual(10, result.Count());
            Assert.AreEqual((ulong)16, result.ElementAt(0));
            Assert.AreEqual((ulong)16, result.ElementAt(1));
            Assert.AreEqual((ulong)16, result.ElementAt(2));
            Assert.AreEqual((ulong)1, result.ElementAt(3));
            Assert.AreEqual((ulong)1, result.ElementAt(4));
            Assert.AreEqual((ulong)1, result.ElementAt(5));
            Assert.AreEqual((ulong)1, result.ElementAt(6));
            Assert.AreEqual((ulong)1, result.ElementAt(7));
            Assert.AreEqual((ulong)1, result.ElementAt(8));
            Assert.AreEqual((ulong)1, result.ElementAt(9));
        }
    }
}
