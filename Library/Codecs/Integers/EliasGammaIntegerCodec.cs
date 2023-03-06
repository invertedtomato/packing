// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasGammaIntegerCodec : IntegerCodec
{
    public static EliasGammaIntegerCodec Default => new();
    
    public override UInt64 MinValue => UInt64.MinValue;
    public override UInt64 MaxValue => UInt64.MaxValue - 1;

    protected override void Encode(UInt64 value, IBitWriter writer)
    {
        // Offset value to allow zeros
        value++;

        // Calculate length
        var length = Bits.CountUsed(value);

        // Write unary zeros
        writer.WriteBits(0, length - 1);

        // Write value
        writer.WriteBits(value, length);
    }

    protected override UInt64 Decode(IBitReader reader)
    {
        // Read length
        var length = 1;
        while (!reader.PeakBit())
        {
            // Note that length is one bit longer
            length++;

            // Remove 0 from input
            reader.ReadBit();
        }

        // Read value
        var value = reader.ReadBits(length);

        // Remove offset from value
        value--;

        return value;
    }

    public override Int32? CalculateEncodedBits(UInt64 value)
    {
        // Offset for zero
        value++;

        return Bits.CountUsed(value) * 2 - 1;
    }
}