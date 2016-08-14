using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VariableLengthIntegers;

namespace InvertedTomato.Common.Tests.VariableLengthIntegers {
    [TestClass]
    public class UVarInt2Tests {
        UVarInt2 Focus;

        [TestInitialize]
        public void Initialize() {
            Focus = new UVarInt2();
        }

        [TestMethod]
        public void Encode_1() {
            Focus.Append(1);
            
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.GetByteArray()));
        }

        [TestMethod]
        public void Encode_2() {
            Focus.Append(2);

            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("0000001", 2),
            }), BitConverter.ToString(Focus.GetByteArray()));
        }

        [TestMethod]
        public void Encode_4() {
            Focus.Append(4);

            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000101", 2),
            }), BitConverter.ToString(Focus.GetByteArray()));
        }

        [TestMethod]
        public void Encode_100() {
            Focus.Append(100);

            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01101101", 2),
                Convert.ToByte("00000010", 2),
            }), BitConverter.ToString(Focus.GetByteArray()));



            // Expected  10110110 01000(000)
 
            // Actual    01101101 00000010

        }

        [TestMethod]
        public void Encode_4_4_4() {
            Focus.Append(4);
            Focus.Append(4);
            Focus.Append(4);

            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000001", 2),
                Convert.ToByte("01101101", 2),
            }), BitConverter.ToString(Focus.GetByteArray()));
        }
    }
}
