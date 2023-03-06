using System.Collections.Generic;

// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasOmegaIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;

    public EliasOmegaIntegerDecoder(IBitReader reader)
    {
        _reader = reader;
    }
    
    protected override UInt64 Decode()
    {
        // #1 Start with a variable N, set to a value of 1.
        UInt64 value = 1;

        // #2 If the next bit is a "0", stop. The decoded number is N.
        while (_reader.PeakBit())
        {
            // #3 If the next bit is a "1", then read it plus N more bits, and use that binary number as the new value of N.
            value = _reader.ReadBits((Int32)value + 1);
        }

        // Burn last bit from input
        _reader.ReadBit();

        // Offset for min value
        return value - 1;
    }
}