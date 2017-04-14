using InvertedTomato.IO.Bits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class ASyncFibonacciUnsignedReaderTests {

        public ulong Read(string input) {
            return ReadMany(input).First();
        }

        public List<ulong> ReadMany(string input) {
            var output = new List<ulong>();

            var reader = new ASyncFibonacciUnsignedReader((value) => {
                output.Add(value);

                return true;
            });

            foreach (var b in BitOperation.ParseToBytes(input)) {
                reader.Insert(b);
            }

            return output;
        }

        [TestMethod]
        public void Read_0() {
            Assert.AreEqual((ulong)0, Read("11000000"));
        }
        [TestMethod]
        public void Read_1() {
            Assert.AreEqual((ulong)1, Read("01100000"));
        }
        [TestMethod]
        public void Read_2() {
            Assert.AreEqual((ulong)2, Read("00110000"));
        }
        [TestMethod]
        public void Read_3() {
            Assert.AreEqual((ulong)3, Read("10110000"));
        }
        [TestMethod]
        public void Read_4() {
            Assert.AreEqual((ulong)4, Read("00011000"));
        }
        [TestMethod]
        public void Read_5() {
            Assert.AreEqual((ulong)5, Read("10011000"));
        }
        [TestMethod]
        public void Read_6() {
            Assert.AreEqual((ulong)6, Read("01011000"));
        }
        [TestMethod]
        public void Read_7() {
            Assert.AreEqual((ulong)7, Read("00001100"));
        }
        [TestMethod]
        public void Read_8() {
            Assert.AreEqual((ulong)8, Read("10001100"));
        }
        [TestMethod]
        public void Read_9() {
            Assert.AreEqual((ulong)9, Read("01001100"));
        }
        [TestMethod]
        public void Read_10() {
            Assert.AreEqual((ulong)10, Read("00101100"));
        }
        [TestMethod]
        public void Read_11() {
            Assert.AreEqual((ulong)11, Read("10101100"));
        }
        [TestMethod]
        public void Read_12() {
            Assert.AreEqual((ulong)12, Read("00000110"));
        }
        [TestMethod]
        public void Read_13() {
            Assert.AreEqual((ulong)13, Read("10000110"));
        }

        [TestMethod]
        public void Read_1_1_1() {
            var result = ReadMany("01101101 10000000");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual((ulong)1, result[0]);
            Assert.AreEqual((ulong)1, result[1]);
            Assert.AreEqual((ulong)1, result[2]);
        }
        [TestMethod]
        public void Read_13_13_13() {
            var result = ReadMany("10000111 00001110 00011000");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual((ulong)13, result[0]);
            Assert.AreEqual((ulong)13, result[1]);
            Assert.AreEqual((ulong)13, result[2]);
        }
    }
}
