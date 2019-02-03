using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InvertedTomato.Compression.Integers {
	public class RawCodec : Codec {
		public static readonly UInt64 MinValue = UInt64.MinValue;
		public static readonly UInt64 MaxValue = UInt64.MaxValue;

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
				// Convert to raw byte array
				var raw = BitConverter.GetBytes(value);

				// Add to output
				output.Write(raw, 0, 8);
				used += 8;
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

			for (var i = 0; i < count; i++) {
				// Get next 8 bytes
				var buffer = new Byte[8];
				try {
					if (input.Read(buffer, 0, 8) != 8) {
						throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
					}
				} catch (ArgumentException) {
					throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
				}

				// Convert to symbol
				var symbol = BitConverter.ToUInt64(buffer, 0);

				// Return symbol
				yield return symbol;
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

			var used = 0;
			foreach (var value in values) {
				// Convert to raw byte array
				var raw = BitConverter.GetBytes(value);

				// Add to output
				await output.WriteAsync(raw, 0, 8);
				used += 8;
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

			var output = new List<UInt64>();
			for (var i = 0; i < count; i++) {
				// Get next 8 bytes
				var buffer = new Byte[8];
				try {
					if (await input.ReadAsync(buffer, 0, 8) != 8) {
						throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
					}
				} catch (ArgumentException) {
					throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
				}

				// Convert to symbol
				var symbol = BitConverter.ToUInt64(buffer, 0);

				// Return symbol
				output.Add(symbol);
			}

			return output;
		}


		public override Int32 CalculateBitLength(UInt64 symbol) {
			return 8;
		}
	}
}