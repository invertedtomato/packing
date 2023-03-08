namespace InvertedTomato.Packing;

public class RawCodecTests
{
    private static Byte[] Encode(UInt64 value)
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new RawIntegerEncoder(writer);
            encoder.EncodeUInt64(value);
        }

        return stream.ToArray();
    }

    [Fact]
    public void CanEncode0() => Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 }.ToHexString(), Encode(0).ToHexString());
    
    [Fact]
    public void CanEncode1() => Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000001 }.ToHexString(), Encode(1).ToHexString());

    [Fact]
    public void CanEncode2() => Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000010 }.ToHexString(), Encode(2).ToHexString());
    
    [Fact]
    public void CanEncode3() => Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000011 }.ToHexString(), Encode(3).ToHexString());
    
    [Fact]
    public void CanEncodeMax() => Assert.Equal(new Byte[] { 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111 }.ToHexString(),
        Encode(UInt64.MaxValue).ToHexString());

    private static UInt64 Decode(Byte[] encoded)
    {
        using var stream = new MemoryStream(encoded);
        using var reader = new StreamBitReader(stream);
        var decoder = new RawIntegerDecoder(reader);
        return decoder.DecodeUInt64();
    }
    
    [Fact]
    public void CanDecode0() => Assert.Equal((UInt64)0, Decode(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000 }));

    [Fact]
    public void CanDecode1() => Assert.Equal((UInt64)1, Decode(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000001 }));

    [Fact]
    public void CanDecode2() => Assert.Equal((UInt64)2, Decode(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000010 }));

    [Fact]
    public void CanDecode3() => Assert.Equal((UInt64)3, Decode(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000000, 0b00000011 }));
    
    [Fact]
    public void CanDecodeMax() => Assert.Equal(RawInteger.MaxValue, Decode(new Byte[] { 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111 }));


    [Fact]
    public void CanEncodeDecodeFirst1000()
    {
        using var stream = new MemoryStream();

        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new RawIntegerEncoder(writer);
            for (UInt64 symbol = 0; symbol < 1000; symbol++) encoder.EncodeUInt64(symbol);
        }

        stream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamBitReader(stream))
        {
            var decoder = new RawIntegerDecoder(reader);
            for (UInt64 symbol = 0; symbol < 1000; symbol++)
            {
                Assert.Equal(symbol, decoder.DecodeUInt64());
            }
        }
    }
}