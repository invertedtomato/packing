// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class ThompsonAlphaIntegerEncoder : IntegerEncoderBase
{
    public UInt64 MinValue => UInt64.MinValue;

    public UInt64 MaxValue => IntegerUtil.Pow(2, IntegerUtil.Pow(2, (UInt64)_lengthBits + 1))  - 1; // (2^(2^(bits+1)))-1

    private readonly IBitWriter _writer;
    private readonly Int32 _lengthBits;

    public ThompsonAlphaIntegerEncoder(IBitWriter writer, Int32 lengthBits)
    {
        if (lengthBits is < 1 or > 6) throw new ArgumentOutOfRangeException($"Must be between 1 and 6, not {lengthBits}.", nameof(lengthBits));

        _writer = writer;
        _lengthBits = lengthBits;
    }

    protected override void Encode(UInt64 value)
    {
        if (value > MaxValue) throw new ArgumentOutOfRangeException($"Value is greater than maximum of {MaxValue}. Consider increasing length bits to support larger numbers.");

        // Offset value to allow zeros
        value++;

        // Count length
        var length = Bits.CountUsed(value);

        // Clip MSB, it's redundant
        length--;
        value = length == 0 ? 0 : value << (Bits.LongBits - length) >> (Bits.LongBits - length);

        // Write length
        _writer.WriteBits(length, _lengthBits);

        // Write number
        _writer.WriteBits(value, length);
    }

    public override Int32? PredictEncodedBits(UInt64 value)
    {
        // Offset value to allow zeros
        value++;

        // Count length
        var length = Bits.CountUsed(value);

        // Check not too large
        if (length > (_lengthBits + 2) * 8) return null;

        // Clip MSB, it's redundant
        length--;

        return _lengthBits + length;
    }
}