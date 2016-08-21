using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Elias Omega universal coding for unsigned values.
    /// </summary>
    public class EliasOmegaUnsignedWriter : IUnsignedWriter, IDisposable {
        /// <summary>
        /// Write all given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(IEnumerable<ulong> values) { return WriteAll(0, values); }

        /// <summary>
        /// Write all given values with options.
        /// </summary>
        /// <param name="minValue">Minimum value to support. To match standard use 1.</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(ulong minValue, IEnumerable<ulong> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasOmegaUnsignedWriter(stream, minValue)) {
                    foreach (var value in values) {
                        writer.Write(value);
                    }
                }

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CalculateBitLength(bool allowZero, ulong value) {
            var result = 1; // Termination bit

            // Offset value to allow for 0s
            if (allowZero) {
                value++;
            }

            // #2 If N=1, stop; encoding is complete.
            while (value > 1) {
                // Calculate the length of value
                var length = Bits.CountUsed(value);

                // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
                result += length;

                // #4 Let N equal the number of bits just prepended, minus one.
                value = (ulong)length - 1;
            }

            return result;
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Underlying stream to be writing encoded values to.
        /// </summary>
        private readonly Stream Output;

        /// <summary>
        /// The minimum value supported in this instance.
        /// </summary>
        private readonly ulong MinValue;

        /// <summary>
        /// The byte currently being worked on.
        /// </summary>
        private byte CurrentByte;

        /// <summary>
        /// The position within the current byte for the next write.
        /// </summary>
        private int CurrentPosition;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public EliasOmegaUnsignedWriter(Stream output) : this(output, 0) { }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="minValue">Minimum value to support. To match standard use 1.</param>
        public EliasOmegaUnsignedWriter(Stream output, ulong minValue) {
            Output = output;
            MinValue = minValue;
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Offset value to allow for 0s
            if (value < MinValue) {
                throw new ArgumentOutOfRangeException(value + " (value) is lower than " + MinValue + " (min value).", "value");
            }

            // Offset min value
            value = value - MinValue + 1;

            // Prepare buffer
            var groups = new Stack<KeyValuePair<ulong, byte>>();

            // #1 Place a "0" at the end of the code.
            groups.Push(new KeyValuePair<ulong, byte>(0, 1));

            // #2 If N=1, stop; encoding is complete.
            while (value > 1) {
                // Calculate the length of value
                var length = Bits.CountUsed(value);

                // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
                groups.Push(new KeyValuePair<ulong, byte>(value, length));

                // #4 Let N equal the number of bits just prepended, minus one.
                value = (ulong)length - 1;
            }

            // Write buffer
            foreach (var item in groups) {
                var bits = item.Value;
                var group = item.Key;

                while (bits > 0) {
                    // Calculate size of chunk
                    var chunk = (byte)Math.Min(bits, 8 - CurrentPosition);

                    // Add to byte
                    if (CurrentPosition + bits > 8) {
                        CurrentByte |= (byte)(group >> (bits - chunk));
                    } else {
                        CurrentByte |= (byte)(group << (8 - CurrentPosition - chunk));
                    }

                    // Update length available
                    bits -= chunk;

                    // Detect if byte is full
                    CurrentPosition += chunk;
                    if (CurrentPosition == 8) {
                        // Write byte
                        Output.WriteByte(CurrentByte);

                        // Reset offset
                        CurrentPosition = 0;

                        // Clear byte
                        CurrentByte = 0;
                    }
                }
            }
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

            // Write out final byte if partially used
            if (CurrentPosition > 0) {
                Output.WriteByte(CurrentByte);
            }

            if (disposing) {
                // Dispose managed state (managed objects).
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
