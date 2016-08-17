using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Writer for Variable-length Quantity unsigned numbers.
    /// 
    /// Since VLQ isn't strictly a standard, this is the most optimal of the variants. Integers are encoded into 7 bits per byte, with the most significant bit
    /// (the 'continuity' bit) indicating if the byte is the last byte in the sequence.
    /// 
    /// Also uses the redundancy removal technique described at https://hbfs.wordpress.com/2014/02/18/universal-coding-part-iii/ .
    /// 
    /// Examples:
    ///   0     encodes to 1000 0000
    ///   1     encodes to 1000 0001
    ///   127   encodes to 1111 1111
    ///   128   encodes to 0000 0000  1000 0000
    ///   16511 encodes to 0111 1111  1111 1111
    ///   16512 encodes to 0000 0000  0000 0000  1000 0000
    /// 
    /// Furthermore, this include an optimization where the minimum number of bytes (minBytes) can be specified. This means that minBytes-1 bytes use 8-bits per
    /// byte rather than 7-bits. This can be used to further increase efficiency when it is knows that the values are going to be consistently large.
    /// 
    ///  e.g.  For values consistently over 127, use minBytes=2
    ///  
    /// For further information on VLQs, see the Wiki page at https://en.wikipedia.org/wiki/Variable-length_quantity .
    /// </summary>
    public class VLQUnsignedWriter : IUnsignedWriter {
        /// <summary>
        /// Write all given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(IEnumerable<ulong> values) { return WriteAll(1, values); }

        /// <summary>
        /// Write all given values with options.
        /// </summary>
        /// <param name="minBytes">(non-standard) The minimum number of bytes to use when encoding. Increases efficiency when encoding consistently large.</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(int minBytes, IEnumerable<ulong> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new VLQUnsignedWriter(stream, minBytes)) {
                    foreach (var value in values) {
                        writer.Write(value);
                    }
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="minBytes">(non-standard) The minimum number of bytes to use when encoding. Increases efficiency when encoding consistently large</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CalculateBitLength(int minBytes, ulong value) {
            var result = 8; // Final byte

            // Add any full bytes to start to fulfill min-bytes requirements
            for (var i = 0; i < minBytes - 1; i++) {
                result += 8;
                value >>= 8;
            }


            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > PAYLOAD_MASK) {
                result += 8;
                value >>= 7;
                value--;
            }

            return result;
        }

        /// <summary>
        /// Mask to extract the data from a byte.
        /// </summary>
        const int PAYLOAD_MASK = 0x7F; // 0111 1111  - this is an int32 to save later casting

        /// <summary>
        /// Mask to extract the 'continuity' bit from a byte.
        /// </summary>
        const int CONTINUITY_MASK = 0x80; // 1000 0000  - this is an int32 to save later casting

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The number of full 8-bit bytes at the start of each value. Derived from MinBytes.
        /// </summary>
        private readonly int PrefixBytes;

        /// <summary>
        /// The stream to output encoded bytes to.
        /// </summary>
        private readonly Stream Output;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public VLQUnsignedWriter(Stream output) : this(output, 1) { }

        /// <summary>
        /// Instantiate with options
        /// </summary>
        /// <param name="output"></param>
        /// <param name="minBytes">(non-standard) The minimum number of bytes to use when encoding. Increases efficiency when encoding consistently large.</param>
        public VLQUnsignedWriter(Stream output, int minBytes) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }
            if (minBytes < 1 || minBytes > 8) {
                throw new ArgumentOutOfRangeException("Must be between 1 and 8.", "minBytes");
            }

            // Store
            Output = output;
            PrefixBytes = minBytes - 1;
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Add any full bytes to start to fulfill min-bytes requirements
            for (var i = 0; i < PrefixBytes; i++) {
                Output.WriteByte((byte)value);
                value >>= 8;
            }


            // Iterate through input, taking 7 bits of data each time, aborting when less than 7 bits left
            while (value > PAYLOAD_MASK) {
                Output.WriteByte((byte)(value & PAYLOAD_MASK));
                value >>= 7;
                value--;
            }

            // Output remaining bits, with the 'final' bit set
            Output.WriteByte((byte)(value | CONTINUITY_MASK));
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects).
            }
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose() {
            Dispose(true);
        }
    }
}
