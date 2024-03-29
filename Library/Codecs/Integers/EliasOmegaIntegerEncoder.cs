using System.Collections.Generic;

// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasOmegaIntegerEncoder : IntegerEncoderBase
{
    private readonly IBitWriter _writer;

    public EliasOmegaIntegerEncoder(IBitWriter writer)
    {
        _writer = writer;
    }
    
    protected override void Encode(UInt64 value)
    {
        // Offset min value
        value++;

        // Prepare buffer
        var groups = new Stack<KeyValuePair<UInt64, Int32>>();

        // #1 Place a "0" at the end of the code.
        groups.Push(new(0, 1));

        // #2 If N=1, stop; encoding is complete.
        while (value > 1)
        {
            // Calculate the length of value
            var length = Bits.CountUsed(value);

            // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
            groups.Push(new(value, length));

            // #4 Let N equal the number of bits just prepended, minus one.
            value = (UInt64)length - 1;
        }

        // Write buffer
        foreach (var item in groups)
        {
            var bits = item.Value;
            var group = item.Key;

            _writer.WriteBits(group, bits);
        }
    }

    public override Int32? PredictEncodedBits(UInt64 value)
    {
        var result = 1; // Termination bit

        // Offset value to allow for 0s
        value++;

        // #2 If N=1, stop; encoding is complete.
        while (value > 1)
        {
            // Calculate the length of value
            var length = Bits.CountUsed(value);

            // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
            result += length;

            // #4 Let N equal the number of bits just prepended, minus one.
            value = (UInt64)length - 1;
        }

        return result;
    }
}