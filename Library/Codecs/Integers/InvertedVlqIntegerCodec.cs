namespace InvertedTomato.Binary.Codecs.Integers;

/// <summary>
/// VLQ similar to https://en.wikipedia.org/wiki/Variable-length_quantity with "Removing Redundancy", but the
/// continuation bit flag is reversed. This might be more performant for datasets with consistently large values.
/// </summary>
public class InvertedVlqIntegerCodec : IntegerCodec
{
    public override UInt64 MinValue => UInt64.MinValue;
    public override UInt64 MaxValue => UInt64.MaxValue - 1;

    public static readonly Byte[] Zero = { 0x80 }; // 10000000
    public static readonly Byte[] One = { 0x81 }; // 10000001
    public static readonly Byte[] Two = { 0x82 }; // 10000010
    public static readonly Byte[] Four = { 0x84 }; // 10000100
    public static readonly Byte[] Eight = { 0x88 };

    private const Byte Nil = 0x80; // 10000000
    private const Byte Mask = 0x7f; // 01111111
    private const Int32 PacketSize = 7;
    private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);

    protected override void Encode(UInt64 value, IBitWriter writer)
    {
#if DEBUG
        if (value > MaxValue) throw new OverflowException($"Symbol is larger than maximum supported value. Must be less than or equal to {MaxValue}");
#endif

        // Iterate through input, taking X bits of data each time, aborting when less than X bits left
        while (value > MinPacketValue)
        {
            // Write payload, skipping MSB bit
            writer.WriteBits((Byte)(value & Mask), Bits.ByteBits);

            // Offset value for next cycle
            value >>= PacketSize;
            value--;
        }

        // Write remaining - marking it as the final byte for symbol
        writer.WriteBits((Byte)(value | Nil), Bits.ByteBits);
    }

    protected override UInt64 Decode(IBitReader reader)
    {
        // Setup symbol
        UInt64 symbol = 0;
        var bit = 0;

        UInt64 b;
        do
        {
            // Read byte
            b = reader.ReadBits(Bits.ByteBits);

            // Add input bits to output
            var chunk = b & Mask;
            var pre = symbol;
            symbol += (chunk + 1) << bit;

#if DEBUG
            // Check for overflow
            if (symbol < pre)throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
#endif

            // Increment bit offset
            bit += PacketSize;
        } while ((b & Nil) == 0); // If not final bit

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