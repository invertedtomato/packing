using System;
using System.Runtime.CompilerServices;

namespace InvertedTomato.Compression.Integers;

public static class BufferUtil
{
    private  const Int32 BUFFER_SIZE = 64;// bits

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBits(UInt64 src, Int32 srcCount, ref UInt64[] dst, ref Int32 dstOffset) => WriteBits(src, 0, srcCount, ref dst, ref dstOffset);
    
    /// <summary>
    /// Write _srcCount_ bits from _src_ to _dst_ at given _dstOffset_, updating the _dstOffset_.
    /// </summary>
    /// <param name="src"></param>
    /// <param name="srcCount"></param>
    /// <param name="dst"></param>
    /// <param name="dstOffset"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBits(UInt64 src, Int32 srcOffset, Int32 srcCount, ref UInt64[] dst, ref Int32 dstOffset)
    {
        for (var i = 0; i < dst.Length; i++)
        {
            // Calculate number of bits available in this buffer
            var available = BUFFER_SIZE - dstOffset % BUFFER_SIZE;
            
            // Calculate number of bits to write to this buffer - the lessor of the available bits and the total bits to write
            var bits = Math.Min(available, srcCount);

            // Mask out bits to be written to this buffer
            var a = src | (UInt64.MaxValue >> srcOffset);
            
            // Write bits to buffer
            dst[i] |= a >> dstOffset;

            // Reduce the number of bits to be written by the number written
            srcCount -= bits;
            
            // Increase the offsets by the number written
            srcOffset += bits;
            dstOffset += bits;
        }

#if DEBUG
        if (srcCount > 0)
        {
            throw new OverflowException($"Insufficient space available in {nameof(dst)} buffer - {srcCount} bits don't fit");
        }
#endif
    }

    public static Byte ReadByte(UInt64[] src, ref Int32 srcOffset)
    {
        throw new NotImplementedException();
    }
}