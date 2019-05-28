using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InvertedTomato.Compression.Integers {
	public class FibonacciCodec : Codec {
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
		public static readonly UInt64[] Lookup = new UInt64[] {1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657, 46368, 75025, 121393, 196418, 317811, 514229, 832040, 1346269, 2178309, 3524578, 5702887, 9227465, 14930352, 24157817, 39088169, 63245986, 102334155, 165580141, 267914296, 433494437, 701408733, 1134903170, 1836311903, 2971215073, 4807526976, 7778742049, 12586269025, 20365011074, 32951280099, 53316291173, 86267571272, 139583862445, 225851433717, 365435296162, 591286729879, 956722026041, 1548008755920, 2504730781961, 4052739537881, 6557470319842, 10610209857723, 17167680177565, 27777890035288, 44945570212853, 72723460248141, 117669030460994, 190392490709135, 308061521170129, 498454011879264, 806515533049393, 1304969544928657, 2111485077978050, 3416454622906707, 5527939700884757, 8944394323791464, 14472334024676221, 23416728348467685, 37889062373143906, 61305790721611591, 99194853094755497, 160500643816367088, 259695496911122585, 420196140727489673, 679891637638612258, 1100087778366101931, 1779979416004714189, 2880067194370816120, 4660046610375530309, 7540113804746346429, 12200160415121876738};

		/// <summary>
		///     The most significant bit in a byte.
		/// </summary>
		private const Byte MSB = 0x80;

		private const Int32 MAX_ENCODED_LENGTH = 11;

		public override void EncodeMany(IByteWriter stream, UInt64[] values, Int32 offset, Int32 count) {
#if DEBUG
			if (null == stream) {
				throw new ArgumentNullException(nameof(stream));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}

			if (offset < 0 || offset > values.Length) {
				throw new ArgumentOutOfRangeException(nameof(offset));
			}

			if (count < 0 || offset + count > values.Length) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			// Allocate working buffer the max length of an encoded UInt64 + 1 byte
			var buffer = new Byte[MAX_ENCODED_LENGTH + 1];
			var maxByte = 0;
			var bitOffset = 0;

			// Iterate through all symbols
			for (var valueIdx = offset; valueIdx < offset + count; valueIdx++) {
				var value = values[valueIdx];
				var residualBits = 0;

#if DEBUG
				// Check for overflow
				if (value > MaxValue) {
					// TODO: Write manually calculated value
					throw new OverflowException("Exceeded FibonacciCodec maximum supported symbol value of " + MaxValue + ".");
				}
#endif

				// Reset size for next symbol
				maxByte = -1;

				// Fibonacci doesn't support 0s, so add 1 to allow for them
				value++;

				// #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
				// #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
				for (var fibIdx = Lookup.Length - 1; fibIdx >= 0; fibIdx--) {
					// #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word (counting the left most digit as place 0).
					if (value >= Lookup[fibIdx]) {
						// Calculate offsets
						var adjustedIdx = fibIdx + bitOffset;
						var byteIdx = adjustedIdx / 8;
						var bitIdx = adjustedIdx % 8;

						// Flag that fib is used
						buffer[byteIdx] |= (Byte) (0x01 << 7 - bitIdx); // Flag bit

						// If this is the termination fib, add termination bit
						if (-1 == maxByte) {
							maxByte = (adjustedIdx + 1) / 8;
							residualBits = (adjustedIdx + 2) % 8; // Add two bits being written

							var terminationByteIdx = (adjustedIdx + 1) / 8;
							var terminationBitIdx = (adjustedIdx + 1) % 8;

							// Append bits to output
							buffer[terminationByteIdx] |= (Byte) (0x01 << 7 - terminationBitIdx); // Termination bit
						}

						// Deduct Fibonacci number from value
						value -= Lookup[fibIdx];
					}
				}

				// Write output
				for (var j = 0; j < maxByte; j++) {
					stream.WriteByte(buffer[j]);
					buffer[j] = 0;
				}

				if (residualBits == 0) {
					stream.WriteByte(buffer[maxByte]);
					buffer[maxByte] = 0;
					bitOffset = 0;
				} else {
					buffer[0] = buffer[maxByte];
					bitOffset = residualBits;
				}
			}

			if (bitOffset > 0) {
				stream.WriteByte(buffer[0]);
			}
		}

		public override void DecodeMany(IByteReader input, UInt64[] values, Int32 offset, Int32 count) {
#if DEBUG

			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}

			if (offset < 0 || offset > values.Length) {
				throw new ArgumentOutOfRangeException(nameof(offset));
			}

			if (count < 0 || offset + count > values.Length) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			// Current symbol being decoded
			UInt64 symbol = 0;

			// Next Fibonacci number to test
			var nextFibIndex = 0;

			// State of the last bit while decoding
			var lastBit = false;

			if (0 == count) {
				return;
			}

			var pos = offset;
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
							values[pos++] = symbol;

							// Stop if expected number of symbols have been found
							if (--count == 0) {
								return;
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
							// TODO: Support full 64bit
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

		public override Int32? CalculateEncodedBits(UInt64 value) {
#if DEBUG
			// Check for overflow
			if (value > MaxValue) {
				throw new OverflowException("Exceeded FibonacciCodec's maximum supported symbol value of " + MaxValue + ".");
			}
#endif

			// Offset for zero
			value++;

			for (var i = Lookup.Length - 1; i >= 0; i--) {
				if (value >= Lookup[i]) {
					return i + 1;
				}
			}

			return 0;
		}
	}
}