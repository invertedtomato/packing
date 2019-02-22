using System;
using System.IO;
using InvertedTomato.IO.Bits;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers.Wave1 {
    /// <summary>
    ///     Reader for Elias Gamma universal coding for unsigned values.
    /// </summary>
    [Obsolete("Consider using InvertedTomato.Compression.Integers.Wave3.EliasGammaCodec instead. It's faster and easier.")]
    public class EliasGammaUnsignedReader : IUnsignedReader {
        /// <summary>
        ///     The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public EliasGammaUnsignedReader(Stream input) {
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

			// Read length
			var length = 1;
			while (!Input.PeakBit()) {
				// Note that length is one bit longer
				length++;

				// Remove 0 from input
				Input.Read(1);
			}

			;


			// Read value
			var value = Input.Read(length);

			// Remove offset from value
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
				using (var reader = new EliasGammaUnsignedReader(stream)) {
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