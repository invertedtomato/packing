namespace InvertedTomato.Packing;

public class ThompsonAlphaTests
{
    // Encode
    private Byte[] Encode(UInt64 value)
    {
        var codec = new ThompsonAlphaIntegerCodec();
        using var stream = new MemoryStream();
        using (var writer = new StreamBitWriter(stream))
        {
            codec.EncodeUInt64(value, writer);
        }

        return stream.ToArray();
    }

    [Fact]
    public void Encode_0() => Assert.Equal(new Byte[] {0b000000_00}, Encode(0)); // Len=0, Val=(1)

    [Fact]
    public void Encode_1() => Assert.Equal(new Byte[] {0b000001_0_0}, Encode(1)); // Len=1, Val=(1)1

    [Fact]
    public void Encode_2() => Assert.Equal(new Byte[] {0b000001_1_0}, Encode(2)); // Len=10, val=(1)10

    [Fact]
    public void Encode_3() => Assert.Equal(new Byte[] {0b000010_00}, Encode(3)); // Len=10, val=(1)11

    [Fact]
    public void Encode_8589934590() => Assert.Equal(new Byte[] {0b100000_11, 0b11111111, 0b11111111, 0b11111111, 0b111111_00}, Encode(8589934590));

    [Fact]
    public void Encode_8589934591() => Assert.Equal(new Byte[] {0b100001_00, 0b00000000, 0b00000000, 0b00000000, 0b0000000_0}, Encode(8589934591));

    [Fact]
    public void Encode_Max() => Assert.Equal(new Byte[] {0b111111_11, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111000}, Encode(UInt64.MaxValue - 1));

    // Decode

    private UInt64 Decode(Byte[] encoded)
    {
        var codec = new ThompsonAlphaIntegerCodec();
        using var stream = new MemoryStream(encoded);
        using var reader = new StreamBitReader(stream);

        return codec.DecodeUInt64(reader);
    }
        
    [Fact]
    public void Decode_0() => Assert.Equal((UInt64) 0, Decode(new Byte[] {0b000000_00}));

    [Fact]
    public void Decode_1() => Assert.Equal((UInt64) 1, Decode(new Byte[] {0b000001_0_0})); // (len)_(val)_(padding)

    [Fact]
    public void Decode_2() => Assert.Equal((UInt64) 2, Decode(new Byte[] {0b000001_1_0}));

    [Fact]
    public void Decode_3() =>  Assert.Equal((UInt64) 3, Decode(new Byte[] {0b000010_00}));

    [Fact]
    public void Decode_8589934590() => Assert.Equal((UInt64) 8589934590, Decode(new Byte[] {0b100000_11, 0b11111111, 0b11111111, 0b11111111, 0b111111_00}));
        
    [Fact]
    public void Decode_8589934591() => Assert.Equal((UInt64) 8589934591, Decode(new Byte[] {0b100001_00, 0b00000000, 0b00000000, 0b00000000, 0b0000000_0}));

    [Fact]
    public void Decode_Max() => Assert.Equal(UInt64.MaxValue - 1, Decode(new Byte[] {0b111111_11, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111111, 0b11111000}));

    [Fact]
    public void EncodeDecode_100000()
    {
        var ta = new ThompsonAlphaIntegerCodec();
        using var stream = new MemoryStream();

        // Encode
        using (var writer = new StreamBitWriter(stream))
        {
            for (UInt64 symbol = 0; symbol < 100000; symbol++)
            {
                ta.EncodeUInt64(symbol, writer);
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        // Decode
        using (var reader = new StreamBitReader(stream))
        {
            for (UInt64 symbol = 0; symbol < 100000; symbol++)
            {
                Assert.Equal(symbol, ta.DecodeUInt64(reader));
            }
        }
    }
}