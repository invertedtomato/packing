using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VariableLengthIntegers;
using System.Collections.Generic;
using System.Linq;

namespace InvertedTomato.Common.Tests.VariableLengthIntegers {
    [TestClass]
    public class SignedEliasOmegaReaderTests {
        [TestMethod]
        public void WriteRead_First1000() {
            for (long input = -500; input < 500; input++) {
                var encoded = SignedEliasOmegaWriter.WriteAll(true, new List<long>() { input });
                var output = SignedEliasOmegaReader.ReadAll(encoded);

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

            var encoded = SignedEliasOmegaWriter.WriteAll(true, input);
            var output = SignedEliasOmegaReader.ReadAll(encoded);

            Assert.IsTrue(output.Count() >= -min + max + 1);

            for (i = min; i <= max; i++) {
                Assert.AreEqual(i, output.ElementAt((int)(i - min)));
            }
        }
    }
}
