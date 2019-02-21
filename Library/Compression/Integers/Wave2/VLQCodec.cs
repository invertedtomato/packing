using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers.Wave2 {
	public class VLQCodec : Codec {
		public const UInt64 MinValue = UInt64.MinValue;
		public const UInt64 MaxValue = UInt64.MaxValue - 1;
		public const Byte Nil = 0x80; // 10000000

		public static readonly Byte[] Zero = new Byte[] {0x80}; // 10000000
		public static readonly Byte[] One = new Byte[] {0x81}; // 10000001
		public static readonly Byte[] Two = new Byte[] {0x82}; // 10000010
		public static readonly Byte[] Four = new Byte[] {0x84}; // 10000100
		public static readonly Byte[] Eight = new Byte[] {0x88};

		private const Byte Mask = 0x7f; // 01111111
		private const Int32 PacketSize = 7;
		private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);

		/// <summary>
		/// Encode an array of values and write them to a stream.
		/// </summary>
		/// <param name="output">Stream to write encoded values to.</param>
		/// <param name="values">Values to encode.</param>
		/// <returns>Total number of bytes values encoded as.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="OverflowException"></exception>
		public override Int32 CompressUnsigned(Stream output, params UInt64[] values) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			var used = 0;
			foreach (var value in values) {
#if DEBUG
				if (value > MaxValue) {
					throw new OverflowException("Symbol is larger than maximum supported value. See VLQCodec.MaxValue");
				}
#endif
				var value2 = value;

				// Iterate through input, taking X bits of data each time, aborting when less than X bits left
				while (value2 > MinPacketValue) {
					// Write payload, skipping MSB bit
					output.WriteByte((Byte) (value2 & Mask));
					used++;

					// Offset value for next cycle
					value2 >>= PacketSize;
					value2--;
				}

				// Write remaining - marking it as the final byte for symbol
				output.WriteByte((Byte) (value2 | Nil));
				used++;
			}

			return used;
		}

		/// <summary>
		/// Decompress a number of VLQ-encoded values from a given stream.
		/// </summary>
		/// <param name="input">Stream to decode from.</param>
		/// <param name="output">Pre-sized array to output decoded values to.</param>
		/// <param name="offset">Starting position in the output array.</param>
		/// <param name="count">Number of values to decode.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="EndOfStreamException"></exception>
		/// <exception cref="OverflowException"></exception>
		public override Int32 DecompressUnsigned(Stream input, ref UInt64[] output, Int32 offset, Int32 count) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (offset < 0 || offset > output.Length) {
				throw new ArgumentOutOfRangeException(nameof(offset));
			}

			if (count < 0 || offset + count > output.Length) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			var used = 0;
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

					used++;

					// Add input bits to output
					var chunk = (UInt64) (b & Mask);
					var pre = symbol;
					symbol += (chunk + 1) << bit;

#if DEBUG
					// Check for overflow
					if (symbol < pre) {
						throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
					}
#endif

					// Increment bit offset
					bit += PacketSize;
				} while ((b & Nil) == 0); // If not final bit

				// Remove zero offset
				symbol--;

				// Add to output
				output[i] = symbol;
			}

			return used;
		}

		/// <summary>
		/// Encode an array of values and write them to a stream.
		/// </summary>
		/// <param name="output">Stream to write encoded values to.</param>
		/// <param name="values">Values to encode.</param>
		/// <returns>Total number of bytes values encoded as.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="OverflowException"></exception>
		public override async Task<Int32> CompressUnsignedAsync(Stream output, params UInt64[] values) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			var used = 0;
			foreach (var value in values) {
#if DEBUG
				if (value > MaxValue) {
					throw new OverflowException("Symbol is larger than maximum value. See VLQCodec.MaxValue");
				}
#endif
				var value2 = value;

				// Iterate through input, taking X bits of data each time, aborting when less than X bits left
				while (value2 > MinPacketValue) {
					// Write payload, skipping MSB bit
					output.WriteByte((Byte) (value2 & Mask)); // TODO: Make this call async?
					used++;

					// Offset value for next cycle
					value2 >>= PacketSize;
					value2--;
				}

				// Write remaining - marking it as the final byte for symbol
				await output.WriteAsync(new[] {(Byte) (value2 | Nil)}, 0, 1);
				used++;
			}

			return used;
		}

		/// <summary>
		/// Decompress a number of VLQ-encoded values from a given stream.
		/// </summary>
		/// <param name="input">Stream to decode from.</param>
		/// <param name="count">Number of values to decode.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		/// <exception cref="EndOfStreamException"></exception>
		/// <exception cref="OverflowException"></exception>
		public override async Task<IEnumerable<UInt64>> DecompressUnsignedAsync(Stream input, Int32 count) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			Byte b;
			var buffer = new Byte[1];
			var output = new List<UInt64>();

			for (var i = 0; i < count; i++) {
				// Setup symbol
				UInt64 symbol = 0;
				var bit = 0;

				do {
					// Read byte of input, and throw error if unavailable
					if (await input.ReadAsync(buffer, 0, 1) != 1) {
						throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
					}

					b = buffer[0];

					// Add input bits to output
					var chunk = (UInt64) (b & Mask);
					var pre = symbol;
					symbol += (chunk + 1) << bit;

#if DEBUG
					// Check for overflow
					if (symbol < pre) {
						throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
					}
#endif

					// Increment bit offset
					bit += PacketSize;
				} while ((b & Nil) == 0); // If not final bit

				// Remove zero offset
				symbol--;

				// Add to output
				output.Add(symbol);
			}

			return output;
		}


		public override Int32 CalculateBitLength(UInt64 symbol) {
			var packets = (Int32) Math.Ceiling(BitOperation.CountUsed(symbol) / (Single) PacketSize);

			return packets * (PacketSize + 1);
		}
	}
}