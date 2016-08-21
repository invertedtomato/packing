using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class EliasGammaUnsignedWriterTests {
        private string TestWrite(params ulong[] values) {
            var result = EliasGammaUnsignedWriter.WriteAll(values);

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
        public void Write_0() {
            Assert.AreEqual("10000000", TestWrite(0));
        }
        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("01000000", TestWrite(1));
        }
        [TestMethod]
        public void Write_2() {
            Assert.AreEqual("01100000", TestWrite(2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("00100000", TestWrite(3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("00101000", TestWrite(4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("00110000", TestWrite(5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("00111000", TestWrite(6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("00010000", TestWrite(7));
        }
        [TestMethod]
        public void Write_8() {
            Assert.AreEqual("00010010", TestWrite(8));
        }
        [TestMethod]
        public void Write_9() {
            Assert.AreEqual("00010100", TestWrite(9));
        }
        [TestMethod]
        public void Write_10() {
            Assert.AreEqual("00010110", TestWrite(10));
        }
        [TestMethod]
        public void Write_11() {
            Assert.AreEqual("00011000", TestWrite(11));
        }
        [TestMethod]
        public void Write_12() {
            Assert.AreEqual("00011010", TestWrite(12));
        }
        [TestMethod]
        public void Write_13() {
            Assert.AreEqual("00011100", TestWrite(13));
        }
        [TestMethod]
        public void Write_14() {
            Assert.AreEqual("00011110", TestWrite(14));
        }
        [TestMethod]
        public void Write_15() {
            Assert.AreEqual("00001000 00000000", TestWrite(15));
        }
        [TestMethod]
        public void Write_16() {
            Assert.AreEqual("00001000 10000000", TestWrite(16));
        }

        [TestMethod]
        public void Write_2_2_2() {
            Assert.AreEqual("01101101 10000000", TestWrite(2, 2, 2));
        }
        [TestMethod]
        public void Write_15_15_15() {
            Assert.AreEqual("00001000 00000100 00000010 00000000", TestWrite(15, 15, 15));
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 1; input < 1000; input++) {
                var encoded = EliasGammaUnsignedWriter.WriteAll(new List<ulong>() { input });
                var output = EliasGammaUnsignedReader.ReadAll(encoded);

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

            var encoded = EliasGammaUnsignedWriter.WriteAll(input);
            var output = EliasGammaUnsignedReader.ReadAll(encoded);

            Assert.IsTrue((ulong)output.Count() >= max); // The padding zeros may cause us to get more values than we input

            for (i = 1; i <= max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i - 1));
            }
        }
    }
}
