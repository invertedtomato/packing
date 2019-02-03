using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InvertedTomato.Compression.Integers {
	public class FibonacciCodec : Codec {
        /// <summary>
        ///     The most significant bit in a byte.
        /// </summary>
        private const Byte MSB = 0x80;

        /// <summary>
        ///     Minimum value this codec can support.
        /// </summary>
        public static readonly UInt64 MinValue = UInt64.MinValue;

        /// <summary>
        ///     The maximum value of a symbol this codec can support.
        /// </summary>
        public static readonly UInt64 MaxValue = UInt64.MaxValue - 1;

        /// <summary>
        ///     Lookup table of Fibonacci numbers that can fit in a ulong.
        /// </summary>
        public static readonly UInt64[] Lookup = new UInt64[92];

		static FibonacciCodec() {
			// Pre-compute all Fibonacci numbers that can fit in a ulong.
			Lookup[0] = 1;
			Lookup[1] = 2;
			for (var i = 2; i < Lookup.Length; i++) {
				Lookup[i] = Lookup[i - 1] + Lookup[i - 2];
			}
		}


		public override Int32 CompressUnsigned(Stream output, params UInt64[] values) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			// Clear currently worked-on byte
			var current = new Byte();
			var offset = 0;

			// Iterate through all symbols
			var used = 0;
			foreach (var value in values) {
				var value2 = value;
#if DEBUG
				// Check for overflow
				if (value2 > MaxValue) {
					throw new OverflowException("Exceeded FibonacciCodec's maximum supported symbol value of " + MaxValue + ".");
				}
#endif

				// Fibonacci doesn't support 0s, so add 1 to allow for them
				value2++;

				// #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
				// #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
				Boolean[] map = null;
				for (var fibIdx = Lookup.Length - 1; fibIdx >= 0; fibIdx--) {
					// #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word(counting the left most digit as place 0).
					if (value2 >= Lookup[fibIdx]) {
						// Detect if this is the largest fib and store
						if (null == map) {
							map = new Boolean[fibIdx + 2];
							map[fibIdx + 1] = true; // Termination bit
						}

						// Write to map
						map[fibIdx] = true;

						// Deduct Fibonacci number from value
						value2 -= Lookup[fibIdx];
					}
				}

				// Output the bits of the map in reverse order
				foreach (var bit in map) {
					if (bit) {
						current |= (Byte) (1 << (7 - offset));
					}

					// Increment offset;
					if (++offset == 8) {
						// Add byte to output
						output.WriteByte(current);
						used++;
						current = 0;
						offset = 0;
					}
				}
			}

			// Flush bit buffer
			if (offset > 0) {
				output.WriteByte(current);
				used++;
			}

			return used;
		}

		public override IEnumerable<UInt64> DecompressUnsigned(Stream input, Int32 count) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			// Current symbol being decoded.
			UInt64 symbol = 0;

			// Next Fibonacci number to test.
			var nextFibIndex = 0;

			// State of the last bit while decoding.
			var lastBit = false;

			if (0 == count) {
				yield break;
			}

			while (true) {
				// Read byte of input, and throw error if unavailable
				var b = input.ReadByte();
				if (b < 0) {
					throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
				}

				// For each bit of buffer
				for (var bi = 0; bi < 8; bi++) {
					// If bit is set...
					if (((b << bi) & MSB) > 0) {
						// If double 1 bits
						if (lastBit) {
							// Remove zero offset
							symbol--;

							// Add to output
							yield return symbol;

							// Stop if expected number of symbols have been found
							if (--count == 0) {
								yield break;
							}

							// Reset for next symbol
							symbol = 0;
							nextFibIndex = 0;
							lastBit = false;
							continue;
						}

#if DEBUG
						// Check for overflow
						if (nextFibIndex >= Lookup.Length) {
							throw new OverflowException("Value too large to decode. Max 64bits supported."); // TODO: Handle this so that it doesn't allow for DoS attacks!
						}
#endif

						// Add value to current symbol
						var pre = symbol;
						symbol += Lookup[nextFibIndex];
#if DEBUG
						// Check for overflow
						if (symbol < pre) {
							throw new OverflowException("Input symbol larger than the supported limit of 64bits. Possible data issue.");
						}
#endif

						// Note bit for next cycle
						lastBit = true;
					} else {
						// Note bit for next cycle
						lastBit = false;
					}

					// Increment bit position
					nextFibIndex++;
				}
			}
		}


		public override async Task<Int32> CompressUnsignedAsync(Stream output, params UInt64[] values) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			// Clear currently worked-on byte
			var current = new Byte();
			var offset = 0;

			// Iterate through all symbols
			var used = 0;
			foreach (var value in values) {
				var value2 = value;
#if DEBUG
				// Check for overflow
				if (value2 > MaxValue) {
					throw new OverflowException("Exceeded FibonacciCodec's maximum supported symbol value of " + MaxValue + ".");
				}
#endif

				// Fibonacci doesn't support 0s, so add 1 to allow for them
				value2++;

				// #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
				// #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
				Boolean[] map = null;
				for (var fibIdx = Lookup.Length - 1; fibIdx >= 0; fibIdx--) {
					// #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word(counting the left most digit as place 0).
					if (value2 >= Lookup[fibIdx]) {
						// Detect if this is the largest fib and store
						if (null == map) {
							map = new Boolean[fibIdx + 2];
							map[fibIdx + 1] = true; // Termination bit
						}

						// Write to map
						map[fibIdx] = true;

						// Deduct Fibonacci number from value
						value2 -= Lookup[fibIdx];
					}
				}

				// Output the bits of the map in reverse order
				foreach (var bit in map) {
					if (bit) {
						current |= (Byte) (1 << (7 - offset));
					}

					// Increment offset;
					if (++offset == 8) {
						// Add byte to output
						await output.WriteAsync(new[] {current}, 0, 1);
						used++;
						current = 0;
						offset = 0;
					}
				}
			}

			// Flush bit buffer
			if (offset > 0) {
				await output.WriteAsync(new[] {current}, 0, 1);
				used++;
			}

			return used;
		}

		public override async Task<IEnumerable<UInt64>> DecompressUnsignedAsync(Stream input, Int32 count) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			// Output
			var output = new List<UInt64>();

			// Don't try to read if no symbols are wanted
			if (0 == count) {
				return output;
			}

			// Current symbol being decoded.
			UInt64 symbol = 0;

			// Next Fibonacci number to test.
			var nextFibIndex = 0;

			// State of the last bit while decoding.
			var lastBit = false;

			Byte b;
			var buffer = new Byte[1];


			while (true) {
				// Read byte of input, and throw error if unavailable
				if (await input.ReadAsync(buffer, 0, 1) != 1) {
					throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
				}

				b = buffer[0];

				// For each bit of buffer
				for (var bi = 0; bi < 8; bi++) {
					// If bit is set...
					if (((b << bi) & MSB) > 0) {
						// If double 1 bits
						if (lastBit) {
							// Remove zero offset
							symbol--;

							// Add to output
							output.Add(symbol);

							// Stop if expected number of symbols have been found
							if (--count == 0) {
								return output;
							}

							// Reset for next symbol
							symbol = 0;
							nextFibIndex = 0;
							lastBit = false;
							continue;
						}

#if DEBUG
						// Check for overflow
						if (nextFibIndex >= Lookup.Length) {
							throw new OverflowException("Value too large to decode. Max 64bits supported."); // TODO: Handle this so that it doesn't allow for DoS attacks!
						}
#endif

						// Add value to current symbol
						var pre = symbol;
						symbol += Lookup[nextFibIndex];
#if DEBUG
						// Check for overflow
						if (symbol < pre) {
							throw new OverflowException("Input symbol larger than the supported limit of 64bits. Possible data issue.");
						}
#endif

						// Note bit for next cycle
						lastBit = true;
					} else {
						// Note bit for next cycle
						lastBit = false;
					}

					// Increment bit position
					nextFibIndex++;
				}
			}
		}


		public override Int32 CalculateBitLength(UInt64 symbol) {
#if DEBUG
			// Check for overflow
			if (symbol > MaxValue) {
				throw new OverflowException("Exceeded FibonacciCodec's maximum supported symbol value of " + MaxValue + ".");
			}
#endif

			// Offset for zero
			symbol++;

			for (var i = Lookup.Length - 1; i >= 0; i--) {
				if (symbol >= Lookup[i]) {
					return i + 1;
				}
			}

			return 0;
		}
	}
}