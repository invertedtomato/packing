using System;
using InvertedTomato.Compression.Integers.Gen3;

namespace InvertedTomato.Compression.Integers;

/// <summary>
/// VLQ similar to https://en.wikipedia.org/wiki/Variable-length_quantity with "Removing Redundancy", but the
/// continuation bit flag is reversed. This might be more performant for datasets with consistently large values.
/// </summary>
public class InvertedVlqCodec : ICodec
{
    public UInt64 MinValue => UInt64.MinValue;
    public UInt64 MaxValue => UInt64.MaxValue - 1;

    public static readonly Byte[] Zero = { 0x80 }; // 10000000
    public static readonly Byte[] One = { 0x81 }; // 10000001
    public static readonly Byte[] Two = { 0x82 }; // 10000010
    public static readonly Byte[] Four = { 0x84 }; // 10000100
    public static readonly Byte[] Eight = { 0x88 };

    private const Byte Nil = 0x80; // 10000000
    private const Byte Mask = 0x7f; // 01111111
    private const Int32 PacketSize = 7;
    private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);

    private void Encode(UInt64 value, IBitWriter buffer)
    {
#if DEBUG
        if (value > MaxValue)
        {
            throw new OverflowException("Symbol is larger than maximum supported value. See VLQCodec.MaxValue");
        }
#endif

        // Iterate through input, taking X bits of data each time, aborting when less than X bits left
        while (value > MinPacketValue)
        {
            // Write payload, skipping MSB bit
            buffer.WriteBits((Byte)(value & Mask), Bits.ByteBits);

            // Offset value for next cycle
            value >>= PacketSize;
            value--;
        }

        // Write remaining - marking it as the final byte for symbol
        buffer.WriteBits((Byte)(value | Nil), Bits.ByteBits);
    }

    private UInt64 Decode(IBitReader buffer)
    {
        // Setup symbol
        UInt64 symbol = 0;
        var bit = 0;

        UInt64 b;
        do
        {
            // Read byte
            b = buffer.ReadBits(Bits.ByteBits);

            // Add input bits to output
            var chunk = b & Mask;
            var pre = symbol;
            symbol += (chunk + 1) << bit;

#if DEBUG
            // Check for overflow
            if (symbol < pre)
            {
                throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
            }
#endif

            // Increment bit offset
            bit += PacketSize;
        } while ((b & Nil) == 0); // If not final bit

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
    public Byte DecodeUInt8(IBitReader buffer) => (Byte)Decode(buffer);
    public UInt16 DecodeUInt16(IBitReader buffer) => (UInt16)Decode(buffer);
    public UInt32 DecodeUInt32(IBitReader buffer) => (UInt32)Decode(buffer);
    public UInt64 DecodeUInt64(IBitReader buffer) => Decode(buffer);
    public SByte DecodeInt8(IBitReader buffer) => (SByte)ZigZag.Decode(Decode(buffer));
    public Int16 DecodeInt16(IBitReader buffer) => (Int16)ZigZag.Decode(Decode(buffer));
    public Int32 DecodeInt32(IBitReader buffer) => (Int32)ZigZag.Decode(Decode(buffer));
    public Int64 DecodeInt64(IBitReader buffer) => ZigZag.Decode(Decode(buffer));

    public Int32? CalculateEncodedBits(UInt64 value)
    {
        var packets = (Int32)Math.Ceiling(Bits.CountUsed(value) / (Single)PacketSize);

        return packets * (PacketSize + 1);
    }
}