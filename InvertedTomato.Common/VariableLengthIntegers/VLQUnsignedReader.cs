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
    public class VLQUnsignedReader : IUnsignedReader {
        public static IEnumerable<ulong> ReadAll(byte[] input) {
            return ReadAll(0, input);
        }
        public static IEnumerable<ulong> ReadAll(int minBytes, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new VLQUnsignedReader(stream, minBytes)) {
                    ulong value;
                    while (reader.TryRead(out value)) {
                        yield return value;
                    }
                }
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

        private readonly Stream Input;

        /// <summary>
        /// The current byte being worked with.
        /// </summary>
        private int CurrentByte;

        public VLQUnsignedReader(Stream input) : this(input, 1) { }

        public VLQUnsignedReader(Stream input, int minBytes = 1) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (minBytes < 1 || minBytes > 8) {
                throw new ArgumentOutOfRangeException("Must be between 1 and 8.", "minBytes");
            }

            // Store
            Input = input;
            PrefixBytes = minBytes - 1;
        }

        public bool TryRead(out ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Setup offset
            var outputPosition = 0;

            // Set value to 0
            value = 0;
            
            // Read any full bytes per min-bytes requirements
            for (var i = 0; i < PrefixBytes; i++) {
                // Read next byte
                if (!ReadByte()) {
                    if (i > 0) {
                        throw new InvalidOperationException("Missing initial byte (" + i + ").");
                    }
                    return false;
                }

                // Add bits to value
                value += (ulong)CurrentByte << outputPosition;
                outputPosition += 8;
            }

            // Read next byte
            if (!ReadByte()) {
                if (PrefixBytes > 0) {
                    throw new InvalidOperationException("Missing initial body byte.");
                }
                return false;
            }

            // Add bits to value
            value += (ulong)((CurrentByte & PAYLOAD_MASK)) << outputPosition;

            while ((CurrentByte & CONTINUITY_MASK) == 0) {
                // Update target offset
                outputPosition += 7;

                // Read next byte
                if (!ReadByte()) {
                    throw new InvalidOperationException("Missing body byte.");
                }

                // Add bits to value
                value += (ulong)((CurrentByte & PAYLOAD_MASK) + 1) << outputPosition;
            }

            return true;
        }

        public ulong Read() {
            ulong value;
            if (!TryRead(out value)) {
                throw new EndOfStreamException();
            }
            return value;
        }

        private bool ReadByte() {
            // Get next byte
            CurrentByte = Input.ReadByte();
            if (CurrentByte < 0) {
                return false;
            }

            return true;
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
