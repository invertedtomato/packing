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

			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
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

			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendString() {
			var payload = new PayloadWriter(0x00);
			payload.Append("VLife");

			Assert.AreEqual("00-05-00-56-4C-69-66-65", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableString() {
			var payload = new PayloadWriter(0x00);
			string value = null;
			payload.AppendNullable(value);

			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}


		[TestMethod]
		public void AppendGuid() {
			var payload = new PayloadWriter(0x00);
			payload.Append(new Guid("5d813059-d9f1-4662-902b-01437bdb7ef9"));

			Assert.AreEqual("00-59-30-81-5D-F1-D9-62-46-90-2B-01-43-7B-DB-7E-F9", BitConverter.ToString(payload.ToByteArray()));
		}
		 

		[TestMethod]
		public void AppendNullableGUI() {
			var payload = new PayloadWriter(0x00);
			Guid? value = null;
			payload.AppendNullable(value);

			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableGUI_WithValue() {
			var payload = new PayloadWriter(0x00);
			Guid? value = new Guid("5d813059-d9f1-4662-902b-01437bdb7ef9");
			payload.AppendNullable(value); 

			Assert.AreEqual("00-01-59-30-81-5D-F1-D9-62-46-90-2B-01-43-7B-DB-7E-F9", BitConverter.ToString(payload.ToByteArray()));
		}
		 

		[TestMethod]
		public void AppendIPAddress() {
			var ip = new IPAddress(3232235527); //192.168.0.7
			var payload = new PayloadWriter(0x00);
			payload.Append(ip);

			Assert.AreEqual("00-04-07-00-A8-C0", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableIpAddress() {
			var payload = new PayloadWriter(0x00);
			payload.AppendNullable((IPAddress)null);

			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendInt8() {
			var payload = new PayloadWriter(0x00);
			payload.Append((sbyte)126);

			Assert.AreEqual("00-7E", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableInt8() {
			var payload = new PayloadWriter(0x00);
			sbyte? value = null;
			payload.AppendNullable(value);


			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendUInt8() {
			var payload = new PayloadWriter(0x00);
			payload.Append((byte)254);

			Assert.AreEqual("00-FE", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableUInt8() {
			var payload = new PayloadWriter(0x00);
			byte? value = null;
			payload.AppendNullable(value);

			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendInt16() {
			var payload = new PayloadWriter(0x00);
			payload.Append((short)100);



			Assert.AreEqual("00-64-00", BitConverter.ToString(payload.ToByteArray()));
		}
		[TestMethod]
		public void AppendNullableInt16() {
			var payload = new PayloadWriter(0x00);
			short? value = null;
			payload.AppendNullable(value);



			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}


		[TestMethod]
		public void AppendUInt16() {
			var payload = new PayloadWriter(0x00);
			payload.Append((ushort)200);


			Assert.AreEqual("00-C8-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableUInt16() {
			var payload = new PayloadWriter(0x00);
			ushort? value = null;
			payload.AppendNullable(value);


			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendInt32() {
			var payload = new PayloadWriter(0x00);
			payload.Append(1400);


			Assert.AreEqual("00-78-05-00-00", BitConverter.ToString(payload.ToByteArray()));
		}
		[TestMethod]
		public void AppendNullableInt32_WithValue() {
			var payload = new PayloadWriter(0x00);
			int? value = 1400;
			payload.AppendNullable(value); 
			Assert.AreEqual("00-01-78-05-00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableInt32() {
			var payload = new PayloadWriter(0x00);
			int? value = null;
			payload.AppendNullable(value);


			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}
		[TestMethod]
		public void AppendUInt32() {
			var payload = new PayloadWriter(0x00);
			payload.Append((uint)200);


			Assert.AreEqual("00-C8-00-00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableUInt32() {
			var payload = new PayloadWriter(0x00);
			uint? value = null;
			payload.AppendNullable(value);


			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendInt64() {
			var payload = new PayloadWriter(0x00);
			payload.Append((long)1400);


			Assert.AreEqual("00-78-05-00-00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
		}
		[TestMethod]
		public void AppendNullableInt64() {
			var payload = new PayloadWriter(0x00);
			long? value = null;
			payload.AppendNullable(value);


			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendUInt64() {
			var payload = new PayloadWriter(0x00);
			payload.Append((ulong)1500);
			//


			Assert.AreEqual("00-DC-05-00-00-00-00-00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableUInt64() {
			var payload = new PayloadWriter(0x00);
			ulong? value = null;
			payload.AppendNullable(value);



			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendDouble() {
			var payload = new PayloadWriter(0x00);
			payload.Append((double)22.34);


			Assert.AreEqual("00-D7-A3-70-3D-0A-57-36-40", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableDouble() {
			var payload = new PayloadWriter(0x00);
			double? value = null;
			payload.AppendNullable(value);


			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendFloat() {
			var payload = new PayloadWriter(0x00);
			payload.Append(4.3f);

			Assert.AreEqual("00-9A-99-89-40", BitConverter.ToString(payload.ToByteArray()));
		}

		[TestMethod]
		public void AppendNullableFloat() {
			var payload = new PayloadWriter(0x00);
			float? value = null;
			payload.AppendNullable(value);

			Assert.AreEqual("00-00", BitConverter.ToString(payload.ToByteArray()));
		}

		// TODO: Finish unit tests
	}
}
