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

    void EncodeBit(Boolean value, IBitWriterBuffer buffer);
    void EncodeUInt8(Byte value, IBitWriterBuffer buffer);
    void EncodeUInt16(UInt16 value, IBitWriterBuffer buffer);
    void EncodeUInt32(UInt32 value, IBitWriterBuffer buffer);
    void EncodeUInt64(UInt64 value, IBitWriterBuffer buffer);
    void EncodeInt8(SByte value, IBitWriterBuffer buffer);
    void EncodeInt16(Int16 value, IBitWriterBuffer buffer);
    void EncodeInt32(Int32 value, IBitWriterBuffer buffer);
    void EncodeInt64(Int64 value, IBitWriterBuffer buffer);

    Boolean DecodeBit(IBitReaderBuffer buffer);
    Byte DecodeUInt8(IBitReaderBuffer buffer);
    UInt16 DecodeUInt16(IBitReaderBuffer buffer);
    UInt32 DecodeUInt32(IBitReaderBuffer buffer);
    UInt64 DecodeUInt64(IBitReaderBuffer buffer);
    SByte DecodeInt8(IBitReaderBuffer buffer);
    Int16 DecodeInt16(IBitReaderBuffer buffer);
    Int32 DecodeInt32(IBitReaderBuffer buffer);
    Int64 DecodeInt64(IBitReaderBuffer buffer);
}