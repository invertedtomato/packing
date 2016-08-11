using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VariableLengthIntegers;

namespace InvertedTomato.Common.Tests {
    [TestClass]
    public class UVarIntTests {
        UVarInt Min1;
        UVarInt Min2;
        UVarInt Min4;

        [TestInitialize]
        public void Initialize() {
            Min1 = new UVarInt();
            Min2 = new UVarInt(2);
            Min4 = new UVarInt(4);
        }

        [TestMethod]
        public void EncodeDecode_Min1_Range() {
            var buffer = new byte[uint.MaxValue / 1000];
            int position = 0;

            for (var input = uint.MinValue; input < uint.MaxValue / 10000; input++) {
                var previousPosition = position;
                Min1.Encode(input, buffer, ref position);
                position = previousPosition;
                var output = Min1.Decode(buffer, ref position);

                Assert.AreEqual(output, input);
            }
        }

        [TestMethod]
        public void EncodeDecode_Min2_Range() {
            var buffer = new byte[uint.MaxValue / 1000];
            int position = 0;

            for (var input = uint.MinValue; input < uint.MaxValue / 10000; input++) {
                var previousPosition = position;
                Min2.Encode(input, buffer, ref position);
                position = previousPosition;
                var output = Min2.Decode(buffer, ref position);

                Assert.AreEqual(output, input);
            }
        }

        [TestMethod]
        public void EncodeDecode_Min4_Range() {
            var buffer = new byte[uint.MaxValue / 1000];
            int position = 0;

            for (var input = uint.MinValue; input < uint.MaxValue / 10000; input++) {
                var previousPosition = position;
                Min4.Encode(input, buffer, ref position);
                position = previousPosition;
                var output = Min4.Decode(buffer, ref position);

                Assert.AreEqual(output, input);
            }
        }

        [TestMethod]
        public void Encode_Min1_Min() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min1.Encode(ulong.MinValue)));
        }

        [TestMethod]
        public void Encode_Min1_1() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000001", 2)
            }), BitConverter.ToString(Min1.Encode(1)));
        }

        [TestMethod]
        public void Encode_Min1_127() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2)
            }), BitConverter.ToString(Min1.Encode(127)));
        }

        [TestMethod]
        public void Encode_Min1_128() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min1.Encode(128)));
        }

        [TestMethod]
        public void Encode_Min1_16511() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("11111111", 2)
            }), BitConverter.ToString(Min1.Encode(16511)));
        }

        [TestMethod]
        public void Encode_Min1_16512() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min1.Encode(16512)));
        }

        [TestMethod]
        public void Encode_Min1_2113663() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("11111111", 2)
            }), BitConverter.ToString(Min1.Encode(2113663)));
        }

        [TestMethod]
        public void Encode_Min1_2113664() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min1.Encode(2113664)));
        }

        [TestMethod]
        public void Encode_Min1_Max() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01111111", 2), // Not absolutely sure these bits are correct - seems to be though
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),

                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),

                Convert.ToByte("01111110", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min1.Encode(ulong.MaxValue)));
        }


        [TestMethod]
        public void Encode_Min2_1() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000001", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min2.Encode(1)));
        }

        [TestMethod]
        public void Encode_Min4_1() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000001", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min4.Encode(1)));
        }
        [TestMethod]
        public void Encode_Min2_255() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min2.Encode(255)));
        }
        [TestMethod]
        public void Encode_Min2_256() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000001", 2)
            }), BitConverter.ToString(Min2.Encode(256)));
        }
        [TestMethod]
        public void Encode_Min2_32767() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2)
            }), BitConverter.ToString(Min2.Encode(32767)));
        }
        [TestMethod]
        public void Encode_Min2_32768() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min2.Encode(32768)));
        }
        [TestMethod]
        public void Encode_Min4_255() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min4.Encode(255)));
        }
        [TestMethod]
        public void Encode_Min4_256() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000001", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }), BitConverter.ToString(Min4.Encode(256)));
        }


        [TestMethod]
        public void Decode_Min1_Min() {
            Assert.AreEqual(ulong.MinValue, Min1.Decode(new byte[] {
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_1() {
            Assert.AreEqual((ulong)1, Min1.Decode(new byte[] {
                Convert.ToByte("10000001", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_127() {
            Assert.AreEqual((ulong)127, Min1.Decode(new byte[] {
                Convert.ToByte("11111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_128() {
            Assert.AreEqual((ulong)128, Min1.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_16511() {
            Assert.AreEqual((ulong)16511, Min1.Decode(new byte[] {
                Convert.ToByte("01111111", 2) ,
                Convert.ToByte("11111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_16512() {
            Assert.AreEqual((ulong)16512, Min1.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_2113663() {
            Assert.AreEqual((ulong)2113663, Min1.Decode(new byte[] {
                Convert.ToByte("01111111", 2),
                Convert.ToByte("01111111", 2),
                Convert.ToByte("11111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_2113664() {
            Assert.AreEqual((ulong)2113664, Min1.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_Max() {
            Assert.AreEqual(ulong.MaxValue, Min1.Decode(new byte[] {
                Convert.ToByte("01111111", 2), // Not absolutely sure these bits are correct - seems to be though
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),

                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),
                Convert.ToByte("01111110", 2),

                Convert.ToByte("01111110", 2),
                Convert.ToByte("10000000", 2)
            }));
        }


        [TestMethod]
        public void Decode_Min2_1() {
            Assert.AreEqual((ulong)1, Min2.Decode(new byte[] {
                Convert.ToByte("00000001", 2),
                Convert.ToByte("10000000", 2)
            }));
        }


        [TestMethod]
        public void Decode_Min4_1() {
            Assert.AreEqual((ulong)1, Min4.Decode(new byte[] {
                Convert.ToByte("00000001", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min2_255() {
            Assert.AreEqual((ulong)255, Min2.Decode(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("10000000", 2)
            }));
        }
        [TestMethod]
        public void Decode_Min2_256() {
            Assert.AreEqual((ulong)256, Min2.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000001", 2)
            }));
        }
        [TestMethod]
        public void Decode_Min2_32767() {
            Assert.AreEqual((ulong)32767, Min2.Decode(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2)
            }));
        }
        [TestMethod]
        public void Decode_Min2_32768() {
            Assert.AreEqual((ulong)32768, Min2.Decode(new byte[] {
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min4_255() {
            Assert.AreEqual((ulong)255, Min4.Decode(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }
        [TestMethod]
        public void Decode_Min4_256() {
            Assert.AreEqual((ulong)256, Min4.Decode(new byte[] {
                 Convert.ToByte("00000000", 2),
                Convert.ToByte("00000001", 2),
                Convert.ToByte("00000000", 2),
                Convert.ToByte("10000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_Min1_UnneededBytes() {
            Assert.AreEqual((ulong)1, Min1.Decode(new byte[] {
                Convert.ToByte("10000001", 2),
                Convert.ToByte("10000000", 2), // Waste
                Convert.ToByte("10000000", 2) // Waste
            }));
        }

        [TestMethod]
        public void Decode_Min1_NotFirstByte() {
            var position = 1;
            Assert.AreEqual((ulong)1, Min1.Decode(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000001", 2)
            }, ref position));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Decode_Min1_InsufficentBytes1() {
            Assert.AreEqual((ulong)1, Min1.Decode(new byte[] { }));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Decode_Min1_InsufficentBytes2() {
            Assert.AreEqual((ulong)1, Min1.Decode(new byte[] {
                Convert.ToByte("00000000", 2)
            }));
        }
    }
}
