// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

/// <summary>
/// VLQ similar to https://en.wikipedia.org/wiki/Variable-length_quantity with "Removing Redundancy", but the
/// continuation bit flag is reversed. This might be more performant for datasets with consistently large values.
/// </summary>
public class InvertedVlqIntegerDecoder : IntegerDecoderBase
{
    private readonly IBitReader _reader;

    public InvertedVlqIntegerDecoder(IBitReader reader)
    {
        _reader = reader;
    }
    
    protected override UInt64 Decode()
    {
        // Setup symbol
        UInt64 symbol = 0;
        var bit = 0;

        UInt64 b;
        do
        {
            // Read byte
            b = _reader.ReadBits(Bits.ByteBits);

            // Add input bits to output
            var chunk = b & InvertedVlqInteger.Mask;
            var pre = symbol;
            symbol += (chunk + 1) << bit;

#if DEBUG
            // Check for overflow
            if (symbol < pre) throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
#endif

            // Increment bit offset
            bit += InvertedVlqInteger.PacketSize;
        } while ((b & InvertedVlqInteger.Nil) == 0); // If not final bit

        // Remove zero offset
        symbol--;

        // Add to output
        return symbol;
    }
}