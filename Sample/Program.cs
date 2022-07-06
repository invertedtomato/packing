using InvertedTomato.Compression.Integers;

// Instantiate the codecs we want
var vlq = new VlqCodec();
var fib = new FibonacciCodec();
var td = new ThompsonAlphaCodec();

// Encode some values...
using var stream = new MemoryStream();
using (var writer = new StreamBitWriter(stream))
{
    // Encode some values using the Fibonacci codec
    fib.EncodeInt32(0, writer);
    fib.EncodeInt32(1, writer);
    
    // Encode a value using the VLQ codec
    vlq.EncodeInt32(2, writer);
    
    // Encode a value using the ThompsonAlpha codec
    td.EncodeInt32(3, writer);
}

// Convert it to binary so we can see what it's done
var binary = String.Join(" ", stream.ToArray().Select(a => Convert.ToString(a, 2).PadLeft(8, '0')));
Console.WriteLine($"Compressed data is {stream.Length} bytes ({binary})");

// Decode the values...
stream.Seek(0, SeekOrigin.Begin);
using (var reader = new StreamBitReader(stream))
{
    // Decode the the Fibonacci values
    Console.WriteLine(fib.DecodeInt32(reader));
    Console.WriteLine(fib.DecodeInt32(reader));
    
    // Decode the VLQ value
    Console.WriteLine(vlq.DecodeInt32(reader));
    
    // Decode the ThompsonAlpha value
    Console.WriteLine(td.DecodeInt32(reader));
}

Console.WriteLine("Done.");
Console.ReadKey(true);