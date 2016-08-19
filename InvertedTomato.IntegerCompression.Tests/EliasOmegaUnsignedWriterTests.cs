using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.IntegerCompression;
using System.Linq;
using System.Collections.Generic;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class EliasOmegaUnsignedWriterTests {
        private string TestWrite(params ulong[] values) {
            var result = EliasOmegaUnsignedWriter.WriteAll(1, values);

            return ByteToBinary(result);
        }

        private string ByteToBinary(byte[] input) {
            var text = "";
            foreach (var b in input) {
                text += Convert.ToString(b, 2).PadLeft(8, '0') + " ";
            }

            return text.Trim();
        }

        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("00000000", TestWrite(1));
        }
        [TestMethod]
        public void Write_2() {
            Assert.AreEqual("10000000", TestWrite(2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("11000000", TestWrite(3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("10100000", TestWrite(4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("10101000", TestWrite(5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("10110000", TestWrite(6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("10111000", TestWrite(7));
        }
        [TestMethod]
        public void Write_8() {
            Assert.AreEqual("11100000", TestWrite(8));
        }
        [TestMethod]
        public void Write_9() {
            Assert.AreEqual("11100100", TestWrite(9));
        }
        [TestMethod]
        public void Write_10() {
            Assert.AreEqual("11101000", TestWrite(10));
        }
        [TestMethod]
        public void Write_11() {
            Assert.AreEqual("11101100", TestWrite(11));
        }
        [TestMethod]
        public void Write_12() {
            Assert.AreEqual("11110000", TestWrite(12));
        }
        [TestMethod]
        public void Write_13() {
            Assert.AreEqual("11110100", TestWrite(13));
        }
        [TestMethod]
        public void Write_14() {
            Assert.AreEqual("11111000", TestWrite(14));
        }
        [TestMethod]
        public void Write_15() {
            Assert.AreEqual("11111100", TestWrite(15));
        }
        [TestMethod]
        public void Write_16() {
            Assert.AreEqual("10100100 00000000", TestWrite(16));
        }
        [TestMethod]
        public void Write_17() {
            Assert.AreEqual("10100100 01000000", TestWrite(17));
        }
        [TestMethod]
        public void Write_100() {
            Assert.AreEqual("10110110 01000000", TestWrite(100));
        }
        [TestMethod]
        public void Write_1000() {
            Assert.AreEqual("11100111 11101000 00000000", TestWrite(1000));
        }
        [TestMethod]
        public void Write_10000() {
            Assert.AreEqual("11110110 01110001 00000000", TestWrite(10000));
        }
        [TestMethod]
        public void Write_100000() {
            Assert.AreEqual("10100100 00110000 11010100 00000000", TestWrite(100000));
        }
        [TestMethod]
        public void Write_1000000() {
            Assert.AreEqual("10100100 11111101 00001001 00000000", TestWrite(1000000));
        }
        [TestMethod]
        public void Write_Max() {
            Assert.AreEqual("10101111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11100000", TestWrite(ulong.MaxValue));
        }
        [TestMethod]
        public void Write_3_3_3() {
            Assert.AreEqual("11011011 00000000", TestWrite(3, 3, 3));
        }
        [TestMethod]
        public void Write_16_16_16() {
            Assert.AreEqual("10100100 00010100 10000010 10010000 00000000", TestWrite(16, 16, 16));
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 1; input < 1000; input++) {
                var encoded = EliasOmegaUnsignedWriter.WriteAll(1, new List<ulong>() { input });
                var output = EliasOmegaUnsignedReader.ReadAll(1, encoded);

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

            var encoded = EliasOmegaUnsignedWriter.WriteAll(1, input);
            var output = EliasOmegaUnsignedReader.ReadAll(1, encoded);

            Assert.IsTrue((ulong)output.Count() >= max); // The padding zeros may cause us to get more values than we input

            for (i = 1; i <= max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i - 1));
            }
        }
    }
}
