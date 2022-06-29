using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitReader
{
     UInt64 Read(Int32 count);

     Boolean PeakBit();
}
