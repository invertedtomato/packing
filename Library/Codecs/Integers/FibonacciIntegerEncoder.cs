// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class FibonacciIntegerEncoder : IntegerEncoderBase
{
    private const UInt64 One = 1;

    private readonly IBitWriter _writer;

    public FibonacciIntegerEncoder(IBitWriter writer)
    {
        _writer = writer;
    }

    protected override void Encode(UInt64 value)
    {
#if DEBUG
        // Check for overflow
        if (value > FibonacciInteger.MaxValue) throw new OverflowException($"Exceeded FibonacciCodec maximum supported symbol value of {FibonacciInteger.MaxValue}.");
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
        for (var i = FibonacciInteger.Table.Length - 1; i >= 0; i--)
        {
            // Do nothing if not a fib match
            if (value < FibonacciInteger.Table[i]) continue;

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
            value -= FibonacciInteger.Table[i];
        }

        // Write out buffers
        for (a = 0; a < buffers!.Length; a++) _writer.WriteBits(buffers[a], counts![a]);
    }

    public override Int32? PredictEncodedBits(UInt64 value)
    {
        // Check for overflow
        if (value > FibonacciInteger.MaxValue)
        {
            return null;
        }

        // Offset for zero
        value++;

        for (var i = FibonacciInteger.Table.Length - 1; i >= 0; i--)
        {
            if (value >= FibonacciInteger.Table[i])
            {
                return i + 1;
            }
        }

        return 0;
    }
}