using System;

namespace InvertedTomato.Compression.Integers {
	public class ThompsonAlphaCodec :Codec{
		private readonly Int32 LengthBits;

		public ThompsonAlphaCodec() : this(6) { }

		/// <summary>
		///     Instantiate with options.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="lengthBits">Number of prefix bits used to store length.</param>
		public ThompsonAlphaCodec( Int32 lengthBits) {
			if (lengthBits < 1 || lengthBits > 6) {
				throw new ArgumentOutOfRangeException("Must be between 1 and 6, not " + lengthBits + ".", nameof(lengthBits));
			}

			LengthBits = lengthBits;
		}
		
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
				// Iterate through all symbols
				for (var i = offset; i < offset + count; i++) {
					var value = values[i];

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
					writer.Write((UInt64) length, LengthBits);

					// Write number
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
					var length = (Int32) reader.Read(LengthBits);

					// Read body
					var value = reader.Read(length);

					// Recover implied MSB
					value |= (UInt64) 1 << length;

					// Remove offset to allow zeros
					value--;

					values[i] = value;
				}
			}
		}

		public override Int32? CalculateEncodedBits(UInt64 value) {
				// Assume 6 length bits
				var lengthBits = 6;
				
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
	}
}