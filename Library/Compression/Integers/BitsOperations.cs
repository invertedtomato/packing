using System;

namespace InvertedTomato.Compression.Integers;

public static class BitOperation
{
    public const Int32 BITS_PER_BYTE = 8;
    public const Int32 BITS_PER_ULONG = 64;

    /// <summary>
    /// Count the number of bits used to express number.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Int32 CountUsed(UInt64 value)
    {
        Byte bits = 0;

        do
        {
            bits++;
            value >>= 1;
        } while (value > 0);

        return bits;
    }

    public static void Push(ref UInt64 dstBuffer, ref Int32 dstCount, UInt64 srcBuffer, Int32 srcCount)
    {
        // Remove any stray bits from the provided buffer (ie, if provided with buffer=00000011 and count=1, we need to remove that left-most '1' bit)
        srcBuffer <<= BITS_PER_ULONG - srcCount;

        // Align the buffer ready to be merged
        srcBuffer >>= dstCount;

        // Add to buffer
        dstBuffer |= srcBuffer;
        dstCount += srcCount;
    }

    public static UInt64 Pop(ref UInt64 dstBuffer, ref Int32 dstCount, Int32 srcBuffer)
    {
        // Extract byte from buffer and write to underlying
        var d = dstBuffer >> BITS_PER_ULONG - srcBuffer;

        // Reduce buffer
        dstBuffer <<= srcBuffer;
        dstCount -= srcBuffer;

        return d;
    }
}