using System;
using System.IO;
using System.Linq;
using InvertedTomato.Compression.Integers.Extensions;

// ReSharper disable MemberCanBePrivate.Global

namespace InvertedTomato.Compression.Integers;

public class StreamBitReader : IBitReader, IDisposable
{
    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;
    private readonly Byte[] Buffer;
    private Int32 Offset;
    private Int32 Count;

    public Boolean IsDisposed { get; private set; }

    public StreamBitReader(Stream underlying, Boolean ownUnderlying = false, Int32 bufferSize = 1024)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
        Buffer = new Byte[bufferSize];
    }

    public UInt64 ReadBits(int count)
    {
#if DEBUG
        if (count is < 0 or > Bits.UlongBits) throw new ArgumentOutOfRangeException(nameof(count), $"Must be between 0 and {Bits.UlongBits}");
#endif

        // If nothing to do, do nothing - we don't want UnderlyingRead trying to read bits when we don't need any
        if (count == 0) return 0;
        
        UInt64 value = 0;
        do
        {
            // Load more bits if needed
            UnderlyingRead();

            // Calculate bit address
            var a = Offset / Bits.ByteBits;
            var b = Offset % Bits.ByteBits;

            // Calculate number of bits available in this byte
            var load = Math.Min(Bits.ByteBits - b, count);

            // Extract bits
            var chunk = (Byte)(Buffer[a] << b) >> Bits.ByteBits - load; // This is a little complex, as it must mask out any previous bits in this byte at the same time

            // Load the bits
            value |= (UInt64) chunk << count - load;
            Offset += load;
            Count -= load;

            // Decrement input
            count -= load;

            // If all bits have been written, end here
        } while (count > 0);

        return value;
    }

    public Boolean ReadBit() => ReadBits(1) > 0;

    public void Align() => ReadBits(Count % Bits.ByteBits);

    public bool PeakBit()
    {
        // Load more bits if needed
        UnderlyingRead();

        // Calculate bit address
        var a = Offset / Bits.ByteBits;
        var b = Offset % Bits.ByteBits;

        // Get bit at that address
        var bit = Buffer[a] & (Byte) (1 << Bits.ByteBits - b - 1);

        // Test if non-zero
        return bit > 0;
    }

    private void UnderlyingRead()
    {
        // If there's more bits in the buffer, do nothing
        if (Count > 0) return;

        // Otherwise load more bits
        Offset = 0;
        Count = Underlying.Read(Buffer) * Bits.ByteBits;

        // If nothing could be loaded, throw exception
        if (Count == 0) throw new EndOfStreamException();
    }

    public void Dispose()
    {
        // Don't allow running twice
        if (IsDisposed) return;
        IsDisposed = true;

        // If we own the underlying, dispose it too
        if (OwnUnderlying) Underlying.Dispose();
    }

    public override string ToString()
    {
        var a = String.Join("", Buffer.Select(a => Convert.ToString(a, 2).PadLeft(8, '0')));
        return a.Substring(Offset, Count);
    }
}