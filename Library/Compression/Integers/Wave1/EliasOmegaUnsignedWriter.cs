using System;
using System.Collections.Generic;
using System.IO;
using InvertedTomato.IO.Bits;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers.Wave1 {
    /// <summary>
    ///     Reader for Elias Omega universal coding for unsigned values.
    /// </summary>
    [Obsolete("Consider using InvertedTomato.Compression.Integers.Wave3.EliasOmegaCodec instead. It's faster and easier.")]
    public class EliasOmegaUnsignedWriter : IUnsignedWriter {
        /// <summary>
        ///     Underlying stream to be writing encoded values to.
        /// </summary>
        private readonly BitWriter Output;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public EliasOmegaUnsignedWriter(Stream output) {
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			Output = new BitWriter(output);
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

			// Offset min value
			value++;

			// Prepare buffer
			var groups = new Stack<KeyValuePair<UInt64, Int32>>();

			// #1 Place a "0" at the end of the code.
			groups.Push(new KeyValuePair<UInt64, Int32>(0, 1));

			// #2 If N=1, stop; encoding is complete.
			while (value > 1) {
				// Calculate the length of value
				var length = BitOperation.CountUsed(value);

				// #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
				groups.Push(new KeyValuePair<UInt64, Int32>(value, length));

				// #4 Let N equal the number of bits just prepended, minus one.
				value = (UInt64) length - 1;
			}

			// Write buffer
			foreach (var item in groups) {
				var bits = item.Value;
				var group = item.Key;

				Output.Write(group, bits);
			}
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
				using (var writer = new EliasOmegaUnsignedWriter(stream)) {
					writer.Write(value);
				}

				return stream.ToArray();
			}
		}

        /// <summary>
        ///     Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32? CalculateBitLength(UInt64 value) {
			var result = 1; // Termination bit

			// Offset value to allow for 0s
			value++;

			// #2 If N=1, stop; encoding is complete.
			while (value > 1) {
				// Calculate the length of value
				var length = BitOperation.CountUsed(value);

				// #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
				result += length;

				// #4 Let N equal the number of bits just prepended, minus one.
				value = (UInt64) length - 1;
			}

			return result;
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