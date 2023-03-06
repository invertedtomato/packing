// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class ThompsonAlphaIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;
    private readonly Int32 _lengthBits;

    public ThompsonAlphaIntegerDecoder(IBitReader reader, Int32 lengthBits)
    {
        if (lengthBits is < 1 or > 6) throw new ArgumentOutOfRangeException($"Must be between 1 and 6, not {lengthBits}.", nameof(lengthBits));

        _reader = reader;
        _lengthBits = lengthBits;
    }

    protected override UInt64 Decode()
    {
        // Read length
        var length = (Int32)_reader.ReadBits(_lengthBits);

        // Read number (max 32 bits can be written in one operation, so split it over two)
        var value = _reader.ReadBits(length);

        // Recover implied MSB
        value |= (UInt64)1 << length;

        // Remove offset to allow zeros
        value--;

        return value;
    }
}