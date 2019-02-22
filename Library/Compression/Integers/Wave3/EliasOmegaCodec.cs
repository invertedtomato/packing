using System;
using System.Collections.Generic;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Wave3 {
	public class EliasOmegaCodec:Codec {
		public override void EncodeMany(IByteWriter output, UInt64[] values, Int32 offset, Int32 count) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
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

			using(var writer = new BitWriter(output)){
				for (var i = offset; i < offset + count; i++) {
					var value = values[i];
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

						writer.Write(group, bits);
					}
				}
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

			using (var reader = new BitReader(input)) {
				for (var i = offset; i < offset + count; i++) {
					// #1 Start with a variable N, set to a value of 1.
					UInt64 value = 1;

					// #2 If the next bit is a "0", stop. The decoded number is N.
					while (reader.PeakBit()) {
						// #3 If the next bit is a "1", then read it plus N more bits, and use that binary number as the new value of N.
						value = reader.Read((Int32) value + 1);
					}

					// Burn last bit from input
					reader.Read(1);

					// Offset for min value
					value = value - 1;

					values[i] = value;
				}
			}
		}

		public override Int32? CalculateEncodedBits(UInt64 value) {
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
	}
}