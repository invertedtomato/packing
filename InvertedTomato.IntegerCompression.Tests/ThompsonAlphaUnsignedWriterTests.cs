using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using InvertedTomato.IO;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class ThompsonAlphaUnsignedWriterTests {
        private string Write(params ulong[] values) {
            return ThompsonAlphaUnsignedWriter.WriteAll(values).ToBinaryString();
        }

        [TestMethod]
        public void Write_0() {
            Assert.AreEqual("00000000", Write(0));
        }
        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("00000100", Write(1));
        }
        [TestMethod]
        public void Write_2() {
            Assert.AreEqual("00000110", Write(2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("00001000", Write(3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("00001001", Write(4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("00001010", Write(5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("00001011", Write(6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("00001100 00000000", Write(7));
        }
        [TestMethod]
        public void Write_8() {
            Assert.AreEqual("00001100 10000000", Write(8));
        }
        [TestMethod]
        public void Write_9() {
            Assert.AreEqual("00001101 00000000", Write(9));
        }
        [TestMethod]
        public void Write_10() {
            Assert.AreEqual("00001101 10000000", Write(10));
        }
        [TestMethod]
        public void Write_11() {
            Assert.AreEqual("00001110 00000000", Write(11));
        }
        [TestMethod]
        public void Write_12() {
            Assert.AreEqual("00001110 10000000", Write(12));
        }
        [TestMethod]
        public void Write_13() {
            Assert.AreEqual("00001111 00000000", Write(13));
        }
        [TestMethod]
        public void Write_14() {
            Assert.AreEqual("00001111 10000000", Write(14));
        }
        [TestMethod]
        public void Write_15() {
            Assert.AreEqual("00010000 00000000", Write(15));
        }
        [TestMethod]
        public void Write_16() {
            Assert.AreEqual("00010000 01000000", Write(16));
        }
        [TestMethod]
        public void Write_Max() {
            Assert.AreEqual("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111000", Write(ulong.MaxValue - 1));
        }
        [TestMethod]
        public void Write_1_1_1() {
            Assert.AreEqual("00000100 00001000 00010000", Write(1, 1, 1));
        }
        [TestMethod]
        public void Write_4_4_4() {
            Assert.AreEqual("00001001 00001001 00001001", Write(4, 4, 4));
        }


        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 1; input < 1000; input++) {
                var encoded = ThompsonAlphaUnsignedWriter.WriteAll(new List<ulong>() { input });
                var output = ThompsonAlphaUnsignedReader.ReadAll(encoded);

                Assert.IsTrue(output.Count() > 0);
                Assert.AreEqual(input, output.First());
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong max = 1000;

            var input = new List<ulong>();
            ulong i;
            for (i = 1; i <= max; i++) {
                input.Add(i);
            }

            var encoded = ThompsonAlphaUnsignedWriter.WriteAll(input);
            var output = ThompsonAlphaUnsignedReader.ReadAll(encoded);

            Assert.IsTrue((ulong)output.Count() >= max); // The padding zeros may cause us to get more values than we input

            for (i = 1; i <= max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i - 1));
            }
        }
    }
}
