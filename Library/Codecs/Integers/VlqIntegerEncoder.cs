// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class VlqIntegerEncoder : IntegerEncoderBase
{
    private readonly IBitWriter _writer;

    public VlqIntegerEncoder(IBitWriter writer)
    {
        _writer = writer;
    }
    
    protected override void Encode(UInt64 value)
    {
#if DEBUG
        if (value > VlqInteger.MaxValue) throw new OverflowException($"Symbol is larger than maximum supported value. Must be less than or equal to {nameof(VlqInteger.MaxValue)}");
#endif

        // Iterate through input, taking X bits of data each time, aborting when less than X bits left
        while (value > VlqInteger.MinPacketValue)
        {
            // Write payload, skipping MSB bit
            _writer.WriteBits((value & VlqInteger.Mask) | VlqInteger.More, 8);

            // Offset value for next cycle
            value >>= VlqInteger.PacketSize;
            value--;
        }

        // Write remaining - marking it as the final byte for symbol
        _writer.WriteBits(value & VlqInteger.Mask, 8);
    }

    public override Int32? PredictEncodedBits(UInt64 value)
    {
        var packets = (Int32)Math.Ceiling(Bits.CountUsed(value) / (Single)VlqInteger.PacketSize);
        return packets * (VlqInteger.PacketSize + 1);
    }
}