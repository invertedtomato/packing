using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitReader
{
     /// <summary>
     /// Max number of bits that can be read in a single operation
     /// </summary>
     Int32 MaxBits { get; } // This must always be more than 32
     
     Boolean PeakBit();
     Boolean ReadBit();
     UInt64 ReadBits(Int32 count);
     
     void Align();
}
