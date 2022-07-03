using System;

// TODO: Rewrite this! It's probably far messier and inefficent than it needs be!

namespace InvertedTomato.Compression.Integers
{
    public class FibonacciCodec : ICodec
    {
        public UInt64 MinValue => UInt64.MinValue;
        public UInt64 MaxValue => UInt64.MaxValue - 1;

        /// <summary>
        /// Lookup table of Fibonacci numbers that can fit in a ulong.
        /// </summary>
        private static readonly UInt64[] Lookup =
        {
            1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657,
            46368, 75025, 121393, 196418, 317811, 514229, 832040, 1346269, 2178309, 3524578, 5702887, 9227465, 14930352,
            24157817, 39088169, 63245986, 102334155, 165580141, 267914296, 433494437, 701408733, 1134903170, 1836311903,
            2971215073, 4807526976, 7778742049, 12586269025, 20365011074, 32951280099, 53316291173, 86267571272,
            139583862445, 225851433717, 365435296162, 591286729879, 956722026041, 1548008755920, 2504730781961,
            4052739537881, 6557470319842, 10610209857723, 17167680177565, 27777890035288, 44945570212853,
            72723460248141, 117669030460994, 190392490709135, 308061521170129, 498454011879264, 806515533049393,
            1304969544928657, 2111485077978050, 3416454622906707, 5527939700884757, 8944394323791464, 14472334024676221,
            23416728348467685, 37889062373143906, 61305790721611591, 99194853094755497, 160500643816367088,
            259695496911122585, 420196140727489673, 679891637638612258, 1100087778366101931, 1779979416004714189,
            2880067194370816120, 4660046610375530309, 7540113804746346429, 12200160415121876738
        };

        /// <summary>
        /// The most significant bit in a byte.
        /// </summary>
        private const Byte MSB = 0x80;

        /// <summary>
        /// Maximum possible length of an encoded symbol.
        /// </summary>
        private const Int32 MAX_ENCODED_LENGTH = 12;

        private void Encode(UInt64 value, IBitWriter writer)
        {
            // Allocate working buffer the max length of an encoded UInt64 + 1 byte
            var buffer = new Byte[MAX_ENCODED_LENGTH];
            var bitOffset = 0;

            var residualBits = 0;
            var maxByte = 0;

#if DEBUG
            // Check for overflow
            if (value > MaxValue)
            {
                throw new OverflowException("Exceeded FibonacciCodec maximum supported symbol value of " + MaxValue + ".");
            }
#endif

            // Reset size for next symbol
            maxByte = -1;

            // Fibonacci doesn't support 0s, so add 1 to allow for them
            value++;

            // #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
            // #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
            for (var fibIdx = Lookup.Length - 1; fibIdx >= 0; fibIdx--)
            {
                // #2 If the number subtracted was the ith Fibonacci number F(i), put a 1 in place i−2 in the code word (counting the left most digit as place 0).
                if (value >= Lookup[fibIdx])
                {
                    // Calculate offsets
                    var adjustedIdx = fibIdx + bitOffset;
                    var byteIdx = adjustedIdx / 8;
                    var bitIdx = adjustedIdx % 8;

                    // If this is the termination fib, add termination bit
                    if (-1 == maxByte)
                    {
                        // Note parameters for this symbol
                        maxByte = (adjustedIdx + 1) / 8;
                        residualBits = (adjustedIdx + 2) % 8; // Add two bits being written

                        // Append bits to output
                        var terminationByteIdx = (adjustedIdx + 1) / 8;
                        var terminationBitIdx = (adjustedIdx + 1) % 8;
                        buffer[terminationByteIdx] |= (Byte) (0x01 << 7 - terminationBitIdx); // Termination bit
                    }

                    // Flag that fib is used
                    buffer[byteIdx] |= (Byte) (0x01 << 7 - bitIdx); // Flag bit

                    // Deduct Fibonacci number from value
                    value -= Lookup[fibIdx];
                }
            }

            // Write n-1 output bytes
            for (var j = 0; j < maxByte; j++)
            {
                writer.WriteBits(buffer[j], 8);
                buffer[j] = 0;
            }

            // Write last byte if complete, or no more symbols to encode
            if (residualBits == 0)
            {
                return;
            }
            else if (maxByte > 0)
            {
                buffer[0] = buffer[maxByte];
                buffer[maxByte] = 0;
            }

            bitOffset = residualBits;
        }

        private UInt64 Decode(IBitReader buffer)
        {
            // Current symbol being decoded
            UInt64 symbol = 0;

            // Next Fibonacci number to test
            var nextFibIndex = 0;

            // State of the last bit while decoding
            var lastBit = false;


            while (true)
            {
                // Read byte of input, and throw error if unavailable
                var b = buffer.ReadBits(8);

                // For each bit of buffer
                for (var bi = 0; bi < 8; bi++)
                {
                    // If bit is set...
                    if (((b << bi) & MSB) > 0)
                    {
                        // If double 1 bits
                        if (lastBit)
                        {
                            // Remove zero offset
                            symbol--;

                            // Add to output
                            return symbol;
                        }

#if DEBUG
                        // Check for overflow
                        if (nextFibIndex >= Lookup.Length)
                        {
                            throw new OverflowException("Value too large to decode. Max 64bits supported."); // TODO: Handle this so that it doesn't allow for DoS attacks!
                        }
#endif

                        // Add value to current symbol
                        var pre = symbol;
                        symbol += Lookup[nextFibIndex];
#if DEBUG
                        // Check for overflow
                        if (symbol < pre)
                        {
                            // TODO: Support full 64bit
                            throw new OverflowException("Input symbol larger than the supported limit of 64bits. Possible data issue.");
                        }
#endif

                        // Note bit for next cycle
                        lastBit = true;
                    }
                    else
                    {
                        // Note bit for next cycle
                        lastBit = false;
                    }

                    // Increment bit position
                    nextFibIndex++;
                }
            }
        }

        public Int32? CalculateEncodedBits(UInt64 value)
        {
            // Check for overflow
            if (value > MaxValue)
            {
                return null;
            }

            // Offset for zero
            value++;

            for (var i = Lookup.Length - 1; i >= 0; i--)
            {
                if (value >= Lookup[i])
                {
                    return i + 1;
                }
            }

            return 0;
        }

        public void EncodeBit(bool value, IBitWriter buffer) => Encode(1, buffer);
        public void EncodeUInt8(byte value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt16(ushort value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt32(uint value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt64(ulong value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeInt8(sbyte value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt16(short value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt32(int value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt64(long value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);

        public Boolean DecodeBit(IBitReader buffer) => Decode(buffer) > 0;
        public Byte DecodeUInt8(IBitReader buffer) => (Byte) Decode(buffer);
        public UInt16 DecodeUInt16(IBitReader buffer) => (UInt16) Decode(buffer);
        public UInt32 DecodeUInt32(IBitReader buffer) => (UInt32) Decode(buffer);
        public UInt64 DecodeUInt64(IBitReader buffer) => Decode(buffer);
        public SByte DecodeInt8(IBitReader buffer) => (SByte) ZigZag.Decode(Decode(buffer));
        public Int16 DecodeInt16(IBitReader buffer) => (Int16) ZigZag.Decode(Decode(buffer));
        public Int32 DecodeInt32(IBitReader buffer) => (Int32) ZigZag.Decode(Decode(buffer));
        public Int64 DecodeInt64(IBitReader buffer) => ZigZag.Decode(Decode(buffer));
    }
}