using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedTomato.Compression.Integers.Legacy {
	/// <summary>
    ///     Tools for managing bit sets.
    /// </summary>
    public static class BitOperation {
		private static readonly Regex Binary = new Regex("^([01]{8})*$");

        /// <summary>
        ///     Count the number of bits used to express number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 CountUsed(UInt64 value) {
			Byte bits = 0;

			do {
				bits++;
				value >>= 1;
			} while (value > 0);

			return bits;
		}

        /// <summary>
        ///     Push bits onto the least-significant side of a ulong.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="bits"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static UInt64 Push(UInt64 host, UInt64 bits, Int32 count) {
			if (count > 64) {
				throw new ArgumentOutOfRangeException("Must be between 0 and 64, not " + count + ".", nameof(count));
			}

			// Add space on host
			host <<= count;

			// Add bits
			host |= bits & (UInt64.MaxValue >> (64 - count));

			return host;
		}

        /// <summary>
        ///     Pop bits off the least-significant side of a ulong.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static UInt64 Pop(UInt64 host, Int32 count) {
			if (count > 64) {
				throw new ArgumentOutOfRangeException("Must be between 0 and 64, not " + count + ".", nameof(count));
			}

			// Extract bits
			var bits = host & (UInt64.MaxValue >> (64 - count));

			// Remove space from host
			host >>= count;

			return bits;
		}

        /// <summary>
        ///     Convert a ulong to a binary string. No byte reordering - the MSB is always on the left, LSB is always on the right.
        ///     A space between bytes. No padding.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String ToString(UInt64 value) {
			return ToString(value, 1);
		}

        /// <summary>
        ///     Convert a ulong to a binary string. No byte reordering - the MSB is always on the left, LSB is always on the right.
        ///     A space between bytes. Padding only if required to meet minBits.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minBits"></param>
        /// <returns></returns>
        public static String ToString(UInt64 value, Int32 minBits) {
			var output = "";

			var pos = 0;
			do {
				output = (value % 2 == 0 ? "0" : "1") + output;
				value >>= 1;
				if (++pos % 8 == 0) {
					output = " " + output;
				}
			} while (pos < minBits || value > 0);

			return output.Trim();
		}

        /// <summary>
        ///     Parse a binary string into a byte array.
        /// </summary>
        /// <param name="input">
        ///     A string of 1s and 0s in groups of 8. Can also include placeholder character '_' which is treated
        ///     as a 0.
        /// </param>
        /// <returns></returns>
        public static Byte[] ParseToBytes(String input) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			// Clean input
			input = input.Replace(" ", "") // Remove any spaces
				.Replace('_', '0'); // Replace placeholder 0

			// Abort if input isn't sane
			if (!Binary.IsMatch(input)) {
				throw new ArgumentException("Not valid binary (" + input.Length + " characters).", nameof(input));
			}

			// Do the conversion
			return Enumerable
				.Range(0, input.Length / 8).Select(i => input.Substring(i * 8, 8)) // Split into 8-character chunks
				.Select(a => Convert.ToByte(a, 2)) // Convert to bytes
				.ToArray(); // Convert to array
		}
	}
}