using System;

#pragma warning disable 612

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Asynchronous Fibonacci coding reader (experimental).
    /// </summary>
    public class ASyncFibonacciUnsignedReader {
		private const Byte MSB_8BIT = 0x80;
		private readonly Func<UInt64, Boolean> Output;
		private UInt64 Buffer;
		private Int32 BufferPosition;
		private Boolean LastBit;

		public ASyncFibonacciUnsignedReader(Func<UInt64, Boolean> output) {
			if (null == output) {
				throw new ArgumentNullException("callback");
			}

			// Store
			Output = output;
		}

		public void Insert(Byte input) {
			// For each bit of buffer
			for (var inputPosition = 0; inputPosition < 8; inputPosition++) {
				// If bit is set...
				if (((input << inputPosition) & MSB_8BIT) > 0) {
					// If double 1 bits
					if (LastBit) {
						// Copy buffer with zero offset
						var value = Buffer - 1;

						// Reset for next value
						Buffer = 0;
						BufferPosition = 0;
						LastBit = false;

						// Output value - this must be after the reset to prevent thread issues
						if (!Output(value)) {
							return;
						}

						continue;
					}

					// Add value to buffer
					Buffer += FibonacciCodec.Lookup[BufferPosition];

					// Note bit for next cycle
					LastBit = true;
				} else {
					LastBit = false;
				}

				// Increment bit position
				BufferPosition++;
			}
		}
	}
}