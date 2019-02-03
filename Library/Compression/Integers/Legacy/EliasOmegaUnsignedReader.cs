using System;
using System.IO;
using InvertedTomato.IO.Bits;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Reader for Elias Omega universal coding for unsigned values.
    /// </summary>
    public class EliasOmegaUnsignedReader : IUnsignedReader {
        /// <summary>
        ///     The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public EliasOmegaUnsignedReader(Stream input) {
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

			// #1 Start with a variable N, set to a value of 1.
			UInt64 value = 1;

			// #2 If the next bit is a "0", stop. The decoded number is N.
			while (Input.PeakBit()) {
				// #3 If the next bit is a "1", then read it plus N more bits, and use that binary number as the new value of N.
				value = Input.Read((Int32) value + 1);
			}

			// Burn last bit from input
			Input.Read(1);

			// Offset for min value
			value = value - 1;

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
				using (var reader = new EliasOmegaUnsignedReader(stream)) {
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