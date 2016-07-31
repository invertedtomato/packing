using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvertedTomato.Common.Tests {
    [TestClass]
    public class VLQTests {
        /*
        [TestMethod]
        public void Encode_UInt64_0() {
            Assert.AreEqual("00", BitConverter.ToString(VLQ.Encode(ulong.MinValue)));
        }
        */

        [TestMethod]
        public void Decode_UInt64_0() {
            var vlq = new VLQ(false);
            vlq.AppendByte(Convert.ToByte("10000000", 2));

            Assert.AreEqual((ulong)0, vlq.ToUInt64());
        }
    }
}
