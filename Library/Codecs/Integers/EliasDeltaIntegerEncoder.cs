// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasDeltaIntegerEncoder : IntegerEncoderBase
{
    private readonly IBitWriter _writer;

    public EliasDeltaIntegerEncoder(IBitWriter writer)
    {
        _writer = writer;
    }
    
    protected override void Encode(UInt64 value)
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
        _writer.WriteBits(0, len - 1);
        _writer.WriteBits(np, len);

        // #3 Append the remaining N binary digits to this representation of N+1.
        _writer.WriteBits(r, n);
    }

    public override Int32? PredictEncodedBits(UInt64 value)
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