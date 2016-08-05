using System;
using System.IO;
using System.Collections.Generic;
using InvertedTomato.Interfaces;

namespace InvertedTomato.VLQ {
    /// <summary>
    /// Utility to encode and decode unsigned numbers to the smallest possible number of raw bytes.
    /// 
    /// The number is encoded into 7 bits per byte, with the most significant bit (the 'more' bit) indicating if
    /// another byte follows.
    /// 
    /// For example:
    ///   0     encodes to 0000 0000
    ///   1     encodes to 0000 0001
    ///   127   encodes to 0111 1111
    ///   128   encodes to 0000 0001  0000 0000
    ///   16383 encodes to 1111 1111  0111 1111
    ///   16384 encodes to 1000 0000  1000 0000  0000 0001
    /// </summary>
    public static class UnsignedVLQ {
        /// <summary>
        /// Mask to extract the data from a byte
        /// </summary>
        const int DATA_MASK = 0x7F; // 0111 0000  - this is an int32 to save later casting

        /// <summary>
        /// Mask to extract the 'more' bit from a byte
        /// </summary>
        const int MORE_MASK = 0x80; // 1000 0000  - this is an int32 to save later casting

        /// <summary>
        /// Encode number into an existing byte array (best performance).
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Buffer to output to.</param>
        /// <param name="position">Position to start reading in the input. Updated to the last position read after execution.</param>
        public static void Encode(ulong value, byte[] output, ref int position) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > DATA_MASK) {
                output[position++] = (byte)(value & DATA_MASK | MORE_MASK); // Set the 'more' bit on each output byte
                value >>= 7;
            }

            // Output remaining bits, without setting the 'more' bit
            output[position++] = (byte)value;
        }

        /// <summary>
        /// Encode a number into a stream.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Stream to output into.</param>
        public static void Encode(ulong value, Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > DATA_MASK) {
                output.WriteByte((byte)(value & DATA_MASK | MORE_MASK)); // Set the 'more' bit on each output byte
                value >>= 7;
            }

            // Output remaining bits, without setting the 'more' bit
            output.WriteByte((byte)value);
        }

        /// <summary>
        /// Encode a number into a generic object.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Object to output to.</param>
        public static void Encode(ulong value, IWriteByte output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > DATA_MASK) {
                output.WriteByte((byte)(value & DATA_MASK | MORE_MASK)); // Set the 'more' bit on each output byte
                value >>= 7;
            }

            // Output remaining bits, without setting the 'more' bit
            output.WriteByte((byte)value);
        }

        /// <summary>
        /// Encode a number into a byte array (low performance).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>VLQ as byte array.</returns>
        public static byte[] Encode(ulong value) {
            // Encode to buffer
            var buffer = new byte[10];
            var position = 0;
            Encode(value, buffer, ref position);

            // Trim unneeded bytes
            var output = new byte[position];
            Buffer.BlockCopy(buffer, 0, output, 0, output.Length);
            return output;
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array starting at a given position (best performance).
        /// </summary>
        /// <param name="input">Array to read from.</param>
        /// <param name="position">Position to start reading at. Updated with last read position after call.</param>
        /// <returns>Next VLQ.</returns>
        public static ulong Decode(byte[] input, ref int position) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            ulong value = 0;
            int currentByte;
            int bitOffset = 0;

            do {
                // Read next byte
                currentByte = input[position];

                // Add bits to value
                value += (ulong)(currentByte & DATA_MASK) << bitOffset; // Unchecked for performance - should it be?

                // Move position for next byte
                position++;
                bitOffset += 7;
            } while ((currentByte & MORE_MASK) > 0);

            return value;
        }

        /// <summary>
        /// Decode the next VLQ from a given stream.
        /// </summary>
        /// <param name="input">Stream to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public static ulong Decode(Stream input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            int position = 0;
            ulong value = 0;
            int currentByte;

            do {
                // Read next byte
                currentByte = input.ReadByte();
                if (currentByte == -1) {
                    throw new EndOfStreamException();
                }

                // Add bits to value
                value += (ulong)(currentByte & DATA_MASK) << position; // Unchecked for performance - should it be?

                // Move position for next byte
                position += 7;
            } while ((currentByte & MORE_MASK) > 0);

            return value;
        }

        /// <summary>
        /// Read the next VLQ from a generic object.
        /// </summary>
        /// <param name="input">Object to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public static ulong Decode(IReadByte input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            int position = 0;
            ulong value = 0;
            int currentByte;

            do {
                // Read next byte
                currentByte = input.ReadByte();
                if (currentByte == -1) {
                    throw new EndOfStreamException();
                }

                // Add bits to value
                value += (ulong)(currentByte & DATA_MASK) << position; // Unchecked for performance - should it be?

                // Move position for next byte
                position += 7;
            } while ((currentByte & MORE_MASK) > 0);

            return value;
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array.
        /// </summary>
        /// <param name="input">Byte array to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public static ulong Decode(byte[] input) {
            var position = 0;
            return Decode(input, ref position);
        }
    }
}
