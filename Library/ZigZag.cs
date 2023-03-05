using System;
using System.Runtime.CompilerServices;

namespace InvertedTomato.Compression.Integers;

/// <summary>
/// Encode signed values as unsigned using ProtoBuffer ZigZag bijection encoding algorithm.
/// https://developers.google.com/protocol-buffers/docs/encoding
/// </summary>
public static class ZigZag
{
    /// <summary>
    /// Encode a signed long into an ZigZag unsigned long
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64 Encode(Int64 value)
    {
        return (UInt64)((value << 1) ^ (value >> 63));
    }

    /// <summary>
    /// Encode an array of signed longs into a ZigZag encoded array of unsigned longs
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static UInt64[] Encode(Int64[] values)
    {
        var output = new UInt64[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            output[i] = Encode(values[i]);
        }

        return output;
    }

    /// <summary>
    /// Decode a ZigZag unsigned long back into a signed long
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64 Decode(UInt64 value)
    {
        return (Int64)((value >> 1) ^ (~(value & 1) + 1));
    }

    /// <summary>
    /// Decode an array of unsigned longs into a ZigZag encoded array of signed longs
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int64[] Decode(UInt64[] values)
    {
        var output = new Int64[values.Length];
        for (var i = 0; i < values.Length; i++)
        {
            output[i] = Decode(values[i]);
        }

        return output;
    }
}