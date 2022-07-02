using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitWriter
{
     void WriteBit(Boolean value);
     void WriteBits(UInt64 buffer, Int32 count);
     
     void Align();
}