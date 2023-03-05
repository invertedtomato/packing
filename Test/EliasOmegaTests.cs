using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers;

public class EliasOmegaTests
{
    // TODO: A full set of tests are required! I haven't bothered yet as I haven't found any use for this codec beyond academic interest

    [Fact]
    public void EncodeDecode_1000()
    {
        var ta = new EliasOmegaCodec();
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