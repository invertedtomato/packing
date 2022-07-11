using System;

namespace InvertedTomato.Compression.Integers.Gen3;

public interface IBitReader
{
     Boolean PeakBit();
     Boolean ReadBit();
     UInt64 ReadBits(Int32 count);
     
     void Align();
}
