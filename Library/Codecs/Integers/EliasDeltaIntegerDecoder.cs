// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class EliasDeltaIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;

    public EliasDeltaIntegerDecoder(IBitReader reader)
    {
        _reader = reader;
    }

    protected override UInt64 Decode()
    {
        // #1 Read and count zeros from the stream until you reach the first one. Call this count of zeros L
        var l = 1;
        while (!_reader.PeakBit())
        {
            // Note that length is one bit longer
            l++;

            // Remove 0 from input
            _reader.ReadBit();
        }

        // #2 Considering the one that was reached to be the first digit of an integer, with a value of 2L, read the remaining L digits of the integer. Call this integer N+1, and subtract one to get N.
        var n = (Int32)_reader.ReadBits(l) - 1;

        // #3 Put a one in the first place of our final output, representing the value 2N.
        // #4 Read and append the following N digits.
        var value = _reader.ReadBits(n) + ((UInt64)1 << n);

        // Remove zero offset
        value--;

        return value;
    }
}