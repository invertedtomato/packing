using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitWriterBuffer
{
     //void Write1(Boolean bit);
     //void Write8(Byte bits, Int32 count);
     //void Write16(UInt16 bits, Int32 count);
     //void Write32(UInt32 bits, Int32 count);
     void Write64(UInt64 bits, Int32 count);
}