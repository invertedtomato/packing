using System;
using System.IO;
using System.Linq;
using InvertedTomato.Compression.Integers.Extensions;

// ReSharper disable MemberCanBePrivate.Global

namespace InvertedTomato.Compression.Integers;

public class StreamBitWriter : IBitWriter, IDisposable
{
    private const UInt64 Zero = 0;
    private const UInt64 One = 1;
    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;
    private readonly Byte[] Buffer;
    private Int32 Count;

    public Boolean IsDisposed { get; private set; }

    public StreamBitWriter(Stream underlying, Boolean ownUnderlying = false, Int32 bufferSize = 1024)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
        Buffer = new Byte[bufferSize];
    }

    public void WriteBits(UInt64 bits, int count)
    {
#if DEBUG
        // Count the count is sane
        if (count is < 0 or > Bits.UlongBits) throw new ArgumentOutOfRangeException(nameof(count), $"Must be between 0 and {Bits.UlongBits} but was {count}");

        // Check that only bits within the count range are used (yep, we could clean this automatically, but that adds operations and slows things down, so we only check when debugging)
        if ((bits << Bits.UlongBits - count >> Bits.UlongBits - count != bits)
            || (count == 0 && bits > 0) // Once again, why does UInt64 >> 64 not equal 0?? Catching and handling this additional case here
           ) throw new ArgumentException("Bits must only have '1' bits within the 'count' range. Ie, if count=1, only the right-most bit can be used", nameof(bits));
#endif

        // Cycle through buffer bytes
        do
        {
            // Calculate bit address
            var a = Count / Bits.ByteBits;
            var b = Count % Bits.ByteBits;

            // Calculate number of bits to load into this byte
            var load = Math.Min(Bits.ByteBits - b, count);

            // Extract bits
            var chunk = (Byte) (bits >> (count - load));

            // Load the bits
            Buffer[a] |= (Byte) (chunk << (Bits.ByteBits - load - b));
            Count += load;

            // Decrement input
            count -= load;

            // If buffer is full..
            if (Count == Buffer.Length * Bits.ByteBits)
            {
                // Flush buffer
                Underlying.Write(Buffer);

                // Clear buffer
                Buffer.Clear();
                Count = 0;
            }

            // If all bits have been written, end here
        } while (count > 0);
    }

    public void WriteBit(Boolean value) => WriteBits(value ? One : Zero, 1);

    public void Align()
    {
        if (HasPartialByte()) WriteBits(0, Bits.ByteBits - Count % Bits.ByteBits);
    }

    public void Dispose()
    {
        // Don't allow running twice
        if (IsDisposed) return;
        IsDisposed = true;

        // Write out any remaining bytes
        var count = Count / Bits.ByteBits;
        if (HasPartialByte()) count++; // If there's an incomplete byte, write it anyway
        Underlying.Write(Buffer, count);

        // If we own the underlying, dispose it too
        if (OwnUnderlying) Underlying.Dispose();
    }

    public override string ToString()
    {
        var a = String.Join("", Buffer.Select(a => Convert.ToString(a, 2).PadLeft(8, '0')));
        return a.Substring(0, Count);
    }

    private Boolean HasPartialByte() => Count % Bits.ByteBits > 0;
}