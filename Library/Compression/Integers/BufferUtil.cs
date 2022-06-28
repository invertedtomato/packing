using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace InvertedTomato.Compression.Integers;

public static class BufferUtil
{
    private const Int32 BUFFER_SIZE = 64; // bits

    /// <summary>
    /// Write _srcCount_ bits from _src_ to _dst_ at given _dstOffset_, updating the _dstOffset_.
    /// </summary>
    /// <param name="src"></param>
    /// <param name="dst"></param>
    /// <param name="dstOffset"></param>
    /// <param name="count"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBits(ulong src, ref ulong[] dst, ref int dstOffset, int count)
    {
        // Iterate through each destination buffer instance after the offset
        for (var dstIndex = dstOffset / BUFFER_SIZE; dstIndex < dst.Length; dstIndex++)
        {
            // Calculate the write offset within this destination buffer instance
            var dstInstanceOffset = dstOffset % BUFFER_SIZE;

            // Calculate number of bits available for use in this destination buffer instance
            var dstInstanceAvailable = BUFFER_SIZE - dstInstanceOffset;

            // Calculate number of bits to write to this buffer - the lessor of the available bits and the total bits to write
            var pending = Math.Min(dstInstanceAvailable, count);

            // Create a mask to extract the desired bits
            var mask = UInt64.MaxValue << (BUFFER_SIZE-pending);

            // Offset and write bits to buffer
            dst[dstIndex] |= (src & mask) >> dstInstanceOffset;

            // Reduce the number of bits to be written by the number written
            count -= pending;
            src <<= pending;
            
            // Increase the offsets by the number written
            dstOffset += pending;

            if (count == 0)
            {
                break;
            }
        }

#if DEBUG
        if (count > 0)
        {
            throw new OverflowException($"Insufficient space available in {nameof(dst)} buffer - {count} bits don't fit");
        }
#endif
    }

    public static Byte ReadByte(UInt64[] src, ref Int32 srcOffset)
    {
        throw new NotImplementedException();
    }


    public static string ToBinaryString(ulong value)
    {
        return Regex.Replace(Convert.ToString((long) value, toBase: 2), ".{8}", "$0 ");
    }
}