namespace InvertedTomato.Packing.Codecs.Integers;

public static class IntegerUtil
{
    public static UInt64 Pow(UInt64 x, UInt64 pow) // Math.Pow only supports doubles
    {
        UInt64 ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1) ret *= x;
            x *= x;
            pow >>= 1;
        }

        return ret;
    }
}