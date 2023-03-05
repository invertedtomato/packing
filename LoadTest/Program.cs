
// #5
// FIBONACCI (Gen2)
// Compress: 5986ms 12.75MB / s Total 38MB
// Decompress: 3455ms 22.08MB / s

// VLQ (Gen2)
// Compress: 386ms 197.65MB / s Total 36MB
// Decompress: 874ms 87.29MB / s

// 220707
// CODEC                      ENCODE TIME         DECODE TIME        RESULT SIZE
//     ThompsonAlpha
// InvertedTomato.Compression.Integers.Gen2.ThompsonAlphaCodec                             839ms             736ms           32.00MB
// InvertedTomato.Compression.Integers.ThompsonAlphaCodec                                  897ms             738ms           32.00MB
//     Fibonacci
// InvertedTomato.Compression.Integers.Gen2.FibonacciCodec                               2,874ms           1,442ms           38.00MB
// InvertedTomato.Compression.Integers.FibonacciCodec                                    8,399ms           6,777ms           38.00MB
//     VLQ
// InvertedTomato.Compression.Integers.Gen2.VlqCodec                                       265ms             346ms           36.00MB
// InvertedTomato.Compression.Integers.VlqCodec                                            959ms           1,112ms           36.00MB
//     Raw
// InvertedTomato.Compression.Integers.Gen2.RawCodec                                       631ms             625ms           76.00MB
// InvertedTomato.Compression.Integers.RawCodec                                          2,000ms           2,093ms           76.00MB

// 220711 Added buffer to StreamBitReader&StreamBitWriter (ie, writes byte[] rather than byte)
// CODEC                      ENCODE TIME         DECODE TIME        RESULT SIZE
//     ThompsonAlpha
// InvertedTomato.Compression.Integers.Gen2.ThompsonAlphaCodec                             860ms             745ms           32.00MB
// InvertedTomato.Compression.Integers.Gen3.ThompsonAlphaCodec                             758ms             625ms           32.00MB
//     Fibonacci
// InvertedTomato.Compression.Integers.Gen2.FibonacciCodec                               2,891ms           1,445ms           38.00MB
// InvertedTomato.Compression.Integers.Gen3.FibonacciCodec                               7,972ms           6,385ms           38.00MB
//     VLQ
// InvertedTomato.Compression.Integers.Gen2.VlqCodec                                       271ms             356ms           36.00MB
// InvertedTomato.Compression.Integers.Gen3.VlqCodec                                       525ms             683ms           36.00MB
//     Raw
// InvertedTomato.Compression.Integers.Gen2.RawCodec                                       647ms             639ms           76.00MB
// InvertedTomato.Compression.Integers.Gen3.RawCodec                                       825ms             850ms           76.00MB

// 220713 Added Fib write buffering rather than pushing raw bits
// Fibonacci
// InvertedTomato.Compression.Integers.Gen2.FibonacciCodec                               2,924ms           1,484ms           38.00MB
// InvertedTomato.Compression.Integers.Gen3.FibonacciCodec                               3,396ms           7,443ms           38.00MB


using System.Diagnostics;

var min = 100000;
var count = 10000000;

// Seed
var input = new List<UInt64>(count);
for (var v = min; v < min + count; v++)
{
    input.Add((UInt64) v);
}

void Gen3Test(InvertedTomato.Compression.Integers.ICodec codec)
{
    // Compress
    using var stream = new MemoryStream(count * 5);
    var compressStopwatch = Stopwatch.StartNew();
    using (var writer = new InvertedTomato.Compression.Integers.StreamBitWriter(stream))
    {
        input.ForEach(a=>codec.EncodeUInt64(a,writer));
    }
    compressStopwatch.Stop();
    
    // Decompress
    stream.Position = 0;
    var decompressStopwatch = Stopwatch.StartNew();
    using (var reader = new InvertedTomato.Compression.Integers.StreamBitReader(stream))
    {
        input.ForEach(a =>
        {
            if (a != codec.DecodeUInt64(reader)) throw new("Incorrect result.");
        });
    }
    decompressStopwatch.Stop();
    
    Console.WriteLine("{0,-75} {1,15:N0}ms {2,15:N0}ms {3,15:N}MB",codec.GetType().FullName, compressStopwatch.ElapsedMilliseconds, decompressStopwatch.ElapsedMilliseconds, stream.Length/1024/1024);
}


Console.WriteLine("CODEC                      ENCODE TIME         DECODE TIME        RESULT SIZE");
Console.WriteLine("ThompsonAlpha");
Gen3Test(new InvertedTomato.Compression.Integers.ThompsonAlphaCodec());

Console.WriteLine("Fibonacci");
Gen3Test(new InvertedTomato.Compression.Integers.FibonacciCodec());

Console.WriteLine("VLQ");
Gen3Test(new InvertedTomato.Compression.Integers.VlqCodec());

Console.WriteLine("Raw");
Gen3Test(new InvertedTomato.Compression.Integers.Gen3.RawCodec());

Console.WriteLine("\nDone.");