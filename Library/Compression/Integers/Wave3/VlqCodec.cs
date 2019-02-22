using System;
using System.IO;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Wave3 {
	
	/// <summary>
	/// Traditional VLQ implementation as per https://en.wikipedia.org/wiki/Variable-length_quantity with "Removing Redundancy".
	/// </summary>
	public class VlqCodec:Codec {
		public const UInt64 MinValue = 0;
		public const UInt64 MaxValue = UInt64.MaxValue - 1;

		private const Byte More = 0b10000000;
		private const Byte Mask = 0b01111111;
		private const Int32 PacketSize = 7;
		private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);

		
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

			for (var i = offset; i < offset + count; i++) {
				var value = values[i];
#if DEBUG
				if (value > MaxValue) {
					throw new OverflowException("Symbol is larger than maximum supported value. See VLQCodec.MaxValue");
				}
#endif

				// Iterate through input, taking X bits of data each time, aborting when less than X bits left
				while (value > MinPacketValue) {
					// Write payload, skipping MSB bit
					output.WriteByte((Byte) ((value & Mask) | More));

					// Offset value for next cycle
					value >>= PacketSize;
					value--;
				}

				// Write remaining - marking it as the final byte for symbol
				output.WriteByte((Byte) (value & Mask));
			}
		}
		
		public override void DecodeMany(IByteReader input, UInt64[] values, Int32 offset, Int32 count){
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

			for (var i = offset; i < offset + count; i++) {
				// Setup symbol
				UInt64 symbol = 0;
				var bit = 0;

				Int32 b;
				do {
					// Read byte
					if ((b = input.ReadByte()) == -1) {
						throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
					}

					// Add input bits to output
					var chunk = (UInt64) (b & Mask);
					var pre = symbol;
					symbol += (chunk + 1) << bit;

#if DEBUG
					// Check for overflow
					if (symbol < pre) {
						throw new OverflowException("Symbol is larger than maximum supported value or is corrupt. See UnsignedVlq.MaxValue.");
					}
#endif

					// Increment bit offset
					bit += PacketSize;
				} while ((b & More) > 0); // If not final byte

				// Remove zero offset
				symbol--;

				// Add to output
				values[i] = symbol;
			}
		}
		
		public override Int32? CalculateEncodedBits(UInt64 value) {
			var packets = (Int32) Math.Ceiling(BitOperation.CountUsed(value) / (Single) PacketSize);

			return packets * (PacketSize + 1);
		}
	}
}