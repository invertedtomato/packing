using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers
{
    public class EliasGammaTests
    {
        
        [Fact]
        public void EncodeDecode_1000()
        {
            var ta = new EliasGammaCodec();
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
}