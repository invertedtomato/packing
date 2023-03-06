// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasGammaIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;

    public EliasGammaIntegerDecoder(IBitReader reader)
    {
        _reader = reader;
    }
    
    protected override UInt64 Decode()
    {
        // Read length
        var length = 1;
        while (!_reader.PeakBit())
        {
            // Note that length is one bit longer
            length++;

            // Remove 0 from input
            _reader.ReadBit();
        }

        // Read value
        var value = _reader.ReadBits(length);

        // Remove offset from value
        value--;

        return value;
    }
}