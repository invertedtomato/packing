// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasGammaIntegerEncoder : IntegerEncoderBase
{
    private readonly IBitWriter _writer;

    public EliasGammaIntegerEncoder(IBitWriter writer)
    {
        _writer = writer;
    }
    
    protected override void Encode(UInt64 value)
    {
        // Offset value to allow zeros
        value++;

        // Calculate length
        var length = Bits.CountUsed(value);

        // Write unary zeros
        _writer.WriteBits(0, length - 1);

        // Write value
        _writer.WriteBits(value, length);
    }

    public override Int32? PredictEncodedBits(UInt64 value)
    {
        // Offset for zero
        value++;

        return Bits.CountUsed(value) * 2 - 1;
    }
}