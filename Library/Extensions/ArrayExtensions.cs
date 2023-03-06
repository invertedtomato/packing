namespace InvertedTomato.Packing.Extensions;

public static class ArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this T[] target) => Array.Clear(target, 0, target.Length);
}