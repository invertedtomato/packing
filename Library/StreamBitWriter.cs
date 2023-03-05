using System.IO;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global

namespace InvertedTomato.Binary;

public class StreamBitWriter : IBitWriter, IDisposable
{
    private const UInt64 Zero = 0;
    private const UInt64 One = 1;
    private readonly Stream _underlying;
    private readonly Boolean _ownUnderlying;
    private readonly Byte[] _buffer;
    private Int32 _count;

    public Boolean IsDisposed { get; private set; }

    public StreamBitWriter(Stream underlying, Boolean ownUnderlying = false, Int32 bufferSize = 1024)
    {
        _underlying = underlying;
        _ownUnderlying = ownUnderlying;
        _buffer = new Byte[bufferSize];
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
            var a = _count / Bits.ByteBits;
            var b = _count % Bits.ByteBits;

            // Calculate number of bits to load into this byte
            var load = Math.Min(Bits.ByteBits - b, count);

            // Extract bits
            var chunk = (Byte)(bits >> (count - load));

            // Load the bits
            _buffer[a] |= (Byte)(chunk << (Bits.ByteBits - load - b));
            _count += load;

            // Decrement input
            count -= load;

            // If buffer is full..
            if (_count == _buffer.Length * Bits.ByteBits)
            {
                // Flush buffer
                _underlying.Write(_buffer);

                // Clear buffer
                _buffer.Clear();
                _count = 0;
            }

            // If all bits have been written, end here
        } while (count > 0);
    }

    public void WriteBit(Boolean value) => WriteBits(value ? One : Zero, 1);

    public void Align()
    {
        if (HasPartialByte()) WriteBits(0, Bits.ByteBits - _count % Bits.ByteBits);
    }

    public void Dispose()
    {
        // Don't allow running twice
        if (IsDisposed) return;
        IsDisposed = true;

        // Write out any remaining bytes
        var count = _count / Bits.ByteBits;
        if (HasPartialByte()) count++; // If there's an incomplete byte, write it anyway
        _underlying.Write(_buffer, count);

        // If we own the underlying, dispose it too
        if (_ownUnderlying) _underlying.Dispose();
    }

    public override string ToString()
    {
        var a = String.Join("", _buffer.Select(a => Convert.ToString(a, 2).PadLeft(8, '0')));
        return a.Substring(0, _count);
    }

    private Boolean HasPartialByte() => _count % Bits.ByteBits > 0;
}