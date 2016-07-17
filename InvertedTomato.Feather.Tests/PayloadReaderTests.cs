using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class PayloadReaderTests {
        [TestMethod]
        public void ReadByteArray() {
            var payload = new PayloadReader(new byte[] { 0x00, 0x03, 0x00, 0x01, 0x02, 0x03 });
            Assert.AreEqual("01-02-03", BitConverter.ToString(payload.ReadByteArray()));
        }

        // TODO: Finish unit tests
    }
}
