using System;
using System.IO;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers.Wave1 {
    /// <summary>
    ///     Reader for Elias Omega universal coding adapted for signed values.
    /// </summary>
    public class EliasOmegaSignedReader : ISignedReader {
        /// <summary>
        ///     The underlying unsigned reader.
        /// </summary>
        private readonly EliasOmegaUnsignedReader Underlying;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public EliasOmegaSignedReader(Stream input) {
			Underlying = new EliasOmegaUnsignedReader(input);
		}

        /// <summary>
        ///     If it's disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Read the next value.
        /// </summary>
        /// <returns></returns>
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
				using (var reader = new EliasOmegaSignedReader(stream)) {
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