using System;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Reader for Thompson-Alpha for signed values.
    /// </summary>
    public class ThompsonAlphaSignedReader : ISignedReader {
        /// <summary>
        ///     The underlying unsigned reader.
        /// </summary>
        private readonly ThompsonAlphaUnsignedReader Underlying;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public ThompsonAlphaSignedReader(Stream input) {
			Underlying = new ThompsonAlphaUnsignedReader(input);
		}

        /// <summary>
        ///     Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        public ThompsonAlphaSignedReader(Stream input, Int32 lengthBits) {
			Underlying = new ThompsonAlphaUnsignedReader(input, lengthBits);
		}

        /// <summary>
        ///     If it's disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Read the next value.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No value was available.</exception>
        public Int64 Read() {
			return ZigZag.Decode(Underlying.Read());
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
        public static Int64 ReadOneDefault(Byte[] input) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			using (var stream = new MemoryStream(input)) {
				using (var reader = new ThompsonAlphaSignedReader(stream)) {
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

			Underlying.Dispose();

			if (disposing) {
				// Dispose managed state (managed objects)
				Underlying?.Dispose();
			}
		}
	}
}