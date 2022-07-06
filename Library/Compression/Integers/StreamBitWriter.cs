using System;
using System.IO;
// ReSharper disable MemberCanBePrivate.Global

namespace InvertedTomato.Compression.Integers;

public class StreamBitWriter : IBitWriter, IDisposable
{
    private const Int32 MAX_BITS = 32; // There must always be room for another byte to be loaded, else bits must be lost

    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;

    private UInt64 Buffer;
    private Int32 Count;

    public Boolean IsDisposed { get; private set; }

    public StreamBitWriter(Stream underlying, Boolean ownUnderlying = false)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
    }

    public void WriteBit(Boolean value) => WriteBits(value ? (UInt64) 1 : 0, 1);

    public void WriteBits(UInt64 buffer, int count)
    {
#if DEBUG
        if (count < 0 || count > MAX_BITS)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Must be between 0 and {MAX_BITS}");
        }
#endif

        if (count == 0)
        {
            return;
        }

        BitOperation.Enqueue(ref Buffer, ref Count, buffer, count);

        UnderlyingWrite();
    }

    public void Align()
    {
        // If already aligned, do nothing
        if (Count % BitOperation.BITS_PER_BYTE == 0)
        {
            return;
        }

        // Burn bits remaining in current byte
        WriteBits(0, BitOperation.BITS_PER_BYTE - Count % BitOperation.BITS_PER_BYTE);
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
    private void UnderlyingWrite()
    {
        while (Count >= BitOperation.BITS_PER_BYTE)
        {
            Underlying.WriteByte((Byte) BitOperation.Dequeue(ref Buffer, ref Count, BitOperation.BITS_PER_BYTE));
        }
    }

    public override string ToString() => Convert.ToString((Int64) Buffer, 2).Substring(0, Count);
}