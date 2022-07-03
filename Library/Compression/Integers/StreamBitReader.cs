using System;
using System.IO;

namespace InvertedTomato.Compression.Integers;

public class StreamBitReader : IBitReader, IDisposable
{
    // Lowest bit is always on the right

    public  Int32 MaxBits => 64 - 8; // There must always be room for another byte to be loaded, else bits must be lost
    
    private const Int32 BUFFER_MIN_BITS = 0;
    private const Int32 BITS_PER_BYTE = 8;
    private const Int32 BITS_PER_ULONG = 64;
    private const UInt64 TOP_BITMASK = 0b10000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;

    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;

    private UInt64 Buffer;
    private Int32 Count;

    public Boolean IsDisposed { get; private set; }

    public StreamBitReader(Stream underlying)
    {
        Underlying = underlying;
        OwnUnderlying = false;
    }

    public StreamBitReader(Stream underlying, Boolean ownUnderlying)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
    }

    public bool PeakBit()
    {
        // Ensure we have at least one bit loaded
        Pull(1);

        // Return that bit
        return (Buffer & TOP_BITMASK) > 0;
    }

    public Boolean ReadBit()
    {
        return ReadBits(1) > 0;
    }

    public UInt64 ReadBits(int count)
    {
        if (count  < BUFFER_MIN_BITS || count > MaxBits)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Must be between {BUFFER_MIN_BITS} and {MaxBits}");
        }

        // You'd expect `UInt64.MaxValue >> 64` would result in 0, but alas, nope, it's the same as `UInt64.MaxValue >> 0` - so let's avoid this by not doing anything for count=0
        if (count == 0)
        {
            return 0;
        }

        // Ensure we have enough bits loaded
        Pull(count);

        // Extract value
        var buffer = Buffer >> (BITS_PER_ULONG - count);

        // Update references
        Buffer <<= count;
        Count -= count;

        // Return
        return buffer;
    }

    public void Align()
    {
        // Burn bits remaining in current byte
        ReadBits(Count % BITS_PER_BYTE);
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        if (OwnUnderlying)
        {
            Underlying.Dispose();
        }
    }

    /// <summary>
    /// Ensure we have the specified number of bits loaded - if not, load them from the underlying stream
    /// </summary>
    /// <param name="count"></param>
    /// <exception cref="EndOfStreamException"></exception>
    private void Pull(int count)
    {
#if DEBUG
        if (count  < BUFFER_MIN_BITS || count > MaxBits)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Must be between {BUFFER_MIN_BITS} and {MaxBits}");
        }
#endif

        // Load bytes until we have enough
        while (Count < count)
        {
            var b = Underlying.ReadByte();
            if (b < 0)
            {
                throw new EndOfStreamException();
            }

            // Align inbound buffer
            var buffer = (UInt64) b << BITS_PER_ULONG - BITS_PER_BYTE - Count;

            // Add to buffer
            Buffer |= buffer;
            Count += BITS_PER_BYTE;
        }
    }
}