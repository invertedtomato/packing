using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitWriter
{
     /// <summary>
     /// Max number of bits that can be read in a single operation
     /// </summary>
     Int32 MaxBits { get; } // This must always be more than 32
     
     void WriteBit(Boolean value);
     void WriteBits(UInt64 buffer, Int32 count);
     
     void Align();
}