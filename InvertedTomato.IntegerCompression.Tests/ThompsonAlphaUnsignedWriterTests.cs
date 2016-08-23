using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class ThompsonAlphaUnsignedWriterTests {
        private string Write(ulong value) {
            return ThompsonAlphaUnsignedWriter.WriteOneDefault(value).ToBinaryString();
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
            using (var stream = new MemoryStream()) {
                using (var writer = new ThompsonAlphaUnsignedWriter(stream)) {
                    writer.Write(1);
                    writer.Write(1);
                    writer.Write(1);
                }
                Assert.AreEqual("00000100 00001000 00010000", stream.ToArray().ToBinaryString());
            }
        }
        [TestMethod]
        public void Write_4_4_4() {
            using (var stream = new MemoryStream()) {
                using (var writer = new ThompsonAlphaUnsignedWriter(stream)) {
                    writer.Write(4);
                    writer.Write(4);
                    writer.Write(4);
                }
                Assert.AreEqual("00001001 00001001 00001001", stream.ToArray().ToBinaryString());
            }
        }


        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = ThompsonAlphaUnsignedWriter.WriteOneDefault(input);
                var output = ThompsonAlphaUnsignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong min = 0;
            ulong max = 1000;

            using (var stream = new MemoryStream()) {
                using (var writer = new ThompsonAlphaUnsignedWriter(stream)) {
                    for (var i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new ThompsonAlphaUnsignedReader(stream)) {
                    for (var i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
