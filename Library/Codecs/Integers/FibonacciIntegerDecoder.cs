// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class FibonacciIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;

    public FibonacciIntegerDecoder(IBitReader reader)
    {
        _reader = reader;
    }
    
    protected override UInt64 Decode()
    {
        // Current symbol being decoded
        UInt64 symbol = 0;

        // State of the last bit while decoding
        var lastBit = false;

        // Loop through each possible fib
        foreach (var fib in FibonacciInteger.Table)
        {
            // Read bit of input
            var bit = _reader.ReadBit();
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
                    throw new OverflowException($"Symbol is larger than the max value of {FibonacciInteger.MaxValue}. Data is probably corrupt");
                }
#endif
            }

            // Note bit for next cycle
            lastBit = bit;
        }

        // If double 1 bits - all done! Return symbol less zero offset (this occurs only when decoding MaxValue)
        if (lastBit && _reader.ReadBit()) return symbol - 1;

        // Input longer than supported
        throw new OverflowException($"Termination not found within supported {FibonacciInteger.Table.Length} bit range. Data is probably corrupt.");
    }
}