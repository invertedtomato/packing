using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public class VLQCodec : Codec, IUnsignedCompressor, IUnsignedDecompressor {
        public static readonly ulong MinValue = ulong.MinValue;
        public static readonly ulong MaxValue = ulong.MaxValue - 1;
        private const byte MSB = 0x80;  // 10000000
        private const byte MASK = 0x7f; // 01111111
        private const int PACKETSIZE = 7;
        private const ulong MINVAL = ulong.MaxValue >> 64 - PACKETSIZE;

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
            while (symbol > MINVAL) {
                // Write payload, skipping MSB bit
                output.Enqueue((byte)(symbol & MASK));

                // Offset value for next cycle
                symbol >>= PACKETSIZE;
                symbol--;
            }

            // Write remaining - marking it as the final byte for symbol
            output.Enqueue((byte)(symbol | MSB));
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
                var chunk = (ulong)(b & MASK);
                var pre = symbol;
                symbol += chunk + 1 << bit;
#if DEBUG
                // Check for overflow
                if (symbol < pre) {
                    throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
                }
#endif
                bit += PACKETSIZE;

                // If last byte in symbol
                if ((b & MSB) > 0) {
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
