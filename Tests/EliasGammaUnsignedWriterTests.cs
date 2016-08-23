using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class EliasGammaUnsignedWriterTests {
        private string Write(ulong value) {
            return EliasGammaUnsignedWriter.WriteOneDefault(value).ToBinaryString();
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
            Assert.AreEqual("01100000", Write(2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("00100000", Write(3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("00101000", Write(4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("00110000", Write(5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("00111000", Write(6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("00010000", Write(7));
        }
        [TestMethod]
        public void Write_8() {
            Assert.AreEqual("00010010", Write(8));
        }
        [TestMethod]
        public void Write_9() {
            Assert.AreEqual("00010100", Write(9));
        }
        [TestMethod]
        public void Write_10() {
            Assert.AreEqual("00010110", Write(10));
        }
        [TestMethod]
        public void Write_11() {
            Assert.AreEqual("00011000", Write(11));
        }
        [TestMethod]
        public void Write_12() {
            Assert.AreEqual("00011010", Write(12));
        }
        [TestMethod]
        public void Write_13() {
            Assert.AreEqual("00011100", Write(13));
        }
        [TestMethod]
        public void Write_14() {
            Assert.AreEqual("00011110", Write(14));
        }
        [TestMethod]
        public void Write_15() {
            Assert.AreEqual("00001000 00000000", Write(15));
        }
        [TestMethod]
        public void Write_16() {
            Assert.AreEqual("00001000 10000000", Write(16));
        }

        [TestMethod]
        public void Write_2_2_2() {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasGammaUnsignedWriter(stream)) {
                    writer.Write(2);
                    writer.Write(2);
                    writer.Write(2);
                }
                Assert.AreEqual("01101101 10000000", stream.ToArray().ToBinaryString());
            }
        }
        [TestMethod]
        public void Write_15_15_15() {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasGammaUnsignedWriter(stream)) {
                    writer.Write(15);
                    writer.Write(15);
                    writer.Write(15);
                }
                Assert.AreEqual("00001000 00000100 00000010 00000000", stream.ToArray().ToBinaryString());
            }
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = EliasGammaUnsignedWriter.WriteOneDefault(input);
                var output = EliasGammaUnsignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong min = 0;
            ulong max = 1000;

            using (var stream = new MemoryStream()) {
                using (var writer = new EliasGammaUnsignedWriter(stream)) {
                    for (var i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new EliasGammaUnsignedReader(stream)) {
                    for (var i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
