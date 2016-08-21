using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Elias Omega universal coding for unsigned values.
    /// </summary>
    public class EliasOmegaUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(byte[] input) {
            return ReadAll(0, input);
        }

        /// <summary>
        /// Read all values in a byte array with options.
        /// </summary>
        /// <param name="minValue">Minimum value to support. To match standard use 1.</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(ulong minValue, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new EliasOmegaUnsignedReader(stream, minValue)) {

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
        /// The minimum value supported in this instance.
        /// </summary>
        private readonly ulong MinValue;

        /// <summary>
        /// The underlying stream to be reading from.
        /// </summary>
        private readonly Stream Input;

        /// <summary>
        /// The current byte being worked with.
        /// </summary>
        private int CurrentByte;

        /// <summary>
        /// The bit offset in the current byte.
        /// </summary>
        private int CurrentOffset = 8;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public EliasOmegaUnsignedReader(Stream input) : this(input, 0) { }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="minValue">Minimum value to support. To match standard use 1.</param>
        public EliasOmegaUnsignedReader(Stream input, ulong minValue) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            Input = input;
            MinValue = minValue;
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

            // Load current byte
            if (CurrentOffset >= 8) {
                if (!ReadByte()) {
                    value = 0;
                    return false; // Missing initial byte
                }
            }

            // #1 Start with a variable N, set to a value of 1.
            value = 1;

            // #2 If the next bit is a "0", stop. The decoded number is N.
            while ((CurrentByte & 1 << (7 - CurrentOffset)) > 0) {
                // #3 If the next bit is a "1", then read it plus N more bits, and use that binary number as the new value of N.
                var length = (byte)value + 1;
                value = 0;
                while (length > 0) {
                    // Calculate size of chunk
                    var chunk = Math.Min(length, (byte)(8 - CurrentOffset));

                    // Add to byte
                    var mask = byte.MaxValue;
                    mask <<= 8 - chunk;
                    mask >>= CurrentOffset;
                    value <<= chunk;
                    value += (ulong)(CurrentByte & mask) >> 8 - chunk - CurrentOffset;

                    // Update length available
                    length -= chunk;

                    // Increment offset, and load next byte if required
                    if ((CurrentOffset += chunk) >= 8) {
                        if (!ReadByte()) {
                            throw new InvalidOperationException("Missing body byte.");
                        }
                    }
                }
            }

            // Increment offset for termination bit
            if (CurrentOffset++ >= 8) {
                ReadByte(); // Can be missing final byte without issue
            }

            // Offset for min value
            value = value - 1 + MinValue;

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
        /// Read a byte from the input stream.
        /// </summary>
        /// <returns>TRUE if successful.</returns>
        private bool ReadByte() {
            // Get next byte
            CurrentByte = Input.ReadByte();
            if (CurrentByte < 0) {
                return false;
            }

            // Reset offset
            CurrentOffset = 0;

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
