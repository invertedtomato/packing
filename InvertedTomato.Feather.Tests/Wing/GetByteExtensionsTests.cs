using NUnit.Framework;
using System;
using InvertedTomato.Feather.Extensions;

namespace InvertedTomato.Feather.Tests {
	[TestFixture]
	public class GetByteExtensionsTests {
		[Test]
		public void GetBytesNullableByteTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<byte>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 127 }, ((byte?)127).GetBytes());
		}
		[Test]
		public void GetBytesNullableSByteTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<sbyte>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 128 }, ((sbyte?)0).GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x00 }, ((sbyte?)-128).GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0xFF }, ((sbyte?)127).GetBytes());
		}
		[Test]
		public void GetBytesNullableUShortTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<ushort>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01, 0x00 }, ((ushort?)1).GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0xFF, 0xFF }, ((ushort?)ushort.MaxValue).GetBytes());
		}
		[Test]
		public void GetBytesNullableshortTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<short>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01, 0x00 }, ((short?)1).GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0xFF, 0X7F }, ((short?)short.MaxValue).GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x00, 0X80 }, ((short?)short.MinValue).GetBytes());
		}
		[Test]
		public void GetBytesNullableuintTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<uint>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01, 0x00, 0x00, 0x00, }, ((uint?)1).GetBytes());
		}
		[Test]
		public void GetBytesNullableintTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<int>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01, 0x00, 0x00, 0X00 }, ((int?)1).GetBytes());
		}
		[Test]
		public void GetBytesNullablelongTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<long>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01, 0x00, 0x00, 0X00, 0x00, 0x00, 0X00, 0x00 }, ((long?)1).GetBytes());
		}
		[Test]
		public void GetBytesNullableULongTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<ulong>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01, 0x00, 0x00, 0X00, 0x00, 0x00, 0X00, 0x00 }, ((ulong?)1).GetBytes());
		}
		[Test]
		public void GetBytesNullableDoubleTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<double>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x00, 0x00, 0x00, 0X00, 0x00, 0x00, 0XF0, 0x3F }, ((double?)1).GetBytes());
		}

		[Test]
		public void GetBytesNullableBoolTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<bool>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01 }, ((bool?)true).GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x00 }, ((bool?)false).GetBytes());
		}
		[Test]
		public void GetBytesDateTimeTest() {
			Assert.AreEqual(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc).GetBytes());
		}
		[Test]
		public void GetBytesNullableDateTimeTest() {
			Assert.AreEqual(new byte[] { 0x0 }, new Nullable<DateTime>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, ((DateTime?)new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc)).GetBytes());
		}
		[Test]
		public void GetBytesStringTest() {
			Assert.AreEqual(new byte[] { 0x00, 0x00 }, "".GetBytes());
			Assert.AreEqual(new byte[] { 0x04, 0x00, 84, 101, 115, 116 }, "Test".GetBytes());
		}
		[Test]
		public void GetBytesNullableGuidTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<Guid>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 ,0x00}, ((Guid?)new Guid()).GetBytes());
		}
		[Test]
		public void GetBytesGuidTest() {
			Assert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new Guid().GetBytes());
		}
		[Test]
		public void GetBytesTimeSpanTest() {
			Assert.AreEqual(new byte[] {0x1E, 0x00, 0x00, 0x00 }, TimeSpan.FromSeconds(30).GetBytes());
		}
		[Test]
		public void GetBytesNullableTimeSpanTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<TimeSpan>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x1E, 0x00, 0x00, 0x00 }, ((TimeSpan?)TimeSpan.FromSeconds(30)).GetBytes());
		}

		[Test]
		public void GetBytesNullablebyteTest() {
			Assert.AreEqual(new byte[] { 0x00 }, new Nullable<byte>().GetBytes());
			Assert.AreEqual(new byte[] { 0x01, 0x01}, ((byte?)1).GetBytes());
		}
	}
}