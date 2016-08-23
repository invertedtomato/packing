using InvertedTomato.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Writer for Fibonacci for unsigned values.
    /// </summary>
    public class FibonacciUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read first value from a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ulong ReadOneDefault(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new FibonacciUnsignedReader(stream)) {
                    return reader.Read();
                }
            }
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public FibonacciUnsignedReader(Stream input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            Input = new BitReader(input);
        }

        /// <summary>
        /// Read the next value. 
        /// </summary>
        /// <returns></returns>
        public ulong Read() {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Set default value
            ulong value = 0;

            // Read first bit
            var bit = Input.Read(1);

            for (var i = 0; i < Fibonacci.Values.Length; i++) {
                // If true
                bool hit = false;
                if (bit > 0) {
                    value += Fibonacci.Values[i];
                    hit = true;
                }

                // Read next bit
                bit = Input.Read(1);

                // If double 1 bit, it's the end
                if (hit && bit > 0) {
                    break;
                } else if (i == Fibonacci.Values.Length) {
                    throw new InvalidOperationException("Haven't encountered final bit.");
                }
            }

            // Remove zero offset
            value--;

            return value;
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                Input.DisposeIfNotNull();
            }
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
    }
}
