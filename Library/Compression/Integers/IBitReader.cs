using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitReader
{
     Boolean PeakBit();
     Boolean ReadBit();
     UInt64 ReadBits(Int32 count); // Max of 32 bits
     
     void Align();
}
