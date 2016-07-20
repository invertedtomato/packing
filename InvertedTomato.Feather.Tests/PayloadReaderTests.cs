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
			IPAddress ip = new IPAddress(3232235527);
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
		public void ReadInt8() {
			var value = new PayloadWriter(0x00).Append((sbyte)15);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual((sbyte)15, payload.ReadInt8());
		}

		[TestMethod]
		public void ReadInt8_Negative() {
			var value = new PayloadWriter(0x00).Append((sbyte)-15);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual((sbyte)-15, payload.ReadInt8());
		}

		[TestMethod]
		public void ReadUInt8() {
			var value = new PayloadWriter(0x00).Append((byte)50);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual((byte)50, payload.ReadUInt8());
		}

		[TestMethod]
		public void ReadInt16() {
			var value = new PayloadWriter(0x00).Append((short)1000);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(1000, payload.ReadInt16());
		}
		[TestMethod]
		public void ReadInt16_Negative() {
			var value = new PayloadWriter(0x00).Append((short)-1000);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(-1000, payload.ReadInt16());
		}

		[TestMethod]
		public void ReadUInt16() {
			var value = new PayloadWriter(0x00).Append((short)2000);
			var payload = new PayloadReader(value.ToByteArray());

			Assert.AreEqual(2000, payload.ReadUInt16());
		}
		[TestMethod]
		public void ReadInt32() {
			var value = new PayloadWriter(0x00).Append(1400);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual(1400, payload.ReadInt32());
		}

		[TestMethod]// Check Negative Values
		public void ReadInt32_NegativeValue() {
			var value = new PayloadWriter(0x00).Append(-1400);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual(-1400, payload.ReadInt32());
		}

		[TestMethod]
		public void ReadUInt32() {
			var value = new PayloadWriter(0x00).Append((uint)1500);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual((uint)1500, payload.ReadUInt32());
		}


		[TestMethod]
		public void ReadInt64() {
			var value = new PayloadWriter(0x00).Append((long)1500);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual((long)1500, payload.ReadInt64());
		}


		[TestMethod]
		public void ReadUInt64() {
			var value = new PayloadWriter(0x00).Append((ulong)300);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual((ulong)300, payload.ReadUInt64());
		}

		[TestMethod]
		public void ReadDouble() {
			var value = new PayloadWriter(0x00).Append(65.67D);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual(65.67D, payload.ReadDouble());
		}

		[TestMethod]
		public void ReadFloat() {
			var value = new PayloadWriter(0x00).Append(65.6F);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual(65.6F, payload.ReadDouble());
		}

		[TestMethod]
		public void ReadNullableByteArray() {
			byte[] arr = null;
			var value = new PayloadWriter(0x00).AppendNullable(arr);
			var payload = new PayloadReader(value.ToByteArray());
			Assert.AreEqual(arr, payload.ReadNullableByteArray());
		}


		// TODO: Finish unit tests
	}
}
