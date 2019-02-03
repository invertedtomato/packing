using System;
using System.IO;
using InvertedTomato.IO.Bits;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Reader for Elias Delta universal coding for unsigned values.
    /// </summary>
    public class EliasDeltaUnsignedReader : IUnsignedReader {
        /// <summary>
        ///     The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public EliasDeltaUnsignedReader(Stream input) {
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

			// #1 Read and count zeros from the stream until you reach the first one. Call this count of zeros L
			var l = 1;
			while (!Input.PeakBit()) {
				// Note that length is one bit longer
				l++;

				// Remove 0 from input
				Input.Read(1);
			}

			;

			// #2 Considering the one that was reached to be the first digit of an integer, with a value of 2L, read the remaining L digits of the integer. Call this integer N+1, and subtract one to get N.
			var n = (Int32) Input.Read(l) - 1;

			// #3 Put a one in the first place of our final output, representing the value 2N.
			// #4 Read and append the following N digits.
			var value = Input.Read(n) + ((UInt64) 1 << n);

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
				using (var reader = new EliasDeltaUnsignedReader(stream)) {
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