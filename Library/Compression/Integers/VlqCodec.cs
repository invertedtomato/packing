using System;

namespace InvertedTomato.Compression.Integers;

public class VlqCodec : ICodec
{
    public const UInt64 MinValue = 0;
    public const UInt64 MaxValue = UInt64.MaxValue - 1;

    private const Byte More = 0b10000000;
    private const Byte Mask = 0b01111111;
    private const Int32 PacketSize = 7;
    private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);

    private void Encode(UInt64 value, ref UInt64[] buffers, ref Int32 offset)
    {
#if DEBUG
        if (value > MaxValue)
        {
            throw new OverflowException($"Symbol is larger than maximum supported value. See {nameof(VlqCodec)}.{nameof(MaxValue)}");
        }
#endif

        // Iterate through input, taking X bits of data each time, aborting when less than X bits left
        while (value > MinPacketValue)
        {
            // Write payload, skipping MSB bit
            BufferUtil.WriteBits((value & Mask) | More, 8, ref buffers, ref offset);

            // Offset value for next cycle
            value >>= PacketSize;
            value--;
        }

        // Write remaining - marking it as the final byte for symbol
        BufferUtil.WriteBits(value & Mask, 8, ref buffers, ref offset);
    }

    private UInt64 Decode(UInt64[] buffers, ref Int32 offset)
    {
        // Setup symbol
        UInt64 symbol = 0;
        var bit = 0;
        Byte b;
        do
        {
            // Read byte
            b = BufferUtil.ReadByte(buffers, ref offset);

            // Add input bits to output
            var chunk = (UInt64) (b & Mask);
            var pre = symbol;
            symbol += (chunk + 1) << bit;

#if DEBUG
            // Check for overflow
            if (symbol < pre)
            {
                throw new OverflowException($"Symbol is larger than maximum supported value or is corrupt. See {nameof(VlqCodec)}.{nameof(MaxValue)}.");
            }
#endif

            // Increment bit offset
            bit += PacketSize;
        } while ((b & More) > 0); // If not final byte

        // Remove zero offset
        symbol--;

        // Add to output
        return symbol;
    }

    public void EncodeBit(bool value, ref ulong[] buffers, ref Int32 offset) => Encode(1, ref buffers, ref offset);
    public void EncodeUInt8(byte value, ref ulong[] buffers, ref Int32 offset) => Encode(value, ref buffers, ref offset);
    public void EncodeUInt16(ushort value, ref ulong[] buffers, ref Int32 offset) => Encode(value, ref buffers, ref offset);
    public void EncodeUInt32(uint value, ref ulong[] buffers, ref Int32 offset) => Encode(value, ref buffers, ref offset);
    public void EncodeUInt64(ulong value, ref ulong[] buffers, ref Int32 offset) => Encode(value, ref buffers, ref offset);
    public void EncodeInt8(sbyte value, ref ulong[] buffers, ref Int32 offset) => Encode(ZigZag.Encode(value), ref buffers, ref offset);
    public void EncodeInt16(short value, ref ulong[] buffers, ref Int32 offset) => Encode(ZigZag.Encode(value), ref buffers, ref offset);
    public void EncodeInt32(int value, ref ulong[] buffers, ref Int32 offset) => Encode(ZigZag.Encode(value), ref buffers, ref offset);
    public void EncodeInt64(long value, ref ulong[] buffers, ref Int32 offset) => Encode(ZigZag.Encode(value), ref buffers, ref offset);

    public Boolean DecodeBit(ref ulong[] buffers, ref Int32 offset) => Decode(buffers, ref offset) > 0;
    public Byte DecodeUInt8(ref ulong[] buffers, ref Int32 offset) => (Byte) Decode(buffers, ref offset);
    public UInt16 DecodeUInt16(ref ulong[] buffers, ref Int32 offset) => (UInt16) Decode(buffers, ref offset);
    public UInt32 DecodeUInt32(ref ulong[] buffers, ref Int32 offset) => (UInt32) Decode(buffers, ref offset);
    public UInt64 DecodeUInt64(ref ulong[] buffers, ref Int32 offset) => Decode(buffers, ref offset);
    public SByte DecodeInt8(ref ulong[] buffers, ref Int32 offset) => (SByte) ZigZag.Decode(Decode(buffers, ref offset));
    public Int16 DecodeInt16(ref ulong[] buffers, ref Int32 offset) => (Int16) ZigZag.Decode(Decode(buffers, ref offset));
    public Int32 DecodeInt32(ref ulong[] buffers, ref Int32 offset) => (Int32) ZigZag.Decode(Decode(buffers, ref offset));
    public Int64 DecodeInt64(ref ulong[] buffers, ref Int32 offset) => ZigZag.Decode(Decode(buffers, ref offset));
}