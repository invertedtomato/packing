using InvertedTomato.IO;
using System;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Writer for Thompson-Alpha for unsigned values.
    /// </summary>
    public class ThompsonAlphaUnsignedReader : IUnsignedReader {
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
                using (var reader = new ThompsonAlphaUnsignedReader(stream)) {
                    return reader.Read();
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

        /// <summary>
        /// Number of prefix bits used to store length.
        /// </summary>
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
        /// <param name="lengthBits">Number of prefix bits used to store length.</param>
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
        public ulong Read() {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Read length
            var length = (byte)Input.Read(LengthBits);
                        
            // Read body
            var value = Input.Read(length);

            // Recover implied MSB
            value |= (ulong)1 << length;

            // Remove offset to allow zeros
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
