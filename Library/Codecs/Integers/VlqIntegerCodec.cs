// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class VlqIntegerCodec : IntegerCodec
{
    public override UInt64 MinValue => 0;
    public override UInt64 MaxValue => UInt64.MaxValue - 1;

    private const Byte More = 0b10000000;
    private const Byte Mask = 0b01111111;
    private const Int32 PacketSize = 7;
    private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);

    protected override void Encode(UInt64 value, IBitWriter buffer)
    {
#if DEBUG
        if (value > MaxValue) throw new OverflowException($"Symbol is larger than maximum supported value. Must be less than or equal to {nameof(MaxValue)}");
#endif

        // Iterate through input, taking X bits of data each time, aborting when less than X bits left
        while (value > MinPacketValue)
        {
            // Write payload, skipping MSB bit
            buffer.WriteBits((value & Mask) | More, 8);

            // Offset value for next cycle
            value >>= PacketSize;
            value--;
        }

        // Write remaining - marking it as the final byte for symbol
        buffer.WriteBits(value & Mask, 8);
    }

    protected override UInt64 Decode(IBitReader buffer)
    {
        // Setup symbol
        UInt64 symbol = 0;
        var bit = 0;
        Byte b;
        do
        {
            // Read byte
            b = (Byte)buffer.ReadBits(Bits.ByteBits);

            // Add input bits to output
            var chunk = (UInt64)(b & Mask);
            var pre = symbol;
            symbol += (chunk + 1) << bit;

#if DEBUG
            // Check for overflow
            if (symbol < pre) throw new OverflowException($"Symbol is larger than maximum supported value or is corrupt. See {nameof(VlqIntegerCodec)}.{nameof(MaxValue)}.");
#endif

            // Increment bit offset
            bit += PacketSize;
        } while ((b & More) > 0); // If not final byte

        // Remove zero offset
        symbol--;

        // Add to output
        return symbol;
    }

    public override Int32? CalculateEncodedBits(UInt64 value)
    {
        var packets = (Int32)Math.Ceiling(Bits.CountUsed(value) / (Single)PacketSize);
        return packets * (PacketSize + 1);
    }
}