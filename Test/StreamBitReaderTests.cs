namespace InvertedTomato.Packing;

public class StreamBitReaderTests
{
    [Fact]
    public void CanReadBit1()
    {
        using var stream = new MemoryStream(new byte[] { 0b_10000000 });
        using var reader = new StreamBitReader(stream);

        Assert.True(reader.ReadBit());
    }

    [Fact]
    public void CanReadBit0()
    {
        using var stream = new MemoryStream(new byte[] { 0b_00000000 });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.ReadBit());
    }

    [Fact]
    public void CanReadBit0_1()
    {
        using var stream = new MemoryStream(new byte[] { 0b_01000000 });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.ReadBit());
        Assert.True(reader.ReadBit());
    }

    [Fact]
    public void CanPeak_ReadBit8()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.True(reader.PeakBit());
        Assert.Equal((ulong)0b11111111, reader.ReadBits(8));
    }

    [Fact]
    public void CanPeak_ReadBit8_Peak_ReadBit8_ReadBit0()
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
    public void CanReadBit1_1_1_1_1_1_1_1_0_0_0_0_0_0_0_0()
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
    public void CanReadBits4_Peak_8_Peak_4()
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
    public void CanReadBits4_Peak_Align_Peak_4()
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
    public void CanReadBits32()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal(0b_11111111_11111111_11111111_11111111, reader.ReadBits(32));
    }

    [Fact]
    public void CanReadBits63()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111110 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((UInt64)0b_01111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111, reader.ReadBits(63));
    }

    [Fact]
    public void CanReadBits64()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal(0b_11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111, reader.ReadBits(64));
    }

    [Fact]
    public void CanReadBits1_32()
    {
        using var stream = new MemoryStream(new byte[] { 0b_01111111, 0b_11111111, 0b_11111111, 0b_11111111, 0b10000000, });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.ReadBit());
        Assert.Equal(0b_11111111_11111111_11111111_11111111, reader.ReadBits(32));
    }


    [Fact]
    public void CanReadBitX_1()
    {
        using var stream = new MemoryStream(new byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000001, });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((UInt64)1, reader.ReadBits(64));
    }

    [Fact]
    public void CanDisposeNotOwned()
    {
        using var stream = new MemoryStream(new byte[] { 0b00000000 });
        using var reader = new StreamBitReader(stream);

        Assert.False(reader.IsDisposed);
        reader.Dispose();
        Assert.True(reader.IsDisposed);
        stream.ReadByte();
    }

    [Fact]
    public void CanDisposeOwned()
    {
        using var stream = new MemoryStream(new byte[] { 0b00000000 });
        using var reader = new StreamBitReader(stream, true);

        Assert.False(reader.IsDisposed);
        reader.Dispose();
        Assert.True(reader.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
    }


    [Fact]
    public void CanReadBlank()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        Assert.Equal((ulong)0b00000000, reader.ReadBits(0));
    }

    [Fact]
    public void ReadEndOfStreamThrows()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        reader.ReadBits(8);
        Assert.Throws<EndOfStreamException>(() => reader.ReadBits(1));
    }

    [Fact]
    public void PeakEndOfStreamThrows()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream);

        reader.ReadBits(8);
        Assert.Throws<EndOfStreamException>(() => reader.PeakBit());
    }

    [Fact]
    public void CanReadBitsB8_8()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111 });
        using var reader = new StreamBitReader(stream, false, 1);

        Assert.Equal((UInt64)0b_11111111, reader.ReadBits(8));
    }

    [Fact]
    public void CanReadBitsB8_9()
    {
        using var stream = new MemoryStream(new byte[] { 0b_11111111, 0b10000000, });
        using var reader = new StreamBitReader(stream, false, 1);

        Assert.Equal((UInt64)0b_00000001_11111111, reader.ReadBits(9));
    }
}