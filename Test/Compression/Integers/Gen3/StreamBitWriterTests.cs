using System;
using System.IO;
using InvertedTomato.Compression.Integers.Gen3.Extensions;
using Xunit;

namespace InvertedTomato.Compression.Integers.Gen3;

public class StreamBitWriterTests
{
    [Fact]
    public void WriteBit_0()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBit(false);
        }

        Assert.Equal(new Byte[] {0b00000000,}, stream.ToArray());
    }
    
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
    public void WriteBit_0_1()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBit(false);
            writer.WriteBit(true);
        }

        Assert.Equal(new Byte[] {0b01000000,}, stream.ToArray());
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

        Assert.Equal(new Byte[] {0b10000000, 0b11000000}, stream.ToArray());
    }

    [Fact]
    public void WriteBits_Align()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.Align();
        }

        Assert.Equal(new Byte[] { }, stream.ToArray());
    }

    [Fact]
    public void WriteBits_8_Align()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b11111111, 8);
            writer.Align();
        }

        Assert.Equal(new Byte[] {0b11111111}, stream.ToArray());
    }

    [Fact]
    public void WriteBits_8_Align_8()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b11111111, 8);
            writer.Align();
            writer.WriteBits(0b11111111, 8);
        }

        Assert.Equal(new Byte[] {0b11111111, 0b11111111}, stream.ToArray());
    }

    [Fact]
    public void WriteBits_32()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b_11111111_11111111_11111111_11111111, 32);
        }

        Assert.Equal(new Byte[] {0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111,}, stream.ToArray());
    }
    
    [Fact]
    public void WriteBits_63()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b_01111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111, 63);
        }

        Assert.Equal(new Byte[] {0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111,0b_11111111, 0b_11111111, 0b_11111111, 0b_11111110,}, stream.ToArray());
    }
    
    [Fact]
    public void WriteBits_64()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111, 64);
        }

        Assert.Equal(new Byte[] {0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111,0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111,}, stream.ToArray());
    }

    [Fact]
    public void WriteBits_1_32()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBit(false);
            writer.WriteBits(0b_11111111_11111111_11111111_11111111, 32);
        }

        Assert.Equal(new Byte[] {0b_01111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b10000000,}, stream.ToArray());
    }
    
    
    [Fact]
    public void WriteBit_x1()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            writer.WriteBits(1,64);
        }

        Assert.Equal(new Byte[] {0b00000000,0b00000000,0b00000000,0b00000000,0b00000000,0b00000000,0b00000000,0b00000001,}.ToHexString(), stream.ToArray().ToHexString());
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
    
    
    [Fact]
    public void WriteBit_B1_8()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream,false,1))
        {
            writer.WriteBits(0b11111111,8);
        }

        Assert.Equal(new Byte[] {0b11111111,}, stream.ToArray());
    }
    
    [Fact]
    public void WriteBit_B1_9()
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream,false,1))
        {
            writer.WriteBits(0b111111111,9);
        }

        Assert.Equal(new Byte[] {0b11111111,0b10000000}, stream.ToArray());
    }
}