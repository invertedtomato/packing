using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for dynamic length unsigned integers.
    /// </summary>
    public class DynamicUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(byte[] input) {
            return ReadAll(ulong.MaxValue, input);
        }

        /// <summary>
        /// Read all values in a byte array with options.
        /// </summary>
        /// <param name="maxValue">The maximum supported value. To match standard use ulong.MaxValue.</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(ulong maxValue, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new DynamicUnsignedReader(stream, maxValue)) {
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
        /// The number of bits used to store length.
        /// </summary>
        private readonly int LengthBits;

        /// <summary>
        /// The underlying stream to be reading from.
        /// </summary>
        private readonly Stream Input;

        /// <summary>
        /// The current byte being worked with.
        /// </summary>
        private int CurrentByte;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public DynamicUnsignedReader(Stream input) : this(input, ulong.MaxValue) { }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxValue">The maximum supported value. To match standard use ulong.MaxValue.</param>
        public DynamicUnsignedReader(Stream input, ulong maxValue) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }


            // Store
            Input = input;

            // TODO: calculate number of bits for length field
            throw new NotImplementedException();
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

            // TODO
            throw new NotImplementedException();
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
        /// Read a byte from the input stream.
        /// </summary>
        /// <returns>TRUE if successful.</returns>
        private bool ReadByte() {
            // Get next byte
            CurrentByte = Input.ReadByte();
            if (CurrentByte < 0) {
                return false;
            }

            return true;
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
