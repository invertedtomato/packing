using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class DynamicSignedReaderTests {
        [TestMethod]
        public void WriteRead_First1000() {
            for (long input = -500; input < 500; input++) {
                var encoded = DynamicSignedWriter.WriteAll(new List<long>() { input });
                var output = DynamicSignedReader.ReadAll(encoded);

                Assert.AreEqual(1, output.Count());
                Assert.AreEqual(input, output.First());
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            long min = -500;
            long max = 500;

            var input = new List<long>();
            long i;
            for (i = min; i <= max; i++) {
                input.Add(i);
            }

            var encoded = DynamicSignedWriter.WriteAll(input);
            var output = DynamicSignedReader.ReadAll(encoded);

            Assert.AreEqual(-min + max+1, output.Count());

            for (i = min; i <= max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)(i - min)));
            }
        }
    }
}
