using System;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    public class SignedOmega {
        private readonly UnsignedOmega Underlying;

        /// <summary>
        /// Instantiate encoder
        /// </summary>
        /// <param name="minBytes">Minimum number of bytes to use when encoding. Used to further optimize output size.</param>
        public SignedOmega() {
            Underlying = new UnsignedOmega();
        }

        /// <summary>
        /// Encode VLQ using lambda expression to retrieve next byte.
        /// </summary>
        /// <param name="value">Signed value to be encoded.</param>
        /// <param name="write">Method to write byte. First parameter contains byte to be OR'd with the existing value. Second parameter indicate is a position move is required after the byte is writted.</param>
        public void Encode(long value, Func<byte> read, Action<byte> write, Action move, ref int offset) {
            Underlying.Encode(ZigZag.Encode(value), read, write, move, ref offset);
        }

        /// <summary>
        /// Encode number into an existing byte array (best performance).
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Buffer to output to.</param>
        /// <param name="position">Position to start reading in the input. Updated to the last position read after execution.</param>
        public void Encode(long value, byte[] output, ref int position, ref int offset) {
            Underlying.Encode(ZigZag.Encode(value), output, ref position, ref offset);
        }

        /// <summary>
        /// Encode a number into a stream.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <param name="output">Stream to output into.</param>
        public void Encode(long value, Stream output, ref int offset) {
            Underlying.Encode(ZigZag.Encode(value), output, ref offset);
        }

        /// <summary>
        /// Encode a number into a byte array (low performance).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>VLQ as byte array.</returns>
        public byte[] Encode(long value) {
            return Underlying.Encode(ZigZag.Encode(value));
        }

        /// <summary>
        /// Decode VLQ using lambda expression to retrieve next byte.
        /// </summary>
        /// <param name="read">Method to acquire next byte. If the parameter is TRUE, move the pointer to the next byte after the read.</param>
        /// <returns></returns>
        public long Decode(Func<bool, byte> read, ref int offset) {
            return ZigZag.Decode(Underlying.Decode(read, ref offset));
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array starting at a given position (best performance).
        /// </summary>
        /// <param name="input">Array to read from.</param>
        /// <param name="position">Position to start reading at. Updated with last read position after call.</param>
        /// <returns>Next VLQ.</returns>
        public long Decode(byte[] input, ref int position, ref int offset) {
            return ZigZag.Decode(Underlying.Decode(input, ref position, ref offset));
        }

        /// <summary>
        /// Decode the next VLQ from a given stream.
        /// </summary>
        /// <param name="input">Stream to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public long Decode(Stream input, ref int offset) {
            return ZigZag.Decode(Underlying.Decode(input, ref offset));
        }

        /// <summary>
        /// Decode the next VLQ from a given byte array.
        /// </summary>
        /// <param name="input">Byte array to read the VLQ from.</param>
        /// <returns>Next VLQ.</returns>
        public long Decode(byte[] input) {
            return ZigZag.Decode(Underlying.Decode(input));
        }
    }
}
