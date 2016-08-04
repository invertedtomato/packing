using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VLQ;

namespace InvertedTomato.Common.Tests {
    [TestClass]
    public class UnsignedVLQTests {
        [TestMethod]
        public void Encode_Min() {
            Assert.AreEqual("00", BitConverter.ToString(UnsignedVLQ.Encode(ulong.MinValue)));
        }

        [TestMethod]
        public void Encode_1() {
            Assert.AreEqual("01", BitConverter.ToString(UnsignedVLQ.Encode(1)));
        }

        [TestMethod]
        public void Encode_127() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("01111111", 2)
            }), BitConverter.ToString(UnsignedVLQ.Encode(127)));
        }

        [TestMethod]
        public void Encode_128() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("00000001", 2)
            }), BitConverter.ToString(UnsignedVLQ.Encode(128)));
        }

        [TestMethod]
        public void Encode_16383() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("01111111", 2)
            }), BitConverter.ToString(UnsignedVLQ.Encode(16383)));
        }

        [TestMethod]
        public void Encode_16384() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000000", 2),
                Convert.ToByte("00000001", 2)
            }), BitConverter.ToString(UnsignedVLQ.Encode(16384)));
        }

        [TestMethod]
        public void Encode_2097151() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("01111111", 2)
            }), BitConverter.ToString(UnsignedVLQ.Encode(2097151)));
        }

        [TestMethod]
        public void Encode_2097152() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000000", 2),
                Convert.ToByte("00000001", 2)
            }), BitConverter.ToString(UnsignedVLQ.Encode(2097152)));
        }

        [TestMethod]
        public void Encode_Max() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),

                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),

                Convert.ToByte("11111111", 2),
                Convert.ToByte("00000001", 2)
            }), BitConverter.ToString(UnsignedVLQ.Encode(ulong.MaxValue)));
        }

        [TestMethod]
        public void Decode_Min() {
            Assert.AreEqual(ulong.MinValue, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("00000000", 2)
            }));
        }

        [TestMethod]
        public void Decode_1() {
            Assert.AreEqual((ulong)1, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("00000001", 2)
            }));
        }

        [TestMethod]
        public void Decode_127() {
            Assert.AreEqual((ulong)127, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("01111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_128() {
            Assert.AreEqual((ulong)128, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("00000001", 2)
            }));
        }

        [TestMethod]
        public void Decode_16383() {
            Assert.AreEqual((ulong)16383, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("11111111", 2) ,
                Convert.ToByte("01111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_16384() {
            Assert.AreEqual((ulong)16384, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000000", 2),
                Convert.ToByte("00000001", 2)
            }));
        }

        [TestMethod]
        public void Decode_2097151() {
            Assert.AreEqual((ulong)2097151, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("01111111", 2)
            }));
        }

        [TestMethod]
        public void Decode_2097152() {
            Assert.AreEqual((ulong)2097152, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000000", 2),
                Convert.ToByte("10000000", 2),
                Convert.ToByte("00000001", 2) }));
        }

        [TestMethod]
        public void Decode_Max() {
            Assert.AreEqual(ulong.MaxValue, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),

                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),

                Convert.ToByte("11111111", 2),
                Convert.ToByte("00000001", 2)
            }));
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Decode_ValueOverflow() {
            UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),

                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),

                Convert.ToByte("11111111", 2),
                Convert.ToByte("00000011", 2)
            });
        }

        [TestMethod]
        public void Decode_UnneededBytes() {
            Assert.AreEqual((ulong)1, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("00000001", 2),
                Convert.ToByte("00000000", 2), // Waste
                Convert.ToByte("00000000", 2) // Waste
            }));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Decode_InsufficentBytes1() {
            Assert.AreEqual((ulong)1, UnsignedVLQ.Decode(new byte[] { }));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Decode_InsufficentBytes2() {
            Assert.AreEqual((ulong)1, UnsignedVLQ.Decode(new byte[] {
                Convert.ToByte("10000000", 2)
            }));
        }
    }
}
