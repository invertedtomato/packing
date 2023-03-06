// ReSharper disable UnusedMember.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public abstract class IntegerDecoderBase
{
    protected abstract UInt64 Decode();

    public Boolean DecodeBit() => Decode() > 0;
    public Byte DecodeUInt8() => (Byte)Decode();
    public UInt16 DecodeUInt16() => (UInt16)Decode();
    public UInt32 DecodeUInt32() => (UInt32)Decode();
    public UInt64 DecodeUInt64() => Decode();
    public SByte DecodeInt8() => (SByte)ZigZagUtility.Decode(Decode());
    public Int16 DecodeInt16() => (Int16)ZigZagUtility.Decode(Decode());
    public Int32 DecodeInt32() => (Int32)ZigZagUtility.Decode(Decode());
    public Int64 DecodeInt64() => ZigZagUtility.Decode(Decode());
}