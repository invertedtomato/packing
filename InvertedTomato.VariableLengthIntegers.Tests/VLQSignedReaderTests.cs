using InvertedTomato.VariableLengthIntegers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace InvertedTomato.Common.Tests.VariableLengthIntegers {
    [TestClass]
    public class VLQSignedReaderTests {
        [TestMethod]
        public void WriteRead_First1000() {
            for (long input = -500; input < 500; input++) {
                var encoded = VLQSignedWriter.WriteAll(new List<long>() { input });
                var output = VLQSignedReader.ReadAll(encoded);

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

            var encoded = VLQSignedWriter.WriteAll(input);
            var output = VLQSignedReader.ReadAll(encoded);

            Assert.AreEqual(-min + max+1, output.Count());

            for (i = min; i <= max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)(i - min)));
            }
        }
    }
}
