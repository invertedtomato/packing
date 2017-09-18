using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public class VLQCodec : Codec{
        public const ulong MinValue = ulong.MinValue;
        public const ulong MaxValue = ulong.MaxValue - 1;
        public const byte Nil = 0x80;  // 10000000

        private const byte Mask = 0x7f; // 01111111
        private const int PacketSize = 7;
        private const ulong MinPacketValue = ulong.MaxValue >> 64 - PacketSize;

        /// <summary>
        /// Compress a UInt to a given buffer.
        /// </summary>
        public override void CompressUnsigned(ulong symbol, Buffer<byte> output) {
#if DEBUG
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Iterate through input, taking X bits of data each time, aborting when less than X bits left
            while (symbol > MinPacketValue) {
                // Write payload, skipping MSB bit
                output.Enqueue((byte)(symbol & Mask));

                // Offset value for next cycle
                symbol >>= PacketSize;
                symbol--;
            }

            // Write remaining - marking it as the final byte for symbol
            output.Enqueue((byte)(symbol | Nil));
        }
        
        /// <summary>
        /// Decompress a UInt from a given buffer.
        /// </summary>
        public override ulong DecompressUnsigned(Buffer<byte> input) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
#endif

            // Setup symbol
            ulong symbol = 0;
            var bit = 0;

            // Iterate through input
            byte b;
            while (input.TryDequeue(out b)) {
                // Add input bits to output
                var chunk = (ulong)(b & Mask);
                var pre = symbol;
                symbol += chunk + 1 << bit;
#if DEBUG
                // Check for overflow
                if (symbol < pre) {
                    throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
                }
#endif
                bit += PacketSize;

                // If last byte in symbol
                if ((b & Nil) > 0) {
                    // Remove zero offset
                    symbol--;

                    // Add to output
                    return symbol;
                }
            }

            // Insufficent input
            throw new InsufficentInputException("Input ends with a partial symbol. More bytes required to decode.");
        }
    }
}
