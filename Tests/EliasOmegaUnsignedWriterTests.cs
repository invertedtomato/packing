using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class EliasOmegaUnsignedWriterTests {
        private string Write( ulong value) {
            return EliasOmegaUnsignedWriter.WriteOneDefault(value).ToBinaryString();
        }

        [TestMethod]
        public void Write_0() {
            Assert.AreEqual("00000000", Write(0));
        }
        [TestMethod]
        public void Write_1() {
            Assert.AreEqual("10000000", Write(1));
        }
        [TestMethod]
        public void Write_2() {
            Assert.AreEqual("11000000", Write(2));
        }
        [TestMethod]
        public void Write_3() {
            Assert.AreEqual("10100000", Write(3));
        }
        [TestMethod]
        public void Write_4() {
            Assert.AreEqual("10101000", Write(4));
        }
        [TestMethod]
        public void Write_5() {
            Assert.AreEqual("10110000", Write(5));
        }
        [TestMethod]
        public void Write_6() {
            Assert.AreEqual("10111000", Write(6));
        }
        [TestMethod]
        public void Write_7() {
            Assert.AreEqual("11100000", Write(7));
        }
        [TestMethod]
        public void Write_8() {
            Assert.AreEqual("11100100", Write(8));
        }
        [TestMethod]
        public void Write_9() {
            Assert.AreEqual("11101000", Write(9));
        }
        [TestMethod]
        public void Write_10() {
            Assert.AreEqual("11101100", Write(10));
        }
        [TestMethod]
        public void Write_11() {
            Assert.AreEqual("11110000", Write(11));
        }
        [TestMethod]
        public void Write_12() {
            Assert.AreEqual("11110100", Write(12));
        }
        [TestMethod]
        public void Write_13() {
            Assert.AreEqual("11111000", Write(13));
        }
        [TestMethod]
        public void Write_14() {
            Assert.AreEqual("11111100", Write(14));
        }
        [TestMethod]
        public void Write_15() {
            Assert.AreEqual("10100100 00000000", Write(15));
        }
        [TestMethod]
        public void Write_16() {
            Assert.AreEqual("10100100 01000000", Write(16));
        }
        [TestMethod]
        public void Write_99() {
            Assert.AreEqual("10110110 01000000", Write(99));
        }
        [TestMethod]
        public void Write_999() {
            Assert.AreEqual("11100111 11101000 00000000", Write(999));
        }
        [TestMethod]
        public void Write_9999() {
            Assert.AreEqual("11110110 01110001 00000000", Write(9999));
        }
        [TestMethod]
        public void Write_99999() {
            Assert.AreEqual("10100100 00110000 11010100 00000000", Write(99999));
        }
        [TestMethod]
        public void Write_999999() {
            Assert.AreEqual("10100100 11111101 00001001 00000000", Write(999999));
        }
        [TestMethod]
        public void Write_Max() {
            Assert.AreEqual("10101111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111 11100000", Write(ulong.MaxValue - 1));
        }
        [TestMethod]
        public void Write_2_2_2() {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasOmegaUnsignedWriter(stream)) {
                    writer.Write(2);
                    writer.Write(2);
                    writer.Write(2);
                }
                Assert.AreEqual("11011011 00000000", stream.ToArray().ToBinaryString());
            }
        }
        [TestMethod]
        public void Write_15_15_15() {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasOmegaUnsignedWriter(stream)) {
                    writer.Write(15);
                    writer.Write(15);
                    writer.Write(15);
                }
                Assert.AreEqual("10100100 00010100 10000010 10010000 00000000", stream.ToArray().ToBinaryString());
            }
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = EliasOmegaUnsignedWriter.WriteOneDefault(input);
                var output = EliasOmegaUnsignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong min = 0;
            ulong max = 1000;

            using (var stream = new MemoryStream()) {
                using (var writer = new EliasOmegaUnsignedWriter(stream)) {
                    for (var i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new EliasOmegaUnsignedReader(stream)) {
                    for (var i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
