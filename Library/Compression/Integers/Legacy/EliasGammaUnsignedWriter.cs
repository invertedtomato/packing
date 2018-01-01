using InvertedTomato.IO;
using InvertedTomato.IO.Bits;
using System;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    /// Reader for Elias Gamma universal coding for unsigned values.
    /// </summary>
    
    public class EliasGammaUnsignedWriter : IUnsignedWriter, IDisposable {
        /// <summary>
        /// Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Byte[] WriteOneDefault(UInt64 value) {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasGammaUnsignedWriter(stream)) {
                    writer.Write(value);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32? CalculateBitLength(UInt64 value) {
            // Offset for zero
            value++;
            
            return BitOperation.CountUsed(value) * 2 - 1;
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        /// Underlying stream to be writing encoded values to.
        /// </summary>
        private readonly BitWriter Output;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public EliasGammaUnsignedWriter(Stream output) {
            if (null == output) {
                throw new ArgumentNullException(nameof(output));
            }

            Output = new BitWriter(output);
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(UInt64 value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Offset value to allow zeros
            value++;

            // Calculate length
            var length = BitOperation.CountUsed(value);

            // Write unary zeros
            Output.Write(0, length - 1);

            // Write value
            Output.Write(value, length);
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                Output?.Dispose();
            }
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose() {
            Dispose(true);
        }
    }
}
