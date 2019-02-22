using System;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Wave3 {
	public class EliasDeltaCodec :Codec{
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

					// #1 Separate X into the highest power of 2 it contains (2N) and the remaining N binary digits.
					var n = 0;
					while (Math.Pow(2, n + 1) <= value) {
						n++;
					}

					var r = value - (UInt64) Math.Pow(2, n);

					// #2 Encode N+1 with Elias gamma coding.
					var np = (UInt64) n + 1;
					var len = BitOperation.CountUsed(np);
					writer.Write(0, len - 1);
					writer.Write(np, len);

					// #3 Append the remaining N binary digits to this representation of N+1.
					writer.Write(r, n);
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
					// #1 Read and count zeros from the stream until you reach the first one. Call this count of zeros L
					var l = 1;
					while (!reader.PeakBit()) {
						// Note that length is one bit longer
						l++;

						// Remove 0 from input
						reader.Read(1);
					}

					;

					// #2 Considering the one that was reached to be the first digit of an integer, with a value of 2L, read the remaining L digits of the integer. Call this integer N+1, and subtract one to get N.
					var n = (Int32) reader.Read(l) - 1;

					// #3 Put a one in the first place of our final output, representing the value 2N.
					// #4 Read and append the following N digits.
					var value = reader.Read(n) + ((UInt64) 1 << n);

					// Remove zero offset
					value--;

					values[i] = value;
				}
			}
		}

		public override Int32? CalculateEncodedBits(UInt64 value) {
			var result = 0;

			// Offset for zero
			value++;

			// #1 Separate X into the highest power of 2 it contains (2N) and the remaining N binary digits.
			Byte n = 0;
			while (Math.Pow(2, n + 1) <= value) {
				n++;
			}

			var r = value - (UInt64) Math.Pow(2, n);

			// #2 Encode N+1 with Elias gamma coding.
			var np = (Byte) (n + 1);
			var len = BitOperation.CountUsed(np);
			result += len - 1;
			result += len;

			// #3 Append the remaining N binary digits to this representation of N+1.
			result += n;

			return result;
		}
	}
}