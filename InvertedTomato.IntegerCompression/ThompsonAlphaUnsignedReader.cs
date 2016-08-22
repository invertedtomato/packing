using InvertedTomato.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Writer for Thompson-Alpha for unsigned values.
    /// </summary>
    public class ThompsonAlphaUnsignedReader : IUnsignedReader {
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
                using (var reader = new ThompsonAlphaUnsignedReader(stream)) {
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

        /// <summary>
        /// The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        private readonly byte LengthBits;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public ThompsonAlphaUnsignedReader(Stream input) : this(input, 6) { }

        /// <summary>
        /// Instantiation with options
        /// </summary>
        /// <param name="input"></param>
        public ThompsonAlphaUnsignedReader(Stream input, int lengthBits) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (lengthBits < 1 || lengthBits > 6) {
                throw new ArgumentOutOfRangeException("Must be between 1 and 6, not " + lengthBits + ".", "lengthBits");
            }

            Input = new BitReader(input);
            LengthBits = (byte)lengthBits;
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

            value = 0;

            // Read length
            ulong length;
            if (!Input.TryRead(out length, LengthBits)) {
                return false;
            }

            // Read body
            if (!Input.TryRead(out value, (byte)length)) {
                return false;
            }

            // Recover implied MSB
            value |= (ulong)1 << (byte)length;

            // Remove offset to allow zeros
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
