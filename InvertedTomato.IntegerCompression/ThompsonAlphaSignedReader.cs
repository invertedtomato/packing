using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Thompson-Alpha for signed values.
    /// </summary>
    public class ThompsonAlphaSignedReader : ISignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<long> ReadAll(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new ThompsonAlphaSignedReader(stream)) {
                    long value;
                    while (reader.TryRead(out value)) {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// If it's disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The underlying unsigned reader.
        /// </summary>
        private readonly ThompsonAlphaUnsignedReader Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public ThompsonAlphaSignedReader(Stream input) {
            Underlying = new ThompsonAlphaUnsignedReader(input);
        }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        public ThompsonAlphaSignedReader(Stream input, int lengthBits) {
            Underlying = new ThompsonAlphaUnsignedReader(input, lengthBits);
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
