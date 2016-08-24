using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class FibonacciUnsignedWriterTests {
        private string Write(ulong value) {
            return FibonacciUnsignedWriter.WriteOneDefault(value).ToBinaryString();
        }

        [TestMethod]
        public void Write_0() {
            Assert.AreEqual("11000000", Write(0));
        }
        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("01100000", Write(1));
        }
        [TestMethod]
        public void Write_2() {
            Assert.AreEqual("00110000", Write(2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("10110000", Write(3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("00011000", Write(4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("10011000", Write(5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("01011000", Write(6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("00001100", Write(7));
        }
        [TestMethod]
        public void Write_8() {
            Assert.AreEqual("10001100", Write(8));
        }
        [TestMethod]
        public void Write_9() {
            Assert.AreEqual("01001100", Write(9));
        }
        [TestMethod]
        public void Write_10() {
            Assert.AreEqual("00101100", Write(10));
        }
        [TestMethod]
        public void Write_11() {
            Assert.AreEqual("10101100", Write(11));
        }
        [TestMethod]
        public void Write_12() {
            Assert.AreEqual("00000110", Write(12));
        }
        [TestMethod]
        public void Write_13() {
            Assert.AreEqual("10000110", Write(13));
        }
        [TestMethod]
        public void Write_63th() {
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000011", Write(10610209857722));
        }
        [TestMethod]
        public void Write_64th() {
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000001 10000000", Write(17167680177564));
        }
        [TestMethod]
        public void Write_65th() {
            Assert.AreEqual("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000 11000000", Write(27777890035287));
        }
        [TestMethod]
        public void Write_Max() {
            Assert.AreEqual("01010000 01010001 01000001 00010101 00010010 00100100 00000010 01000100 10001000 10100000 10001010 01011000", Write(ulong.MaxValue - 1)); // Not completely sure about this value
        }
        [TestMethod]
        public void Write_1_1_1() {
            using (var stream = new MemoryStream()) {
                using (var writer = new FibonacciUnsignedWriter(stream)) {
                    writer.Write(1);
                    writer.Write(1);
                    writer.Write(1);
                }
                Assert.AreEqual("01101101 10000000", stream.ToArray().ToBinaryString());
            }
        }
        [TestMethod]
        public void Write_13_13_13() {
            using (var stream = new MemoryStream()) {
                using (var writer = new FibonacciUnsignedWriter(stream)) {
                    writer.Write(13);
                    writer.Write(13);
                    writer.Write(13);
                }
                Assert.AreEqual("10000111 00001110 00011000", stream.ToArray().ToBinaryString());
            }
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = FibonacciUnsignedWriter.WriteOneDefault(input);
                var output = FibonacciUnsignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void WriteRead_100000000000000() {
            var encoded = FibonacciUnsignedWriter.WriteOneDefault(100000000000000);
            Assert.AreEqual((ulong)100000000000000, FibonacciUnsignedReader.ReadOneDefault(encoded));
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong min = 0;
            ulong max = 1000;

            using (var stream = new MemoryStream()) {
                using (var writer = new FibonacciUnsignedWriter(stream)) {
                    for (var i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new FibonacciUnsignedReader(stream)) {
                    for (var i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
