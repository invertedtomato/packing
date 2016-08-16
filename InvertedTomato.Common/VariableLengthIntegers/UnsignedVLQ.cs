using System;
using System.IO;
using System.Collections.Generic;
using InvertedTomato.Interfaces;

namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Utility to encode and decode unsigned numbers to the smallest possible number of raw bytes.
    /// 
    /// The number is encoded into 7 bits per byte, with the most significant bit (the 'final' bit) indicating if
    /// that byte is the last byte in the sequence.
    /// 
    /// Also uses the redundancy removal technique.
    /// 
    /// See https://hbfs.wordpress.com/2014/02/18/universal-coding-part-iii/ for details.
    /// 
    /// Examples:
    ///   0     encodes to 1000 0000
    ///   1     encodes to 1000 0001
    ///   127   encodes to 1111 1111
    ///   128   encodes to 0000 0000  1000 0000
    ///   16511 encodes to 0111 1111  1111 1111
    ///   16512 encodes to 0000 0000  0000 0000  1000 0000
    /// </summary>
    public class UnsignedVLQ {
        /// <summary>
        /// Mask to extract the data from a byte
        /// </summary>
        const int PAYLOAD_MASK = 0x7F; // 0111 1111  - this is an int32 to save later casting

        /// <summary>
        /// Mask to extract the 'final' bit from a byte.
        /// </summary>
        const int CONTINUITY_MASK = 0x80; // 1000 0000  - this is an int32 to save later casting

        /// <summary>
        /// Minimum length (in bytes) of the output of each encoded number.
        /// </summary>
        private readonly int PrefixBytes;

        /// <summary>
        /// Instantiate the UVarInt encoder.
        /// </summary>
        /// <param name="minBytes">Minimum number of bytes to include in output (between 1 and 8). Higher values increase efficiency when encoding larger values. Decoder must use these same values.</param>
        public UnsignedVLQ(int minBytes = 1) {
            if (minBytes < 1 || minBytes > 8) {
                throw new ArgumentOutOfRangeException("Must be between 1 and 8.", "minBytes");
            }

            // Store
            PrefixBytes = minBytes - 1;
        }

        /// <summary>
        /// Encode VLQ using lambda expression to retrieve next byte.
        /// </summary>
        /// <param name="value">Signed value to be encoded.</param>
        /// <param name="writeMove">Method to write byte and move pointer to the next byte.</param>
        public void Encode(ulong value, Action<byte> writeMove) {
            // Add any full bytes to start to fulfill min-bytes requirements
            for (var i = 0; i < PrefixBytes; i++) {
                writeMove((byte)value);
                value >>= 8;
            }


            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > PAYLOAD_MASK) {
                writeMove((byte)(value & PAYLOAD_MASK));
                value >>= 7;
                value--;
            }

            // Output remaining bits, with the 'final' bit set
            writeMove((byte)(value | CONTINUITY_MASK));
        }

        /// <summary>
        /// Encode number into an existing byte array (best performance).
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Buffer to output to.</param>
        /// <param name="position">Position to start reading in the input. Updated to the last position read after execution.</param>
        public void Encode(ulong value, byte[] output, ref int position) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            var innerPosition = position;
            Encode(value, (b) => {
                output[innerPosition++] |= b;
            });
            position = innerPosition;
        }

        /// <summary>
        /// Encode a number into a stream.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Stream to output into.</param>
        public void Encode(ulong value, Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            Encode(value, (b) => {
                output.WriteByte(b);
            });
        }

        /// <summary>
        /// Encode a number into a byte array (low performance).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>VLQ as byte array.</returns>
        public byte[] Encode(ulong value) {
            // Encode to buffer
            var buffer = new byte[10];
            var position = 0;

            Encode(value, (b) => {
                buffer[position++] = b;
            });

            // Trim unneeded bytes
            var output = new byte[position];
            Buffer.BlockCopy(buffer, 0, output, 0, output.Length);
            return output;
        }

        /// <summary>
        /// Decode VLQ using lambda expression to retrieve next byte.
        /// </summary>
        /// <param name="readMove">Method to acquire next byte and move pointer forward.</param>
        /// <returns></returns>
        public ulong Decode(Func<byte> readMove) {
            ulong value = 0;
            int currentByte;
            int bitOffset = 0;

            // Read any full bytes per min-bytes requirements
            for (var i = 0; i < PrefixBytes; i++) {
                // Read next byte
                currentByte = readMove();

                // Add bits to value
                value += (ulong)currentByte << bitOffset;
                bitOffset += 8;
            }

            // Read next byte
            currentByte = readMove();

            // Add bits to value
            value += (ulong)((currentByte & PAYLOAD_MASK)) << bitOffset;

            while ((currentByte & CONTINUITY_MASK) == 0) {
                // Update target offset
                bitOffset += 7;

                // Read next byte
                currentByte = readMove();

                // Add bits to value
                value += (ulong)((currentByte & PAYLOAD_MASK) + 1) << bitOffset;
            }

            return value;
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array starting at a given position (best performance).
        /// </summary>
        /// <param name="input">Array to read from.</param>
        /// <param name="position">Position to start reading at. Updated with last read position after call.</param>
        /// <returns>Next VLQ.</returns>
        public ulong Decode(byte[] input, ref int position) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            var innerPosition = position;
            var value = Decode(() => {
                return input[innerPosition++];
            });
            position = innerPosition;

            return value;
        }

        /// <summary>
        /// Decode the next VLQ from a given stream.
        /// </summary>
        /// <param name="input">Stream to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public ulong Decode(Stream input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            return Decode(() => {
                var b = input.ReadByte();
                if (b < 0) {
                    throw new EndOfStreamException();
                }
                return (byte)b;
            });
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array.
        /// </summary>
        /// <param name="input">Byte array to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public ulong Decode(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            var position = 0;
            var value = Decode(() => {
                return input[position++];
            });

            return value;
        }
    }
}
