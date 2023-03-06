// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasDeltaIntegerCodec : IntegerCodec
{
    public static EliasDeltaIntegerCodec Default => new();

    public override UInt64 MinValue => UInt64.MinValue;
    public override UInt64 MaxValue => UInt64.MaxValue - 1;

    protected override void Encode(UInt64 value, IBitWriter writer)
    {
        // Offset value to allow zeros
        value++;

        // #1 Separate X into the highest power of 2 it contains (2N) and the remaining N binary digits.
        var n = 0;
        while (Math.Pow(2, n + 1) <= value) n++;
        var r = value - (UInt64)Math.Pow(2, n);

        // #2 Encode N+1 with Elias gamma coding.
        var np = (Byte)(n + 1);
        var len = Bits.CountUsed(np);
        writer.WriteBits(0, len - 1);
        writer.WriteBits(np, len);

        // #3 Append the remaining N binary digits to this representation of N+1.
        writer.WriteBits(r, n);
    }

    protected override UInt64 Decode(IBitReader reader)
    {
        // #1 Read and count zeros from the stream until you reach the first one. Call this count of zeros L
        var l = 1;
        while (!reader.PeakBit())
        {
            // Note that length is one bit longer
            l++;

            // Remove 0 from input
            reader.ReadBit();
        }

        // #2 Considering the one that was reached to be the first digit of an integer, with a value of 2L, read the remaining L digits of the integer. Call this integer N+1, and subtract one to get N.
        var n = (Int32)reader.ReadBits(l) - 1;

        // #3 Put a one in the first place of our final output, representing the value 2N.
        // #4 Read and append the following N digits.
        var value = reader.ReadBits(n) + ((UInt64)1 << n);

        // Remove zero offset
        value--;

        return value;
    }

    public override Int32? CalculateEncodedBits(UInt64 value)
    {
        var result = 0;

        // Offset for zero
        value++;

        // #1 Separate X into the highest power of 2 it contains (2N) and the remaining N binary digits.
        Byte n = 0;
        while (Math.Pow(2, n + 1) <= value) n++;

        // #2 Encode N+1 with Elias gamma coding.
        var np = (Byte)(n + 1);
        var len = Bits.CountUsed(np);
        result += len - 1;
        result += len;

        // #3 Append the remaining N binary digits to this representation of N+1.
        result += n;

        return result;
    }
}