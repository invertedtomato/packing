namespace InvertedTomato.Packing;

public static class Bits
{
    public const Int32 ByteBits = 1 * 8;
    public const Int32 LongBits = 8 * 8;

    /// <summary>
    /// Count the number of bits used to express number
    /// </summary>
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

    /// <summary>
    /// Count the number of bits used to express number
    /// </summary>
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