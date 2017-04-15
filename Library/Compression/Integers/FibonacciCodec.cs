using System;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public class FibonacciCodec : IIntegerCodec {
        public void Compress(ulong input, Buffer<byte> output) {
            if (!Compress(new Buffer<ulong>(new ulong[] { input }), output)) {
                throw new BufferOverflowException("Insufficent space in output buffer.");
            }
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

            // Initialise completed counter
            var done = 0;

            // Initialise pending bytes counter
            var pending = 0;

            // Clear currently worked-on byte
            var current = new byte();
            var offset = 0;

            // Iterate through all symbols
            ulong value;
            while (input.TryDequeue(out value)) {
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
                            input.MoveStart(-done);
                            output.MoveEnd(-pending);

                            return false; // OUTPUT is full
                        }

                        // Add byte to output
                        output.Enqueue(current);
                        current = 0;
                        offset = 0;
                        pending++;
                    }
                }
                done++;
            }

            // Flush bit buffer
            if (offset > 0) {
                // We were part way through a symbol when we ran out of output space - reset to start of set (we can't go to start of symbol because multiple symbols could be using each byte)
                if (output.IsFull) {
                    input.MoveStart(-done);
                    output.MoveEnd(-pending);

                    return false; // OUTPUT is full
                }

                output.Enqueue(current);
            }

            return true; // OUTPUT isn't full
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
            var done = 0;

            // Initialise pending bytes counter
            var pending = 0;

            // Current symbol being decoded.
            ulong symbol = 0;

            // Next Fibonacci number to test.
            int nextFibIndex = 0;

            // State of the last bit while decoding.
            bool lastBit = false;
            
            byte b;
            while (input.TryDequeue(out b)) {
                pending++;

                // For each bit of buffer
                for(var bi=0; bi<7; bi++) {
                    // If bit is set...
                    if (((b << bi) & MSB) > 0) {
                        // If double 1 bits
                        if (lastBit) {
                            // Remove zero offset
                            symbol--;

                            // Add to output
                            output.Enqueue(symbol);
                            done++;
                            
                            // If we've run out of output buffer
                            if (output.IsFull) {
                                // Return 
                                return true; // OUTPUT is full
                            }

                            // Reset for next symbol
                            symbol = 0;
                            nextFibIndex = 0;
                            lastBit = false;
                            continue;
                        }

                        // Add value to current symbol
                        symbol += Lookup[nextFibIndex];

                        // Note bit for next cycle
                        lastBit = true;
                    } else {
                        // Note bit for next cycle
                        lastBit = false;
                    }

                    // Increment bit position
                    nextFibIndex++;

#if DEBUG
                    // Check for overflow
                    if (nextFibIndex > Lookup.Length) {
                        throw new OverflowException("Value too large to decode. Max 64bits supported.");  // TODO: Handle this so that it doesn't allow for DoS attacks!
                    }
#endif
                }
            }

            // Revert whole read (since Fib uses part-bytes we can't just revert the last symbol)
            input.MoveStart(-pending);
            output.MoveEnd(-done);

            return false; // OUTPUT isn't full
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
