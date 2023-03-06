namespace InvertedTomato.Packing;

public class EliasGammaTests
{
    // TODO: A full set of tests are required! I haven't bothered yet as I haven't found any use for this codec beyond academic interest

    [Fact]
    public void CanEncodeDecodeFirst1000()
    {
        using var stream = new MemoryStream();

        using (var writer = new StreamBitWriter(stream))
        {
            var encoder = new EliasGammaIntegerEncoder(writer);
            for (UInt64 symbol = 0; symbol < 1000; symbol++) encoder.EncodeUInt64(symbol);
        }

        stream.Seek(0, SeekOrigin.Begin);

        using (var reader = new StreamBitReader(stream))
        {
            var decoder = new EliasGammaIntegerDecoder(reader);
            for (UInt64 symbol = 0; symbol < 1000; symbol++)
            {
                Assert.Equal(symbol, decoder.DecodeUInt64());
            }
        }
    }
}