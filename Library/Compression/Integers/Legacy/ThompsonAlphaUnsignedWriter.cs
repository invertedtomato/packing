using System;
using System.IO;
using InvertedTomato.IO.Bits;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Reader for Thompson-Alpha for unsigned values.
    /// </summary>
    public class ThompsonAlphaUnsignedWriter : IUnsignedWriter, IDisposable {
        /// <summary>
        ///     Number of prefix bits used to store length.
        /// </summary>
        private readonly Int32 LengthBits;

        /// <summary>
        ///     Underlying stream to be writing encoded values to.
        /// </summary>
        private readonly BitWriter Output;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public ThompsonAlphaUnsignedWriter(Stream output) : this(output, 6) { }

        /// <summary>
        ///     Instantiate with options.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="lengthBits">Number of prefix bits used to store length.</param>
        public ThompsonAlphaUnsignedWriter(Stream output, Int32 lengthBits) {
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (lengthBits < 1 || lengthBits > 6) {
				throw new ArgumentOutOfRangeException("Must be between 1 and 6, not " + lengthBits + ".", "lengthBits");
			}

			Output = new BitWriter(output);
			LengthBits = lengthBits;
		}

        /// <summary>
        ///     If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(UInt64 value) {
			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			// Offset value to allow zeros
			value++;

			// Count length
			var length = BitOperation.CountUsed(value);

			// Check not too large
			if (length > (LengthBits + 2) * 8) {
				throw new ArgumentOutOfRangeException("Value is greater than maximum of " + (UInt64.MaxValue >> (64 - LengthBits - 1)) + ". Increase length bits to support larger numbers.");
			}

			// Clip MSB, it's redundant
			length--;

			// Write length
			Output.Write((UInt64) length, LengthBits);

			// Write number
			Output.Write(value, length);
		}

        /// <summary>
        ///     Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose() {
			Dispose(true);
		}

        /// <summary>
        ///     Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Byte[] WriteOneDefault(UInt64 value) {
			using (var stream = new MemoryStream()) {
				using (var writer = new ThompsonAlphaUnsignedWriter(stream)) {
					writer.Write(value);
				}

				return stream.ToArray();
			}
		}

        /// <summary>
        ///     Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="lengthBits">Number of prefix bits used to store length.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32? CalculateBitLength(Int32 lengthBits, UInt64 value) {
			// Offset to allow for zero
			value++;

			// Calculate length
			var length = BitOperation.CountUsed(value);

			// Remove implied MSB
			length--;

			// Abort if it doesn't fit
			if (BitOperation.CountUsed((UInt64) length) > lengthBits) {
				return null;
			}

			// Return size
			return lengthBits + length;
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

			if (disposing) {
				// Dispose managed state (managed objects)
				Output?.Dispose();
			}
		}
	}
}