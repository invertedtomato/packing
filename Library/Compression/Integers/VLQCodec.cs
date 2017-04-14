using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public class VLQCodec : IIntegerCodec {
        private const byte MSB = 0x80;  // 10000000
        private const byte MASK = 0x7f; // 01111111
        private const int PACKETSIZE = 7;

        /// <summary>
        /// The guessed size of buffer when there is no indication otherwise.
        /// </summary>
        public int BufferDefaultSize { get; set; } = 8;

        /// <summary>
        /// When BufferDefaultSize proves to be too small, increase the size by this factor.
        /// </summary>
        public int BufferGrowthFactor { get; set; } = 2;

        public bool IncludeHeader { get; set; }
        public Buffer<ulong> DecompressedSet { get; set; }
        public Buffer<byte> CompressedSet { get; set; }

        public void Compress() {
#if DEBUG
            if (null == DecompressedSet) {
                throw new InvalidOperationException("DecompressedSet is null.");
            }
#endif

            // Quickly handle empty sets with no headers - they'll cause issues later if not handled here
            if (!IncludeHeader && DecompressedSet.IsEmpty) {
                CompressedSet = new Buffer<byte>(0);
                return;
            }

            // Allocate space for output
            CompressedSet = new Buffer<byte>(BufferDefaultSize);

            // Calculate size of non-final packet
            var min = ulong.MaxValue >> 64 - PACKETSIZE;

            // Get first symbol
            var value = IncludeHeader ? (ulong)DecompressedSet.Used : DecompressedSet.Dequeue();

            // Process decompressed set
            do {
                // Iterate through input, taking X bits of data each time, aborting when less than X bits left
                while (value > min) {
                    // If compressed buffer is full - grow it
                    if (CompressedSet.IsFull) {
                        CompressedSet = CompressedSet.Resize(CompressedSet.Used * BufferGrowthFactor);
                    }

                    // Write payload, skipping MSB bit
                    CompressedSet.Enqueue((byte)(value & MASK));

                    // Offset value for next cycle
                    value >>= PACKETSIZE;
                    value--;
                }

                // If compressed buffer is full - grow it
                if (CompressedSet.IsFull) {
                    CompressedSet = CompressedSet.Resize(CompressedSet.Used * BufferGrowthFactor);
                }

                // Write remaining - marking it as the final byte for symbol
                CompressedSet.Enqueue((byte)(value | MSB));
            } while (DecompressedSet.TryDequeue(out value));
        }

        private ulong DecompressSymbol = 0;
        private int DecompressBit = 0;

        public int Decompress() {
#if DEBUG
            if (null == CompressedSet) {
                throw new InvalidOperationException("CompressedSet is null.");
            }
#endif

            // If there's no header, lets assume the set is "default" sized
            if (!IncludeHeader && null == DecompressedSet) {
                DecompressedSet = new Buffer<ulong>(BufferDefaultSize);
            }

            // Setup symbol
            

            // Iterate through input
            byte input;
            while (CompressedSet.TryDequeue(out input)) {
                // Add input bits to output
                var chunk = (ulong)(input & MASK);
                DecompressSymbol += chunk + 1 << DecompressBit;
                DecompressBit += PACKETSIZE;

                // If last byte in symbol
                if ((input & MSB) > 0) {
                    // Remove zero offset
                    DecompressSymbol--;

                    // If output hasn't been allocated...
                    if (null == DecompressedSet) {
                        // Allocate output
                        DecompressedSet = new Buffer<ulong>((int)DecompressSymbol);
                    } else {
                        // Add to output
                        DecompressedSet.Enqueue(DecompressSymbol);

                        // If we've run out of output buffer
                        if (DecompressedSet.IsFull) {
                            // This had a header, so that must be all the data
                            if (IncludeHeader) {
                                return 0;
                            } else {
                                // There's no header - we don't know how big the set it, and the output is full - grow it
                                DecompressedSet = DecompressedSet.Resize(DecompressedSet.Used * BufferGrowthFactor);
                            }
                        }
                    }

                    // Reset for next symbol
                    DecompressSymbol = 0;
                    DecompressBit = 0;
                }
            }

            // Without a header we didn't know how much data to expect anyway. This must be all.
            if (!IncludeHeader) {
                return 0;
            }

            // No complete sets were found
            return 1;
        }
    }
}
