using InvertedTomato.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    /// Reader for Fibonacci for unsigned values.
    /// </summary>
    public class FibonacciUnsignedWriter : IUnsignedWriter, IDisposable {
        /// <summary>
        /// Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteOneDefault(ulong value) {
            using (var stream = new MemoryStream()) {
                using (var writer = new FibonacciUnsignedWriter(stream)) {
                    writer.Write(value);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? CalculateBitLength(ulong value) {
            // Offset for zero
            value++;

            for (var i = Fibonacci.Values.Length - 1; i >= 0; i--) {
                if (value >= Fibonacci.Values[i]) {
                    return i + 1;
                }
            }

            return null;
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Underlying stream to be writing encoded values to.
        /// </summary>
        private readonly BitWriter Output;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public FibonacciUnsignedWriter(Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            // Store
            Output = new BitWriter(output);
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Offset for zero
            value++;

            // #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
            ulong[] buffer = new ulong[2];
            int maxFibIdx = 0;
            for (var fibIdx = Fibonacci.Values.Length - 1; fibIdx >= 0; fibIdx--) {
                // #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word(counting the left most digit as place 0).
                if (value >= Fibonacci.Values[fibIdx]) {
                    // Detect if this is the largest fib and store
                    if (maxFibIdx == 0) {
                        maxFibIdx = fibIdx;
                    }

                    // Write to buffer
                    if (maxFibIdx < 64) {
                        buffer[0] |= (ulong)1 << maxFibIdx - fibIdx;
                    } else if (fibIdx >= 64) {
                        buffer[1] |= (ulong)1 << maxFibIdx - fibIdx;
                    } else {
                        buffer[0] |= (ulong)1 << 63 - fibIdx;
                    }

                    // Deduct Fibonacci number from value
                    value -= Fibonacci.Values[fibIdx];
                }
            }
            // #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.

            // Write to output
            if (maxFibIdx >= 64) {
                Output.Write(buffer[0], 64);
                Output.Write(buffer[1], maxFibIdx - 64 + 1);
            } else {
                Output.Write(buffer[0], maxFibIdx + 1);
            }

            // #4 Place an additional 1 after the rightmost digit in the code word.
            Output.Write(1, 1);
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                Output.DisposeIfNotNull();
            }
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose() {
            Dispose(true);
        }
    }
}
