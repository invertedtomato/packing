using System;
using System.Text;

namespace InvertedTomato.Compression.Integers;

public interface ICodec
{
    /*
    Int64 WastedBits { get; }
    Int64 OverheadBits { get; }
    Int64 DataBits { get; }

    void ResetStatistics();
    */

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
}