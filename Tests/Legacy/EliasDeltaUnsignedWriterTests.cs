using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class EliasDeltaUnsignedWriterTests {
        private string Write(ulong value) {
            return EliasDeltaUnsignedWriter.WriteOneDefault(value).ToBinaryString();
        }

        [TestMethod]
        public void Write_0() {
            Assert.AreEqual("10000000", Write(0));
        }
        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("01000000", Write(1));
        }
        [TestMethod]
        public void Write_2() {
            Assert.AreEqual("01010000", Write(2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("01100000", Write(3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("01101000", Write(4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("01110000", Write(5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("01111000", Write(6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("00100000", Write(7));
        }
        [TestMethod]
        public void Write_8() {
            Assert.AreEqual("00100001", Write(8));
        }
        [TestMethod]
        public void Write_9() {
            Assert.AreEqual("00100010", Write(9));
        }
        [TestMethod]
        public void Write_10() {
            Assert.AreEqual("00100011", Write(10));
        }
        [TestMethod]
        public void Write_11() {
            Assert.AreEqual("00100100", Write(11));
        }
        [TestMethod]
        public void Write_12() {
            Assert.AreEqual("00100101", Write(12));
        }
        [TestMethod]
        public void Write_13() {
            Assert.AreEqual("00100110", Write(13));
        }
        [TestMethod]
        public void Write_14() {
            Assert.AreEqual("00100111", Write(14));
        }
        [TestMethod]
        public void Write_15() {
            Assert.AreEqual("00101000 00000000", Write(15));
        }
        [TestMethod]
        public void Write_16() {
            Assert.AreEqual("00101000 10000000", Write(16));
        }

        [TestMethod]
        public void Write_2_2_2() {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasDeltaUnsignedWriter(stream)) {
                    writer.Write(2);
                    writer.Write(2);
                    writer.Write(2);
                }
                Assert.AreEqual("01010101 01010000", stream.ToArray().ToBinaryString());
            }
        }
        [TestMethod]
        public void Write_15_15_15() {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasDeltaUnsignedWriter(stream)) {
                    writer.Write(15);
                    writer.Write(15);
                    writer.Write(15);
                }
                Assert.AreEqual("00101000 00010100 00001010 00000000", stream.ToArray().ToBinaryString());
            }
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = EliasDeltaUnsignedWriter.WriteOneDefault(input);
                var output = EliasDeltaUnsignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong min = 0;
            ulong max = 1000;

            using (var stream = new MemoryStream()) {
                using (var writer = new EliasDeltaUnsignedWriter(stream)) {
                    for (var i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new EliasDeltaUnsignedReader(stream)) {
                    for (var i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
