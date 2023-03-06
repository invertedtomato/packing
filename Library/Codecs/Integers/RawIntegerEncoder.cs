// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class RawIntegerEncoder : IntegerEncoderBase
{
    private readonly IBitWriter _writer;

    public RawIntegerEncoder(IBitWriter writer)
    {
        _writer = writer;
    }

    protected override void Encode(UInt64 value) => _writer.WriteBits(value, Bits.LongBits);
    
    public override Int32? PredictEncodedBits(UInt64 value) => Bits.LongBits;
}