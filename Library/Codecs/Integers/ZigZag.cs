namespace InvertedTomato.Binary.Codecs.Integers;

/// <summary>
/// Encode signed values as unsigned using ProtoBuffer ZigZag bijection encoding algorithm.
/// https://developers.google.com/protocol-buffers/docs/encoding
/// </summary>
public static class ZigZagUtility
{
    /// <summary>
    /// Encode a signed long into an ZigZag unsigned long
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 Encode(Int64 value) => (UInt64)((value << 1) ^ (value >> 63));

    /// <summary>
    /// Decode a ZigZag unsigned long back into a signed long
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 Decode(UInt64 value) => (Int64)((value >> 1) ^ (~(value & 1) + 1));
}