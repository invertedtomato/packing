using System;

namespace InvertedTomato.Compression.Integers;

public class EliasDeltaCodec : ICodec
{
    public UInt64 MinValue => UInt64.MinValue;
    public UInt64 MaxValue => UInt64.MaxValue - 1; // TODO: Check!

    private void Encode(UInt64 value, IBitWriter buffer)
    {
        // Offset value to allow zeros
        value++;

        // #1 Separate X into the highest power of 2 it contains (2N) and the remaining N binary digits.
        var n = 0;
        while (Math.Pow(2, n + 1) <= value)
        {
            n++;
        }

        var r = value - (UInt64)Math.Pow(2, n);

        // #2 Encode N+1 with Elias gamma coding.
        var np = (Byte)(n + 1);
        var len = Bits.CountUsed(np);
        buffer.WriteBits(0, len - 1);
        buffer.WriteBits(np, len);

        // #3 Append the remaining N binary digits to this representation of N+1.
        buffer.WriteBits(r, n);
    }

    private UInt64 Decode(IBitReader buffer)
    {
        // #1 Read and count zeros from the stream until you reach the first one. Call this count of zeros L
        var l = 1;
        while (!buffer.PeakBit())
        {
            // Note that length is one bit longer
            l++;

            // Remove 0 from input
            buffer.ReadBit();
        }

        // #2 Considering the one that was reached to be the first digit of an integer, with a value of 2L, read the remaining L digits of the integer. Call this integer N+1, and subtract one to get N.
        var n = (Int32)buffer.ReadBits(l) - 1;

        // #3 Put a one in the first place of our final output, representing the value 2N.
        // #4 Read and append the following N digits.
        var value = buffer.ReadBits(n) + ((UInt64)1 << n);

        // Remove zero offset
        value--;

        return value;
    }

    public Int32? CalculateEncodedBits(UInt64 value)
    {
        var result = 0;

        // Offset for zero
        value++;

        // #1 Separate X into the highest power of 2 it contains (2N) and the remaining N binary digits.
        Byte n = 0;
        while (Math.Pow(2, n + 1) <= value)
        {
            n++;
        }

        var r = value - (UInt64)Math.Pow(2, n);

        // #2 Encode N+1 with Elias gamma coding.
        var np = (Byte)(n + 1);
        var len = Bits.CountUsed(np);
        result += len - 1;
        result += len;

        // #3 Append the remaining N binary digits to this representation of N+1.
        result += n;

        return result;
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
}