// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class FibonacciIntegerCodec : IntegerCodec
{
    private const UInt64 One = 1;
    public override UInt64 MinValue => UInt64.MinValue;
    public override UInt64 MaxValue => UInt64.MaxValue - 1;

    /// <summary>
    /// Lookup table of Fibonacci numbers that can fit in a UInt64
    /// </summary>
    private static readonly UInt64[] FibonacciTable =
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
        2880067194370816120, 4660046610375530309, 7540113804746346429, 12200160415121876738,
    };

    protected override void Encode(UInt64 value, IBitWriter writer)
    {
#if DEBUG
        // Check for overflow
        if (value > MaxValue) throw new OverflowException($"Exceeded FibonacciCodec maximum supported symbol value of {MaxValue}.");
#endif

        // Fibonacci doesn't support 0s, so offset by 1 to allow for them
        value++;

        // #1 Find the largest Fibonacci number equal to or less than N; subtract this number from N, keeping track of the remainder.
        // #3 Repeat the previous steps, substituting the remainder for N, until a remainder of 0 is reached.
        UInt64[]? buffers = null;
        Int32[]? counts = null;
        Int32 a;
        // ReSharper disable once TooWideLocalVariableScope
        Int32 b;
        for (var i = FibonacciTable.Length - 1; i >= 0; i--)
        {
            // Do nothing if not a fib match
            if (value < FibonacciTable[i]) continue;

            // If this is the first fib match...
            if (buffers == null)
            {
                // Calculate the total bit count
                var totalCount = i + 2; // The current index, add one to make it a count, and add another one for the termination bit

                // Allocate buffers
                buffers = new UInt64[totalCount / Bits.LongBits + 1];
                counts = new Int32[totalCount / Bits.LongBits + 1];

                // Calculate the count of bits for each buffer
                for (var j = 0; j < counts.Length; j++)
                {
                    counts[j] = Math.Min(totalCount, Bits.LongBits);
                    totalCount -= counts[j];
                }

                // Calculate address for termination bit
                a = (i + 1) / Bits.LongBits;

                // Set termination bit
                buffers[a] |= One;
            }

            // Calculate address
            a = i / Bits.LongBits;
            b = counts![a] - i - 1;

            // Write to buffer
            buffers[a] |= One << b;

            // Deduct Fibonacci number from value
            value -= FibonacciTable[i];
        }

        // Write out buffers
        for (a = 0; a < buffers!.Length; a++) writer.WriteBits(buffers[a], counts![a]);
    }

    protected override UInt64 Decode(IBitReader reader)
    {
        // Current symbol being decoded
        UInt64 symbol = 0;

        // State of the last bit while decoding
        var lastBit = false;

        // Loop through each possible fib
        foreach (var fib in FibonacciTable)
        {
            // Read bit of input
            var bit = reader.ReadBit();
            if (bit)
            {
                // If double 1 bits - all done! Return symbol less zero offset
                if (lastBit) return symbol - 1;

                // Add value to current symbol
                var pre = symbol;
                symbol += fib;
#if DEBUG
                if (symbol < pre)
                {
                    // Input is larger than expected
                    throw new OverflowException($"Symbol is larger than the max value of {MaxValue}. Data is probably corrupt");
                }
#endif
            }

            // Note bit for next cycle
            lastBit = bit;
        }

        // If double 1 bits - all done! Return symbol less zero offset (this occurs only when decoding MaxValue)
        if (lastBit && reader.ReadBit()) return symbol - 1;

        // Input longer than supported
        throw new OverflowException($"Termination not found within supported {FibonacciTable.Length} bit range. Data is probably corrupt.");
    }

    public override Int32? CalculateEncodedBits(UInt64 value)
    {
        // Check for overflow
        if (value > MaxValue)
        {
            return null;
        }

        // Offset for zero
        value++;

        for (var i = FibonacciTable.Length - 1; i >= 0; i--)
        {
            if (value >= FibonacciTable[i])
            {
                return i + 1;
            }
        }

        return 0;
    }
}