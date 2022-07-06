using System;
using Xunit;

namespace InvertedTomato.Compression.Integers;

public class BufferUtilTests
{
    [Fact]
    public void WriteBits_64_0()
    {
        var buffers = new[] {UInt64.MinValue, UInt64.MinValue};
        var offset = 0;
        BufferUtil.WriteBits(UInt64.MaxValue, ref buffers, ref offset, 64);
        Assert.Equal(2, buffers.Length);
        Assert.Equal("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111", B(buffers[0]));
        Assert.Equal("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000", B(buffers[1]));
        Assert.Equal(64, offset);
    }

    [Fact]
    public void WriteBits_64_8()
    {
        var buffers = new[] {UInt64.MinValue, UInt64.MinValue};
        var offset = 8;
        BufferUtil.WriteBits(UInt64.MaxValue, ref buffers, ref offset, 64);
        Assert.Equal(2, buffers.Length);
        Assert.Equal("00000000 11111111 11111111 11111111 11111111", B(buffers[0]));
        Assert.Equal("11111111 11111111 11111111 11111111 00000000", B(buffers[1]));
        Assert.Equal(64 + 8, offset);
    }

    [Fact]
    public void WriteBits_64_64()
    {
        var buffers = new[] {UInt64.MinValue, UInt64.MinValue};
        var offset = 64;
        BufferUtil.WriteBits(UInt64.MaxValue, ref buffers, ref offset, 64);
        Assert.Equal(2, buffers.Length);
        Assert.Equal("00000000 00000000 00000000 00000000 00000000 00000000 00000000 00000000", B(buffers[0]));
        Assert.Equal("11111111 11111111 11111111 11111111 11111111 11111111 11111111 11111111", B(buffers[1]));
        Assert.Equal(64 + 64, offset);
    }

    private string B(ulong value) => BufferUtil.ToBinaryString(value);
}