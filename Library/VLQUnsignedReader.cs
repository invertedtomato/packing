using InvertedTomato.IO;
using System;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    /// Reader for Variable-length Quantity (VLQ) unsigned numbers.
    /// </summary>
    public class VLQUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read first value from a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ulong ReadOneDefault(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new VLQUnsignedReader(stream)) {
                    return reader.Read();
                }
            }
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Number of bits to include in each packet.
        /// </summary>
        private readonly int PacketSize;

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
            PacketSize = packetSize;
        }

        /// <summary>
        /// Read the next value. 
        /// </summary>
        /// <returns></returns>
        public ulong Read() {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Setup offset
            var outputPosition = 0;

            // Set value to 0
            ulong value = 0;

            bool final;
            do {
                // Read if this is the final packet
                final = Input.Read(1) > 0;

                // Read payload
                var chunk = Input.Read(PacketSize);

                // Add payload to value
                value += chunk + 1 << outputPosition;

                // Update target offset
                outputPosition += PacketSize;
            } while (!final);

            // Remove zero offset
            value--;

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
                // Dispose managed state (managed objects)
                Input.DisposeIfNotNull();
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
