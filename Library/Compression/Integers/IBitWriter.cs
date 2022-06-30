using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitWriter
{
     void WriteBits(UInt64 bits, Int32 count);
     void Flush();
}