using System;

namespace InvertedTomato.Compression.Integers;

public interface ICodec
{
    /// <summary>
    /// Minimum value this codec can support.
    /// </summary>
    UInt64 MinValue { get; }

    /// <summary>
    /// The maximum value of a symbol this codec can support.
    /// </summary>
    UInt64 MaxValue { get; }

    void EncodeBit(Boolean value, IBitWriter buffer);
    void EncodeUInt8(Byte value, IBitWriter buffer);
    void EncodeUInt16(UInt16 value, IBitWriter buffer);
    void EncodeUInt32(UInt32 value, IBitWriter buffer);
    void EncodeUInt64(UInt64 value, IBitWriter buffer);
    void EncodeInt8(SByte value, IBitWriter buffer);
    void EncodeInt16(Int16 value, IBitWriter buffer);
    void EncodeInt32(Int32 value, IBitWriter buffer);
    void EncodeInt64(Int64 value, IBitWriter buffer);

    Boolean DecodeBit(IBitReader buffer);
    Byte DecodeUInt8(IBitReader buffer);
    UInt16 DecodeUInt16(IBitReader buffer);
    UInt32 DecodeUInt32(IBitReader buffer);
    UInt64 DecodeUInt64(IBitReader buffer);
    SByte DecodeInt8(IBitReader buffer);
    Int16 DecodeInt16(IBitReader buffer);
    Int32 DecodeInt32(IBitReader buffer);
    Int64 DecodeInt64(IBitReader buffer);

    /// <summary>
    /// Predict how many bits would be used to encode a given value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Number of bits that would be used to encode value, or NULL if value is not supported</returns>
    Int32? CalculateEncodedBits(UInt64 value);
}