using System.Collections.Generic;

namespace InvertedTomato.Binary.Codecs.Integers;

public class EliasOmegaIntegerCodec : IntegerCodec
{
    public override UInt64 MinValue => UInt64.MinValue;
    public override UInt64 MaxValue => UInt64.MaxValue - 1;

    protected override void Encode(UInt64 value, IBitWriter writer)
    {
        // Offset min value
        value++;

        // Prepare buffer
        var groups = new Stack<KeyValuePair<UInt64, Int32>>();

        // #1 Place a "0" at the end of the code.
        groups.Push(new (0, 1));

        // #2 If N=1, stop; encoding is complete.
        while (value > 1)
        {
            // Calculate the length of value
            var length = Bits.CountUsed(value);

            // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
            groups.Push(new (value, length));

            // #4 Let N equal the number of bits just prepended, minus one.
            value = (UInt64)length - 1;
        }

        // Write buffer
        foreach (var item in groups)
        {
            var bits = item.Value;
            var group = item.Key;

            writer.WriteBits(group, bits);
        }
    }

    protected override UInt64 Decode(IBitReader buffer)
    {
        // #1 Start with a variable N, set to a value of 1.
        UInt64 value = 1;

        // #2 If the next bit is a "0", stop. The decoded number is N.
        while (buffer.PeakBit())
        {
            // #3 If the next bit is a "1", then read it plus N more bits, and use that binary number as the new value of N.
            value = buffer.ReadBits((Int32)value + 1);
        }

        // Burn last bit from input
        buffer.ReadBit();

        // Offset for min value
        return value - 1;
    }

    public override Int32? CalculateEncodedBits(UInt64 value)
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