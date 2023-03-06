// ReSharper disable UnusedType.Global

namespace InvertedTomato.Packing.Codecs.Integers;

/// <summary>
/// VLQ similar to https://en.wikipedia.org/wiki/Variable-length_quantity with "Removing Redundancy", but the
/// continuation bit flag is reversed. This might be more performant for datasets with consistently large values.
/// </summary>
public static class InvertedVlqInteger
{
    public const UInt64 MinValue = UInt64.MinValue;
    public const UInt64 MaxValue = UInt64.MaxValue - 1;

    public static readonly Byte[] Zero = { 0x80 }; // 10000000
    public static readonly Byte[] One = { 0x81 }; // 10000001
    public static readonly Byte[] Two = { 0x82 }; // 10000010
    public static readonly Byte[] Four = { 0x84 }; // 10000100
    public static readonly Byte[] Eight = { 0x88 };

    internal const Byte Nil = 0x80; // 10000000
    internal const Byte Mask = 0x7f; // 01111111
    internal const Int32 PacketSize = 7;
    internal const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);
}