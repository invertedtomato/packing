using System.Collections.Generic;

namespace InvertedTomato.Binary.Integers;

public class EliasOmegaIntegerCodec : IIntegerCodec
{
    public UInt64 MinValue => UInt64.MinValue;
    public UInt64 MaxValue => UInt64.MaxValue - 1; // TODO: Check!

    private void Encode(UInt64 value, IBitWriter buffer)
    {
        // Offset min value
        value++;

        // Prepare buffer
        var groups = new Stack<KeyValuePair<UInt64, Int32>>();

        // #1 Place a "0" at the end of the code.
        groups.Push(new (0, 1));

        // #2 If N=1, stop; encoding is complete.
        while (value > 1)
        {
            // Calculate the length of value
            var length = Bits.CountUsed(value);

            // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
            groups.Push(new (value, length));

            // #4 Let N equal the number of bits just prepended, minus one.
            value = (UInt64)length - 1;
        }

        // Write buffer
        foreach (var item in groups)
        {
            var bits = item.Value;
            var group = item.Key;

            buffer.WriteBits(group, bits);
        }
    }

    private UInt64 Decode(IBitReader buffer)
    {
        // #1 Start with a variable N, set to a value of 1.
        UInt64 value = 1;

        // #2 If the next bit is a "0", stop. The decoded number is N.
        while (buffer.PeakBit())
        {
            // #3 If the next bit is a "1", then read it plus N more bits, and use that binary number as the new value of N.
            value = buffer.ReadBits((Int32)value + 1);
        }

        // Burn last bit from input
        buffer.ReadBit();

        // Offset for min value
        return value - 1;
    }

    public Int32? CalculateEncodedBits(UInt64 value)
    {
        var result = 1; // Termination bit

        // Offset value to allow for 0s
        value++;

        // #2 If N=1, stop; encoding is complete.
        while (value > 1)
        {
            // Calculate the length of value
            var length = Bits.CountUsed(value);

            // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
            result += length;

            // #4 Let N equal the number of bits just prepended, minus one.
            value = (UInt64)length - 1;
        }

        return result;
    }

    public void EncodeBit(bool value, IBitWriter buffer) => Encode(value ? 1UL : 0UL, buffer);
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