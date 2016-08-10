using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VariableLengthIntegers;

namespace InvertedTomato.Common.Tests {
    [TestClass]
    public class UVarIntTests {
        UVarInt UVarInt;

        [TestInitialize]
        public void Initialize() {
            UVarInt = new UVarInt();
        }

        [TestMethod]
        public void EncodeDecode_Range() {
            var buffer = new byte[uint.MaxValue / 1000];
            int position = 0;

            for (var input = uint.MinValue; input < uint.MaxValue / 10000; input++) {
                var previousPosition = position;
                UVarInt.Encode(input, buffer, ref position);
                position = previousPosition;
                var output = UVarInt.Decode(buffer, ref position);

                Assert.AreEqual(output, input);
            }
        }

        [TestMethod]
        public void Encode_Min() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(UVarInt.Encode(ulong.MinValue)));
        }

        [TestMethod]
        public void Encode_1() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000001", 2)
            }), BitConverter.ToString(UVarInt.Encode(1)));
        }

        [TestMethod]
        public void Encode_127() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2)
            }), BitConverter.ToString(UVarInt.Encode(127)));
        }

        [TestMethod]
        public void Encode_128() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(UVarInt.Encode(128)));
        }

        [TestMethod]
        public void Encode_16511() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("11111111", 2)
            }), BitConverter.ToString(UVarInt.Encode(16511)));
        }

        [TestMethod]
        public void Encode_16512() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(UVarInt.Encode(16512)));
        }

        [TestMethod]
        public void Encode_2113663() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("11111111", 2)
            }), BitConverter.ToString(UVarInt.Encode(2113663)));
        }

        [TestMethod]
        public void Encode_2113664() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(UVarInt.Encode(2113664)));
        }
        /*
        [TestMethod]
        public void Encode_Max() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),

                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),

                Convert.ToByte("01111111", 2),
                Convert.ToByte("10000001", 2)
            }), BitConverter.ToString(UVarInt.Encode(ulong.MaxValue)));
        }
        */
        [TestMethod]
        public void Decode_Min() {
            Assert.AreEqual(ulong.MinValue, UVarInt.Decode(new byte[] {
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_1() {
            Assert.AreEqual((ulong)1, UVarInt.Decode(new byte[] {
                Convert.ToByte("10000001", 2)
            }));
        }

        [TestMethod]
        public void Decode_127() {
            Assert.AreEqual((ulong)127, UVarInt.Decode(new byte[] {
                Convert.ToByte("11111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_128() {
            Assert.AreEqual((ulong)128, UVarInt.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_16511() {
            Assert.AreEqual((ulong)16511, UVarInt.Decode(new byte[] {
                Convert.ToByte("01111111", 2) ,
                Convert.ToByte("11111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_16512() {
            Assert.AreEqual((ulong)16512, UVarInt.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_2113663() {
            Assert.AreEqual((ulong)2113663, UVarInt.Decode(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("11111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_2113664() {
            Assert.AreEqual((ulong)2113664, UVarInt.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }
        /* TODO
        [TestMethod]
        public void Decode_Max() {
            Assert.AreEqual(ulong.MaxValue, UVarInt.Decode(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),

                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),

                Convert.ToByte("01111111", 2),
                Convert.ToByte("10000001", 2)
            }));
        }
        */
        [TestMethod]
        public void Decode_UnneededBytes() {
            Assert.AreEqual((ulong)1, UVarInt.Decode(new byte[] {
                Convert.ToByte("10000001", 2),
                Convert.ToByte("10000000", 2), // Waste
                Convert.ToByte("10000000", 2) // Waste
            }));
        }

        [TestMethod]
        public void Decode_NotFirstByte() {
            var position = 1;
            Assert.AreEqual((ulong)1, UVarInt.Decode(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000001", 2)
            }, ref position));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Decode_InsufficentBytes1() {
            Assert.AreEqual((ulong)1, UVarInt.Decode(new byte[] { }));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Decode_InsufficentBytes2() {
            Assert.AreEqual((ulong)1, UVarInt.Decode(new byte[] {
                Convert.ToByte("00000000", 2)
            }));
        }
    }
}
