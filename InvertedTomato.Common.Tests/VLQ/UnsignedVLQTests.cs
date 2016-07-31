using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VLQ;

namespace InvertedTomato.Common.Tests {
    [TestClass]
    public class UnsignedVLQTests {
        // TODO: Add UnsignedVLQ encoding tests for all the values below
        // TODO: Add SignedVLQ encoding tests
        // TODO: Add SignedVLQ decoding tests
        /*
        [TestMethod]
        public void Encode_UInt64_0() {
            Assert.AreEqual("00", BitConverter.ToString(VLQ.Encode(ulong.MinValue)));
        }
        */

        [TestMethod]
        public void Decode_Min() {
            var vlq = new UnsignedVLQ();
            Assert.IsTrue( vlq.AppendByte(Convert.ToByte("10000000", 2)));
            Assert.AreEqual(ulong.MinValue, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_1() {
            var vlq = new UnsignedVLQ();
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("10000001", 2)));
            Assert.AreEqual((ulong)1, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_127() {
            var vlq = new UnsignedVLQ();
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("11111111", 2)));
            Assert.AreEqual((ulong)127, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_128() {
            var vlq = new UnsignedVLQ();
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("00000000", 2)));
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("10000001", 2)));
            Assert.AreEqual((ulong)128, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_16383() {
            var vlq = new UnsignedVLQ();
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("11111111", 2)));
            Assert.AreEqual((ulong)16383, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_16384() {
            var vlq = new UnsignedVLQ();
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("00000000", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("00000000", 2)));
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("10000001", 2)));
            Assert.AreEqual((ulong)16384, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_2097151() {
            var vlq = new UnsignedVLQ();
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("11111111", 2)));
            Assert.AreEqual((ulong)2097151, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_2097152() {
            var vlq = new UnsignedVLQ();
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("00000000", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("00000000", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("00000000", 2)));
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("10000001", 2)));
            Assert.AreEqual((ulong)2097152, vlq.ToValue());
        }

        [TestMethod]
        public void Decode_Max() {
            var vlq = new UnsignedVLQ();
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte( "10000001", 2)));
            Assert.AreEqual(ulong.MaxValue, vlq.ToValue());
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Decode_ValueOverflow() {
            var vlq = new UnsignedVLQ();
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            Assert.IsFalse(vlq.AppendByte(Convert.ToByte("01111111", 2)));
            vlq.AppendByte(Convert.ToByte("10000011", 2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Decode_TooManyBytes() {
            var vlq = new UnsignedVLQ();
            Assert.IsTrue(vlq.AppendByte(Convert.ToByte("10000000", 2)));
            vlq.AppendByte(Convert.ToByte("10000000", 2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Decode_InsufficentBytes1() {
            var vlq = new UnsignedVLQ();
            vlq.ToValue();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Decode_InsufficentBytes2() {
            var vlq = new UnsignedVLQ();
            vlq.AppendByte(Convert.ToByte("00000000", 2));
            vlq.ToValue();
        }

    }
}
