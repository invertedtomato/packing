using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitWriter
{
     void Write(UInt64 bits, Int32 count);
     void Align();
     void Flush();
}