using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Elias Omega universal coding adapted for signed values. Values are translated to unsigned values using ProtoBuffers ZigZag algorithm.
    /// 
    /// Per Elias standard, zeros are not normally supported. They can however be supported by an offset technique - pass TRUE in the constructor to allow this functionality.
    /// </summary>
    public class EliasOmegaSignedReader : ISignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<long> ReadAll(byte[] input) {
            return ReadAll(0, input);
        }

        /// <summary>
        /// Read all values in a byte array with options.
        /// </summary>
        /// <param name="minValue">Minimum value to support. To match standard use 1.</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<long> ReadAll(ulong minValue, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new EliasOmegaSignedReader(stream, minValue)) {
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
        private readonly EliasOmegaUnsignedReader Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public EliasOmegaSignedReader(Stream input) {
            Underlying = new EliasOmegaUnsignedReader(input);
        }

        /// <summary>
        /// Instantiate passing options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minValue">Minimum value to support. To match standard use 1.</param>
        public EliasOmegaSignedReader(Stream input, ulong minValue) {
            Underlying = new EliasOmegaUnsignedReader(input, minValue);
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
