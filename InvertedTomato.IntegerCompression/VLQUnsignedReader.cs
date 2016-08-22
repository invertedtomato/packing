using InvertedTomato.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Variable-length Quantity (VLQ) unsigned numbers.
    /// </summary>
    public class VLQUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new VLQUnsignedReader(stream)) {
                    ulong value;
                    while (reader.TryRead(out value)) {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private readonly byte PacketSize;

        /// <summary>
        /// The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public VLQUnsignedReader(Stream input) : this(input, 7) { }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="packetSize">The number of bits to include in each packet.</param>
        public VLQUnsignedReader(Stream input, int packetSize) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (packetSize < 1 || packetSize > 32) {
                throw new ArgumentOutOfRangeException("PacketSize must be 1<=x<=32 not " + packetSize + ".", "packetSize");
            }

            // Store
            Input = new BitReader(input);
            PacketSize = (byte)packetSize;
        }

        /// <summary>
        /// Attempt to read the next value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>If a read was successful.</returns>
        public bool TryRead(out ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Setup offset
            var outputPosition = 0;

            // Set value to 0
            value = 0;

            ulong continuity;
            do {
                // Read if this is the final packet
                if (!Input.TryRead(out continuity, 1)) {
                    return false;
                }

                // Read payload
                ulong chunk;
                if (!Input.TryRead(out chunk, PacketSize)) {
                    throw new InvalidOperationException("Missing some/all payload bits.");
                }

                // Add payload to value
                value += chunk  + 1 << outputPosition;
                
                // Update target offset
                outputPosition += PacketSize;
            } while (continuity == 0);

            value--;
            return true;
        }

        /// <summary>
        /// Read the next value. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No value was available.</exception>
        public ulong Read() {
            ulong value;
            if (!TryRead(out value)) {
                throw new EndOfStreamException();
            }
            return value;
        }

        /// <summary>
        /// Dispose.
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
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
    }
}
