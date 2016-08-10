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
    public class UVarInt {

        //TODO: mode
        //TODO: return size?


        /// <summary>
        /// Mask to extract the data from a byte
        /// </summary>
        const int PAYLOAD_MASK = 0x7F; // 0111 1111  - this is an int32 to save later casting

        /// <summary>
        /// Mask to extract the 'final' bit from a byte.
        /// </summary>
        const int CONTINUITY_MASK = 0x80; // 1000 0000  - this is an int32 to save later casting

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

            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > PAYLOAD_MASK) {
                output[position++] = (byte)(value & PAYLOAD_MASK);
                value >>= 7;
                value -= 1;
            }

            // Output remaining bits, with the 'final' bit set
            output[position++] = (byte)(value | CONTINUITY_MASK);
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

            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > PAYLOAD_MASK) {
                output.WriteByte((byte)(value & PAYLOAD_MASK)); // Set the 'more' bit on each output byte
                value >>= 7;
                value -= 1;
            }

            // Output remaining bits, with the 'final' bit set
            output.WriteByte((byte)(value | CONTINUITY_MASK));
        }

        /// <summary>
        /// Encode a number into a generic object.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Object to output to.</param>
        public void Encode(ulong value, IWriteByte output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > PAYLOAD_MASK) {
                output.WriteByte((byte)(value & PAYLOAD_MASK)); // Set the 'more' bit on each output byte
                value >>= 7;
                value -= 1;
            }

            // Output remaining bits, with the 'final' bit set
            output.WriteByte((byte)(value | CONTINUITY_MASK));
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
        public ulong Decode(byte[] input, ref int position) {
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
                value += (ulong)((currentByte & PAYLOAD_MASK) + 1) << bitOffset;

                // Move position for next byte
                position++;
                bitOffset += 7;
            } while ((currentByte & CONTINUITY_MASK) == 0);

            return value - 1;
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
                value += (ulong)((currentByte & PAYLOAD_MASK) + 1) << position;

                // Move position for next byte
                position += 7;
            } while ((currentByte & CONTINUITY_MASK) == 0);

            return value - 1;
        }

        /// <summary>
        /// Read the next VLQ from a generic object.
        /// </summary>
        /// <param name="input">Object to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public ulong Decode(IReadByte input) {
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
                value += (ulong)((currentByte & PAYLOAD_MASK) + 1) << position;

                // Move position for next byte
                position += 7;
            } while ((currentByte & CONTINUITY_MASK) == 0);

            return value - 1;
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array.
        /// </summary>
        /// <param name="input">Byte array to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public ulong Decode(byte[] input) {
            var position = 0;
            return Decode(input, ref position);
        }


        /* Alternate approach that allows for Mode. The Decode has issues.
        public void Encode(ulong value, byte[] output, ref int position) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            var mode = Mode;

            while (true) {
                if ((mode & 1) == 0) { // Next byte needs 8 bits
                    output[position++] = (byte)value;
                    value >>= 8;
                } else if (value > PAYLOAD_MASK) { // Next byte needs 7 bits
                    output[position++] = (byte)(value & PAYLOAD_MASK);
                    value >>= 7;
                    value--;
                } else { // Next takes <7 bits
                    output[position++] = (byte)(value | CONTINUITY_MASK);
                    return;
                }

                mode >>= 1;
            }
        }


        public ulong Decode(byte[] input, ref int position) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            var mode = Mode;
            ulong value = 0;
            int bitOffset = 0;

            while (true) {
                // Read next byte
                int currentByte = input[position++];

                if ((mode & 1) == 0) {
                    value += (ulong)currentByte << bitOffset;
                    bitOffset += 8;
                } else {
                    value += (ulong)((currentByte & PAYLOAD_MASK) + 1) << bitOffset;

                    if ((currentByte & CONTINUITY_MASK) > 0) {
                        return value - 1;
                    }
                    bitOffset += 7;
                }

                mode >>= 1;
            }
        }*/
    }
}
