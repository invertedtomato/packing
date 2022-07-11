using System;

namespace InvertedTomato.Compression.Integers.Gen3;

public interface IBitWriter
{
    void WriteBit(Boolean value);
    void WriteBits(UInt64 bits, Int32 count);

    void Align();
}