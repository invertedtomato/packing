// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

public static class VlqInteger
{
    public const UInt64 MinValue = 0;
    public const UInt64 MaxValue = UInt64.MaxValue - 1;

    internal const Byte More = 0b10000000;
    internal const Byte Mask = 0b01111111;
    internal const Int32 PacketSize = 7;
    internal const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);
}