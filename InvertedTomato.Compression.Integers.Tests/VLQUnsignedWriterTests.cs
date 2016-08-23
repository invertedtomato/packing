using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class VLQUnsignedWriterTests {
        private string Write(ulong value) {
            return VLQUnsignedWriter.WriteOneDefault(value).ToBinaryString();
        }
        
        [TestMethod]
        public void Write_0() {
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
        public void Write_1_1_1() {
            using (var stream = new MemoryStream()) {
                using (var writer = new VLQUnsignedWriter(stream)) {
                    writer.Write(1);
                    writer.Write(1);
                    writer.Write(1);
                }
                Assert.AreEqual("10000001 10000001 10000001", stream.ToArray().ToBinaryString());
            }
        }
        [TestMethod]
        public void Write_128_128_128() {
            using (var stream = new MemoryStream()) {
                using (var writer = new VLQUnsignedWriter(stream)) {
                    writer.Write(128);
                    writer.Write(128);
                    writer.Write(128);
                }
                Assert.AreEqual("00000000 10000000 00000000 10000000 00000000 10000000", stream.ToArray().ToBinaryString());
            }
        }

        [TestMethod]
        public void WriteRead_First1000() {
            for (ulong input = 0; input < 1000; input++) {
                var encoded = VLQUnsignedWriter.WriteOneDefault(input);
                var output = VLQUnsignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }
        [TestMethod]
        public void WriteRead_First1000_Appending() {
            ulong min = 0;
            ulong max = 1000;

            using (var stream = new MemoryStream()) {
                using (var writer = new VLQUnsignedWriter(stream)) {
                    for (var i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new VLQUnsignedReader(stream)) {
                    for (var i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
