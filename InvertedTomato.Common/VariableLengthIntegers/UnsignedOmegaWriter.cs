using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Implementation of Elias Omega encoding for unsigned values. Optionally (and by default) zeros are permitted by passing TRUE in
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
    public class UnsignedOmegaWriter : IIntegerWriter<ulong>, IDisposable {
        public static byte[] WriteAll(IEnumerable<ulong> values) { return WriteAll(false, values); }
        public static byte[] WriteAll(bool allowZeros, IEnumerable<ulong> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new UnsignedOmegaWriter(stream, allowZeros)) {
                    foreach (var value in values) {
                        writer.Write(value);
                    }
                }

                return stream.ToArray();
            }
        }

        public bool IsDisposed { get; private set; }
        private readonly Stream Output;
        private readonly bool AllowZeros;
        private byte CurrentByte;
        private int CurrentOffset;

        public UnsignedOmegaWriter(Stream output) : this(output, false) { }

        public UnsignedOmegaWriter(Stream output, bool allowZeros) {
            Output = output;
            AllowZeros = allowZeros;
        }

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
            var buffer = new Stack<KeyValuePair<ulong, byte>>();

            // #1 Place a "0" at the end of the code.
            buffer.Push(new KeyValuePair<ulong, byte>(0, 1));

            // #2 If N=1, stop; encoding is complete.
            while (value > 1) {
                // Calculate the length of value
                var length = CountBits(value);

                // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
                buffer.Push(new KeyValuePair<ulong, byte>(value, length));

                // #4 Let N equal the number of bits just prepended, minus one.
                value = (ulong)length - 1;
            }

            // Write buffer
            foreach (var item in buffer) {
                var bits = item.Value;
                var group = item.Key;

                while (bits > 0) {
                    // Calculate size of chunk
                    var chunk = (byte)Math.Min(bits, 8 - CurrentOffset);

                    // Add to byte
                    if (CurrentOffset + bits > 8) {
                        CurrentByte |= (byte)(group >> (bits - chunk));
                    } else {
                        CurrentByte |= (byte)(group << (8 - CurrentOffset - chunk));
                    }

                    // Update length available
                    bits -= chunk;

                    // Detect if byte is full
                    CurrentOffset += chunk;
                    if (CurrentOffset == 8) {
                        // Write byte
                        Output.WriteByte(CurrentByte);

                        // Reset offset
                        CurrentOffset = 0;

                        // Clear byte
                        CurrentByte = 0;
                    }
                }
            }
        }

        private byte CountBits(ulong value) {
            byte bits = 0;

            do {
                bits++;
                value >>= 1;
            } while (value > 0);

            return bits;
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            // Write out final byte if partially used
            if (CurrentOffset > 0) {
                Output.WriteByte(CurrentByte);
            }

            if (disposing) {
                // Dispose managed state (managed objects).
            }

            // Free unmanaged resources (unmanaged objects) and override a finalizer below.
            // Set large fields to null.
        }

        public void Dispose() {
            Dispose(true);
        }
    }
}
