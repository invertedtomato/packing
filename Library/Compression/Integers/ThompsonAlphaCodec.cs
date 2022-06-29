using System;

namespace InvertedTomato.Compression.Integers;

public class ThompsonAlphaCodec : ICodec
{
    private readonly Int32 LengthBits;

    public ThompsonAlphaCodec() : this(6)
    {
    }

    /// <summary>
    /// Instantiate with options
    /// </summary>
    /// <param name="lengthBits">Number of prefix bits used to store length.</param>
    public ThompsonAlphaCodec(Int32 lengthBits)
    {
        if (lengthBits is < 1 or > 6)
        {
            throw new ArgumentOutOfRangeException($"Must be between 1 and 6, not {lengthBits}.", nameof(lengthBits));
        }

        LengthBits = lengthBits;
    }

    private void Encode(UInt64 value, IBitWriterBuffer buffer)
    {
        // Offset value to allow zeros
        value++;

        // Count length
        var length = CountUsed(value);

        // Check not too large
        if (length > (LengthBits + 2) * 8)
        {
            throw new ArgumentOutOfRangeException($"Value is greater than maximum of {UInt64.MaxValue >> (64 - LengthBits - 1)}. Increase length bits to support larger numbers.");
        }

        // Clip MSB, it's redundant
        length--;

        // Write length
        buffer.Write64((UInt64) length, LengthBits);

        // Write number
        buffer.Write64(value, length);
    }

    private UInt64 Decode(IBitReaderBuffer buffer)
    {
        // Read length
        var length = (Int32) buffer.Read64(LengthBits);

        // Read body
        var value = buffer.Read64(length);

        // Recover implied MSB
        value |= (UInt64) 1 << length;

        // Remove offset to allow zeros
        value--;

        return value;
    }

    public void EncodeBit(bool value, IBitWriterBuffer buffer) => Encode(1, buffer);
    public void EncodeUInt8(byte value, IBitWriterBuffer buffer) => Encode(value, buffer);
    public void EncodeUInt16(ushort value, IBitWriterBuffer buffer) => Encode(value, buffer);
    public void EncodeUInt32(uint value, IBitWriterBuffer buffer) => Encode(value, buffer);
    public void EncodeUInt64(ulong value, IBitWriterBuffer buffer) => Encode(value, buffer);
    public void EncodeInt8(sbyte value, IBitWriterBuffer buffer) => Encode(ZigZag.Encode(value), buffer);
    public void EncodeInt16(short value, IBitWriterBuffer buffer) => Encode(ZigZag.Encode(value), buffer);
    public void EncodeInt32(int value, IBitWriterBuffer buffer) => Encode(ZigZag.Encode(value), buffer);
    public void EncodeInt64(long value, IBitWriterBuffer buffer) => Encode(ZigZag.Encode(value), buffer);

    public Boolean DecodeBit(IBitReaderBuffer buffer) => Decode(buffer) > 0;
    public Byte DecodeUInt8(IBitReaderBuffer buffer) => (Byte) Decode(buffer);
    public UInt16 DecodeUInt16(IBitReaderBuffer buffer) => (UInt16) Decode(buffer);
    public UInt32 DecodeUInt32(IBitReaderBuffer buffer) => (UInt32) Decode(buffer);
    public UInt64 DecodeUInt64(IBitReaderBuffer buffer) => Decode(buffer);
    public SByte DecodeInt8(IBitReaderBuffer buffer) => (SByte) ZigZag.Decode(Decode(buffer));
    public Int16 DecodeInt16(IBitReaderBuffer buffer) => (Int16) ZigZag.Decode(Decode(buffer));
    public Int32 DecodeInt32(IBitReaderBuffer buffer) => (Int32) ZigZag.Decode(Decode(buffer));
    public Int64 DecodeInt64(IBitReaderBuffer buffer) => ZigZag.Decode(Decode(buffer));

    /// <summary>
    ///     Count the number of bits used to express number
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static Int32 CountUsed(UInt64 value)
    {
        Byte bits = 0;

        do
        {
            bits++;
            value >>= 1;
        } while (value > 0);

        return bits;
    }
}