using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class DynamicUnsignedWriterTests {
        private string TestWrite(ulong expectedMinValue, params ulong[] values) {
            var result = DynamicUnsignedWriter.WriteAll(expectedMinValue, values);

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
            Assert.AreEqual("00000000", TestWrite(ulong.MaxValue, 0));
        }
        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("00000010", TestWrite(ulong.MaxValue, 1));
        }
        [TestMethod]
        public void Write_2() {
            Assert.AreEqual("00000110", TestWrite(ulong.MaxValue, 2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("00000111", TestWrite(ulong.MaxValue, 3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("00001010 00000000", TestWrite(ulong.MaxValue, 4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("00001010 00000000", TestWrite(ulong.MaxValue, 5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("00001110 10000000", TestWrite(ulong.MaxValue, 6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("00001110 11000000", TestWrite(ulong.MaxValue, 7));
        }
        [TestMethod]
        public void Write_Max() {
            Assert.AreEqual("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111100", TestWrite(0, ulong.MaxValue));
        }
        

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 1; input < 1000; input++) {
                var encoded = DynamicUnsignedWriter.WriteAll(1, new List<ulong>() { input });
                var output = DynamicUnsignedReader.ReadAll(1, encoded);

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

            var encoded = DynamicUnsignedWriter.WriteAll(1, input);
            var output = DynamicUnsignedReader.ReadAll(1, encoded);

            Assert.IsTrue((ulong)output.Count() == max);

            for (i = 0; i < max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i));
            }
        }
    }
}
