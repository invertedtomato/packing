using System;
using System.IO;
using System.Collections.Generic;

namespace InvertedTomato.VLQ {
    public static class UnsignedVLQ {
        /// <summary>
        /// Encode integer as unsigned VLQ.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Encode(ulong value) {
            var input = BitConverter.GetBytes(value);
            var buffer = new byte[10];
            byte i;

            // Remodulate 8-bits down to 7-bits
            for (i = 0; i < input.Length * 8; i++) {
                if (input[i / 8].GetBit(i % 8)) {
                    buffer[i / 7] |= (byte)(1 << (i % 7));
                }
            }

            // Find how many bytes were actually used
            int usedBytes = 1; // Must be 1 to allow for at least one byte in output
            for (i = (byte)(buffer.Length - 1); i > 0; i--) {
                if (buffer[i] > 0) {
                    usedBytes = i+1;
                    break;
                }
            }

            // Set 'more' bits
            for (i = 0; i < usedBytes - 1; i++) {
                buffer[i] |= (1 << 7);
            }

            // Move into output array
            var output = new byte[usedBytes];
            Buffer.BlockCopy(buffer, 0, output, 0, output.Length);

            return output;
        }
        public static void Encode(ulong value, Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            var buffer = Encode(value);
            output.Write(buffer);
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
        public static ulong Decode(Stream input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            byte inputValue;
            int inputPosition;
            ulong outputValue = 0;
            byte outputPosition = 0;

            while (true) {
                // Get next byte
                inputValue = input.ReadUInt8();

                // Add bits
                for (inputPosition = 0; inputPosition < 7; inputPosition++) {
                    if (inputValue.GetBit(inputPosition)) {
                        checked {
                            outputValue += 1UL << outputPosition;
                        }
                    }
                    outputPosition++;
                }

                // Abort if last
                if (!inputValue.GetBit(7)) {
                    return outputValue;
                }
            }
        }

        public static ulong Decode(IEnumerator<byte> input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            byte inputValue;
            int inputPosition;
            ulong outputValue = 0;
            byte outputPosition = 0;

            while (true) {
                // Get byte
                inputValue = input.Current;

                // Add bits
                for (inputPosition = 0; inputPosition < 7; inputPosition++) {
                    if (inputValue.GetBit(inputPosition)) {
                        checked {
                            outputValue += 1UL << outputPosition;
                        }
                    }
                    outputPosition++;
                }

                // Abort if last
                if (!inputValue.GetBit(7)) {
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

            byte inputValue;
            int inputPosition;
            ulong outputValue = 0;
            byte outputPosition = 0;

            while (true) {
                // Get next byte
                inputValue = input();

                // Add bits
                for (inputPosition = 0; inputPosition < 7; inputPosition++) {
                    if (inputValue.GetBit(inputPosition)) {
                        checked {
                            outputValue += 1UL << outputPosition;
                        }
                    }
                    outputPosition++;
                }

                // Abort if last
                if (!inputValue.GetBit(7)) {
                    return outputValue;
                }
            }
        }
    }
}
