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
        /// Attempt to read the next value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>If a read was successful.</returns>
        public bool TryRead(out ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Set default value
            value = 0;
            
            // Read first bit
            ulong bit;
            if (!Input.TryRead(out bit, 1)) {
                return false;
            }

            for (var i = 0; i < Fibonacci.Values.Length; i++) {
                // If true
                bool hit = false;
                if (bit > 0) {
                    value += Fibonacci.Values[i];
                    hit = true;
                }

                // Read next bit
                if (!Input.TryRead(out bit, 1)) {
                    throw new InvalidOperationException("Missing payload bits.");
                }

                // If double 1 bit, it's the end
                if (hit && bit > 0) {
                    break;
                } else if (i == Fibonacci.Values.Length) {
                    throw new InvalidOperationException("Haven't encountered final bit.");
                }
            }

            // Remove zero offset
            value--;

            return true;
        }

        /// <summary>
        /// Read the next value. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No value was available.</exception>
        public ulong Read() {
            ulong value;
            if (!TryRead(out value)) {
                throw new EndOfStreamException();
            }
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
                // Dispose managed state (managed objects).
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
