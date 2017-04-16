using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public class VLQCodec : IIntegerCodec {
        public void Compress(ulong input, Buffer<byte> output) {
            if (!Compress(new Buffer<ulong>(new ulong[] { input }), output)) {
                throw new BufferOverflowException("Insufficent space in output buffer.");
            }
        }

        public bool Compress(Buffer<ulong> input, Buffer<byte> output) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Initialise completed counter
            var pendingOutputs = 0;

            // Process decompressed set
            ulong value;
            while (input.TryDequeue(out value)) {
                // Initialise pending bytes counter
                var pendingInputs = 0;

                // Iterate through input, taking X bits of data each time, aborting when less than X bits left
                while (value > MINVAL) {
                    // If compressed buffer is full - stop
                    if (output.IsFull) {
                        // We were part way through a symbol when we ran out of output space - reset to the start of this symbol
                        input.MoveStart(-1);
                        output.MoveEnd(-pendingInputs);

                        // Return
                        return false; // INPUT isn't empty
                    }

                    // Write payload, skipping MSB bit
                    output.Enqueue((byte)(value & MASK));
                    pendingInputs++;

                    // Offset value for next cycle
                    value >>= PACKETSIZE;
                    value--;
                }

                // If compressed buffer is full - stop
                if (output.IsFull) {
                    // We were part way through a symbol when we ran out of output space - reset to the start of this symbol
                    input.MoveStart(-1);
                    output.MoveEnd(-pendingInputs);

                    // Return
                    return false; // INPUT isn't empty
                }

                // Write remaining - marking it as the final byte for symbol
                output.Enqueue((byte)(value | MSB));

                // Increment number of completed symbols
                pendingOutputs++;
            }

            // Return
            return true; // INPUT is empty
        }

        public ulong Decompress(Buffer<byte> input) {
            var output = new Buffer<ulong>(1);
            if (!Decompress(input, output)) {
                throw new BufferOverflowException("Insufficent space in output buffer.");
            }
            return output.Dequeue();
        }

        public bool Decompress(Buffer<byte> input, Buffer<ulong> output) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Initialise completed counter
            var pendingOutputs = 0;

            // Initialise pending bytes counter
            var pendingInputs = 0;

            // Setup symbol
            ulong symbol = 0;
            var bit = 0;

            // Iterate through input
            byte b;
            while (input.TryDequeue(out b)) {
                pendingInputs++;

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
                    pendingOutputs++;

                    // If we've run out of output buffer
                    if (output.IsFull) {
                        // Return 
                        return true; // OUTPUT is full
                    }

                    // Reset for next symbol
                    pendingInputs = 0;
                    symbol = 0;
                    bit = 0;
                }
            }

            // Revert the partially read symbol
            input.MoveStart(-pendingInputs);

            // Return
            return output.IsFull;
        }


        private const byte MSB = 0x80;  // 10000000
        private const byte MASK = 0x7f; // 01111111
        private const int PACKETSIZE = 7;
        private const ulong MINVAL = ulong.MaxValue >> 64 - PACKETSIZE;
    }
}
