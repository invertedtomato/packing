using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;

namespace InvertedTomato.Feather.Tests {
	[TestClass]
	public class PayloadReaderTests {
		[TestMethod]
		public void ReadByteArray() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x03, 0x00, 0x01, 0x02, 0x03 });
			Assert.AreEqual("01-02-03", BitConverter.ToString(payload.ReadByteArray()));
		}

		[TestMethod]
		public void ReadDateTime() {
			var value = new PayloadWriter(0x00).Append(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc));
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc), payload.ReadDateTime());
		}

		[TestMethod]
		public void ReadNullableDateTime() {
			DateTime? date = null;
			var value = new PayloadWriter(0x00).AppendNullable(date);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(date, payload.ReadNullableDateTime());
		}
		[TestMethod]
		public void ReadTimeSpan() {
			var value = new PayloadWriter(0x00).Append(TimeSpan.FromSeconds(30));
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(TimeSpan.FromSeconds(30), payload.ReadTimeSpan());
		}

		[TestMethod]
		public void ReadString() {
			var value = new PayloadWriter(0x00).Append("VLife");
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual("VLife", payload.ReadString());
		}

		[TestMethod]
		public void ReadNullableString() {
			string name = null;
			var value = new PayloadWriter(0x00).AppendNullable(name);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(name, payload.ReadNullableString());
		}

		[TestMethod]
		public void ReadGuid() {
			var value = new PayloadWriter(0x00).Append(new Guid("5d813059-d9f1-4662-902b-01437bdb7ef9"));
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual("5d813059-d9f1-4662-902b-01437bdb7ef9", payload.ReadGuid().ToString());
		}

		[TestMethod]
		public void ReadNullableGuid() {
			Guid? guid = null;
			var value = new PayloadWriter(0x00).AppendNullable(guid);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(guid, payload.ReadNullableGuid());
		}

		[TestMethod]
		public void ReadBoolean() {
			var value = new PayloadWriter(0x00).Append(true);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(true, payload.ReadBoolean());
		}

		// TODO Boolean []

		[TestMethod]
		public void ReadIPAddress() {
			var ip = new IPAddress(3232235527);
			var value = new PayloadWriter(0x00).Append(ip);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(ip, payload.ReadIPAddress());
		}

		[TestMethod]
		public void ReadNullableIPAddress() {
			IPAddress ip = null;
			var value = new PayloadWriter(0x00).AppendNullable(ip);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(ip, payload.ReadNullableIPAddress());
		}

		[TestMethod]
		public void ReadInt8_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF });
			Assert.AreEqual(sbyte.MaxValue, payload.ReadInt8());
		}

		[TestMethod]
		public void ReadInt8_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00 });
			Assert.AreEqual(sbyte.MinValue, payload.ReadInt8());
		}

		[TestMethod]
		public void ReadNullableInt8() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00 });
			Assert.AreEqual(null, payload.ReadNullableInt8());
		}

		[TestMethod]
		public void ReadUInt8_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF });
			Assert.AreEqual(byte.MaxValue, payload.ReadUInt8());
		}

		[TestMethod]
		public void ReadUInt8_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00 });
			Assert.AreEqual(byte.MinValue, payload.ReadUInt8());
		}
		[TestMethod]
		public void ReadNullableUInt8() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00 });
			Assert.AreEqual(null, payload.ReadNullableUInt8());
		}


		[TestMethod]
		public void ReadInt16_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0x7F });
			Assert.AreEqual(short.MaxValue, payload.ReadInt16());
		}

		[TestMethod]
		public void ReadInt16_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x80 });
			Assert.AreEqual(short.MinValue, payload.ReadInt16());
		}

		[TestMethod]
		public void ReadNullableInt16() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00  });
			Assert.AreEqual(null, payload.ReadNullableInt16());
		}

		[TestMethod]
		public void ReadUInt16_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF });
			Assert.AreEqual(ushort.MaxValue, payload.ReadUInt16());
		}

		[TestMethod]
		public void ReadUInt16_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00 });
			Assert.AreEqual(ushort.MinValue, payload.ReadUInt16());
		}

		[TestMethod]
		public void ReadNullableUInt16() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00  });
			Assert.AreEqual(null, payload.ReadNullableUInt16());
		}

		[TestMethod]
		public void ReadInt32_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0x7F });
			Assert.AreEqual(int.MaxValue, payload.ReadInt32());
		}

		[TestMethod]
		public void ReadInt32_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x80 });
			Assert.AreEqual(int.MinValue, payload.ReadInt32());
		}

		[TestMethod]
		public void ReadNullableInt32_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
			Assert.AreEqual(null, payload.ReadNullableInt32());
		}

		[TestMethod]
		public void ReadUInt32_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF });
			Assert.AreEqual(uint.MaxValue, payload.ReadUInt32());
		}

		[TestMethod]
		public void ReadUInt32_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 });
			Assert.AreEqual(uint.MinValue, payload.ReadUInt32());
		}

		[TestMethod]
		public void ReadNullableUInt32() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00  });
			Assert.AreEqual(null, payload.ReadNullableUInt32());
		}

		[TestMethod]
		public void ReadInt64_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F });
			Assert.AreEqual(long.MaxValue, payload.ReadInt64());
		}

		[TestMethod]
		public void ReadInt64_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 });
			Assert.AreEqual(long.MinValue, payload.ReadInt64());
		}

		[TestMethod]
		public void ReadNullableInt64 () {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00  });
			Assert.AreEqual(null, payload.ReadNullableInt64());
		}

		[TestMethod]
		public void ReadUInt64_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
			Assert.AreEqual(ulong.MaxValue, payload.ReadUInt64());
		}

		[TestMethod]
		public void ReadUInt64_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
			Assert.AreEqual(ulong.MinValue, payload.ReadUInt64());
		}

		[TestMethod]
		public void ReadNullableUInt64() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00  });
			Assert.AreEqual(null, payload.ReadNullableUInt64());
		}

		[TestMethod]
		public void ReadFloat_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0x7F, 0x7F });
			Assert.AreEqual(float.MaxValue, payload.ReadFloat());
		}

		[TestMethod]
		public void ReadFloat_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0x7F, 0xFF });
			Assert.AreEqual(float.MinValue, payload.ReadFloat());
		}

		[TestMethod]
		public void ReadNullableFloat() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00  });
			Assert.AreEqual(null, payload.ReadNullableFloat());
		}

		[TestMethod]
		public void ReadDouble_Max() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0x7F });
			Assert.AreEqual(double.MaxValue, payload.ReadDouble());
		}

		[TestMethod]
		public void ReadDouble_Min() {
			var payload = new PayloadReader(new byte[] { 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xEF, 0xFF });
			Assert.AreEqual(double.MinValue, payload.ReadDouble());
		}


		[TestMethod]
		public void ReadNullableDouble() {
			var payload = new PayloadReader(new byte[] { 0x00, 0x00, 0x00 });
			Assert.AreEqual(null, payload.ReadNullableDouble());
		}

		[TestMethod]
		public void ReadNullableByteArray() {
			byte[] arr = null;
			var value = new PayloadWriter(0x00).AppendNullable(arr);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual(arr, payload.ReadNullableByteArray());
		}
		

		[TestMethod]
		public void ReadByteArrayFixedLength() { 
			var value = new PayloadWriter(0x00).Append(new byte[] {0x01, 0x02, 0x03, 0x04, 0x01, 0x01, 0x01, 0x02, 0x01, 0x01, 0x06, 0x01, 0x01, 0x09, 0x01, 0x01 });
			var payload = new PayloadReader(value.ToByteArray()); 
			Assert.AreEqual("01-02-03-04-01-01-01-02-01-01-06-01-01-09-01-01", BitConverter.ToString(payload.ReadByteArrayFixedLength(16)));
		}

		// TODO: Finish unit tests
	}
}
