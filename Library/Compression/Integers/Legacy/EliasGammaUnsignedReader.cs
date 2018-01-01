using InvertedTomato.IO;
using InvertedTomato.IO.Bits;
using System;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    /// Reader for Elias Gamma universal coding for unsigned values.
    /// </summary>
    
    public class EliasGammaUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read first value from a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static UInt64 ReadOneDefault(Byte[] input) {
            if (null == input) {
                throw new ArgumentNullException(nameof(input));
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new EliasGammaUnsignedReader(stream)) {
                    return reader.Read();
                }
            }
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        /// The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public EliasGammaUnsignedReader(Stream input) {
            if (null == input) {
                throw new ArgumentNullException(nameof(input));
            }

            Input = new BitReader(input);
        }

        /// <summary>
        /// Read the next value. 
        /// </summary>
        /// <returns></returns>
        public UInt64 Read() {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Read length
            Int32 length = 1;
            while (!Input.PeakBit()) {
                // Note that length is one bit longer
                length++;

                // Remove 0 from input
                Input.Read(1);
            };


            // Read value
            var value = Input.Read(length);

            // Remove offset from value
            value--;

            return value;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                Input?.Dispose();
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
