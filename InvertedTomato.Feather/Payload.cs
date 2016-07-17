using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace InvertedTomato.Feather {
    public sealed class Payload {
        public int Length { get { return (int)Inner.Length; } }
        public int Position { get { return (int)Inner.Position; } }
        public int Remaining { get { return (int)(Inner.Length - Inner.Position); } }
        public byte Opcode { get; }

        private MemoryStream Inner;

        /// <summary>
        /// Initialize payload in read-only mode with a byte-array equivalent.
        /// </summary>
        public Payload(byte[] payload) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
            if (payload.Length < 1) {
                throw new ArgumentException("Must be at least one byte long.", "payload");
            }

            // Setup inner store
            Inner = new MemoryStream(payload, false);
            Inner.Position = 1;

            // Note opcode separately
            Opcode = payload[0];
        }
        
        // TODO: fix unit tests

        public Payload(byte opcode) {
            // Setup inner store
            Inner = new MemoryStream();
            Inner.Write(opcode);

            // Note opcode separately
            Opcode = opcode;
        }

        public void Rewind() {
            Inner.Position = 1;
        }


        public byte[] ReadByteArray() {
            var valueLength = Inner.ReadUInt16();
            var rawValue = Inner.Read(valueLength);
            return rawValue;
        }
        public Payload Append(byte[] value) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }
            if (value.Length > ushort.MaxValue) {
                throw new InternalBufferOverflowException("Must be less than 65KB.");
            }

            var rawLength = (ushort)value.Length;
            Inner.Write(rawLength);
            Inner.Write(value);
            return this; // Allow chaining
        }
        public byte[] ReadNullableByteArray() {
            var hasValue = Inner.ReadByte() > 0;
            if (!hasValue) {
                return null;
            }

            return ReadByteArray();
        }
        public Payload AppendNullable(byte[] value) {
            if (null == value) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }
            if (value.Length > ushort.MaxValue) {
                throw new InternalBufferOverflowException("Must be less than 65KB.");
            }

            Inner.Write(0x01);
            return Append(value); // Allow chaining
        }
        public byte[] ReadByteArrayFixedLength(int count) {
            return Inner.Read(count);
        }
        public Payload AppendFixedLength(byte[] value) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            Inner.Write(value);
            return this; // Allow chaining
        }

        public DateTime ReadDateTime() {
            var rawValue = Inner.ReadUInt64();
            return DateUtility.FromUnixTimestamp(rawValue);
        }
        public Payload Append(DateTime value) {
            var rawValue = value.ToUnixTimeAsUInt64();
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public DateTime? ReadNullableDateTime() {
            var hasValue = Inner.ReadByte() > 0;
            if (!hasValue) {
                return null;
            }

            return ReadDateTime();
        }
        public Payload AppendNullable(DateTime? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public TimeSpan ReadTimeSpan() {
            var rawValue = Inner.ReadUInt32();
            return TimeSpan.FromSeconds(rawValue);
        }
        public Payload Append(TimeSpan value) {
            var rawValue = (int)value.TotalSeconds;
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public TimeSpan? ReadNullableTimeSpan() {
            var hasValue = Inner.ReadByte() > 0;
            if (!hasValue) {
                return null;
            }

            return ReadTimeSpan();
        }
        public Payload AppendNullable(TimeSpan? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public string ReadString() {
            var valueLength = Inner.ReadUInt16();
            var rawValue = Inner.Read(valueLength);
            return Encoding.UTF8.GetString(rawValue);
        }
        public Payload Append(string value) {
            if (null == value) {
                throw new ArgumentNullException("Value cannot be null. If this was intentional use WriteNullable instead.", "value");
            }

            var rawValue = Encoding.UTF8.GetBytes(value);
            if (rawValue.Length > ushort.MaxValue) {
                throw new InternalBufferOverflowException("Must be less than 65KB when encoded in UTF8.");
            }
            Inner.Write((ushort)rawValue.Length);
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public string ReadNullableString() {
            var hasValue = Inner.ReadUInt8() > 0;
            if (!hasValue) {
                return null;
            }
            return ReadString();
        }
        public Payload AppendNullable(string value) {
            if (null == value) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value); // Allow chaining
        }

        public Guid ReadGuid() {
            var rawValue = Inner.Read(16);
            return new Guid(rawValue);
        }
        public Payload Append(Guid value) {
            var rawValue = value.ToByteArray();
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public Guid? ReadNullableGuid() {
            var hasValue = Inner.ReadUInt8() > 0;
            if (!hasValue) {
                return null;
            }
            return ReadGuid();
        }
        public Payload AppendNullable(Guid? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public bool ReadBoolean() {
            var rawValue = Inner.ReadByte();
            return rawValue > 0;
        }
        public bool[] ReadBooleans() {
            throw new NotImplementedException(); // TODO:
            /*
            var rawValue = (byte)Inner.ReadByte();
            var value = new bool[8];
            value[0] = rawValue & 0x01;
            value[1] = rawValue & 0x02;
            value[2] = rawValue & 0x04;
            value[3] = rawValue & 0x08;
            value[4] = rawValue & 0x0F;
            value[5] = rawValue & 0x20;
            value[6] = rawValue & 0x40;
            value[7] = rawValue & 0x80;
            return value;*/
        }
        public Payload Append(bool value0, bool value1 = false, bool value2 = false, bool value3 = false, bool value4 = false, bool value5 = false, bool value6 = false, bool value7 = false) {
            byte rawValue = 0;
            if (value0) {
                rawValue += 0x01;
            }
            if (value1) {
                rawValue += 0x02;
            }
            if (value2) {
                rawValue += 0x04;
            }
            if (value3) {
                rawValue += 0x08;
            }
            if (value4) {
                rawValue += 0x0F;
            }
            if (value5) {
                rawValue += 0x20;
            }
            if (value6) {
                rawValue += 0x40;
            }
            if (value7) {
                rawValue += 0x80;
            }

            Inner.Write(rawValue);
            return this;
        }
        // TODO: Nullable boolean?

        public IPAddress ReadIPAddress() {
            var valueLength = (byte)Inner.ReadByte();
            var rawValue = Inner.Read(valueLength);
            return new IPAddress(rawValue);
        }
        public Payload Append(IPAddress value) {
            if (null == value) {
                throw new ArgumentNullException("Value cannot be null. If this was intentional use WriteNullable instead.", "value");
            }

            var rawValue = value.GetAddressBytes();
            var valueLength = (byte)rawValue.Length;
            Inner.Write(valueLength);
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public IPAddress ReadNullableIPAddress() {
            var hasValue = Inner.ReadByte() > 0;
            if (!hasValue) {
                return null;
            }

            return ReadIPAddress();
        }
        public Payload AppendNullable(IPAddress value) {
            if (null == value) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value); // Allow chaining
        }

        public sbyte ReadInt8() {
            return Inner.ReadInt8();
        }
        public Payload Append(sbyte value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public byte ReadUInt8() {
            return Inner.ReadUInt8();
        }
        public Payload Append(byte value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public short ReadInt16() {
            return Inner.ReadInt16();
        }
        public Payload Append(short value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public ushort ReadUInt16() {
            return Inner.ReadUInt16();
        }
        public Payload Append(ushort value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public int ReadInt32() {
            return Inner.ReadInt32();
        }
        public Payload Append(int value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public uint ReadUInt32() {
            return Inner.ReadUInt32();
        }
        public Payload Append(uint value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public long ReadInt64() {
            return Inner.ReadInt64();
        }
        public Payload Append(long value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public ulong ReadUInt64() {
            return Inner.ReadUInt64();
        }
        public Payload Append(ulong value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public double ReadDouble() {
            return Inner.ReadDouble();
        }
        public Payload Append(double value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        public float ReadFloat() {
            return Inner.ReadFloat();
        }
        public Payload Append(float value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }

        /*
        public Payload(byte opcode, byte[] parameters) {
            if (null == parameters) {
                throw new ArgumentNullException("parameters");
            }

            Opcode = opcode;
            Parameters2 = new MemoryStream(parameters,false);
            //Parameters = parameters;
        }
        public Payload(byte opcode, params byte[][] parameters) {
            Opcode = opcode;

            // Merge everything to be sent into a buffer
            Parameters = new byte[parameters.Sum(a => a.Length)];
            var pos = 0;
            foreach (var parameter in parameters) {
                Buffer.BlockCopy(parameter, 0, Parameters, pos, parameter.Length);
                pos += parameter.Length;
            }
        }
        */

        public byte[] ToByteArray() {
            return Inner.ToArray();
        }
    }
}
