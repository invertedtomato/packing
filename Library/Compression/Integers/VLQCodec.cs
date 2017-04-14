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

            // Setup offset
            ulong symbol = 0;
            //var symbolPosition = 0;

            byte input;
            while (CompressedSet.TryDequeue(out input)) {
                // Add bits to symbol
                symbol <<= 7;
                symbol |= (ulong)(input & MASK);
                
                // If last byte in symbol
                if ((input & MSB) > 0) {
                    // If output is full, resize to make room
                    if (DecompressedSet.IsFull) {
                        DecompressedSet.Resize(DecompressedSet.Used * BufferGrowthFactor);
                    }
                    DecompressedSet.Enqueue(symbol);

                    // Reset for next symbol
                    //symbolPosition = 0;
                    symbol = 0;
                }


                // Remove zero offset
                symbol++;

                /*
                // Set value to 0
                ulong value = 0;

                bool final;
                do {
                    // Read if this is the final packet
                    final = Input.Read(1) > 0;

                    // Read payload
                    var chunk = Input.Read(PacketSize);

                    // Add payload to value
                    value += chunk + 1 << symbolPosition;

                    // Update target offset
                    symbolPosition += PacketSize;
                } while (!final);
                */

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
