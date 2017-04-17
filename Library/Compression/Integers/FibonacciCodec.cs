using System;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public class FibonacciCodec : IIntegerCodec {
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

        public bool Compress(Buffer<ulong> input, Buffer<byte> output) { // TODO: rollback if we run out of buffer space part way through a symbol
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Initialise pending counters
            var pendingInputs = 0;
            var pendingOutputs = 0;

            // Clear currently worked-on byte
            var current = new byte();
            var offset = 0;

            // Iterate through all symbols
            ulong value;
            while (input.TryDequeue(out value)) {
                pendingInputs++;
#if DEBUG
                if (value > MaxValue) {
                    throw new OverflowException("Exceeded FibonacciCodec's maximum supported symbol value of " + MaxValue + ".");
                }
#endif

                // Fibbonacci doesn't support 0s, so add 1 to allow for them
                value++;

                // #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
                // #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
                bool[] map = null;
                for (var fibIdx = Lookup.Length - 1; fibIdx >= 0; fibIdx--) {
                    // #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word(counting the left most digit as place 0).
                    if (value >= Lookup[fibIdx]) {
                        // Detect if this is the largest fib and store
                        if (null == map) {
                            map = new bool[fibIdx + 2];
                            map[fibIdx + 1] = true; // Termination bit
                        }

                        // Write to map
                        map[fibIdx] = true;

                        // Deduct Fibonacci number from value
                        value -= Lookup[fibIdx];
                    }
                }

                // Output the bits of the map in reverse order
                foreach (var bit in map) {
                    if (bit) {
                        current |= (byte)(1 << (7 - offset));
                    }

                    // Increment offset;
                    if (++offset == 8) {
                        // We were part way through a symbol when we ran out of output space - reset to start of set (we can't go to start of symbol because multiple symbols could be using each byte)
                        if (output.IsFull) {
                            input.MoveStart(-pendingInputs);
                            output.MoveEnd(-pendingOutputs);

                            return false; // INPUT isn't empty
                        }

                        // Add byte to output
                        output.Enqueue(current);
                        current = 0;
                        offset = 0;
                        pendingOutputs++;
                    }
                }
            }

            // Flush bit buffer
            if (offset > 0) {
                // We were part way through a symbol when we ran out of output space - reset to start of set (we can't go to start of symbol because multiple symbols could be using each byte)
                if (output.IsFull) {
                    input.MoveStart(-pendingInputs);
                    output.MoveEnd(-pendingOutputs);

                    return false; // INPUT isn't empty
                }

                output.Enqueue(current);
            }

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

            // Initialise pending counters
            var pendingInputs = 0;
            var pendingOutputs = 0;

            // Current symbol being decoded.
            ulong symbol = 0;

            // Next Fibonacci number to test.
            int nextFibIndex = 0;

            // State of the last bit while decoding.
            bool lastBit = false;

            byte b;
            while (input.TryDequeue(out b)) {
                pendingInputs++;

                // For each bit of buffer
                for (var bi = 0; bi < 8; bi++) {
                    // If bit is set...
                    if (((b << bi) & MSB) > 0) {
                        // If double 1 bits
                        if (lastBit) {
                            // Remove zero offset
                            symbol--;

                            // If we've run out of output buffer
                            if (output.IsFull) {
                                // Remove partial outputs
                                input.MoveStart(-pendingInputs);
                                output.MoveEnd(-pendingOutputs);

                                // Return 
                                return false;
                            }

                            // Add to output
                            output.Enqueue(symbol);
                            pendingOutputs++;

                            // Reset for next symbol
                            symbol = 0;
                            nextFibIndex = 0;
                            lastBit = false;
                            continue;
                        }

#if DEBUG
                        // Check for overflow
                        if (nextFibIndex >= Lookup.Length) {
                            throw new OverflowException("Value too large to decode. Max 64bits supported.");  // TODO: Handle this so that it doesn't allow for DoS attacks!
                        }
#endif

                        // Add value to current symbol
                        var pre = symbol;
                        symbol += Lookup[nextFibIndex];
#if DEBUG
                        // Check for overflow
                        if (symbol < pre) {
                            throw new OverflowException("Input symbol larger than the supported limit of 64bits. Possible data issue.");
                        }
#endif

                        // Note bit for next cycle
                        lastBit = true;
                    } else {
                        // Note bit for next cycle
                        lastBit = false;
                    }

                    // Increment bit position
                    nextFibIndex++;
                }
            }

#if DEBUG
            // With fib there will usually be trailing unused "pad" bits - however these will always be zeros - so make sure they were always zeros
            if (symbol > 0) {
                throw new FormatException("Input ends with a partial symbol - bytes are missing or the input is corrupt.");
            }
#endif

            // Return
            return true; // INPUT is empty
        }


        /// <summary>
        /// The most significant bit in a byte.
        /// </summary>
        private const byte MSB = 0x80;

        /// <summary>
        /// The maximum value of a symbol this codec can support.
        /// </summary>
        public static readonly ulong MaxValue = ulong.MaxValue - 1;

        /// <summary>
        /// Lookup table of Fibonacci numbers that can fit in a ulong.
        /// </summary>
        public static readonly ulong[] Lookup = new ulong[92];

        static FibonacciCodec() {
            // Pre-compute all Fibonacci numbers that can fit in a ulong.
            Lookup[0] = 1;
            Lookup[1] = 2;
            for (var i = 2; i < Lookup.Length; i++) {
                Lookup[i] = Lookup[i - 1] + Lookup[i - 2];
            }
        }
    }
}
