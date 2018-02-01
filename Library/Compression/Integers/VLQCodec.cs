using InvertedTomato.IO.Bits;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    public class VLQCodec : Codec {
        public const UInt64 MinValue = UInt64.MinValue;
        public const UInt64 MaxValue = UInt64.MaxValue - 1;
        public const Byte Nil = 0x80;  // 10000000

        private const Byte Mask = 0x7f; // 01111111
        private const Int32 PacketSize = 7;
        private const UInt64 MinPacketValue = UInt64.MaxValue >> 64 - PacketSize;

        public override void CompressUnsigned(Stream output, params UInt64[] values) {
#if DEBUG
            if (null == output) {
                throw new ArgumentNullException(nameof(output));
            }
            if (null == values) {
                throw new ArgumentNullException(nameof(values));
            }
#endif

            foreach (var value in values) {
#if DEBUG
                if (value > MaxValue) {
                    throw new OverflowException($"Symbol is larger than maximum value. See VLQCodec.MaxValue");
                }
#endif
                var value2 = value;

                // Iterate through input, taking X bits of data each time, aborting when less than X bits left
                while (value2 > MinPacketValue) {
                    // Write payload, skipping MSB bit
                    output.WriteByte((Byte)(value2 & Mask));

                    // Offset value for next cycle
                    value2 >>= PacketSize;
                    value2--;
                }

                // Write remaining - marking it as the final byte for symbol
                output.WriteByte((Byte)(value2 | Nil));
            }
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
                    var chunk = (UInt64)(b & Mask);
                    var pre = symbol;
                    symbol += chunk + 1 << bit;

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
                yield return symbol;
            }
        }
        
        public override Int32 CalculateBitLength(UInt64 symbol) {
            var packets = (Int32)Math.Ceiling((Single)BitOperation.CountUsed(symbol) / (Single)PacketSize);

            return packets * (PacketSize + 1);
        }
    }
}
