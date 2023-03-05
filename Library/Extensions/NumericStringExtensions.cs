using System.Linq;
using System.Text.RegularExpressions;

namespace InvertedTomato.Binary.Extensions;

public static class NumericStringExtensions
{
    public static String ToBinaryString(this Byte[] target) => String.Join(" ", target.Select(a => Convert.ToString(a, 2).PadLeft(8, '0')));
    public static String ToBinaryString(this UInt64 target) => Regex.Replace( Convert.ToString((Int64)target, 2).PadLeft(64, '0'),".{8}", "$0 ");
    public static String ToHexString(this Byte[] target) => BitConverter.ToString(target).Replace(" ","");
}