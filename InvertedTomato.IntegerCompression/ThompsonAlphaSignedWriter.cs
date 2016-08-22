using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Writer for Thompson-Alpha for signed values.
    /// </summary>
    public class ThompsonAlphaSignedWriter : ISignedWriter {
        /// <summary>
        /// Write all given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(IEnumerable<long> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new ThompsonAlphaSignedWriter(stream)) {
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
        /// The underlying unsigned writer.
        /// </summary>
        private readonly ThompsonAlphaUnsignedWriter Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public ThompsonAlphaSignedWriter(Stream output) {
            Underlying = new ThompsonAlphaUnsignedWriter(output);
        }

        /// <summary>
        /// Instantiate with options
        /// </summary>
        /// <param name="output"></param>
        public ThompsonAlphaSignedWriter(Stream output, int lengthBits) {
            Underlying = new ThompsonAlphaUnsignedWriter(output, lengthBits);
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
