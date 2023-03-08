using FluentAssertions;

namespace InvertedTomato.Packing;

public class ThompsonAlphaTests
{
    // Max | Bin | Value
    //  1 | _1 | 2
    //  2 | _11 | 6
    //  3 | _111 | 14
    //  4 | _1111 | 30
    //  5 | _11111
    
    // 2^(bits + 1)
    
    [Fact]
    public void CanCalculateMaxValue1() => new ThompsonAlphaIntegerEncoder(null!, 1).MaxValue.Should().Be(15);
    [Fact]
    public void CanCalculateMaxValue2() => new ThompsonAlphaIntegerEncoder(null!, 2).MaxValue.Should().Be(255);
    [Fact]
    public void CanCalculateMaxValue3() => new ThompsonAlphaIntegerEncoder(null!, 3).MaxValue.Should().Be(65535);
    [Fact]
    public void CanCalculateMaxValue4() => new ThompsonAlphaIntegerEncoder(null!, 4).MaxValue.Should().Be(4294967295);
    [Fact]
    public void CanCalculateMaxValue5() => new ThompsonAlphaIntegerEncoder(null!, 5).MaxValue.Should().Be(18446744073709551615);
    [Fact]
    public void CanCalculateMaxValue6() => new ThompsonAlphaIntegerEncoder(null!, 6).MaxValue.Should().Be(18446744073709551615);
    
    private static Byte[] Encode(UInt64 value)
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new ThompsonAlphaIntegerEncoder(writer, 6);
            encoder.EncodeUInt64(value);
        }

        return stream.ToArray();
    }

    [Fact]
    public void CanEncode0() => Assert.Equal(new Byte[] {0b000000_00}, Encode(0)); // Len=0, Val=(1)

    [Fact]
    public void CanEncode1() => Assert.Equal(new Byte[] {0b000001_0_0}, Encode(1)); // Len=1, Val=(1)1

    [Fact]
    public void CanEncode2() => Assert.Equal(new Byte[] {0b000001_1_0}, Encode(2)); // Len=10, val=(1)10

    [Fact]
    public void CanEncode3() => Assert.Equal(new Byte[] {0b000010_00}, Encode(3)); // Len=10, val=(1)11

    [Fact]
    public void CanEncode8589934590() => Assert.Equal(new Byte[] {0b100000_11, 0b11111111, 0b11111111, 0b11111111, 0b111111_00}, Encode(8589934590));

    [Fact]
    public void CanEncode8589934591() => Assert.Equal(new Byte[] {0b100001_00, 0b00000000, 0b00000000, 0b00000000, 0b0000000_0}, Encode(8589934591));

    [Fact]
    public void CanEncodeMax() => Assert.Equal(new Byte[] {0b111111_11, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111000}, Encode(UInt64.MaxValue - 1));

    private static UInt64 Decode(Byte[] encoded)
    {
        using var stream = new MemoryStream(encoded);
        using var reader = new StreamBitReader(stream);
        var decoder = new ThompsonAlphaIntegerDecoder(reader, 6);

        return decoder.DecodeUInt64();
    }
        
    [Fact]
    public void CanDecode0() => Assert.Equal((UInt64) 0, Decode(new Byte[] {0b000000_00}));

    [Fact]
    public void CanDecode1() => Assert.Equal((UInt64) 1, Decode(new Byte[] {0b000001_0_0})); // (len)_(val)_(padding)

    [Fact]
    public void CanDecode2() => Assert.Equal((UInt64) 2, Decode(new Byte[] {0b000001_1_0}));

    [Fact]
    public void CanDecode3() =>  Assert.Equal((UInt64) 3, Decode(new Byte[] {0b000010_00}));

    [Fact]
    public void CanDecode8589934590() => Assert.Equal((UInt64) 8589934590, Decode(new Byte[] {0b100000_11, 0b11111111, 0b11111111, 0b11111111, 0b111111_00}));
        
    [Fact]
    public void CanDecode8589934591() => Assert.Equal((UInt64) 8589934591, Decode(new Byte[] {0b100001_00, 0b00000000, 0b00000000, 0b00000000, 0b0000000_0}));

    [Fact]
    public void CanDecodeMax() => Assert.Equal(UInt64.MaxValue - 1, Decode(new Byte[] {0b111111_11, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111000}));

    [Fact]
    public void CanEncodeDecodeFirst1000()
    {
        using var stream = new MemoryStream();

        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new ThompsonAlphaIntegerEncoder(writer, 6);
            for (UInt64 symbol = 0; symbol < 1000; symbol++) encoder.EncodeUInt64(symbol);
        }

        stream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamBitReader(stream))
        {
            var decoder = new ThompsonAlphaIntegerDecoder(reader, 6);
            for (UInt64 symbol = 0; symbol < 1000; symbol++)
            {
                Assert.Equal(symbol, decoder.DecodeUInt64());
            }
        }
    }
}