using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitReader
{
     UInt64 ReadBits(Int32 count);

     Boolean PeakBit();
}
