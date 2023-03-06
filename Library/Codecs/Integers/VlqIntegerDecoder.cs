// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public class VlqIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;

    public VlqIntegerDecoder(IBitReader reader)
    {
        _reader = reader;
    }
    
    protected override UInt64 Decode()
    {
        // Setup symbol
        UInt64 symbol = 0;
        var bit = 0;
        Byte b;
        do
        {
            // Read byte
            b = (Byte)_reader.ReadBits(Bits.ByteBits);

            // Add input bits to output
            var chunk = (UInt64)(b & VlqInteger.Mask);
            var pre = symbol;
            symbol += (chunk + 1) << bit;

#if DEBUG
            // Check for overflow
            if (symbol < pre) throw new OverflowException($"Symbol is larger than maximum supported value or is corrupt. See {nameof(VlqInteger)}.{nameof(VlqInteger.MaxValue)}.");
#endif

            // Increment bit offset
            bit += VlqInteger.PacketSize;
        } while ((b & VlqInteger.More) > 0); // If not final byte

        // Remove zero offset
        symbol--;

        // Add to output
        return symbol;
    }
}