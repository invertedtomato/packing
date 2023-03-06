// ReSharper disable MemberCanBePrivate.Global

namespace InvertedTomato.Binary.Codecs.Integers;

public class ThompsonAlphaIntegerCodec : IntegerCodec
{
    public override UInt64 MinValue => UInt64.MinValue;
    public override UInt64 MaxValue => UInt64.MaxValue >> Bits.LongBits - _lengthBits + 6 - 1; // TODO: Check logic

    private readonly Int32 _lengthBits;

    public ThompsonAlphaIntegerCodec() : this(6)
    {
    }

    /// <summary>
    /// Instantiate with options
    /// </summary>
    /// <param name="lengthBits">Number of prefix bits used to store length.</param>
    public ThompsonAlphaIntegerCodec(Int32 lengthBits)
    {
        if (lengthBits is < 1 or > 6) throw new ArgumentOutOfRangeException($"Must be between 1 and 6, not {lengthBits}.", nameof(lengthBits));

        _lengthBits = lengthBits;
    }

    protected override void Encode(UInt64 value, IBitWriter writer)
    {
        // Offset value to allow zeros
        value++;

        // Count length
        var length = Bits.CountUsed(value);

        // Check not too large
        if (length > (_lengthBits + 2) * 8)
            throw new ArgumentOutOfRangeException($"Value is greater than maximum of {UInt64.MaxValue >> (64 - _lengthBits - 1)}. Increase length bits to support larger numbers.");

        // Clip MSB, it's redundant
        length--;
        value = length == 0 ? 0 : value << (Bits.LongBits - length) >> (Bits.LongBits - length);

        // Write length
        writer.WriteBits(length, _lengthBits);

        // Write number
        writer.WriteBits(value, length);
    }

    protected override UInt64 Decode(IBitReader buffer)
    {
        // Read length
        var length = (Int32)buffer.ReadBits(_lengthBits);

        // Read number (max 32 bits can be written in one operation, so split it over two)
        var value = buffer.ReadBits(length);

        // Recover implied MSB
        value |= (UInt64)1 << length;

        // Remove offset to allow zeros
        value--;

        return value;
    }

    public override Int32? CalculateEncodedBits(UInt64 value)
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