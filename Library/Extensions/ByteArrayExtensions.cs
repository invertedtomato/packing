using System.Linq;

namespace InvertedTomato.Binary.Extensions;

public static class ByteArrayExtensions
{
    public static String ToBinaryString(this byte[] target, int offset, int count)
    {
        var a = String.Join("", target.Select(a => Convert.ToString(a, 2).PadLeft(8, '0')));
        return a.Substring(offset, count);
    }
}