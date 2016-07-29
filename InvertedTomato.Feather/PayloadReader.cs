using System;
using System.IO;
using System.Net;
using System.Text;

namespace InvertedTomato.Feather {
	public class PayloadReader : Payload {
		public int Position { get { return (int)Inner.Position; } }
		public int Remaining { get { return (int)(Inner.Length - Inner.Position); } }


		public PayloadReader(byte[] rawPayload) {
			if (null == rawPayload) {
				throw new ArgumentNullException("rawPayload");
			}
			if (rawPayload.Length < 1) {
				throw new ArgumentException("Must be at least one byte long.", "rawPayload");
			}

			// Setup inner store
			Inner = new MemoryStream(rawPayload, false);
			Inner.Position = 1;

			// Note opcode separately
			OpCode = rawPayload[0];
		}

		public DateTime ReadDateTime() {
			var rawValue = Inner.ReadUInt64();
			return DateUtility.FromUnixTimestamp(rawValue);
		}
		public DateTime? ReadNullableDateTime() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}

			return ReadDateTime();
		}

		public TimeSpan ReadTimeSpan() {
			var rawValue = Inner.ReadUInt32();
			return TimeSpan.FromSeconds(rawValue);
		}
		public TimeSpan? ReadNullableTimeSpan() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}

			return ReadTimeSpan();
		}

		public string ReadString() {
			var valueLength = Inner.ReadUInt16();
			var rawValue = Inner.Read(valueLength);
			return Encoding.UTF8.GetString(rawValue);
		}
		public string ReadNullableString() {
			var hasValue = Inner.ReadUInt8() > 0;
			if (!hasValue) {
				return null;
			}
			return ReadString();
		}

		public Guid ReadGuid() {
			var rawValue = Inner.Read(16);
			return new Guid(rawValue);
		}
		public Guid? ReadNullableGuid() {
			var hasValue = Inner.ReadUInt8() > 0;
			if (!hasValue) {
				return null;
			}
			return ReadGuid();
		}

		public bool ReadBoolean() {
			var rawValue = Inner.ReadByte();
			return rawValue > 0;
		}
		public bool[] ReadBooleans() {

			var rawValue = (byte)Inner.ReadByte();
			var value = new bool[8];
			for (byte i = 0; i < 8; i++) {
				value[i] = BoolHeaper(i, rawValue);
			}
			return value;
		}
		public bool?[] ReadNullableBooleans() {
			var nullableValue = (byte)Inner.ReadByte();
			var rawValue = (byte)Inner.ReadByte();

			var value = new bool?[8];
			for (byte i = 0; i < 8; i++) {
				value[i] = BoolHeaper(i, rawValue, nullableValue);
			}
			return value;
		}

		public IPAddress ReadIPAddress() {
			var valueLength = (byte)Inner.ReadByte();
			var rawValue = Inner.Read(valueLength);
			return new IPAddress(rawValue);
		}
		public IPAddress ReadNullableIPAddress() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}

			return ReadIPAddress();
		}
		 
		public sbyte ReadInt8() {
			return Inner.ReadInt8();
		}

		public sbyte? ReadNullableInt8() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return ReadInt8();
		}

		public byte ReadUInt8() {
			return Inner.ReadUInt8();
		}

		public byte? ReadNullableUInt8() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return ReadUInt8();
		}

		public short ReadInt16() {
			return Inner.ReadInt16();
		}
		public short? ReadNullableInt16() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadInt16();
		}

		public ushort ReadUInt16() {
			return Inner.ReadUInt16();
		}
		public ushort? ReadNullableUInt16() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadUInt16();
		}

		public int ReadInt32() {
			return Inner.ReadInt32();
		}
		public int? ReadNullableInt32() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadInt32();
		}

		public uint ReadUInt32() {
			return Inner.ReadUInt32();
		}
		public uint? ReadNullableUInt32() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadUInt32();
		}

		public long ReadInt64() {
			return Inner.ReadInt64();
		}
		public long? ReadNullableInt64() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadInt64();
		}

		public ulong ReadUInt64() {
			return Inner.ReadUInt64();
		}
		public ulong? ReadNullableUInt64() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadUInt64();
		}

		public double ReadDouble() {
			return Inner.ReadDouble();
		}
		public double? ReadNullableDouble() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadDouble();
		}

		public float ReadFloat() {
			return Inner.ReadFloat();
		}

		public float? ReadNullableFloat() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}
			return Inner.ReadFloat();
		}

		public byte[] ReadByteArray() {
			var valueLength = Inner.ReadUInt16();
			var rawValue = Inner.Read(valueLength);
			return rawValue;
		}
		public byte[] ReadNullableByteArray() {
			var hasValue = Inner.ReadByte() > 0;
			if (!hasValue) {
				return null;
			}

			return ReadByteArray();
		}
		public byte[] ReadByteArrayFixedLength(int count) {
			return Inner.Read(count);
		}


		private bool BoolHeaper(byte offset, byte rawValue) {
			var mask = 1 >> offset;
			throw new NotImplementedException(); // TODO
												 //return (int)rawValue && mask; // Something like this
		}
		private bool? BoolHeaper(byte offset, byte rawValue, byte nullableValue) {
			var mask = 1 >> offset;
			throw new NotImplementedException(); // TODO
												 /*
												 if ((int)nullableValue && mask) {// Something like this
													 return null;
												 } else {
													 return (int)rawValue && mask; 
												 }*/
		}

	}
}
