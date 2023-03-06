namespace InvertedTomato.Packing.Codecs.Integers;

public class RawIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;

    public RawIntegerDecoder(IBitReader reader)
    {
        _reader = reader;
    }
    
    protected override UInt64 Decode() => _reader.ReadBits(Bits.LongBits);
}