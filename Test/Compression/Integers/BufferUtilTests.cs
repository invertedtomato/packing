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
        BufferUtil.WriteBits(UInt64.MaxValue, 64, ref buffers, ref offset);
        Assert.Equal(2, buffers.Length);
        Assert.Equal(UInt64.MaxValue, buffers[0]);
        Assert.Equal(UInt64.MinValue, buffers[1]);
        Assert.Equal(64, offset);
    }
    
    [Fact]
    public void WriteBits_64_8()
    {
        var buffers = new[] {UInt64.MinValue, UInt64.MinValue};
        var offset = 8;
        BufferUtil.WriteBits(UInt64.MaxValue, 64, ref buffers, ref offset);
        Assert.Equal(2, buffers.Length);
        Assert.Equal(UInt64.MaxValue >> 8, buffers[0]);
        Assert.Equal(UInt64.MaxValue << (64-8), buffers[1]);
        Assert.Equal(64+8, offset);
    }
    
    [Fact]
    public void WriteBits_64_64()
    {
        var buffers = new[] {UInt64.MinValue, UInt64.MinValue};
        var offset = 64;
        BufferUtil.WriteBits(UInt64.MaxValue, 64, ref buffers, ref offset);
        Assert.Equal(2, buffers.Length);
        Assert.Equal(UInt64.MinValue , buffers[0]);
        Assert.Equal(UInt64.MaxValue, buffers[1]);
        Assert.Equal(64+64, offset);
    }
}