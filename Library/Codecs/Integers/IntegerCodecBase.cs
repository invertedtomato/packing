// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public abstract class IntegerCodec
{
    /// <summary>
    /// Minimum value this codec can support.
    /// </summary>
    public abstract UInt64 MinValue { get; }

    /// <summary>
    /// The maximum value of a symbol this codec can support.
    /// </summary>
    public abstract UInt64 MaxValue { get; }

    protected abstract void Encode(UInt64 value, IBitWriter writer);

    protected abstract UInt64 Decode(IBitReader reader);

    /// <summary>
    /// Predict how many bits would be used to encode a given value
    /// </summary>
    public abstract Int32? CalculateEncodedBits(UInt64 value);
    
    public void EncodeBit(Boolean value, IBitWriter buffer) => Encode(value ? 1UL : 0UL, buffer);
    public void EncodeUInt8(Byte value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeUInt16(UInt16 value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeUInt32(UInt32 value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeUInt64(UInt64 value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeInt8(SByte value, IBitWriter buffer) => Encode(ZigZagUtility.Encode(value), buffer);
    public void EncodeInt16(Int16 value, IBitWriter buffer) => Encode(ZigZagUtility.Encode(value), buffer);
    public void EncodeInt32(Int32 value, IBitWriter buffer) => Encode(ZigZagUtility.Encode(value), buffer);
    public void EncodeInt64(Int64 value, IBitWriter buffer) => Encode(ZigZagUtility.Encode(value), buffer);

    public Boolean DecodeBit(IBitReader buffer) => Decode(buffer) > 0;
    public Byte DecodeUInt8(IBitReader buffer) => (Byte)Decode(buffer);
    public UInt16 DecodeUInt16(IBitReader buffer) => (UInt16)Decode(buffer);
    public UInt32 DecodeUInt32(IBitReader buffer) => (UInt32)Decode(buffer);
    public UInt64 DecodeUInt64(IBitReader buffer) => Decode(buffer);
    public SByte DecodeInt8(IBitReader buffer) => (SByte)ZigZagUtility.Decode(Decode(buffer));
    public Int16 DecodeInt16(IBitReader buffer) => (Int16)ZigZagUtility.Decode(Decode(buffer));
    public Int32 DecodeInt32(IBitReader buffer) => (Int32)ZigZagUtility.Decode(Decode(buffer));
    public Int64 DecodeInt64(IBitReader buffer) => ZigZagUtility.Decode(Decode(buffer));
}