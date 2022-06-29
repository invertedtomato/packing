using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitReaderBuffer
{
     //Boolean Read1();
     Byte Read8(Int32 count);
     //UInt16 Read16(Int32 count);
     //UInt32 Read32(Int32 count);
     UInt64 Read64(Int32 count);

     Boolean Peak1();
}
