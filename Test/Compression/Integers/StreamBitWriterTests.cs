using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers;

public class StreamBitWriterTests
{
    [Fact]
    public void WriteBit_1()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBit(true);
        }

        Assert.Equal(new Byte[] {0b10000000,}, stream.ToArray());
    }

    [Fact]
    public void WriteBit_1_1_1_1_1_1_1_1()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
        }

        Assert.Equal(new Byte[] {0b11111111,}, stream.ToArray());
    }

    [Fact]
    public void WriteBit_1_1_1_1_1_1_1_1_1()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
            writer.WriteBit(true);
        }

        Assert.Equal(new Byte[] {0b11111111, 0b10000000}, stream.ToArray());
    }

    [Fact]
    public void WriteBits_10_1()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b10, 2);
            writer.WriteBits(0b1, 1);
        }

        Assert.Equal(new Byte[] {0b10100000,}, stream.ToArray());
    }
    
    [Fact]
    public void WriteBits_1_0_1_0_1_0()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b1, 1);
            writer.WriteBits(0b0, 1);
            writer.WriteBits(0b1, 1);
            writer.WriteBits(0b0, 1);
            writer.WriteBits(0b1, 1);
            writer.WriteBits(0b0, 1);
            writer.WriteBits(0b1, 1);
            writer.WriteBits(0b0, 1);
        }

        Assert.Equal(new Byte[] {0b10101010,}, stream.ToArray());
    }


    [Fact]
    public void WriteBits_10_10_10_101()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b10, 2);
            writer.WriteBits(0b10, 2);
            writer.WriteBits(0b10, 2);
            writer.WriteBits(0b101, 3);
        }

        Assert.Equal(new Byte[] {0b10101010, 0b10000000,}, stream.ToArray());
    }


    [Fact]
    public void WriteBits_10_Align_10()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b10, 2);
            writer.Align();
            writer.WriteBits(0b11, 2);
        }

        Assert.Equal(new Byte[] {0b10000000,0b11000000}, stream.ToArray());
    }

    [Fact]
    public void WriteBits_56()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111, 56);
        }

        Assert.Equal(new Byte[] {0b_11111111,0b_11111111,0b_11111111,0b_11111111,0b_11111111,0b_11111111,0b_11111111,}, stream.ToArray());
    }
    
    [Fact]
    public void WriteBits_1_56()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBit(true);
            writer.WriteBits(0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111, 56);
        }

        Assert.Equal(new Byte[] {0b_01111111,0b_11111111,0b_11111111,0b_11111111,0b_11111111,0b_11111111,0b_11111111,0b10000000,}, stream.ToArray());
    }

    [Fact]
    public void DisposeNotOwned()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamBitWriter(stream);

        Assert.False(writer.IsDisposed);
        writer.Dispose();
        Assert.True(writer.IsDisposed);
        stream.ReadByte();
    }

    [Fact]
    public void DisposeOwned()
    {
        using var stream = new MemoryStream();
        using var writer = new StreamBitWriter(stream, true);

        Assert.False(writer.IsDisposed);
        writer.Dispose();
        Assert.True(writer.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }
    
    // TODO: Writing 64bits
}