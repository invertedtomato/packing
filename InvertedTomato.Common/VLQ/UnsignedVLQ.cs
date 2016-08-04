using System;
using System.IO;
using System.Collections.Generic;

namespace InvertedTomato.VLQ {
    public static class UnsignedVLQ {
        public static void Encode(ulong value, Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            while (true) {
                // Add 7 bits to buffer, setting the 'more' bit at the same time
                var buffer = (byte)(value & 0x7F | 0x80);

                // Shift the input by 7 bits ready for the next byte
                value = value >> 7;

                // If there's no more input remaining...
                if (value == 0) {
                    // Write byte without 'more' but
                    output.WriteByte((byte)(buffer & 0x7F));
                    break;
                } else {
                    // Write byte
                    output.Write(buffer);
                }
            }
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

            int inputPosition = 0;
            ulong outputValue = 0;

            while (true) {
                // Read next byte
                var b = input.ReadByte();
                if (b == -1) {
                    throw new EndOfStreamException();
                }

                // Add bits to putput
                outputValue += (ulong)((b & 0x7F) << inputPosition);
                inputPosition += 7;

                // Abort if last
                if (b >= 0x7F) {
                    return outputValue;
                }
            }
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

            int inputPosition = 0;
            ulong outputValue = 0;

            while (true) {
                // Read next byte
                var b = input.Current;

                // Add bits to putput
                outputValue += (ulong)((b & 0x7F) << inputPosition);
                inputPosition += 7;

                // Abort if last
                if (b >= 0x7F) {
                    return outputValue;
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
            
            int inputPosition = 0;
            ulong outputValue = 0;

            while (true) {
                // Read next byte
                var b = input();

                // Add bits to putput
                outputValue += (ulong)((b & 0x7F) << inputPosition);
                inputPosition += 7;

                // Abort if last
                if (b >= 0x7F) {
                    return outputValue;
                }
            }
        }
    }
}
