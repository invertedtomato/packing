using System;
using System.IO;
using InvertedTomato.IO.Bits;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Reader for Fibonacci for unsigned values.
    /// </summary>
    [Obsolete("Consider using FibonacciCodec instead. It's faster and easier.")]
	public class FibonacciUnsignedWriter : IUnsignedWriter, IDisposable {
        /// <summary>
        ///     Underlying stream to be writing encoded values to.
        /// </summary>
        private readonly BitWriter Output;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public FibonacciUnsignedWriter(Stream output) {
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			// Store
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

			// Offset for zero
			value++;

			// #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
			// #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
			var buffer = new UInt64[2];
			var maxFibIdx = 0;
			for (var fibIdx = FibonacciCodec.Lookup.Length - 1; fibIdx >= 0; fibIdx--) {
				// #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word(counting the left most digit as place 0).
				if (value >= FibonacciCodec.Lookup[fibIdx]) {
					// Detect if this is the largest fib and store
					if (maxFibIdx == 0) {
						maxFibIdx = fibIdx;
					}

					// Write to buffer
					if (maxFibIdx < 64) {
						buffer[0] |= (UInt64) 1 << (maxFibIdx - fibIdx);
					} else if (fibIdx >= 64) {
						buffer[1] |= (UInt64) 1 << (maxFibIdx - fibIdx);
					} else {
						buffer[0] |= (UInt64) 1 << (63 - fibIdx);
					}

					// Deduct Fibonacci number from value
					value -= FibonacciCodec.Lookup[fibIdx];
				}
			}

			// Write to output
			if (maxFibIdx >= 64) {
				Output.Write(buffer[0], 64);
				Output.Write(buffer[1], maxFibIdx - 64 + 1);
			} else {
				Output.Write(buffer[0], maxFibIdx + 1);
			}

			// #4 Place an additional 1 after the rightmost digit in the code word.
			Output.Write(1, 1);
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
				using (var writer = new FibonacciUnsignedWriter(stream)) {
					writer.Write(value);
				}

				return stream.ToArray();
			}
		}

        /// <summary>
        ///     Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32? CalculateBitLength(UInt64 value) {
			// Offset for zero
			value++;

			for (var i = FibonacciCodec.Lookup.Length - 1; i >= 0; i--) {
				if (value >= FibonacciCodec.Lookup[i]) {
					return i + 1;
				}
			}

			return null;
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