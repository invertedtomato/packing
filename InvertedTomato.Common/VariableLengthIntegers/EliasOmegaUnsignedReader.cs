using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Implementation of Elias Omega encoding for unsigned values. Optionally zeros are permitted by passing TRUE in
    /// the constructor. Keep in mind that doing this breaks standard.
    /// 
    /// Example values with allowZeros enabled:
    /// 
    ///      VALUE  ENCODED
    ///          0  0_______
    ///          1  100_____  
    ///          2  110_____  
    ///          3  101000__  
    ///          6  101110__  
    ///          7  1110000_  
    ///         14  1111110_  
    ///         15  10100100 000_____ 
    ///         31  10101100 0000____ 
    ///         99  10110110 01000___ 
    ///        999  11100111 11101000 0_______ 
    ///      9,999  11110110 01110001 00000___ 
    ///     99,999  10100100 00110000 11010100 0000____ 
    ///    999,999  10100100 11111101 00001001 0000000_
    /// 
    /// For more information on Elias Omega see https://en.wikipedia.org/wiki/Elias_omega_coding.
    /// To see how Elias compares to other universal codes, see https://en.wikipedia.org/wiki/Elias_omega_coding
    /// 
    /// This implementation is loosely based on http://www.dupuis.me/node/39.
    /// </summary>
    public class EliasOmegaUnsignedReader : IUnsignedReader {
        public static IEnumerable<ulong> ReadAll(byte[] input) {
            return ReadAll(false, input);
        }

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


        public bool IsDisposed { get; private set; }
        private readonly bool AllowZeros;
        private readonly Stream Input;
        private int CurrentByte;
        private int CurrentOffset = 8;

        public EliasOmegaUnsignedReader(Stream input) : this(input, false) { }

        public EliasOmegaUnsignedReader(Stream input, bool allowZeros) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            Input = input;
            AllowZeros = allowZeros;
        }

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

        public ulong Read() {
            ulong value;
            if (!TryRead(out value)) {
                throw new EndOfStreamException();
            }
            return value;
        }

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

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects).
            }
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
