using InvertedTomato.Packing;
using InvertedTomato.Packing.Codecs.Integers;

// InvertedTomato.Packing

// Encode some values...
using var stream = new MemoryStream();
using (var writer = new StreamBitWriter(stream))
{
    var fibbonacci = new FibonacciIntegerEncoder(writer);
    var vlq = new VlqIntegerEncoder(writer);
    var thompsonAlpha = new ThompsonAlphaIntegerEncoder(writer, 6);
    //var utf8 = new Utf8StringEncoder(writer);
    
    // Encode some values using the Fibonacci codec
    fibbonacci.EncodeInt32(0);
    fibbonacci.EncodeInt32(1);
    
    // Encode a value using the VLQ codec
    vlq.EncodeInt32(0);
    
    // Encode a value using the ThompsonAlpha codec
    thompsonAlpha.EncodeInt32(0);

    //utf8.EncodeString("test");
}

// Convert it to binary so we can see what it's done
var binary = String.Join(" ", stream.ToArray().Select(a => Convert.ToString(a, 2).PadLeft(8, '0')));
Console.WriteLine($"Compressed data is {stream.Length} bytes ({binary})");

// Decode the values...
stream.Seek(0, SeekOrigin.Begin);
using (var reader = new StreamBitReader(stream))
{
    var vlq = new VlqIntegerDecoder(reader);
    var fib = new FibonacciIntegerDecoder(reader);
    var td = new ThompsonAlphaIntegerDecoder(reader, 6);
    
    // Decode the the Fibonacci values
    Console.WriteLine(fib.DecodeInt32());
    Console.WriteLine(fib.DecodeInt32());
    
    // Decode the VLQ value
    Console.WriteLine(vlq.DecodeInt32());
    
    // Decode the ThompsonAlpha value
    Console.WriteLine(td.DecodeInt32());
}

Console.WriteLine("Done.");
Console.ReadKey(true);