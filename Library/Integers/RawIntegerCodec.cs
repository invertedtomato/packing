namespace InvertedTomato.Binary.Integers;

public class RawIntegerCodec : IIntegerCodec
{
    public UInt64 MinValue => UInt64.MinValue;
    public UInt64 MaxValue => UInt64.MaxValue;

    public Int32? CalculateEncodedBits(UInt64 value) => Bits.UlongBits;

    private void Encode(UInt64 value, IBitWriter buffer) => buffer.WriteBits(value, Bits.UlongBits);

    private UInt64 Decode(IBitReader buffer) => buffer.ReadBits(Bits.UlongBits);
    
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