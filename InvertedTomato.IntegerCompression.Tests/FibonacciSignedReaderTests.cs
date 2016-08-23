using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class FibonacciSignedReaderTests {
        [TestMethod]
        public void WriteRead_First1000() {
            for (long input = -500; input < 500; input++) {
                var encoded = FibonacciSignedWriter.WriteOneDefault(input);
                var output = FibonacciSignedReader.ReadOneDefault(encoded);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            long min = -500;
            long max = 500;

            using (var stream = new MemoryStream()) {
                using (var writer = new FibonacciSignedWriter(stream)) {
                    for (long i = min; i < max; i++) {
                        writer.Write(i);
                    }
                }
                stream.Position = 0;
                using (var reader = new FibonacciSignedReader(stream)) {
                    for (long i = min; i < max; i++) {
                        Assert.AreEqual(i, reader.Read());
                    }
                }
            }
        }
    }
}
