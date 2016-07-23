using System;
using System.IO;
using System.Net;
using System.Text;

namespace InvertedTomato.Feather {
    public class PayloadWriter : Payload {
        //public int Position { get { return (int)Inner.Position; } }
        //public int Remaining { get { return (int)(Inner.Length - Inner.Position); } }

        public PayloadWriter(byte opCode) {
            // Setup inner store
            Inner = new MemoryStream();
            Inner.Write(opCode);

            // Note opcode separately
            OpCode = opCode;
        }

        public PayloadWriter(byte opCode, int expectedCapacity) {
            // Setup inner store
            Inner = new MemoryStream(expectedCapacity);
            Inner.Write(opCode);

            // Note opcode separately
            OpCode = opCode;
        }

        public PayloadWriter Append(DateTime value) {
            var rawValue = value.ToUnixTimeAsUInt64();
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public PayloadWriter AppendNullable(DateTime? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(TimeSpan value) {
            var rawValue = (int)value.TotalSeconds;
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public PayloadWriter AppendNullable(TimeSpan? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(string value) {
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
        public PayloadWriter AppendNullable(string value) {
            if (null == value) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value); // Allow chaining
        }

        public PayloadWriter Append(Guid value) {
            var rawValue = value.ToByteArray();
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public PayloadWriter AppendNullable(Guid? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(bool value0, bool value1 = false, bool value2 = false, bool value3 = false, bool value4 = false, bool value5 = false, bool value6 = false, bool value7 = false) {
            byte rawValue = 0;

            BoolHelper(value0, 0, ref rawValue);
            BoolHelper(value1, 1, ref rawValue);
            BoolHelper(value2, 2, ref rawValue);
            BoolHelper(value3, 3, ref rawValue);
            BoolHelper(value4, 4, ref rawValue);
            BoolHelper(value5, 5, ref rawValue);
            BoolHelper(value6, 6, ref rawValue);
            BoolHelper(value7, 7, ref rawValue);

            Inner.Write(rawValue);

            return this;
        }
        public PayloadWriter AppendNullable(bool? value0, bool? value1 = null, bool? value2 = null, bool? value3 = null, bool? value4 = null, bool? value5 = null, bool? value6 = null, bool? value7 = null) {
            byte nullableValue = 0;
            byte rawValue = 0;

            BoolHelper(value0, 0, ref rawValue, ref nullableValue);
            BoolHelper(value1, 1, ref rawValue, ref nullableValue);
            BoolHelper(value2, 2, ref rawValue, ref nullableValue);
            BoolHelper(value3, 3, ref rawValue, ref nullableValue);
            BoolHelper(value4, 4, ref rawValue, ref nullableValue);
            BoolHelper(value5, 5, ref rawValue, ref nullableValue);
            BoolHelper(value6, 6, ref rawValue, ref nullableValue);
            BoolHelper(value7, 7, ref rawValue, ref nullableValue);

            Inner.Write(nullableValue);
            Inner.Write(rawValue);

            return this;
        }

        public PayloadWriter Append(IPAddress value) {
            if (null == value) {
                throw new ArgumentNullException("Value cannot be null. If this was intentional use WriteNullable instead.", "value");
            }

            var rawValue = value.GetAddressBytes();
            var valueLength = (byte)rawValue.Length;
            Inner.Write(valueLength);
            Inner.Write(rawValue);
            return this; // Allow chaining
        }
        public PayloadWriter AppendNullable(IPAddress value) {
            if (null == value) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value); // Allow chaining
        }

        // TODO: nullable numbers
        public PayloadWriter Append(sbyte value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(sbyte? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }
            Inner.Write(0x01);
            return Append(value.Value); ; // Allow chaining; 
        }

        public PayloadWriter Append(byte value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(byte? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(short value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(short? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(ushort value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(ushort? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(int value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(int? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(uint value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(uint? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(long value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(long? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(ulong value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(ulong? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(double value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter AppendNullable(double? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(float value) {
            Inner.Write(value);
            return this; // Allow chaining;
        }
        public PayloadWriter Append(float? value) {
            if (!value.HasValue) {
                Inner.Write(0x00);
                return this; // Allow chaining
            }

            Inner.Write(0x01);
            return Append(value.Value); // Allow chaining
        }

        public PayloadWriter Append(byte[] value) {
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
        public PayloadWriter AppendNullable(byte[] value) {
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
        public PayloadWriter AppendFixedLength(byte[] value) {
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            Inner.Write(value);
            return this; // Allow chaining
        }

        private void BoolHelper(bool? value, byte offset, ref byte outValue, ref byte outNullable) {
            if (!value.HasValue) {
                outNullable += (byte)(1 >> offset);
            } else if (value.Value) {
                outValue += (byte)(1 >> offset);
            }
        }
        private void BoolHelper(bool value, byte offset, ref byte outValue) {
            if (value) {
                outValue += (byte)(1 >> offset);
            }
        }


    }
}
