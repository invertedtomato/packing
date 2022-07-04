using System;
using System.Runtime.CompilerServices;

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

    /// <summary>
    /// Add bits to a buffer
    /// </summary>
    /// <param name="targetBuffer"></param>
    /// <param name="targetCount"></param>
    /// <param name="buffer"></param>
    /// <param name="count"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Push(ref UInt64 targetBuffer, ref Int32 targetCount, UInt64 buffer, Int32 count)
    {
        // Remove any stray bits from the provided buffer (ie, if provided with buffer=00000011 and count=1, we need to remove that left-most '1' bit)
        buffer <<= BITS_PER_ULONG - count;

        // Align the buffer ready to be merged
        buffer >>= targetCount;

        // Add to buffer
        targetBuffer |= buffer;
        targetCount += count;
    }

    /// <summary>
    /// Pop bits off of a bugger
    /// </summary>
    /// <param name="targetBuffer"></param>
    /// <param name="targetCount"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 Pop(ref UInt64 targetBuffer, ref Int32 targetCount, Int32 count)
    {
        // Extract byte from buffer and write to underlying
        var d = targetBuffer >> BITS_PER_ULONG - count;

        // Reduce buffer
        targetBuffer <<= count;
        targetCount -= count;

        return d;
    }
}