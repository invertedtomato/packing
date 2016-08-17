using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
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
    public class EliasOmegaUnsignedWriter : IUnsignedWriter, IDisposable {
        /// <summary>
        /// Write all given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(IEnumerable<ulong> values) { return WriteAll(false, values); }

        /// <summary>
        /// Write all given values with options.
        /// </summary>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(bool allowZeros, IEnumerable<ulong> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasOmegaUnsignedWriter(stream, allowZeros)) {
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
        /// If value offsetting is enabled so that zero can be supported.
        /// </summary>
        private readonly bool AllowZeros;

        /// <summary>
        /// The byte currently being worked on.
        /// </summary>
        private byte CurrentByte;

        /// <summary>
        /// The position within the current byte for the next write.
        /// </summary>
        private int CurrrentPosition;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public EliasOmegaUnsignedWriter(Stream output) : this(output, false) { }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        public EliasOmegaUnsignedWriter(Stream output, bool allowZeros) {
            Output = output;
            AllowZeros = allowZeros;
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
            if (AllowZeros) {
                if (value > ulong.MaxValue - 1) {
                    throw new ArgumentOutOfRangeException("Value must be less than ulong.MaxValue-1 when AllowZeros is enabled in constructor.");
                }

                value++;
            } else if (value == 0) {
                throw new ArgumentOutOfRangeException("Zeros are not permitted without AllowZeros enabled in constructor.");
            }

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
                    var chunk = (byte)Math.Min(bits, 8 - CurrrentPosition);

                    // Add to byte
                    if (CurrrentPosition + bits > 8) {
                        CurrentByte |= (byte)(group >> (bits - chunk));
                    } else {
                        CurrentByte |= (byte)(group << (8 - CurrrentPosition - chunk));
                    }

                    // Update length available
                    bits -= chunk;

                    // Detect if byte is full
                    CurrrentPosition += chunk;
                    if (CurrrentPosition == 8) {
                        // Write byte
                        Output.WriteByte(CurrentByte);

                        // Reset offset
                        CurrrentPosition = 0;

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
            if (CurrrentPosition > 0) {
                Output.WriteByte(CurrentByte);
            }

            if (disposing) {
                // Dispose managed state (managed objects).
            }

            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
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
