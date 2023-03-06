namespace InvertedTomato.Packing.Codecs.Integers;

public class RawIntegerCodec : IntegerCodec
{
    public override UInt64 MinValue => UInt64.MinValue;
    public override UInt64 MaxValue => UInt64.MaxValue;

    public override Int32? CalculateEncodedBits(UInt64 value) => Bits.LongBits;

    protected override void Encode(UInt64 value, IBitWriter buffer) => buffer.WriteBits(value, Bits.LongBits);

    protected override UInt64 Decode(IBitReader buffer) => buffer.ReadBits(Bits.LongBits);
}