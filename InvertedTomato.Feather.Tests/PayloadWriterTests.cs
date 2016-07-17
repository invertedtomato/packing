using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class PayloadWriterTests {
        [TestMethod]
        public void AppendByteArray() {
            var payload = new PayloadWriter(0x00);
            payload.Append(new byte[] { 0x01, 0x02, 0x03 });

            Assert.AreEqual("00-03-00-01-02-03", BitConverter.ToString(payload.ToByteArray()));
        }
        
        // TODO: Finish unit tests
    }
}
