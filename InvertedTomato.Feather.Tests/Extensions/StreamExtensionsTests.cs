using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using InvertedTomato.Feather;

namespace InvertedTomato.Feather.Tests.Extensions {
	[TestFixture]
	public class StreamExtensionsTests {

		#region DateTime
		[Test]
		public void ReadDateTimeTest() {
			using (var stream = new MemoryStream()) {
				stream.Write(DateTime.UtcNow.Date);
				stream.Position = 0;
				Assert.AreEqual(DateTime.UtcNow.Date, stream.ReadDateTime());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}

		[Test]
		public void WriteDateTimeTest() {
			using (var stream = new MemoryStream()) {
				stream.Write(DateTime.UtcNow.Date.AddDays(-1));
				stream.Position = 0;
				Assert.AreEqual(DateTime.UtcNow.Date.AddDays(-1), stream.ReadDateTime());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());

			}
		}

		#endregion

		#region String
		[Test]
		public void ReadStringTest() {
			var text = "Hi David";
			var byteArray = Encoding.UTF8.GetBytes(text);
			using (var stream = new MemoryStream()) {
				stream.Write((UInt16)byteArray.Length);
				stream.Write(byteArray);
				stream.Position = 0; // reset the stream to the beginning again
				var returnValue = stream.ReadString();
				Assert.AreEqual(text, returnValue);
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}

		[Test]
		public void ReadMultipleStringTest() {
			var text = "Hi David¢€"; // includes multibyte characters
			using (var stream = new MemoryStream()) {
				stream.Write(text.GetBytes());
				stream.Write(text.GetBytes());
				stream.Position = 0; // reset the stream to the beginning again
				Assert.AreEqual(text, stream.ReadString());
				Assert.AreEqual(text, stream.ReadString());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}

		}


		[Test]
		public void WriteStringTest() {
			var text = "Hello Vlife";
			var byteArray = Encoding.UTF8.GetBytes(text);
			using (var stream = new MemoryStream(100)) {
				InvertedTomato.Feather.StreamExtensions.Write(stream, text);
				stream.Position = 0; // reset the stream to the beginning again
				Assert.AreEqual(text, stream.ReadString());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}
		[Test]
		public void WriteReadStringTest_2ByteChar() {
			var text = "¢¢¢¢¢";
			var byteArray = Encoding.UTF8.GetBytes(text);
			using (var stream = new MemoryStream(100)) {
				InvertedTomato.Feather.StreamExtensions.Write(stream, text);
				stream.Position = 0; // reset the stream to the beginning again
				Assert.AreEqual(text, stream.ReadString());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}
		[Test]
		public void WriteReadStringTest_3ByteChar() {
			var text = "€€€€";
			var byteArray = Encoding.UTF8.GetBytes(text);
			using (var stream = new MemoryStream(100)) {
				InvertedTomato.Feather.StreamExtensions.Write(stream, text);
				stream.Position = 0; // reset the stream to the beginning again
				Assert.AreEqual(text, stream.ReadString());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}
		#endregion

		#region Guid

		[Test]
		public void ReadGuidTest() {
			var guid = Guid.NewGuid();
			using (var stream = new MemoryStream()) {
				stream.Write(guid);
				stream.Position = 0;
				Assert.AreEqual(guid, stream.ReadGuid());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}

		[Test]
		public void WriteGuidTest() {
			var guid = Guid.NewGuid();
			using (var stream = new MemoryStream()) {
				var returnValue = stream.Write(guid);
				stream.Position = 0;
				Assert.AreEqual(guid, stream.ReadGuid());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}

		#endregion

		#region Boolean

		[Test]
		public void ReadBooleanTest() {
			using (var stream = new MemoryStream()) {
				stream.Write(true.GetBytes());
				stream.Write(false.GetBytes());
				stream.Position = 0;
				Assert.IsTrue(stream.ReadBoolean());
				Assert.IsFalse(stream.ReadBoolean());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}

		[Test]
		public void WriteBooleanTest() {
			using (var stream = new MemoryStream()) {
				stream.Write(false);
				stream.Write(true);
				stream.Position = 0;
				Assert.AreEqual(false, stream.ReadBoolean());
				Assert.AreEqual(true, stream.ReadBoolean());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}

		#endregion

		#region TimeSpan

		[Test]
		public void ReadTimeSpanTest() {
			var timeSpan = new TimeSpan(2, 4, 56);
			using (var stream = new MemoryStream()) {
				stream.Write(timeSpan);
				stream.Position = 0;
				Assert.AreEqual(timeSpan, stream.ReadTimeSpan());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}
		}

		[Test]
		public void WriteTimeSpanTest() {
			var timeSpan = new TimeSpan(12, 54, 12);
			using (var stream = new MemoryStream()) {
				stream.Write(timeSpan);
				stream.Position = 0;
				Assert.AreEqual(timeSpan, stream.ReadTimeSpan());
				// Check nothing left in stream 
				Assert.AreEqual(-1, stream.ReadByte());
			}

		}

		#endregion
	}
}
