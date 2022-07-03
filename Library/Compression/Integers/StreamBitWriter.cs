using System;
using System.IO;

namespace InvertedTomato.Compression.Integers;

public class StreamBitWriter : IBitWriter, IDisposable
{
    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;


    private UInt64 Buffer;
    private Int32 Count;

    private const Int32 BUFFER_MIN_BITS = 0;
    private const Int32 BUFFER_MAX_BITS = 64 - 8; // There must always be room for another byte to be loaded, else bits must be lost
    private const Int32 BITS_PER_BYTE = 8;
    private const Int32 BITS_PER_ULONG = 64;

    public Boolean IsDisposed { get; private set; }

    public StreamBitWriter(Stream underlying)
    {
        Underlying = underlying;
        OwnUnderlying = false;
    }

    public StreamBitWriter(Stream underlying, Boolean ownUnderlying)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
    }

    public void WriteBit(Boolean value)
    {
        WriteBits(value ? (UInt64) 1 : 0, 1);
    }

    public void WriteBits(UInt64 buffer, int count)
    {
        if (count is < BUFFER_MIN_BITS or > BUFFER_MAX_BITS)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Must be between {BUFFER_MIN_BITS} and {BUFFER_MAX_BITS}");
        }

        if (count == 0)
        {
            return;
        }

        // Remove any stray bits from the provided buffer (ie, if provided with buffer=00000011 and count=1, we need to remove that left-most '1' bit)
        buffer <<= BITS_PER_ULONG - count;

        // Align the buffer ready to be merged
        buffer >>= Count;

        // Add to buffer
        Buffer |= buffer;
        Count += count;

        Push();
    }

    public void Align()
    {
        // If already aligned, do nothing
        if (Count % BITS_PER_BYTE == 0)
        {
            return;
        }

        // Burn bits remaining in current byte
        WriteBits(0, BITS_PER_BYTE - Count % BITS_PER_BYTE);
    }

    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        // Flush any partial byte
        Align();

        if (OwnUnderlying)
        {
            Underlying.Dispose();
        }
    }

    /// <summary>
    /// Write any completed bits to the underlying stream
    /// </summary>
    private void Push()
    {
        while (Count >= BITS_PER_BYTE)
        {
            // Extract byte from buffer and write to underlying
            var b = (Byte) (Buffer >> BITS_PER_ULONG - BITS_PER_BYTE);
            Underlying.WriteByte(b);

            // Reduce buffer
            Buffer <<= BITS_PER_BYTE;
            Count -= BITS_PER_BYTE;
        }
    }
}