using System;
using System.IO;

namespace InvertedTomato.Compression.Integers;

public class StreamBitReader : IBitReader, IDisposable
{
    private const Int32 MAX_READ = 32; // bits

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

    public ulong Read(int count)
    {
        if (count is < 1 or > MAX_READ)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Must be between 1 and {MAX_READ}");
        }

        // Ensure we have enough bits loaded
        EnsureLoad(count);

        // Copy buffer
        var buffer = Buffer;

        // Update references
        Count -= count;
        Buffer <<= count;

        // Return
        return buffer;
    }

    public bool PeakBit()
    {
        // Ensure we have at least one bit loaded
        EnsureLoad(1);

        // Return that bit
        return (Buffer & 1) > 0;
    }

    public void Align()
    {
        // Burn bits remaining in current byte
        Read(Count % 8);
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
        if (count > MAX_READ)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
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

            Buffer |= (Byte) (b >> count);
            Count += 8;
        }
    }
}