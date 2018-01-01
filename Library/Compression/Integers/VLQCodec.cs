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

        public override void CompressUnsigned(Stream output, params UInt64[] symbols) {
#if DEBUG
            if(null == output) {
                throw new ArgumentNullException(nameof(output));
            }
            if(null == symbols) {
                throw new ArgumentNullException(nameof(symbols));
            }
#endif

            foreach(var symbol in symbols) {
                var symbol2 = symbol;

                // Iterate through input, taking X bits of data each time, aborting when less than X bits left
                while(symbol2 > MinPacketValue) {
                    // Write payload, skipping MSB bit
                    output.WriteByte((Byte)(symbol2 & Mask));

                    // Offset value for next cycle
                    symbol2 >>= PacketSize;
                    symbol2--;
                }

                // Write remaining - marking it as the final byte for symbol
                output.WriteByte((Byte)(symbol2 | Nil));
            }
        }

        public override IEnumerable<UInt64> DecompressUnsigned(Stream input, Int32 count) {
#if DEBUG
            if(null == input) {
                throw new ArgumentNullException(nameof(input));
            }
            if(count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
#endif

            for(var i = 0; i < count; i++) {
                // Setup symbol
                UInt64 symbol = 0;
                var bit = 0;
                
                Int32 b;
                do {
                    // Read byte
                    if((b = input.ReadByte()) == -1) {
                        throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
                    }

                    // Add input bits to output
                    var chunk = (UInt64)(b & Mask);
                    var pre = symbol;
                    symbol += chunk + 1 << bit;

#if DEBUG
                    // Check for overflow
                    if(symbol < pre) {
                        throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
                    }
#endif

                    // Increment bit offset
                    bit += PacketSize;
                } while((b & Nil) == 0); // If not final bit
                
                // Remove zero offset
                symbol--;

                // Add to output
                yield return symbol;
            }
        }
    }
}
