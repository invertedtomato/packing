using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public class VLQCodec : IIntegerCodec {
        public byte[] Compress(long[] symbols) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
#endif

            // Prepare input buffer
            var input = new Buffer<ulong>(symbols.Length);

            // Perform signed conversion
            foreach (var i in symbols) {
                input.Enqueue(ZigZag.Encode(i));
            }

            // Compress
            var output = new Buffer<byte>(input.Used * 2);
            while (!Compress(input, output)) {
                output = output.Resize(output.MaxCapacity * 2);
            }

            return output.ToArray();
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

            // Process decompressed set
            ulong value;
            while (input.TryDequeue(out value)) {
                // Initialise pending output counter
                var pending = 0;

                // Iterate through input, taking X bits of data each time, aborting when less than X bits left
                while (value > MINVAL) {
                    // If compressed buffer is full - stop
                    if (output.IsFull) {
                        // We were part way through a symbol when we ran out of output space - reset to the start of this symbol
                        input.MoveStart(-1);
                        output.MoveEnd(-pending);

                        // Return
                        return false; // INPUT isn't empty
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

                    // Return
                    return false; // INPUT isn't empty
                }

                // Write remaining - marking it as the final byte for symbol
                output.Enqueue((byte)(value | MSB));
            }

            // Return
            return true; // INPUT is empty
        }

        public long[] Decompress(byte[] raw) {
#if DEBUG
            if (null == raw) {
                throw new ArgumentNullException("raw");
            }
#endif

            // Prepare buffers
            var input = new Buffer<byte>(raw);
            var output = new Buffer<ulong>(raw.Length);

            // Decompress
            while (!Decompress(input, output)) {
                output = output.Resize(output.MaxCapacity * 2);
            }

            // Perform signed conversion
            var symbols = new long[output.Used];
            for (var i = 0; i < symbols.Length; i++) {
                symbols[i] = ZigZag.Decode(output.Dequeue());
            }

            return symbols;
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

            // Initialise pending output counter
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
                var pre = symbol;
                symbol += chunk + 1 << bit;
#if DEBUG
                // Check for overflow
                if (symbol < pre) {
                    throw new OverflowException("Input symbol larger than the supported limit of 64bits. Possible data issue.");
                }
#endif
                bit += PACKETSIZE;

                // If last byte in symbol
                if ((b & MSB) > 0) {
                    // Remove zero offset
                    symbol--;
                    
                    // If we've run out of output buffer
                    if (output.IsFull) {
                        // Remove partial outputs
                        input.MoveStart(-pending);

                        // Return 
                        return false;
                    }

                    // Add to output
                    output.Enqueue(symbol);
                    
                    // Reset for next symbol
                    symbol = 0;
                    bit = 0;
                    pending = 0;
                }
            }

            // Bail if we were part way through a symbol
#if DEBUG
            if (bit > 0) {
                throw new FormatException("Input ends with a partial symbol - bytes are missing or the input is corrupt.");
            }
#endif

            // Return
            return true;
        }

        public static readonly ulong MaxValue = ulong.MaxValue - 1;
        private const byte MSB = 0x80;  // 10000000
        private const byte MASK = 0x7f; // 01111111
        private const int PACKETSIZE = 7;
        private const ulong MINVAL = ulong.MaxValue >> 64 - PACKETSIZE;
    }
}
