using System;
using NUnit.Framework;
using System.Net;
using InvertedTomato;

namespace InvertedTomato.Common.Tests {
    [TestFixture]
    public class IPAddressExtensions {
        [Test]
        public void EncodeIPv4() {
            var raw = IPAddress.Parse("1.2.3.4").GetBytes(16);

            Assert.AreEqual("00-00-00-00-00-00-00-00-00-00-00-00-01-02-03-04", BitConverter.ToString(raw));
        }

        [Test]
        public void EncodeIPv6() {
            var raw = IPAddress.Parse("FE80:0000:0000:0000:0202:B3FF:FE1E:8329").GetBytes(16);

            Assert.AreEqual("FE-80-00-00-00-00-00-00-02-02-B3-FF-FE-1E-83-29", BitConverter.ToString(raw));
        }

        [Test]
        public void DecodeIPv4() {
            var raw = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04 };

            var address = IPAddressUtility.FromBytes(raw);

            Assert.AreEqual("1.2.3.4", address.ToString());
        }

        [Test]
        public void DecodeIPv6() {
            var raw = new byte[] { 0xFE, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x02, 0xB3, 0xFF, 0xFE, 0x1E, 0x83, 0x29 };

            var address = IPAddressUtility.FromBytes(raw);

            Assert.AreEqual("fe80::202:b3ff:fe1e:8329", address.ToString());
        }

        [Test]
        public void DecodeIPv6_AllZeros() {
            var raw = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            var address = IPAddressUtility.FromBytes(raw);

            Assert.AreEqual("::", address.ToString());
        }
    }
}
