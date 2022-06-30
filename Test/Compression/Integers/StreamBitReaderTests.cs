using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers;

public class StreamBitReaderTests
{
    [Fact]
    public void ReadBit_1_1_1_1()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111, 0b00000000});
        using var reader = new StreamBitReader(stream);

        Assert.True(reader.ReadBit());
        Assert.True(reader.ReadBit());
        Assert.True(reader.ReadBit());
        Assert.True(reader.ReadBit());
        Assert.True(reader.ReadBit());
        Assert.True(reader.ReadBit());
        Assert.True(reader.ReadBit());
        Assert.True(reader.ReadBit());

        Assert.False(reader.ReadBit());
        Assert.False(reader.ReadBit());
        Assert.False(reader.ReadBit());
        Assert.False(reader.ReadBit());
        Assert.False(reader.ReadBit());
        Assert.False(reader.ReadBit());
        Assert.False(reader.ReadBit());
        Assert.False(reader.ReadBit());
    }

    [Fact]
    public void ReadBits_8_8_0()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111, 0b00000000});
        using var reader = new StreamBitReader(stream);

        Assert.True(reader.PeakBit());
        Assert.Equal((ulong) 0b11111111, reader.ReadBits(8));

        Assert.False(reader.PeakBit());
        Assert.Equal((ulong) 0b00000000, reader.ReadBits(8));
        Assert.Equal((ulong) 0b00000000, reader.ReadBits(0));
    }

    [Fact]
    public void ReadBits_4_8_4()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111, 0b00000000});
        using var reader = new StreamBitReader(stream);

        Assert.Equal((ulong) 0b00001111, reader.ReadBits(4));

        Assert.True(reader.PeakBit());
        Assert.Equal((ulong) 0b00001111, reader.ReadBits(8));

        Assert.False(reader.PeakBit());
        Assert.Equal((ulong) 0b00000000, reader.ReadBits(4));
    }

    [Fact]
    public void ReadBits_4_Align_4()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111, 0b00000000});
        using var reader = new StreamBitReader(stream);

        Assert.Equal((ulong) 0b00001111, reader.ReadBits(4));
        Assert.True(reader.PeakBit());
        reader.Align();

        Assert.False(reader.PeakBit());
        Assert.Equal((ulong) 0b00000000, reader.ReadBits(4));
        Assert.Equal((ulong) 0b00000000, reader.ReadBits(4));
    }

    [Fact]
    public void ReadByte()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111, 0b00000000});
        using var reader = new StreamBitReader(stream);

        Assert.Equal((ulong) 0b11111111, reader.ReadByte());
        Assert.Equal((ulong) 0b00000000, reader.ReadByte());
    }

    [Fact]
    public void DisposeNotOwned()
    {
        using var stream = new MemoryStream(new byte[] {0b00000000});
        using var reader = new StreamBitReader(stream);
        
        Assert.False(reader.IsDisposed);
        reader.Dispose();
        Assert.True(reader.IsDisposed);
        stream.ReadByte();
    }

    [Fact]
    public void DisposeOwned()
    {
        using var stream = new MemoryStream(new byte[] {0b00000000});
        using var reader = new StreamBitReader(stream, true);
        
        Assert.False(reader.IsDisposed);
        reader.Dispose();
        Assert.True(reader.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }


    [Fact]
    public void Blank()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111});
        using var reader = new StreamBitReader(stream);
        Assert.Equal((ulong) 0b00000000, reader.ReadBits(0));
    }

    [Fact]
    public void EndOfStream_Read()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111});
        using var reader = new StreamBitReader(stream);
        reader.ReadBits(8);
        Assert.Throws<EndOfStreamException>(() => reader.ReadBits(1));
    }

    [Fact]
    public void EndOfStream_Peak()
    {
        using var stream = new MemoryStream(new byte[] {0b_11111111});
        using var reader = new StreamBitReader(stream);
        reader.ReadBits(8);
        Assert.Throws<EndOfStreamException>(() => reader.PeakBit());
    }

    // TODO: align
}