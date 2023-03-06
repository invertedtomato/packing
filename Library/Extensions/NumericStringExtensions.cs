using System.Text.RegularExpressions;

// ReSharper disable MemberCanBePrivate.Global

namespace InvertedTomato.Packing.Extensions;

public static class NumericStringExtensions
{
    public static String ToBinaryString(this Byte[] target) =>
        String.Join(" ", target.Select(b => Convert.ToString(b, 2).PadLeft(Bits.ByteBits, '0')));

    public static String ToBinaryString(this Byte[] target, Int32 offset, Int32 count) =>
        target.ToBinaryString().Substring(offset, count);

    public static String ToBinaryString(this UInt64 target) =>
        Regex.Replace(Convert.ToString((Int64)target, 2).PadLeft(Bits.LongBits, '0'), ".{8}", "$0 ");

    public static String ToHexString(this Byte[] target) =>
        BitConverter.ToString(target).Replace(" ", "");
}