using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.VariableLengthIntegers {
    public class UnsignedOmega {
        // https://en.wikipedia.org/wiki/Elias_omega_coding
        // http://www.firstpr.com.au/audiocomp/lossless/TechRep137.pdf
        // http://www.dupuis.me/node/39 Didn't like the implimentation


        public void Encode(ulong value, Action<byte, bool> write, ref int offset) {
            // #1 Place a "0" at the end of the code.
            // #2 If N=1, stop; encoding is complete.
            // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
            // #4 Let N equal the number of bits just prepended, minus one.
            // #5 Return to step 2 to prepend the encoding of the new N.

            /*
            var t = new BitArray(8192);
            var l = 0;
            while (value > 1) {
                int len = 0;
                for (var temp = value; temp > 0; temp >>= 1) {  // calculate 1+floor(log2(num))
                    len++;
                }
                for (int i = 0; i < len; i++) {
                    t[l++] = ((value >> i) & 1) != 0;
                }
                value = len - 1;
            }
            for (--l; l >= 0; --l) WriteBit(t[l]);
            
            WriteBit(false);*/
        }

        public void Encode(ulong value, byte[] output, ref int position, ref int offset) {
            var innerPosition = position;

            Encode(value, (b, move) => {
                output[innerPosition] |= b;
                if (move) {
                    innerPosition++;
                }
            }, ref offset);

            position = innerPosition;
        }

        public void Encode(ulong value, Stream output, ref int offset) {
            Encode(value, (b, move) => {
                var a = output.ReadByte();
                output.Position--;
                output.WriteByte((byte)(a | b));
                if (!move) {
                    output.Position--;
                }
            }, ref offset);
        }

        public byte[] Encode(ulong value) {
            // Encode to buffer
            var buffer = new byte[10];
            var position = 0;
            var offset = 0;

            Encode(value, (b, move) => {
                buffer[position] |= b;
                if (move) {
                    position++;
                }
            }, ref offset);

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
    }
}
