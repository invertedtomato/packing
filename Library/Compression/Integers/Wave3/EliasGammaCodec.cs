using System;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Wave3 {
	public class EliasGammaCodec:Codec {
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

			using (var writer = new BitWriter(output)) {
				for (var i = offset; i < offset + count; i++) {
					var value = values[i];

					// Offset value to allow zeros
					value++;

					// Calculate length
					var length = BitOperation.CountUsed(value);

					// Write unary zeros
					writer.Write(0, length - 1);

					// Write value
					writer.Write(value, length);
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

					// Read length
					var length = 1;
					while (!reader.PeakBit()) {
						// Note that length is one bit longer
						length++;

						// Remove 0 from input
						reader.Read(1);
					}

					// Read value
					var value = reader.Read(length);

					// Remove offset from value
					value--;

					values[i] = value;
				}
			}
		}

		public override Int32? CalculateEncodedBits(UInt64 value) {
			// Offset for zero
			value++;

			return BitOperation.CountUsed(value) * 2 - 1;
		}
	}
}