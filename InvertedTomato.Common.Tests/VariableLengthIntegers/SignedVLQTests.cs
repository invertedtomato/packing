using InvertedTomato.VariableLengthIntegers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvertedTomato.Common.Tests.VariableLengthIntegers {
    [TestClass]
    public class SignedVLQTests {
        SignedVLQ VarInt;

        [TestInitialize]
        public void Initialize() {
            VarInt = new SignedVLQ();
        }

        [TestMethod]
        public void EncodeDecode_Range() {
            var buffer = new byte[uint.MaxValue / 500];
            int position = 0;

            for (var input = int.MinValue / 10000; input < int.MaxValue / 10000; input++) {
                var previousPosition = position;
                VarInt.Encode(input, buffer, ref position);
                position = previousPosition;
                var output = VarInt.Decode(buffer, ref position);

                Assert.AreEqual(output, input);
            }
        }
    }
}
