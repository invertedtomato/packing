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

    private void Encode(UInt64 value, IBitWriter buffer)
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
            buffer.Write((value & Mask) | More, 8);

            // Offset value for next cycle
            value >>= PacketSize;
            value--;
        }

        // Write remaining - marking it as the final byte for symbol
        buffer.Write(value & Mask, 8);
    }

    private UInt64 Decode(IBitReader buffer)
    {
        // Setup symbol
        UInt64 symbol = 0;
        var bit = 0;
        Byte b;
        do
        {
            // Read byte
            b = (Byte)buffer.Read(8);

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

    public void EncodeBit(bool value, IBitWriter buffer) => Encode(1, buffer);
    public void EncodeUInt8(byte value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeUInt16(ushort value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeUInt32(uint value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeUInt64(ulong value, IBitWriter buffer) => Encode(value, buffer);
    public void EncodeInt8(sbyte value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
    public void EncodeInt16(short value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
    public void EncodeInt32(int value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
    public void EncodeInt64(long value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);

    public Boolean DecodeBit(IBitReader buffer) => Decode(buffer) > 0;
    public Byte DecodeUInt8(IBitReader buffer) => (Byte) Decode(buffer);
    public UInt16 DecodeUInt16(IBitReader buffer) => (UInt16) Decode(buffer);
    public UInt32 DecodeUInt32(IBitReader buffer) => (UInt32) Decode(buffer);
    public UInt64 DecodeUInt64(IBitReader buffer) => Decode(buffer);
    public SByte DecodeInt8(IBitReader buffer) => (SByte) ZigZag.Decode(Decode(buffer));
    public Int16 DecodeInt16(IBitReader buffer) => (Int16) ZigZag.Decode(Decode(buffer));
    public Int32 DecodeInt32(IBitReader buffer) => (Int32) ZigZag.Decode(Decode(buffer));
    public Int64 DecodeInt64(IBitReader buffer) => ZigZag.Decode(Decode(buffer));
}