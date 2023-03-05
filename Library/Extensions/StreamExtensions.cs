using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace InvertedTomato.Compression.Integers.Gen3.Extensions;

public static class StreamExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream target, Byte[] buffer) => target.Write(buffer, 0, buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream target, Byte[] buffer, Int32 count) => target.Write(buffer, 0, count);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Read(this Stream target, Byte[] buffer) => target.Read(buffer, 0, buffer.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Int32 Read(this Stream target, Byte[] buffer, Int32 count) => target.Read(buffer, 0, count);
}