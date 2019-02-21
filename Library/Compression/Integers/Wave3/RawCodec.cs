using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InvertedTomato.Compression.Integers.Wave3 {
	public class RawCodec : Codec {
		public static readonly UInt64 MinValue = UInt64.MinValue;
		public static readonly UInt64 MaxValue = UInt64.MaxValue;

		public override void EncodeMany(IByteWriter output, UInt64[] values, Int32 offset, Int32 count) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			foreach (var value in values) {
				// Convert to raw byte array
				var raw = BitConverter.GetBytes(value);

				// Add to output
				foreach (var b in raw) {
					output.WriteByte(b);
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

			for (var i = offset; i < offset + count; i++) {
				// Get next 8 bytes
				var buffer = new Byte[8];
				try {
					for (var j = 0; j < buffer.Length; j++) {
						buffer[j] = input.ReadByte();
					}
				} catch (ArgumentException) {
					throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
				}

				// Convert to symbol
				var symbol = BitConverter.ToUInt64(buffer, 0);

				// Return symbol
				values[i] = symbol;
			}
		}

		public override Int32 CalculateBitLength(UInt64 symbol) {
			return 8;
		}
	}
}