using System;
using System.IO;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Writer for Thompson-Alpha for unsigned values.
    /// </summary>
    public class ThompsonAlphaUnsignedReader : IUnsignedReader {
        /// <summary>
        ///     The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        ///     Number of prefix bits used to store length.
        /// </summary>
        private readonly Int32 LengthBits;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public ThompsonAlphaUnsignedReader(Stream input) : this(input, 6) { }

        /// <summary>
        ///     Instantiation with options
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lengthBits">Number of prefix bits used to store length.</param>
        public ThompsonAlphaUnsignedReader(Stream input, Int32 lengthBits) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (lengthBits < 1 || lengthBits > 6) {
				throw new ArgumentOutOfRangeException("Must be between 1 and 6, not " + lengthBits + ".", "lengthBits");
			}

			Input = new BitReader(input);
			LengthBits = lengthBits;
		}

        /// <summary>
        ///     If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Attempt to read the next value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>If a read was successful.</returns>
        public UInt64 Read() {
			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			// Read length
			var length = (Int32) Input.Read(LengthBits);

			// Read body
			var value = Input.Read(length);

			// Recover implied MSB
			value |= (UInt64) 1 << length;

			// Remove offset to allow zeros
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
				using (var reader = new ThompsonAlphaUnsignedReader(stream)) {
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