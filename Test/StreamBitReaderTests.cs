namespace InvertedTomato.Binary;

public class StreamBitReaderTests
{
    [Fact]
    public void ReadBit_1()
    {
        using var stream = new MemoryStream(new byte[] { 0b_10000000 });
        using var reader = new StreamBitReader(stream);

        Assert.True(reader.ReadBit());
    }

    [Fact]
    public void ReadBit_0()
    {
        using var stream = new MemoryStream(new byte[] { 0b_00000000 });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.ReadBit());
    }

    [Fact]
    public void ReadBit_0_1()
    {
        using var stream = new MemoryStream(new byte[] { 0b_01000000 });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.ReadBit());
        Assert.True(reader.ReadBit());
    }

    [Fact]
    public void ReadBits_Peak_8()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.True(reader.PeakBit());
        Assert.Equal((ulong)0b11111111, reader.ReadBits(8));
    }

    [Fact]
    public void ReadBits_Peak_8_Peak_8_0()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b00000000 });
        using var reader = new StreamBitReader(stream);

        Assert.True(reader.PeakBit());
        Assert.Equal((ulong)0b11111111, reader.ReadBits(8));

        Assert.False(reader.PeakBit());
        Assert.Equal((ulong)0b00000000, reader.ReadBits(8));
        Assert.Equal((ulong)0b00000000, reader.ReadBits(0));
    }

    [Fact]
    public void ReadBit_1_1_1_1_1_1_1_1_0_0_0_0_0_0_0_0()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b00000000 });
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
    public void ReadBits_4_Peak_8_Peak_4()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b00000000 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((ulong)0b1111, reader.ReadBits(4));

        Assert.True(reader.PeakBit());
        Assert.Equal((ulong)0b11110000, reader.ReadBits(8));

        Assert.False(reader.PeakBit());
        Assert.Equal((ulong)0b0000, reader.ReadBits(4));
    }

    [Fact]
    public void ReadBits_4_Peak_Align_Peak_4()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b00000000 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((ulong)0b1111, reader.ReadBits(4));
        Assert.True(reader.PeakBit());
        reader.Align();

        Assert.False(reader.PeakBit());
        Assert.Equal((ulong)0b0000, reader.ReadBits(4));
        Assert.Equal((ulong)0b0000, reader.ReadBits(4));
    }

    [Fact]
    public void ReadBits_32()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal(0b_11111111_11111111_11111111_11111111, reader.ReadBits(32));
    }

    [Fact]
    public void ReadBits_63()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111110 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((UInt64)0b_01111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111, reader.ReadBits(63));
    }

    [Fact]
    public void ReadBits_64()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal(0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111, reader.ReadBits(64));
    }

    [Fact]
    public void ReadBits_1_32()
    {
        using var stream = new MemoryStream(new byte[] { 0b_01111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b10000000, });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.ReadBit());
        Assert.Equal(0b_11111111_11111111_11111111_11111111, reader.ReadBits(32));
    }


    [Fact]
    public void ReadBit_x1()
    {
        using var stream = new MemoryStream(new byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000001, });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((UInt64)1, reader.ReadBits(64));
    }

    [Fact]
    public void DisposeNotOwned()
    {
        using var stream = new MemoryStream(new byte[] { 0b00000000 });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.IsDisposed);
        reader.Dispose();
        Assert.True(reader.IsDisposed);
        stream.ReadByte();
    }

    [Fact]
    public void DisposeOwned()
    {
        using var stream = new MemoryStream(new byte[] { 0b00000000 });
        using var reader = new StreamBitReader(stream, true);

        Assert.False(reader.IsDisposed);
        reader.Dispose();
        Assert.True(reader.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }


    [Fact]
    public void Blank()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((ulong)0b00000000, reader.ReadBits(0));
    }

    [Fact]
    public void EndOfStream_Read()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        reader.ReadBits(8);
        Assert.Throws<EndOfStreamException>(() => reader.ReadBits(1));
    }

    [Fact]
    public void EndOfStream_Peak()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        reader.ReadBits(8);
        Assert.Throws<EndOfStreamException>(() => reader.PeakBit());
    }

    [Fact]
    public void ReadBits_B8_8()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream, false, 1);

        Assert.Equal((UInt64)0b_11111111, reader.ReadBits(8));
    }

    [Fact]
    public void ReadBits_B8_9()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b10000000, });
        using var reader = new StreamBitReader(stream, false, 1);

        Assert.Equal((UInt64)0b_00000001_11111111, reader.ReadBits(9));
    }
}