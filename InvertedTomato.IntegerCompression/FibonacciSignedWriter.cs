using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Writer for Fibonacci for signed values.
    /// </summary>
    public class FibonacciSignedWriter : ISignedWriter {
        /// <summary>
        /// Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteOneDefault(long value) {
            using (var stream = new MemoryStream()) {
                using (var writer = new FibonacciSignedWriter(stream)) {
                    writer.Write(value);
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
        private readonly FibonacciUnsignedWriter Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public FibonacciSignedWriter(Stream output) {
            Underlying = new FibonacciUnsignedWriter(output);
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value) {
            Underlying.Write(ZigZag.Encode(value));
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
