using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    public class UnsignedOmega {
        // https://en.wikipedia.org/wiki/Elias_omega_coding
        // http://www.firstpr.com.au/audiocomp/lossless/TechRep137.pdf
        // http://www.dupuis.me/node/39 Didn't like the implementation


        public void Encode(ulong value, Func<byte> read, Action<byte> write, Action move, ref int offset) {
            // #1 Place a "0" at the end of the code.
            // #2 If N=1, stop; encoding is complete.
            // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
            // #4 Let N equal the number of bits just prepended, minus one.
            // #5 Return to step 2 to prepend the encoding of the new N.

            // Offset value to allow for 0s
            value++;

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

            // Load current byte (skip if 0 offset - optimization)
            var currentByte = offset > 0 ? read() : (byte)0x00;

            // Write buffer
            foreach (var item in buffer) {
                var len = item.Value;
                var val = item.Key;

                while (len > 0) {
                    // Size of chunk
                    var chunk = (byte)Math.Min(len, 8 - offset);

                    // Add to byte
                    if (offset + len > 8) {
                        currentByte |= (byte)(val >> (len - chunk));
                    } else {
                        currentByte |= (byte)(val << (8 - offset - chunk));
                    }

                    // Update length available
                    len -= chunk;
                    
                    // Detect if byte is full
                    offset += chunk;
                    if (offset == 8) {
                        // Write byte
                        write(currentByte);

                        // Move to next position
                        move();

                        // Reset offset
                        offset = 0;

                        // Clear byte
                        currentByte = 0;
                    }
                }
            }

            if (offset > 0) {
                write(currentByte);
            }
        }

        public void Encode(ulong value, byte[] output, ref int position, ref int offset) {
            var innerPosition = position;

            Encode(value,
                () => {
                    return output[innerPosition];
                },
                (b) => {
                    output[innerPosition] = b;
                },
                () => {
                    innerPosition++;
                },
                ref offset
            );

            position = innerPosition;
        }

        public void Encode(ulong value, Stream output, ref int offset) {
            Encode(value,
                () => {
                    var b = output.ReadByte();
                    if (b < 0) {
                        throw new EndOfStreamException();
                    }
                    return (byte)b;
                },
                (b) => {
                    output.WriteByte(b);
                    output.Position--;
                },
                () => {
                    output.Position++;
                },
                ref offset
            );
        }

        public byte[] Encode(ulong value) {
            // Encode to buffer
            var buffer = new byte[10];
            var position = 0;
            var offset = 0;

            Encode(value,
                () => {
                    return buffer[position];
                },
                (b) => {
                    buffer[position] = b;
                },
                () => {
                    position++;
                },
                ref offset
            );

            // If there's a partial byte at the end, include it in output
            if (offset > 0) {
                position++;
            }

            // Trim unneeded bytes
            var output = new byte[position];
            Buffer.BlockCopy(buffer, 0, output, 0, output.Length);
            return output;
        }



        public ulong Decode(Func<bool, byte> read, ref int offset) {
            // #1 Start with a variable N, set to a value of 1.
            // #2 If the next bit is a "0", stop. The decoded number is N.
            // #3 If the next bit is a "1", then read it plus N more bits, and use that binary number as the new value of N.
            // #4 Go back to step 2.

            throw new NotImplementedException();
        }

        public ulong Decode(byte[] input, ref int position, ref int offset) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            var innerPosition = position;
            var value = Decode((move) => {
                return input[move ? innerPosition++ : innerPosition];
            }, ref offset);
            position = innerPosition;

            return value;
        }

        public ulong Decode(Stream input, ref int offset) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            return Decode((move) => {
                var b = input.ReadByte();
                if (b < 0) {
                    throw new EndOfStreamException();
                }
                if (!move) {
                    input.Position--;
                }
                return (byte)b;
            }, ref offset);
        }

        public ulong Decode(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            var position = 0;
            var offset = 0;
            var value = Decode((move) => {
                return input[move ? position++ : position];
            }, ref offset);

            return value;
        }

        private byte CountBits(ulong value) {
            byte bits = 0;

            do {
                bits++;
                value >>= 1;
            } while (value > 0);

            return bits;
        }
    }
}
