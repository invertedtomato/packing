using System;
using System.IO;

namespace InvertedTomato.Compression.Integers;

public class StreamBitReader : IBitReader, IDisposable
{
    // Lowest bit is always on the right
    
    private const Int32 BUFFER_MIN_BITS = 0;
    private const Int32 BUFFER_MAX_BITS = 64-8; // There must always be room for another byte to be loaded, else bits must be lost
    private const Int32 BITS_PER_BYTE = 8;
    
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

    public ulong ReadBits(int count)
    {
        if (count is < BUFFER_MIN_BITS or > BUFFER_MAX_BITS)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Must be between {BUFFER_MIN_BITS} and {BUFFER_MAX_BITS}");
        }

        // You'd expect `UInt64.MaxValue >> 64` would result in 0, but alas, nope, it's the same as `UInt64.MaxValue >> 0` - so let's avoid this by not doing anything for count=0
        if (count == 0)
        {
            return 0;
        }

        // Ensure we have enough bits loaded
        EnsureLoad(count);

        // Copy buffer
        var buffer = Buffer;
        
        // Mask out extract buffer
        buffer &= UInt64.MaxValue >> (64 - count);

        // Update references
        Count -= count;
        Buffer >>= count;

        // Return
        return buffer;
    }

    public bool PeakBit()
    {
        // Ensure we have at least one bit loaded
        EnsureLoad(1);

        // Return that bit
        return (Buffer & 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) > 0;
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
    /// Ensure we have the provided number of bits loaded - if not, load them
    /// </summary>
    /// <param name="count"></param>
    /// <exception cref="EndOfStreamException"></exception>
    private void EnsureLoad(int count)
    {
#if DEBUG
        if (count is < BUFFER_MIN_BITS or > BUFFER_MAX_BITS)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Must be between {BUFFER_MIN_BITS} and {BUFFER_MAX_BITS}");
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

            var a = (UInt64)b << Count;
            
            // Add bits to buffer
            Buffer |= a;
            
            // Increment count
            Count += BITS_PER_BYTE;
        }
    }
}