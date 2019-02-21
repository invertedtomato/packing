using System;
using System.IO;
using InvertedTomato.Compression.Integers.Wave2;
using InvertedTomato.IO.Bits;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers.Wave1 {
    /// <summary>
    ///     Writer for Fibonacci for unsigned values.
    /// </summary>
    [Obsolete("Consider using FibonacciCodec instead. It's faster and easier.")]
	public class FibonacciUnsignedReader : IUnsignedReader {
        /// <summary>
        ///     The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public FibonacciUnsignedReader(Stream input) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			Input = new BitReader(input);
		}

        /// <summary>
        ///     If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Read the next value.
        /// </summary>
        /// <returns></returns>
        public UInt64 Read() {
			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			// Set default value
			UInt64 value = 0;

			var lastBit = false;
			var fibIdx = 0;
			do {
				if (Input.Read(1) > 0) {
					if (lastBit) {
						break;
					}

					value += FibonacciCodec.Lookup[fibIdx];
					lastBit = true;
				} else {
					lastBit = false;
				}

				fibIdx++;
#if DEBUG
				if (fibIdx == FibonacciCodec.Lookup.Length - 1) {
					throw new OverflowException("Value too long to decode.");
				}
#endif
			} while (true);

			// Remove zero offset
			value--;

			return value;
		}

        /// <summary>
        ///     Dispose.
        /// </summary>
        public void Dispose() {
			Dispose(true);
		}

        /// <summary>
        ///     Read first value from a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static UInt64 ReadOneDefault(Byte[] input) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			using (var stream = new MemoryStream(input)) {
				using (var reader = new FibonacciUnsignedReader(stream)) {
					return reader.Read();
				}
			}
		}

        /// <summary>
        ///     Dispose.
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
	}
}