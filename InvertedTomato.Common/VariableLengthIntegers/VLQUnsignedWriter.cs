using System;
using System.Collections.Generic;
using System.IO;

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
    public class VLQUnsignedWriter : IUnsignedWriter {
        public static byte[] WriteAll(IEnumerable<ulong> values) { return WriteAll(1, values); }

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
        /// Mask to extract the data from a byte
        /// </summary>
        const int PAYLOAD_MASK = 0x7F; // 0111 1111  - this is an int32 to save later casting

        /// <summary>
        /// Mask to extract the 'final' bit from a byte.
        /// </summary>
        const int CONTINUITY_MASK = 0x80; // 1000 0000  - this is an int32 to save later casting


        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Minimum length (in bytes) of the output of each encoded number.
        /// </summary>
        private readonly int PrefixBytes;

        private readonly Stream Output;

        public VLQUnsignedWriter(Stream output) : this(output, 1) { }

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

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects).
            }
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
