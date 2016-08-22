using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.IntegerCompression;
using System.Linq;
using System.Collections.Generic;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class VLQUnsignedWriterTests {
        private string Write(params ulong[] values) {
            var result = VLQUnsignedWriter.WriteAll(values);

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
        public void Write_Min() {
            Assert.AreEqual("10000000", Write(0));
        }
        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("10000001", Write(1));
        }
        [TestMethod]
        public void Write_127() {
            Assert.AreEqual("11111111", Write(127));
        }
        [TestMethod]
        public void Write_128() {
            Assert.AreEqual("00000000 10000000", Write(128));
        }
        [TestMethod]
        public void Write_129() {
            Assert.AreEqual("00000001 10000000", Write(129));
        }
        [TestMethod]
        public void Write_16511() {
            Assert.AreEqual("01111111 11111111", Write(16511));
        }
        [TestMethod]
        public void Write_16512() {
            Assert.AreEqual("00000000 00000000 10000000", Write(16512));
        }
        [TestMethod]
        public void Write_2113663() {
            Assert.AreEqual("01111111 01111111 11111111", Write(2113663));
        }
        [TestMethod]
        public void Write_2113664() {
            Assert.AreEqual("00000000 00000000 00000000 10000000", Write(2113664));
        }
        [TestMethod]
        public void Write_Max() {
            Assert.AreEqual("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", Write(ulong.MaxValue));
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = VLQUnsignedWriter.WriteAll(new List<ulong>() { input });
                var output = VLQUnsignedReader.ReadAll(encoded);

                Assert.AreEqual(1, output.Count());
                Assert.AreEqual(input, output.First());
            }
        }
        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong max = 1000;

            var input = new List<ulong>();
            ulong i;
            for (i = 0; i < max; i++) {
                input.Add(i);
            }

            var encoded = VLQUnsignedWriter.WriteAll(input);
            var output = VLQUnsignedReader.ReadAll(encoded);

            Assert.IsTrue((ulong)output.Count() == max);

            for (i = 0; i < max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i));
            }
        }
    }
}
