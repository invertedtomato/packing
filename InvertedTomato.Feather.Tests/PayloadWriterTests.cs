using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class PayloadWriterTests {
        [TestMethod]
        public void AppendByteArray() {
            var payload = new PayloadWriter(0x00);
            payload.Append(new byte[] { 0x01, 0x02, 0x03 });

            Assert.AreEqual("00-03-00-01-02-03", BitConverter.ToString(payload.ToByteArray()));
        }


        [TestMethod]
        public void AppendDateTime() {
            var payload = new PayloadWriter(0x00);
            payload.Append(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc));

            Assert.AreEqual("00-01-00-00-00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendNullableDateTime() {
            var payload = new PayloadWriter(0x00);
            DateTime? date = null;
            payload.AppendNullable(date);

            Assert.AreEqual("00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendTimeSpan() {
            var payload = new PayloadWriter(0x00);
            payload.Append(TimeSpan.FromSeconds(30));

            Assert.AreEqual("00-1E-00-00-00", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendNullableTimeSpan() {
            var payload = new PayloadWriter(0x00);
            TimeSpan? timeSpan = null;
            payload.AppendNullable(timeSpan);

            Assert.AreEqual("00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendString() {
            var payload = new PayloadWriter(0x00);
            payload.Append("VLife");

            Assert.AreEqual("VLife", new PayloadReader(payload.ToByteArray()).ReadString());
        }

        [TestMethod]
        public void AppendNullableString() {
            var payload = new PayloadWriter(0x00);
            string value = null;
            payload.AppendNullable(value);

            Assert.AreEqual("00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
        }


        [TestMethod]
        public void AppendGuid() {
            var payload = new PayloadWriter(0x00);
            payload.Append(new Guid("5d813059-d9f1-4662-902b-01437bdb7ef9"));

            Assert.AreEqual("5d813059-d9f1-4662-902b-01437bdb7ef9", new PayloadReader(payload.ToByteArray()).ReadGuid().ToString());
        }

        [TestMethod]
        public void AppendNullableGUI() {
            var payload = new PayloadWriter(0x00);
            Guid? value = null;
            payload.AppendNullable(value);

            Assert.AreEqual("00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
        }


        [TestMethod]
        public void AppendIPAddress() {
            IPAddress ip = new IPAddress(3232235527); //192.168.0.7
            var payload = new PayloadWriter(0x00);
            payload.Append(ip);

            Assert.AreEqual("00-04-07-00-A8-C0", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendNullableIpAddress() {
            IPAddress ip = null;
            var payload = new PayloadWriter(0x00);
            payload.AppendNullable(ip);

            Assert.AreEqual("00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendInt8() {
            var payload = new PayloadWriter(0x00);
            payload.Append((sbyte)126);

            Assert.AreEqual("00-7E", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendUInt8() {
            var payload = new PayloadWriter(0x00);
            payload.Append((byte)254);

            Assert.AreEqual("00-FE", BitConverter.ToString(payload.ToByteArray()));
        }

        [TestMethod]
        public void AppendInt16() {
            var payload = new PayloadWriter(0x00);
            payload.Append((short)100);

            Assert.AreEqual(100, new PayloadReader(payload.ToByteArray()).ReadInt16());
        }

        [TestMethod]
        public void AppendUInt16() {
            var payload = new PayloadWriter(0x00);
            payload.Append((ushort)200);

            Assert.AreEqual(200, new PayloadReader(payload.ToByteArray()).ReadUInt16());
        }

        [TestMethod]
        public void AppendInt32() {
            var payload = new PayloadWriter(0x00);
            payload.Append(1400);

            Assert.AreEqual(1400, new PayloadReader(payload.ToByteArray()).ReadInt32());
        }

        [TestMethod]
        public void AppendUint32() {
            var payload = new PayloadWriter(0x00);
            payload.Append((uint)200);

            Assert.AreEqual((uint)200, new PayloadReader(payload.ToByteArray()).ReadUInt32());
        }

        [TestMethod]
        public void AppendLong() {
            var payload = new PayloadWriter(0x00);
            payload.Append((long)1400);

            Assert.AreEqual((long)1400, new PayloadReader(payload.ToByteArray()).ReadInt64());
        }


        [TestMethod]
        public void AppendUlong() {
            var payload = new PayloadWriter(0x00);
            payload.Append((ulong)1500);
            //var ddf = BitConverter.ToString(payload.ToByteArray());

            Assert.AreEqual((ulong)1500, new PayloadReader(payload.ToByteArray()).ReadUInt64());
        }

        [TestMethod]
        public void AppendDouble() {
            var payload = new PayloadWriter(0x00);
            payload.Append((double)22.34);

            Assert.AreEqual((double)22.34, new PayloadReader(payload.ToByteArray()).ReadDouble());
        }

        [TestMethod]
        public void AppendFloat() {
            var payload = new PayloadWriter(0x00);
            payload.Append(4.3f);

            Assert.AreEqual("00-9A-99-89-40", BitConverter.ToString(payload.ToByteArray()));
        }



        // TODO: Finish unit tests
    }
}
