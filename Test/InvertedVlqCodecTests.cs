namespace InvertedTomato.Binary;

public class InvertedVlqCodecTests
{
    private Byte[] Encode(UInt64 value, Int32 expectedCount)
    {
        var codec = new InvertedVlqIntegerCodec();
        using var stream = new MemoryStream(expectedCount);
        using (var writer = new StreamBitWriter(stream))
        {
            codec.EncodeUInt64(value, writer);
        }

        return stream.ToArray();
    }

    private UInt64 Decode(Byte[] encoded, Int32 expectedUsed)
    {
        var codec = new InvertedVlqIntegerCodec();
        using var stream = new MemoryStream(encoded);
        using var reader = new StreamBitReader(stream);

        return codec.DecodeUInt64(reader);
    }

    [Fact]
    public void Encode_0()
    {
        Assert.Equal(new Byte[] { 0b10000000 }, Encode(0, 1));
    }

    [Fact]
    public void Encode_1()
    {
        Assert.Equal(new Byte[] { 0b10000001 }, Encode(1, 1));
    }

    [Fact]
    public void Encode_2()
    {
        Assert.Equal(new Byte[] { 0b10000010 }, Encode(2, 1));
    }

    [Fact]
    public void Encode_3()
    {
        Assert.Equal(new Byte[] { 0b10000011 }, Encode(3, 1));
    }

    [Fact]
    public void Encode_127()
    {
        Assert.Equal(new Byte[] { 0b11111111 }, Encode(127, 1));
    }

    [Fact]
    public void Encode_128()
    {
        Assert.Equal(new Byte[] { 0b00000000, 0b10000000 }, Encode(128, 2));
    }

    [Fact]
    public void Encode_129()
    {
        Assert.Equal(new Byte[] { 0b00000001, 0b10000000 }, Encode(129, 2));
    }

    [Fact]
    public void Encode_16511()
    {
        Assert.Equal(new Byte[] { 0b01111111, 0b11111111 }, Encode(16511, 2));
    }

    [Fact]
    public void Encode_16512()
    {
        Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b10000000 }, Encode(16512, 3));
    }

    [Fact]
    public void Encode_2113663()
    {
        Assert.Equal(new Byte[] { 0b01111111, 0b01111111, 0b11111111 }, Encode(2113663, 3));
    }

    [Fact]
    public void Encode_2113664()
    {
        Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b10000000 }, Encode(2113664, 4));
    }

    [Fact]
    public void Encode_Max()
    {
        Assert.Equal(new Byte[] { 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b10000000 },
            Encode(new InvertedVlqIntegerCodec().MaxValue, 10));
    }

    [Fact]
    public void Encode_Overflow()
    {
        Assert.Throws<OverflowException>(() => { Encode(UInt64.MaxValue, 32); });
    }

    [Fact]
    public void Decode_0()
    {
        Assert.Equal((UInt64)0, Decode(new Byte[] { 0b10000000 }, 1));
    }

    [Fact]
    public void Decode_1()
    {
        Assert.Equal((UInt64)1, Decode(new Byte[] { 0b10000001 }, 1));
    }

    [Fact]
    public void Decode_2()
    {
        Assert.Equal((UInt64)2, Decode(new Byte[] { 0b10000010 }, 1));
    }

    [Fact]
    public void Decode_3()
    {
        Assert.Equal((UInt64)3, Decode(new Byte[] { 0b10000011 }, 1));
    }

    [Fact]
    public void Decode_127()
    {
        Assert.Equal((UInt64)127, Decode(new Byte[] { 0b11111111 }, 1));
    }

    [Fact]
    public void Decode_128()
    {
        Assert.Equal((UInt64)128, Decode(new Byte[] { 0b00000000, 0b10000000 }, 2));
    }

    [Fact]
    public void Decode_129()
    {
        Assert.Equal((UInt64)129, Decode(new Byte[] { 0b00000001, 0b10000000 }, 2));
    }

    [Fact]
    public void Decode_16511()
    {
        Assert.Equal((UInt64)16511, Decode(new Byte[] { 0b01111111, 0b11111111 }, 2));
    }

    [Fact]
    public void Decode_16512()
    {
        Assert.Equal((UInt64)16512, Decode(new Byte[] { 0b00000000, 0b00000000, 0b10000000 }, 3));
    }

    [Fact]
    public void Decode_16513()
    {
        Assert.Equal((UInt64)16513, Decode(new Byte[] { 0b00000001, 0b00000000, 0b10000000 }, 3));
    }

    [Fact]
    public void Decode_2113663()
    {
        Assert.Equal((UInt64)2113663, Decode(new Byte[] { 0b01111111, 0b01111111, 0b11111111 }, 3));
    }

    [Fact]
    public void Decode_2113664()
    {
        Assert.Equal((UInt64)2113664, Decode(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b10000000 }, 4));
    }

    [Fact]
    public void Decode_Max()
    {
        Assert.Equal(new InvertedVlqIntegerCodec().MaxValue,
            Decode(new Byte[] { 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b10000000 }, 10));
    }

    [Fact]
    public void Decode_Overflow()
    {
        Assert.Throws<OverflowException>(() =>
        {
            Decode(new Byte[] { 0b01111111, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b10000000 }, 11);
        });
    }


    [Fact]
    public void EncodeDecode_1000()
    {
        var ta = new InvertedVlqIntegerCodec();
        using var stream = new MemoryStream();

        // Encode
        using (var writer = new StreamBitWriter(stream))
        {
            for (UInt64 symbol = 0; symbol < 1000; symbol++)
            {
                ta.EncodeUInt64(symbol, writer);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        // Decode
        using (var reader = new StreamBitReader(stream))
        {
            for (UInt64 symbol = 0; symbol < 1000; symbol++)
            {
                Assert.Equal(symbol, ta.DecodeUInt64(reader));
            }
        }
    }
}