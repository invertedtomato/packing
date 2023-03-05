using System;
using System.Runtime.CompilerServices;

namespace InvertedTomato.Compression.Integers;

public static class Bits
{
    public const Int32 ByteBits = 8;
    public const Int32 UlongBits = 64;

    /// <summary>
    /// Count the number of bits used to express number
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte CountUsed(UInt64 value)
    {
        Byte bits = 0;

        do
        {
            bits++;
            value >>= 1;
        } while (value > 0);

        return bits;
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Byte CountUsed(Byte value)
    {
        Byte bits = 0;

        do
        {
            bits++;
            value >>= 1;
        } while (value > 0);

        return bits;
    }
}