using System;
using System.IO;

#pragma warning disable 612
#pragma warning disable 618

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Writer for Fibonacci for signed values.
    /// </summary>
    public class FibonacciSignedWriter : ISignedWriter {
        /// <summary>
        ///     The underlying unsigned writer.
        /// </summary>
        private readonly FibonacciUnsignedWriter Underlying;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public FibonacciSignedWriter(Stream output) {
			Underlying = new FibonacciUnsignedWriter(output);
		}

        /// <summary>
        ///     If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(Int64 value) {
			Underlying.Write(ZigZag.Encode(value));
		}

        /// <summary>
        ///     Flush any unwritten bits and dispose.
        /// </summary>
        public void Dispose() {
			Dispose(true);
		}

        /// <summary>
        ///     Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Byte[] WriteOneDefault(Int64 value) {
			using (var stream = new MemoryStream()) {
				using (var writer = new FibonacciSignedWriter(stream)) {
					writer.Write(value);
				}

				return stream.ToArray();
			}
		}

        /// <summary>
        ///     Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing) {
			if (IsDisposed) {
				return;
			}

			IsDisposed = true;

			Underlying.Dispose();

			if (disposing) {
				// Dispose managed state (managed objects)
				Underlying?.Dispose();
			}
		}
	}
}