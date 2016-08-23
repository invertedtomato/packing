using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InvertedTomato.Compression.Integers.Tests {
    [TestClass]
    public class VLQSignedReaderTests {
        [TestMethod]
        public void WriteRead_First1000() {
            for (long input = -500; input < 500; input++) {
                var encoded = VLQSignedWriter.WriteOneDefault(input);
                var output = VLQSignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            long min = -500;
            long max = 500;

            using (var stream = new MemoryStream()) {
                using (var writer = new VLQSignedWriter(stream)) {
                    for (long i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new VLQSignedReader(stream)) {
                    for (long i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
