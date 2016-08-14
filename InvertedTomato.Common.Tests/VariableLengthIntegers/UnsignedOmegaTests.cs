using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VariableLengthIntegers;

namespace InvertedTomato.Common.Tests.VariableLengthIntegers {
    [TestClass]
    public class UVarInt2Tests {
        UnsignedOmega Focus;

        [TestInitialize]
        public void Initialize() {
            Focus = new UnsignedOmega();
        }

        /*
              0  _______0
              1  _____001
              2  _____011
              3  __000101
              6  __011101
              7  _0000111
             14  _0111111
             15  00100101 _____000
             31  00110101 ____0000
             99  01101101 ___00010
            999  11100111 00010111 _______0
          9,999  01001111 10001110 ___00000
         99,999  00100101 00001100 00101011 ____0000
        999,999  00100101 10111111 10010000 _0000000	
        */

        [TestMethod]
        public void Encode_0() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(0)));
        }
        [TestMethod]
        public void Encode_1() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("0000001", 2),
            }), BitConverter.ToString(Focus.Encode(1)));
        }
        [TestMethod]
        public void Encode_2() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000011", 2),
            }), BitConverter.ToString(Focus.Encode(2)));
        }
        [TestMethod]
        public void Encode_3() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000101", 2),
            }), BitConverter.ToString(Focus.Encode(3)));
        }
        [TestMethod]
        public void Encode_6() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00011101", 2),
            }), BitConverter.ToString(Focus.Encode(6)));
        }
        [TestMethod]
        public void Encode_7() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000111", 2),
            }), BitConverter.ToString(Focus.Encode(7)));
        }
        [TestMethod]
        public void Encode_14() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00111111", 2),
            }), BitConverter.ToString(Focus.Encode(14)));
        }
        [TestMethod]
        public void Encode_15() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00100101", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(15)));
        }
        [TestMethod]
        public void Encode_31() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00110101", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(31)));
        }
        [TestMethod]
        public void Encode_99() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01101101", 2),
                Convert.ToByte("00000010", 2),
            }), BitConverter.ToString(Focus.Encode(99)));
        }
        [TestMethod]
        public void Encode_999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11100111", 2),
                Convert.ToByte("00010111", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(999)));
        }
        [TestMethod]
        public void Encode_9999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01001111", 2),
                Convert.ToByte("10001110", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(99999)));
        }
        [TestMethod]
        public void Encode_99999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00100101", 2),
                Convert.ToByte("00101011", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(99999)));
        }
        [TestMethod]
        public void Encode_999999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00100101", 2),
                Convert.ToByte("10111111", 2),
                Convert.ToByte("10010000", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(999999)));
        }

        [TestMethod]
        public void Encode_Max() {
            throw new NotImplementedException();

            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(ulong.MaxValue)));
        }

        [TestMethod]
        public void Encode_2_2_2() {
            var position = 0;
            var offset = 0;
            var buffer = new byte[2];

            Focus.Encode(2, buffer, ref position, ref offset);
            Assert.AreEqual(0, position);
            Assert.AreEqual(3,offset);

            Focus.Encode(2, buffer, ref position, ref offset);
            Assert.AreEqual(0, position);
            Assert.AreEqual(6, offset);

            Focus.Encode(2, buffer, ref position, ref offset);
            Assert.AreEqual(1, position);
            Assert.AreEqual(1, offset);

            // 01101101 _______1
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01101101", 2),
                Convert.ToByte("00000001", 2),
            }), BitConverter.ToString(buffer));
        }


        [TestMethod]
        public void Decode_0() {
            Assert.AreEqual((ulong)0, Focus.Decode(new byte[] {
                Convert.ToByte("00000000", 2)
            }));
        }
        [TestMethod]
        public void Decode_1() {
            Assert.AreEqual((ulong)1, Focus.Decode(new byte[] {
                Convert.ToByte("0000001", 2)
            }));
        }
        [TestMethod]
        public void Decode_2() {
            Assert.AreEqual((ulong)2, Focus.Decode(new byte[] {
                Convert.ToByte("00000011", 2)
            }));
        }
        [TestMethod]
        public void Decode_3() {
            Assert.AreEqual((ulong)3, Focus.Decode(new byte[] {
                Convert.ToByte("00000101", 2)
            }));
        }
        [TestMethod]
        public void Decode_6() {
            Assert.AreEqual((ulong)6, Focus.Decode(new byte[] {
                Convert.ToByte("00011101", 2)
            }));
        }
        [TestMethod]
        public void Decode_7() {
            Assert.AreEqual((ulong)7, Focus.Decode(new byte[] {
                Convert.ToByte("00000111", 2)
            }));
        }
        [TestMethod]
        public void Decode_14() {
            Assert.AreEqual((ulong)14, Focus.Decode(new byte[] {
                Convert.ToByte("00111111", 2)
            }));
        }
        [TestMethod]
        public void Decode_15() {
            Assert.AreEqual((ulong)15, Focus.Decode(new byte[] {
                 Convert.ToByte("00100101", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_31() {
            Assert.AreEqual((ulong)31, Focus.Decode(new byte[] {
                 Convert.ToByte("00110101", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_99() {
            Assert.AreEqual((ulong)99, Focus.Decode(new byte[] {
                 Convert.ToByte("01101101", 2),
                Convert.ToByte("00000010", 2),
            }));
        }
        [TestMethod]
        public void Decode_999() {
            Assert.AreEqual((ulong)999, Focus.Decode(new byte[] {
                Convert.ToByte("11100111", 2),
                Convert.ToByte("00010111", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_9999() {
            Assert.AreEqual((ulong)9999, Focus.Decode(new byte[] {
                Convert.ToByte("01001111", 2),
                Convert.ToByte("10001110", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_99999() {
            Assert.AreEqual((ulong)99999, Focus.Decode(new byte[] {
                Convert.ToByte("00100101", 2),
                Convert.ToByte("00101011", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_999999() {
            Assert.AreEqual((ulong)999999, Focus.Decode(new byte[] {
                Convert.ToByte("00100101", 2),
                Convert.ToByte("10111111", 2),
                Convert.ToByte("10010000", 2),
                Convert.ToByte("00000000", 2),
            }));
        }

        [TestMethod]
        public void Decode_Max() {
            throw new NotImplementedException();
            Assert.AreEqual(ulong.MaxValue, Focus.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
            }));
        }

        [TestMethod]
        public void Decode_2_2_2() {
            var position = 0;
            var offset = 0;
            var buffer = new byte[] {
                Convert.ToByte("01101101", 2),
                Convert.ToByte("00000001", 2),
            };

            Assert.AreEqual((ulong)2, Focus.Decode(buffer, ref position, ref offset));
            Assert.AreEqual(0, position);
            Assert.AreEqual(3, offset);

            Assert.AreEqual((ulong)2, Focus.Decode(buffer, ref position, ref offset));
            Assert.AreEqual(0, position);
            Assert.AreEqual(6, offset);

            Assert.AreEqual((ulong)2, Focus.Decode(buffer, ref position, ref offset));
            Assert.AreEqual(1, position);
            Assert.AreEqual(1, offset);

        }
    }
}
