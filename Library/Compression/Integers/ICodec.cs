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

    void EncodeBit(Boolean value, ref UInt64[] buffers, ref Int32 offset);
    void EncodeUInt8(Byte value,  ref UInt64[] buffers, ref Int32 offset);
    void EncodeUInt16(UInt16 value,  ref UInt64[] buffers, ref Int32 offset);
    void EncodeUInt32(UInt32 value,  ref UInt64[] buffers, ref Int32 offset);
    void EncodeUInt64(UInt64 value,  ref UInt64[] buffers, ref Int32 offset);
    void EncodeInt8(SByte value,  ref UInt64[] buffers, ref Int32 offset);
    void EncodeInt16(Int16 value,  ref UInt64[] buffers, ref Int32 offset);
    void EncodeInt32(Int32 value,  ref UInt64[] buffers, ref Int32 offset);
    void EncodeInt64(Int64 value,  ref UInt64[] buffers, ref Int32 offset);

    Boolean DecodeBit( ref UInt64[] buffers, ref Int32 offset);
    Byte DecodeUInt8( ref UInt64[] buffers, ref Int32 offset);
    UInt16 DecodeUInt16( ref UInt64[] buffers, ref Int32 offset);
    UInt32 DecodeUInt32( ref UInt64[] buffers, ref Int32 offset);
    UInt64 DecodeUInt64( ref UInt64[] buffers, ref Int32 offset);
    SByte DecodeInt8( ref UInt64[] buffers, ref Int32 offset);
    Int16 DecodeInt16( ref UInt64[] buffers, ref Int32 offset);
    Int32 DecodeInt32( ref UInt64[] buffers, ref Int32 offset);
    Int64 DecodeInt64( ref UInt64[] buffers, ref Int32 offset);
}