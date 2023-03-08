namespace InvertedTomato.Packing;

public class InvertedVlqCodecTests
{
    private static Byte[] Encode(UInt64 value, Int32 expectedCount)
    {
        using var stream = new MemoryStream(expectedCount);
        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new InvertedVlqIntegerEncoder(writer);
            encoder.EncodeUInt64(value);
        }

        return stream.ToArray();
    }

    [Fact]
    public void CanEncode0() => Assert.Equal(new Byte[] { 0b10000000 }, Encode(0, 1));
    
    [Fact]
    public void CanEncode1() => Assert.Equal(new Byte[] { 0b10000001 }, Encode(1, 1));
    
    [Fact]
    public void CanEncode2() => Assert.Equal(new Byte[] { 0b10000010 }, Encode(2, 1));
    
    [Fact]
    public void CanEncode3() => Assert.Equal(new Byte[] { 0b10000011 }, Encode(3, 1));
    
    [Fact]
    public void CanEncode127() => Assert.Equal(new Byte[] { 0b11111111 }, Encode(127, 1));
    
    [Fact]
    public void CanEncode128() => Assert.Equal(new Byte[] { 0b00000000, 0b10000000 }, Encode(128, 2));
    
    [Fact]
    public void CanEncode129() => Assert.Equal(new Byte[] { 0b00000001, 0b10000000 }, Encode(129, 2));
    
    [Fact]
    public void CanEncode16511() => Assert.Equal(new Byte[] { 0b01111111, 0b11111111 }, Encode(16511, 2));
    
    [Fact]
    public void CanEncode16512() => Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b10000000 }, Encode(16512, 3));
    
    [Fact]
    public void CanEncode2113663() => Assert.Equal(new Byte[] { 0b01111111, 0b01111111, 0b11111111 }, Encode(2113663, 3));
    
    [Fact]
    public void CanEncode2113664() => Assert.Equal(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b10000000 }, Encode(2113664, 4));
    
    [Fact]
    public void CanEncodeMax() => Assert.Equal(new Byte[] { 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b10000000 },
        Encode(InvertedVlqInteger.MaxValue, 10));
    
    [Fact]
    public void EncoderOverflowThrows() => Assert.Throws<OverflowException>(() => { Encode(UInt64.MaxValue, 32); });
    
    private static UInt64 Decode(Byte[] encoded, Int32 expectedUsed)
    {
        using var stream = new MemoryStream(encoded);
        using var reader = new StreamBitReader(stream);
        var decoder = new InvertedVlqIntegerDecoder(reader);

        return decoder.DecodeUInt64();
    }

    [Fact]
    public void CanDecode0() => Assert.Equal((UInt64)0, Decode(new Byte[] { 0b10000000 }, 1));
    
    [Fact]
    public void CanDecode1() => Assert.Equal((UInt64)1, Decode(new Byte[] { 0b10000001 }, 1));
    
    [Fact]
    public void CanDecode2() => Assert.Equal((UInt64)2, Decode(new Byte[] { 0b10000010 }, 1));
    
    [Fact]
    public void CanDecode3() => Assert.Equal((UInt64)3, Decode(new Byte[] { 0b10000011 }, 1));
    
    [Fact]
    public void CanDecode127() => Assert.Equal((UInt64)127, Decode(new Byte[] { 0b11111111 }, 1));

    [Fact]
    public void CanDecode128() => Assert.Equal((UInt64)128, Decode(new Byte[] { 0b00000000, 0b10000000 }, 2));
    
    [Fact]
    public void CanDecode129() => Assert.Equal((UInt64)129, Decode(new Byte[] { 0b00000001, 0b10000000 }, 2));
    
    [Fact]
    public void CanDecode16511() => Assert.Equal((UInt64)16511, Decode(new Byte[] { 0b01111111, 0b11111111 }, 2));
    
    [Fact]
    public void CanDecode16512() => Assert.Equal((UInt64)16512, Decode(new Byte[] { 0b00000000, 0b00000000, 0b10000000 }, 3));
    
    [Fact]
    public void CanDecode16513() => Assert.Equal((UInt64)16513, Decode(new Byte[] { 0b00000001, 0b00000000, 0b10000000 }, 3));
    
    [Fact]
    public void CanDecode2113663() => Assert.Equal((UInt64)2113663, Decode(new Byte[] { 0b01111111, 0b01111111, 0b11111111 }, 3));
    
    [Fact]
    public void CanDecode2113664() => Assert.Equal((UInt64)2113664, Decode(new Byte[] { 0b00000000, 0b00000000, 0b00000000, 0b10000000 }, 4));
    
    [Fact]
    public void CanDecodeMax() => Assert.Equal(InvertedVlqInteger.MaxValue,
        Decode(new Byte[] { 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b10000000 }, 10));

    [Fact]
    public void DecodingOverflowThrows() => Assert.Throws<OverflowException>(() =>
    {
        Decode(new Byte[] { 0b01111111, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b01111110, 0b10000000 }, 11);
    });


    [Fact]
    public void CanEncodeDecodeFirst1000()
    {
        using var stream = new MemoryStream();

        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new InvertedVlqIntegerEncoder(writer);
            for (UInt64 symbol = 0; symbol < 1000; symbol++) encoder.EncodeUInt64(symbol);
        }

        stream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamBitReader(stream))
        {
            var decoder = new InvertedVlqIntegerDecoder(reader);
            for (UInt64 symbol = 0; symbol < 1000; symbol++)
            {
                Assert.Equal(symbol, decoder.DecodeUInt64());
            }
        }
    }
}