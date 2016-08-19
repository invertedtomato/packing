using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Writer for dynamic length unsigned integers.
    /// </summary>
    public class DynamicUnsignedWriter : IUnsignedWriter {
        /// <summary>
        /// Write all given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(IEnumerable<ulong> values) { return WriteAll(0, values); }

        /// <summary>
        /// Write all given values with options.
        /// </summary>
        /// <param name="maxValue">The maximum supported value. To match standard use ulong.MaxValue.</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(ulong maxValue, IEnumerable<ulong> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new DynamicUnsignedWriter(stream, maxValue)) {
                    foreach (var value in values) {
                        writer.Write(value);
                    }
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="maxValue">The maximum supported value. To match standard use ulong.MaxValue.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CalculateBitLength(ulong maxValue, ulong value) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The number of full 8-bit bytes at the start of each value. Derived from MinBytes.
        /// </summary>
        private readonly int PrefixBytes;

        /// <summary>
        /// The stream to output encoded bytes to.
        /// </summary>
        private readonly Stream Output;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public DynamicUnsignedWriter(Stream output) : this(output, ulong.MaxValue) { }

        /// <summary>
        /// Instantiate with options
        /// </summary>
        /// <param name="output"></param>
        /// <param name="maxValue">The maximum supported value. To match standard use ulong.MaxValue.</param>
        public DynamicUnsignedWriter(Stream output, ulong maxValue) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            // Store
            Output = output;

            // TODO: calculate number of length bits
            throw new NotImplementedException();
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
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
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose() {
            Dispose(true);
        }
    }
}
