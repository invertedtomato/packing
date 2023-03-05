namespace InvertedTomato.Binary;

public interface IBitWriter
{
    void WriteBit(Boolean value);
    void WriteBits(UInt64 bits, Int32 count);

    void Align();
}