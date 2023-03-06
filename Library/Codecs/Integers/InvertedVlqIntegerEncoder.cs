// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

/// <summary>
/// VLQ similar to https://en.wikipedia.org/wiki/Variable-length_quantity with "Removing Redundancy", but the
/// continuation bit flag is reversed. This might be more performant for datasets with consistently large values.
/// </summary>
public class InvertedVlqIntegerEncoder : IntegerEncoderBase
{
    private readonly IBitWriter _writer;

    public InvertedVlqIntegerEncoder(IBitWriter writer)
    {
        _writer = writer;
    }

    protected override void Encode(UInt64 value)
    {
#if DEBUG
        if (value > InvertedVlqInteger.MaxValue) throw new OverflowException($"Symbol is larger than maximum supported value. Must be less than or equal to {InvertedVlqInteger.MaxValue}");
#endif

        // Iterate through input, taking X bits of data each time, aborting when less than X bits left
        while (value > InvertedVlqInteger.MinPacketValue)
        {
            // Write payload, skipping MSB bit
            _writer.WriteBits((Byte)(value & InvertedVlqInteger.Mask), Bits.ByteBits);

            // Offset value for next cycle
            value >>= InvertedVlqInteger.PacketSize;
            value--;
        }

        // Write remaining - marking it as the final byte for symbol
        _writer.WriteBits((Byte)(value | InvertedVlqInteger.Nil), Bits.ByteBits);
    }

    public override Int32? PredictEncodedBits(UInt64 value)
    {
        var packets = (Int32)Math.Ceiling(Bits.CountUsed(value) / (Single)InvertedVlqInteger.PacketSize);
        return packets * (InvertedVlqInteger.PacketSize + 1);
    }
}