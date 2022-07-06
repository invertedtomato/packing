
// #5
// FIBONACCI (Gen2)
// Compress: 5986ms 12.75MB / s Total 38MB
// Decompress: 3455ms 22.08MB / s

// VLQ (Gen2)
// Compress: 386ms 197.65MB / s Total 36MB
// Decompress: 874ms 87.29MB / s

using System.Diagnostics;

var min = 100000;
var count = 10000000;

// Seed
var input = new List<UInt64>(count);
for (var v = min; v < min + count; v++)
{
    input.Add((UInt64) v);
}

void Gen2Test(InvertedTomato.Compression.Integers.Gen2.Codec codec)
{
    // Compress
    using var stream = new MemoryStream(count * 5);
    var compressStopwatch = Stopwatch.StartNew();
    codec.EncodeMany(stream, input.ToArray());
    compressStopwatch.Stop();
    
    // Decompress
    stream.Position = 0;
    var decompressStopwatch = Stopwatch.StartNew();
    var output = new UInt64[count];
    codec.DecodeMany(stream, output);
    decompressStopwatch.Stop();
    
    // Validate
    var pos = 0;
    for (var v = min; v < min + count; v++)
    {
        if ((Int32) output[pos++] != v) throw new("Incorrect result. Expected " + v + " got " + output[v] + ".");
    }
    
    Console.WriteLine("{0,-75} {1,15:N0}ms {2,15:N0}ms {3,15:N}MB",codec.GetType().FullName, compressStopwatch.ElapsedMilliseconds, decompressStopwatch.ElapsedMilliseconds, stream.Length/1024/1024);
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
Gen2Test(new InvertedTomato.Compression.Integers.Gen2.ThompsonAlphaCodec());
Gen3Test(new InvertedTomato.Compression.Integers.ThompsonAlphaCodec());

Console.WriteLine("Fibonacci");
Gen2Test(new InvertedTomato.Compression.Integers.Gen2.FibonacciCodec());
Gen3Test(new InvertedTomato.Compression.Integers.FibonacciCodec());

Console.WriteLine("VLQ");
Gen2Test(new InvertedTomato.Compression.Integers.Gen2.VlqCodec());
Gen3Test(new InvertedTomato.Compression.Integers.VlqCodec());

Console.WriteLine("Raw");
Gen2Test(new InvertedTomato.Compression.Integers.Gen2.RawCodec());
Gen3Test(new InvertedTomato.Compression.Integers.RawCodec());

Console.WriteLine("\nDone.");
Console.ReadKey(true);