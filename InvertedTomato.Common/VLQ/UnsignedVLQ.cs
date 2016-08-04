using System;
using System.IO;
using System.Collections.Generic;

namespace InvertedTomato.VLQ {
    public static class UnsignedVLQ {
        public static void Encode(ulong value, Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            while (value > 0x7F) {
                output.WriteByte((byte)(value & 0x7F | 0x80));
                value >>= 7;
            }

            // Output last byte
            output.WriteByte((byte)value);
        }
        public static byte[] Encode(ulong value) {
            using (var stream = new MemoryStream()) {
                Encode(value, stream);
                return stream.ToArray();
            }
        }

        public static ulong Decode(Stream input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            int position = 0;
            ulong value = 0;
            int b;

            do {
                // Read next byte
                b = input.ReadByte();
                if (b == -1) {
                    throw new EndOfStreamException();
                }

                // Add bits to value
                value += (ulong)(b & 0x7F) << position; // Unchecked for performance - should it be?

                // Move position for next byte
                position += 7;
            } while ((b & 0x80) > 0);

            return value;
        }
        public static ulong Decode(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            var position = 0;
            return Decode(() => {
                var b = input[position];
                position++;
                return b;
            });
        }
        
        public static ulong Decode(IEnumerator<byte> input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            byte position = 0;
            ulong value = 0;

            while (true) {
                // Read next byte
                var b = input.Current;

                // Add bits to putput
                value += (ulong)(b & 0x7F) << position; // Unchecked for performance - should it be?
                position += 7;

                // Abort if last
                if (b >= 0x7F) {
                    return value;
                }

                // Check if more bytes are available
                if (!input.MoveNext()) { // This is the reason this can't use Decode(Func<byte>)
                    throw new EndOfStreamException();
                }
            }
        }
        public static ulong Decode(Func<byte> input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            ulong value = 0;
            byte position = 0;
            byte b;

            do {
                // Read next byte
                b = input();

                // Add bits to value
                value += (ulong)(b & 0x7F) << position; // Unchecked for performance - should it be?

                // Move position for next byte
                position += 7;
            } while ((b & 0x80) > 0);

            return value;
        }
    }
}
