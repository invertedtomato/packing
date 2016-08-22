using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace InvertedTomato.IntegerCompression.Tests {
    [TestClass]
    public class ThompsonAlphaSignedReaderTests {
        [TestMethod]
        public void WriteRead_First1000() {
            for (long input = -500; input < 500; input++) {
                var encoded = ThompsonAlphaSignedWriter.WriteAll(new List<long>() { input });
                var output = ThompsonAlphaSignedReader.ReadAll(encoded);

                Assert.IsTrue(output.Count() >= 1);
                Assert.AreEqual(input, output.First());
            }
        }

        [TestMethod]
        public void WriteRead_First1000_Appending() {
            long min = -2;
            long max = 2;

            var input = new List<long>();
            long i;
            for (i = min; i <= max; i++) {
                input.Add(i);
            }

            var encoded = ThompsonAlphaSignedWriter.WriteAll(input);
            var output = ThompsonAlphaSignedReader.ReadAll(encoded);

            Assert.IsTrue(output.Count() >= -min + max + 1);

            for (i = min; i <= max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)(i - min)));
            }
        }
    }
}
