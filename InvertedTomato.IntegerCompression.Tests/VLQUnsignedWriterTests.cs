using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.IntegerCompression;
using System.Linq;
using System.Collections.Generic;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class VLQUnsignedWriterTests {
        private string TestWrite(ulong expectedMinValue, params ulong[] values) {
            var result = VLQUnsignedWriter.WriteAll(expectedMinValue, values);

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
        public void Write_Min1_Min() {
            Assert.AreEqual("10000000", TestWrite(0, 0));
        }
        [TestMethod]
        public void Write_Min1_1() {
            Assert.AreEqual("10000001", TestWrite(0, 1));
        }
        [TestMethod]
        public void Write_Min1_127() {
            Assert.AreEqual("11111111", TestWrite(0, 127));
        }
        [TestMethod]
        public void Write_Min1_128() {
            Assert.AreEqual("00000000 10000000", TestWrite(0, 128));
        }
        [TestMethod]
        public void Write_Min1_129() {
            Assert.AreEqual("00000001 10000000", TestWrite(0, 129));
        }
        [TestMethod]
        public void Write_Min1_16511() {
            Assert.AreEqual("01111111 11111111", TestWrite(0, 16511));
        }
        [TestMethod]
        public void Write_Min1_16512() {
            Assert.AreEqual("00000000 00000000 10000000", TestWrite(0, 16512));
        }
        [TestMethod]
        public void Write_Min1_2113663() {
            Assert.AreEqual("01111111 01111111 11111111", TestWrite(0, 2113663));
        }
        [TestMethod]
        public void Write_Min1_2113664() {
            Assert.AreEqual("00000000 00000000 00000000 10000000", TestWrite(0, 2113664));
        }
        [TestMethod]
        public void Write_Min1_Max() {
            Assert.AreEqual("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", TestWrite(0, ulong.MaxValue));
        }

        [TestMethod]
        public void Write_Min2_1() {
            Assert.AreEqual("00000001 10000000", TestWrite(256, 1));
        }
        [TestMethod]
        public void Write_Min2_255() {
            Assert.AreEqual("11111111 10000000", TestWrite(256, 255));
        }
        [TestMethod]
        public void Write_Min2_256() {
            Assert.AreEqual("00000000 10000001", TestWrite(256, 256));
        }
        [TestMethod]
        public void Write_Min2_32767() {
            Assert.AreEqual("11111111 11111111", TestWrite(256, 32767));
        }
        [TestMethod]
        public void Write_Min2_32768() {
            Assert.AreEqual("00000000 00000000 10000000", TestWrite(256, 32768));
        }

        [TestMethod]
        public void Write_Min4_1() {
            Assert.AreEqual("00000001 00000000 00000000 10000000", TestWrite(16777216, 1));
        }
        [TestMethod]
        public void Write_Min4_255() {
            Assert.AreEqual("11111111 00000000 00000000 10000000", TestWrite(16777216, 255));
        }
        [TestMethod]
        public void Write_Min4_256() {
            Assert.AreEqual("00000000 00000001 00000000 10000000", TestWrite(16777216, 256));
        }

        [TestMethod]
        public void WriteRead_Min1_First1000() {
            for (ulong input = 1; input < 1000; input++) {
                var encoded = VLQUnsignedWriter.WriteAll(1, new List<ulong>() { input });
                var output = VLQUnsignedReader.ReadAll(1, encoded);

                Assert.AreEqual(1, output.Count());
                Assert.AreEqual(input, output.First());
            }
        }
        [TestMethod]
        public void WriteRead_Min1_First1000_Appending() {
            ulong max = 1000;

            var input = new List<ulong>();
            ulong i;
            for (i = 0; i < max; i++) {
                input.Add(i);
            }

            var encoded = VLQUnsignedWriter.WriteAll(1, input);
            var output = VLQUnsignedReader.ReadAll(1, encoded);

            Assert.IsTrue((ulong)output.Count() == max);

            for (i = 0; i < max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i));
            }
        }

        [TestMethod]
        public void WriteRead_Min2_First1000() {
            for (ulong input = 1; input < 1000; input++) {
                var encoded = VLQUnsignedWriter.WriteAll(2, new List<ulong>() { input });
                var output = VLQUnsignedReader.ReadAll(2, encoded);

                Assert.AreEqual(1, output.Count());
                Assert.AreEqual(input, output.First());
            }
        }
        [TestMethod]
        public void WriteRead_Min2_First1000_Appending() {
            ulong max = 1000;

            var input = new List<ulong>();
            ulong i;
            for (i = 0; i < max; i++) {
                input.Add(i);
            }

            var encoded = VLQUnsignedWriter.WriteAll(2, input);
            var output = VLQUnsignedReader.ReadAll(2, encoded);

            Assert.IsTrue((ulong)output.Count() == max);

            for (i = 0; i < max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i));
            }
        }

        [TestMethod]
        public void WriteRead_Min4_First1000() {
            for (ulong input = 1; input < 1000; input++) {
                var encoded = VLQUnsignedWriter.WriteAll(4, new List<ulong>() { input });
                var output = VLQUnsignedReader.ReadAll(4, encoded);

                Assert.AreEqual(1, output.Count());
                Assert.AreEqual(input, output.First());
            }
        }
        [TestMethod]
        public void WriteRead_Min4_First1000_Appending() {
            ulong max = 1000;

            var input = new List<ulong>();
            ulong i;
            for (i = 0; i < max; i++) {
                input.Add(i);
            }

            var encoded = VLQUnsignedWriter.WriteAll(4, input);
            var output = VLQUnsignedReader.ReadAll(4, encoded);

            Assert.IsTrue((ulong)output.Count() == max);

            for (i = 0; i < max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)i));
            }
        }
    }
}
