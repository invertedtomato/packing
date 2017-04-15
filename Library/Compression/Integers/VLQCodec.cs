using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public class VLQCodec : IIntegerCodec {
        public void CompressOne(ulong input, Buffer<byte> output) {
            if (CompressMany(new Buffer<ulong>(new ulong[] { input }), output) < 1) {
                throw new InvalidOperationException("Insufficent space in output buffer.");
            }
        }

        public int CompressMany(Buffer<ulong> input, Buffer<byte> output) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Initialise completed counter
            var done = 0;

            // Process decompressed set
            ulong value;
            while (input.TryDequeue(out value)) {
                // Initialise pending bytes counter
                var pending = 0;

                // Iterate through input, taking X bits of data each time, aborting when less than X bits left
                while (value > MINVAL) {
                    // If compressed buffer is full - stop
                    if (output.IsFull) {
                        // We were part way through a symbol when we ran out of output space - reset to the start of this symbol
                        input.MoveStart(-1);
                        output.MoveEnd(-pending);

                        // Return number of completed bytes
                        return done;
                    }

                    // Write payload, skipping MSB bit
                    output.Enqueue((byte)(value & MASK));
                    pending++;

                    // Offset value for next cycle
                    value >>= PACKETSIZE;
                    value--;
                }

                // If compressed buffer is full - stop
                if (output.IsFull) {
                    // We were part way through a symbol when we ran out of output space - reset to the start of this symbol
                    input.MoveStart(-1);
                    output.MoveEnd(-pending);
                    
                    // Return number of completed bytes
                    return done;
                }

                // Write remaining - marking it as the final byte for symbol
                output.Enqueue((byte)(value | MSB));

                // Increment number of completed symbols
                done++;
            }

            return done;
        }

        public ulong DecompressOne(Buffer<byte> input) {
            var output = new Buffer<ulong>(1);
            if (DecompressMany(input, output) < 1) {
                throw new InvalidOperationException("Insufficent space in output buffer.");
            }
            return output.Dequeue();
        }

        public int DecompressMany(Buffer<byte> input, Buffer<ulong> output) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif
            
            // Initialise completed counter
            var done = 0;
            
            // Initialise pending bytes counter
            var pending = 0;

            // Setup symbol
            ulong symbol = 0;
            var bit = 0;

            // Iterate through input
            byte b;
            while (input.TryDequeue(out b)) {
                pending++;

                // Add input bits to output
                var chunk = (ulong)(b & MASK);
                symbol += chunk + 1 << bit;
                bit += PACKETSIZE;

                // If last byte in symbol
                if ((b & MSB) > 0) {
                    // Remove zero offset
                    symbol--;

                    // Add to output
                    output.Enqueue(symbol);
                    done++;

                    // If we've run out of output buffer
                    if (output.IsFull) {
                        // Return the number compleed
                        return done;
                    }

                    // Reset for next symbol
                    pending = 0;
                    symbol = 0;
                    bit = 0;
                }
            }

            // We were part way through a symbol when we ran out of output space - reset to the start of this symbol
            if (pending > 0) {
                input.MoveStart(-pending);
            }

            // Return number completed
            return done;
        }


        private const byte MSB = 0x80;  // 10000000
        private const byte MASK = 0x7f; // 01111111
        private const int PACKETSIZE = 7;
        private const ulong MINVAL = ulong.MaxValue >> 64 - PACKETSIZE;

    }
}
