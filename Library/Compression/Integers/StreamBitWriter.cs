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
        Count++;
        Buffer <<= 1;
        if (value)
        {
            Buffer |= 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001;
        }

        Push();
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

        // Mask out extract buffer
        buffer &= UInt64.MaxValue >> (64 - count);  // TODO: faster to shift the buffer left and right again to truncate bits?

        // Add to buffer
        Count += count;
        Buffer |= buffer;

        Push();
    }

    public void WriteByte(Byte buffer)
    {
        if (Count > 0)
        {
            throw new InvalidOperationException("ReadByte cannot be used while bits are buffered - avoid using ReadBits before");
        }

        Underlying.WriteByte(buffer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Implicitly causes AlignBytes</remarks>
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

        // Flush partial byte
        Align();

        if (OwnUnderlying)
        {
            Underlying.Dispose();
        }
    }

    private void Push()
    {
        while (Count >= BITS_PER_BYTE)
        {
            // Extract byte from buffer and write to underlying
            var b = (Byte) (Buffer >> (Count - BITS_PER_BYTE));
            Underlying.WriteByte(b);
            
            // Reduce buffer
            Buffer >>= BITS_PER_BYTE;
            Count -= BITS_PER_BYTE;
        }
    }
}