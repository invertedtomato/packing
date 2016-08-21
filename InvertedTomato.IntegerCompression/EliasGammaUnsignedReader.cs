using InvertedTomato.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Elias Gamma universal coding for unsigned values.
    /// </summary>
    public class EliasGammaUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new EliasGammaUnsignedReader(stream)) {
                    ulong value;
                    while (reader.TryRead(out value)) {
                        yield return value;
                    }
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
        public EliasGammaUnsignedReader(Stream input) {
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

            value = 0;

            // Read length
            byte length = 1;
            while(true) {
                
                bool a;
                if(!Input.TryPeakBit(out a)) {
                    return false;
                }

                if (a) {
                    break;
                }else { 
                    length++;
                    ulong b;
                    if (!Input.TryRead(out b,1)) {
                        return false;
                    }
                }
            };


            // Read value
            if (!Input.TryRead(out value, length)) {
                value = 0;
                return false;
            }

            // Remove offset from value
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
