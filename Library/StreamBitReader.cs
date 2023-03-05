using System.IO;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global

namespace InvertedTomato.Binary;

public class StreamBitReader : IBitReader, IDisposable
{
    private readonly Stream _underlying;
    private readonly Boolean _ownUnderlying;
    private readonly Byte[] _buffer;
    private Int32 _offset;
    private Int32 _count;

    public Boolean IsDisposed { get; private set; }

    public StreamBitReader(Stream underlying, Boolean ownUnderlying = false, Int32 bufferSize = 1024)
    {
        _underlying = underlying;
        _ownUnderlying = ownUnderlying;
        _buffer = new Byte[bufferSize];
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
            var a = _offset / Bits.ByteBits;
            var b = _offset % Bits.ByteBits;

            // Calculate number of bits available in this byte
            var load = Math.Min(Bits.ByteBits - b, count);

            // Extract bits
            var chunk = (Byte)(_buffer[a] << b) >> Bits.ByteBits - load; // This is a little complex, as it must mask out any previous bits in this byte at the same time

            // Load the bits
            value |= (UInt64)chunk << count - load;
            _offset += load;
            _count -= load;

            // Decrement input
            count -= load;

            // If all bits have been written, end here
        } while (count > 0);

        return value;
    }

    public Boolean ReadBit() => ReadBits(1) > 0;

    public void Align() => ReadBits(_count % Bits.ByteBits);

    public bool PeakBit()
    {
        // Load more bits if needed
        UnderlyingRead();

        // Calculate bit address
        var a = _offset / Bits.ByteBits;
        var b = _offset % Bits.ByteBits;

        // Get bit at that address
        var bit = _buffer[a] & (Byte)(1 << Bits.ByteBits - b - 1);

        // Test if non-zero
        return bit > 0;
    }

    private void UnderlyingRead()
    {
        // If there's more bits in the buffer, do nothing
        if (_count > 0) return;

        // Otherwise load more bits
        _offset = 0;
        _count = _underlying.Read(_buffer) * Bits.ByteBits;

        // If nothing could be loaded, throw exception
        if (_count == 0) throw new EndOfStreamException();
    }

    public void Dispose()
    {
        // Don't allow running twice
        if (IsDisposed) return;
        IsDisposed = true;

        // If we own the underlying, dispose it too
        if (_ownUnderlying) _underlying.Dispose();
    }

    public override String ToString() => _buffer.ToBinaryString(_offset, _count);
}