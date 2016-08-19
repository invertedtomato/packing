using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Writer for dynamic length signed integers.
    /// </summary>
    public class DynamicSignedWriter : ISignedWriter {
        /// <summary>
        /// Write all given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(IEnumerable<long> values) { return WriteAll(ulong.MaxValue, values); }

        /// <summary>
        /// Write all given values with options.
        /// </summary>
        /// <param name="maxValue">The maximum supported value. To match standard use ulong.MaxValue.</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(ulong maxValue, IEnumerable<long> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new DynamicSignedWriter(stream, maxValue)) {
                    foreach (var value in values) {
                        writer.Write(value);
                    }
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Underlying unsigned writer.
        /// </summary>
        private readonly DynamicUnsignedWriter Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public DynamicSignedWriter(Stream output) {
            Underlying = new DynamicUnsignedWriter(output);
        }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxValue">The maximum supported value. To match standard use ulong.MaxValue.</param>
        public DynamicSignedWriter(Stream input, ulong maxValue) {
            Underlying = new DynamicUnsignedWriter(input, maxValue);
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value) {
            var innerValue = ZigZag.Encode(value);
            Underlying.Write(innerValue);
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

            Underlying.Dispose();

            if (disposing) {
                // Dispose managed state (managed objects).
            }
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
    }
}
