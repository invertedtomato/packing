using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class DynamicUnsignedReaderTests {
        //     LLLLLLVV VVVVVVVV
        //   0 0000000_
        //   1 0000001_
        //   2 00000110
        //   3 00000111
        //   4 00001010 0______
        //   5 00001010 1______
        //   6 00001110 10______
        //   7 00001110 11______

        
        private ulong TestRead(ulong expectedMinValue, string input) {
            return DynamicUnsignedReader.ReadAll(expectedMinValue, BinaryToByte(input)).First();
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
        public void Read_Min() {
            Assert.AreEqual(ulong.MinValue, TestRead(0, "00000000"));
        }
        [TestMethod]
        public void Read_1() {
            Assert.AreEqual((ulong)1, TestRead(0, "00000010"));
        }
        [TestMethod]
        public void Read_2() {
            Assert.AreEqual((ulong)2, TestRead(0, "00000110"));
        }
        [TestMethod]
        public void Read_3() {
            Assert.AreEqual((ulong)3, TestRead(0, "00000111"));
        }
        [TestMethod]
        public void Read_4() {
            Assert.AreEqual((ulong)4, TestRead(0, "00001010 00000000"));
        }
        [TestMethod]
        public void Read_5() {
            Assert.AreEqual((ulong)5, TestRead(0, "00001010 00000000"));
        }
        [TestMethod]
        public void Read_6() {
            Assert.AreEqual((ulong)6, TestRead(0, "00001110 10000000"));
        }
        [TestMethod]
        public void Read_7() {
            Assert.AreEqual((ulong)7, TestRead(0, "00001110 11000000"));
        }
        [TestMethod]
        public void Read_Max() {
            Assert.AreEqual(ulong.MaxValue, TestRead(0, "11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111100"));
        }
        
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Read_InsufficentBytes1() {
            Assert.AreEqual((ulong)1, TestRead(0, ""));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Read_InsufficentBytes2() {
            Assert.AreEqual((ulong)1, TestRead(0, "00010000"));
        }
    }
}
