using System;
using System.Text;

namespace InvertedTomato.Compression.Integers;

public interface ICodec
{
    /// <summary>
    /// Takes a set of buffers and the number of bits already used in them, and returns the number of bits added
    /// </summary>
    /// <param name="buffers"></param>
    /// <param name="offset"></param>
    /// <returns></returns>


    Int64 WastedBits { get; }

    Int64 OverheadBits { get; }
    Int64 DataBits { get; }

    void ResetStatistics();

    UInt64 EncodeUInt8(Byte value, UInt64[] buffers, UInt64 offset);
    UInt64 EncodeUInt16(UInt16 value, UInt64[] buffers, UInt64 offset);
    UInt64 EncodeUInt32(UInt32 value, UInt64[] buffers, UInt64 offset);
    UInt64 EncodeUInt64(UInt64 value, UInt64[] buffers, UInt64 offset);

    UInt64 EncodeInt8(SByte value, UInt64[] buffers, UInt64 offset);
    UInt64 EncodeInt16(Int16 value, UInt64[] buffers, UInt64 offset);
    UInt64 EncodeInt32(Int32 value, UInt64[] buffers, UInt64 offset);
    UInt64 EncodeInt64(Int64 value, UInt64[] buffers, UInt64 offset);

    SByte DecodeUInt8(UInt64[] buffers, UInt64 offset);
    UInt16 DecodeUInt16(UInt64[] buffers, UInt64 offset);
    UInt32 DecodeUInt32(UInt64[] buffers, UInt64 offset);
    UInt64 DecodeUInt64(UInt64[] buffers, UInt64 offset);

    Byte DecodeInt8(UInt64[] buffers, UInt64 offset);
    Int16 DecodeInt16(UInt64[] buffers, UInt64 offset);
    Int32 DecodeInt32(UInt64[] buffers, UInt64 offset);
    Int64 DecodeInt64(UInt64[] buffers, UInt64 offset);


    // TODO: Stats
}