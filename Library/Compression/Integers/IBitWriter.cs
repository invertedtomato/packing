using System;

namespace InvertedTomato.Compression.Integers;

public interface IBitWriter
{
     void WriteBit(Boolean bit);
     void WriteBits(UInt64 bits, Int32 count); // TODO: Write unit tests
     
     void WriteByte(Byte b); // TODO: Write tests
     
     void Align(); // TODO: Write tests
}