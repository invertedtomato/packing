using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for VLQ signed numbers. Values are translated to unsigned values using ProtoBuffer's ZigZag algorithm.
    /// </summary>
    public class VLQSignedReader : ISignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<long> ReadAll(byte[] input) {
            return ReadAll(1, input);
        }

        /// <summary>
        /// Read all values into a byte array with options.
        /// </summary>
        /// <param name="expectedMinValue">The expected minimum value to optimize encoded values for. To match standard use 0.</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<long> ReadAll(ulong expectedMinValue, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new VLQSignedReader(stream, expectedMinValue)) {
                    long value;
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

        /// <summary>
        /// Underlying unsigned reader.
        /// </summary>
        private readonly VLQUnsignedReader Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public VLQSignedReader(Stream input) {
            Underlying = new VLQUnsignedReader(input);
        }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expectedMinValue">The expected minimum value to optimize encoded values for. To match standard use 0.</param>
        public VLQSignedReader(Stream input, ulong expectedMinValue) {
            Underlying = new VLQUnsignedReader(input, expectedMinValue);
        }

        /// <summary>
        /// Attempt to read the next value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>If a read was successful.</returns>
        public bool TryRead(out long value) {
            ulong innerValue;
            var success = Underlying.TryRead(out innerValue);
            value = ZigZag.Decode(innerValue);
            return success;
        }

        /// <summary>
        /// Read the next value. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No value was available.</exception>
        public long Read() {
            long value;
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

            Underlying.Dispose();

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