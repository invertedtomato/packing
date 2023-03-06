// ReSharper disable UnusedMember.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public abstract class IntegerEncoderBase
{
    protected abstract void Encode(UInt64 value);

    public void EncodeBit(Boolean value) => Encode(value ? 1UL : 0UL);
    public void EncodeUInt8(Byte value) => Encode(value);
    public void EncodeUInt16(UInt16 value) => Encode(value);
    public void EncodeUInt32(UInt32 value) => Encode(value);
    public void EncodeUInt64(UInt64 value) => Encode(value);
    public void EncodeInt8(SByte value) => Encode(ZigZagUtility.Encode(value));
    public void EncodeInt16(Int16 value) => Encode(ZigZagUtility.Encode(value));
    public void EncodeInt32(Int32 value) => Encode(ZigZagUtility.Encode(value));
    public void EncodeInt64(Int64 value) => Encode(ZigZagUtility.Encode(value));

    public abstract Int32? PredictEncodedBits(UInt64 value);
}