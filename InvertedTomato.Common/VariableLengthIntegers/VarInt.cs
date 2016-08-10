using InvertedTomato.Interfaces;
using System;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Utility to encode and decode signed numbers to the smallest possible number of raw bytes.
    /// </summary>
    public class VarInt {
        private readonly UVarInt UVarInt;

        public VarInt() {
            UVarInt = new UVarInt();
        }

        /// <summary>
        /// Encode number into an existing byte array (best performance).
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Buffer to output to.</param>
        /// <param name="position">Position to start reading in the input. Updated to the last position read after execution.</param>
        public void Encode(long value, byte[] output, ref int position) {
            UVarInt.Encode(ZigZagEncode(value), output, ref position);
        }

        /// <summary>
        /// Encode a number into a stream.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Stream to output into.</param>
        public void Encode(long value, Stream output) {
            UVarInt.Encode(ZigZagEncode(value), output);
        }

        /// <summary>
        /// Encode a number into a generic object.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Object to output to.</param>
        public void Encode(long value, IWriteByte output) {
            UVarInt.Encode(ZigZagEncode(value), output);
        }

        /// <summary>
        /// Encode a number into a byte array (low performance).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>VLQ as byte array.</returns>
        public byte[] Encode(long value) {
            return UVarInt.Encode(ZigZagEncode(value));
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array starting at a given position (best performance).
        /// </summary>
        /// <param name="input">Array to read from.</param>
        /// <param name="position">Position to start reading at. Updated with last read position after call.</param>
        /// <returns>Next VLQ.</returns>
        public long Decode(byte[] input, ref int position) {
            return ZigZagDecode(UVarInt.Decode(input, ref position));
        }

        /// <summary>
        /// Decode the next VLQ from a given stream.
        /// </summary>
        /// <param name="input">Stream to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public long Decode(Stream input) {
            return ZigZagDecode(UVarInt.Decode(input));
        }

        /// <summary>
        /// Read the next VLQ from a generic object.
        /// </summary>
        /// <param name="input">Object to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public long Decode(IReadByte input) {
            return ZigZagDecode(UVarInt.Decode(input));
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array.
        /// </summary>
        /// <param name="input">Byte array to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public long Decode(byte[] input) {
            return ZigZagDecode(UVarInt.Decode(input));
        }


        private ulong ZigZagEncode(long value) {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        private long ZigZagDecode(ulong value) {
            var a = (long)value;
            return (a >> 1) ^ (-(a & 1));
        }
    }
}