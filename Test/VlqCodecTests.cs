namespace InvertedTomato.Packing;

public class VlqCodecTests
{
    // Encode

    private Byte[] Encode(UInt64 value)
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new VlqIntegerEncoder(writer);
            encoder.EncodeUInt64(value);
        }

        return stream.ToArray();
    }

    [Fact]
    public void Encode_0() => Assert.Equal(new Byte[] { 0b00000000 }, Encode(0));

    [Fact]
    public void Encode_1() => Assert.Equal(new Byte[] { 0b00000001 }, Encode(1));

    [Fact]
    public void Encode_2() => Assert.Equal(new Byte[] { 0b00000010 }, Encode(2));

    [Fact]
    public void Encode_3() => Assert.Equal(new Byte[] { 0b00000011 }, Encode(3));

    [Fact]
    public void Encode_127() => Assert.Equal(new Byte[] { 0b01111111 }, Encode(127));

    [Fact]
    public void Encode_128() => Assert.Equal(new Byte[] { 0b10000000, 0b00000000 }, Encode(128));

    [Fact]
    public void Encode_129() => Assert.Equal(new Byte[] { 0b10000001, 0b00000000 }, Encode(129));

    [Fact]
    public void Encode_16511() => Assert.Equal(new Byte[] { 0b11111111, 0b01111111 }, Encode(16511));

    [Fact]
    public void Encode_16512() => Assert.Equal(new Byte[] { 0b10000000, 0b10000000, 0b00000000 }, Encode(16512));

    [Fact]
    public void Encode_2113663() => Assert.Equal(new Byte[] { 0b11111111, 0b11111111, 0b01111111 }, Encode(2113663));

    [Fact]
    public void Encode_2113664() => Assert.Equal(new Byte[] { 0b10000000, 0b10000000, 0b10000000, 0b00000000 }, Encode(2113664));

    [Fact]
    public void EncodeMax() => Assert.Equal(new Byte[] { 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000 },
        Encode(VlqInteger.MaxValue));

    [Fact]
    public void EncodeOverflow() => Assert.Throws<OverflowException>(() => { Encode(UInt64.MaxValue); });

    // Decode

    private UInt64 Decode(Byte[] encoded)
    {
        using var stream = new MemoryStream(encoded);
        using var reader = new StreamBitReader(stream);
        var decoder = new VlqIntegerDecoder(reader);
        return decoder.DecodeUInt64();
    }

    [Fact]
    public void Decode_0() => Assert.Equal((UInt64)0, Decode(new Byte[] { 0b00000000 }));

    [Fact]
    public void Decode_1() => Assert.Equal((UInt64)1, Decode(new Byte[] { 0b00000001 }));

    [Fact]
    public void Decode_2() => Assert.Equal((UInt64)2, Decode(new Byte[] { 0b00000010 }));

    [Fact]
    public void Decode_3() => Assert.Equal((UInt64)3, Decode(new Byte[] { 0b00000011 }));

    [Fact]
    public void Decode_127() => Assert.Equal((UInt64)127, Decode(new Byte[] { 0b01111111 }));

    [Fact]
    public void Decode_128() => Assert.Equal((UInt64)128, Decode(new Byte[] { 0b10000000, 0b00000000 }));

    [Fact]
    public void Decode_129() => Assert.Equal((UInt64)129, Decode(new Byte[] { 0b10000001, 0b00000000 }));

    [Fact]
    public void Decode_16511() => Assert.Equal((UInt64)16511, Decode(new Byte[] { 0b11111111, 0b01111111 }));

    [Fact]
    public void Decode_16512() => Assert.Equal((UInt64)16512, Decode(new Byte[] { 0b10000000, 0b10000000, 0b00000000 }));

    [Fact]
    public void Decode_16513() => Assert.Equal((UInt64)16513, Decode(new Byte[] { 0b10000001, 0b10000000, 0b00000000 }));

    [Fact]
    public void Decode_2113663() => Assert.Equal((UInt64)2113663, Decode(new Byte[] { 0b11111111, 0b11111111, 0b01111111 }));

    [Fact]
    public void Decode_2113664() => Assert.Equal((UInt64)2113664, Decode(new Byte[] { 0b10000000, 0b10000000, 0b10000000, 0b00000000 }));

    [Fact]
    public void DecodeMax() => Assert.Equal(VlqInteger.MaxValue,
        Decode(new Byte[] { 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000 }));

    [Fact]
    public void Decode_1_1_1()
    {
        using var stream = new MemoryStream(new Byte[] { 0b00000001, 0b00000001, 0b00000001 });
        using var reader = new StreamBitReader(stream);
        var decoder = new VlqIntegerDecoder(reader);

        Assert.Equal((UInt64)1, decoder.DecodeUInt64());
        Assert.Equal((UInt64)1, decoder.DecodeUInt64());
        Assert.Equal((UInt64)1, decoder.DecodeUInt64());
        Assert.Throws<EndOfStreamException>(() => decoder.DecodeUInt64());
    }

    [Fact]
    public void DecodeInputClipped() => Assert.Throws<EndOfStreamException>(() => { Decode(new Byte[] { 0b10000000 }); });

    [Fact]
    public void DecodeOverflow() => Assert.Throws<OverflowException>(() =>
    {
        Decode(new Byte[] { 0b11111111, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000 });
    });

    [Fact]
    public void Decode_1_X() => Assert.Equal((UInt64)1, Decode(new Byte[] { 0b00000001, 0b10000011 }));


    [Fact]
    public void CanEncodeDecodeFirst1000()
    {
        using var stream = new MemoryStream();

        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new VlqIntegerEncoder(writer);
            for (UInt64 symbol = 0; symbol < 1000; symbol++) encoder.EncodeUInt64(symbol);
        }

        stream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamBitReader(stream))
        {
            var decoder = new VlqIntegerDecoder(reader);
            for (UInt64 symbol = 0; symbol < 1000; symbol++)
            {
                Assert.Equal(symbol, decoder.DecodeUInt64());
            }
        }
    }
}