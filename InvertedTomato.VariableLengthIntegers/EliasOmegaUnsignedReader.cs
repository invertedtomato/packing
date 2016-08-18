using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Elias Omega universal coding for unsigned values. Using this coding scheme multiple values can be expressed in one byte, and there is no wastage to padding bits. 
    /// 
    /// Following are some example codings:
    /// 
    ///      VALUE  ENCODED
    ///          1  0_______
    ///          2  100_____  
    ///          3  110_____  
    ///          4  101000__  
    ///          5  101010__
    ///          6  101110__  
    ///          7  1110000_  
    ///         15  1111110_  
    ///         16  10100100 000_____ 
    ///         32  10101100 0000____ 
    ///        100  10110110 01000___ 
    ///       1000  11100111 11101000 0_______ 
    ///     10,000  11110110 01110001 00000___ 
    ///    100,000  10100100 00110000 11010100 0000____ 
    ///  1,000,000  10100100 11111101 00001001 0000000_
    ///    
    /// Normally under Elias zeros cannot be encoded. Passing TRUE into the constructor implements a value offset for all values so that this becomes possible. Following
    /// are examples with AllowZeros enabled:
    /// 
    ///      VALUE  ENCODED
    ///          0  0_______
    ///          1  100_____  
    ///          2  110_____  
    ///          3  101000__  
    ///          4  101010__
    ///          5  101110__  
    ///          6  1110000_  
    ///         14  1111110_  
    ///         15  10100100 000_____ 
    ///         31  10101100 0000____ 
    ///         99  10110110 01000___ 
    ///        999  11100111 11101000 0_______ 
    ///      9,999  11110110 01110001 00000___ 
    ///     99,999  10100100 00110000 11010100 0000____ 
    ///    999,999  10100100 11111101 00001001 0000000_
    /// 
    /// For more information on Elias Omega see https://en.wikipedia.org/wiki/Elias_omega_coding. To see how Elias compares to other universal codes, see
    /// https://en.wikipedia.org/wiki/Elias_omega_coding .
    /// </summary>
    public class EliasOmegaUnsignedReader : IUnsignedReader {
        /// <summary>
        /// Read all values in a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(byte[] input) {
            return ReadAll(false, input);
        }

        /// <summary>
        /// Read all values in a byte array with options.
        /// </summary>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> ReadAll(bool allowZeros, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new EliasOmegaUnsignedReader(stream, allowZeros)) {

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
        /// If value offsetting is enabled so that zero can be supported.
        /// </summary>
        private readonly bool AllowZeros;

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
        public EliasOmegaUnsignedReader(Stream input) : this(input, false) { }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        public EliasOmegaUnsignedReader(Stream input, bool allowZeros) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            Input = input;
            AllowZeros = allowZeros;
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
                    value += (ulong)(CurrentByte & mask) >> (8 - chunk - CurrentOffset);

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

            // Offset value to allow for 0s
            if (AllowZeros) {
                value--;
            }

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
