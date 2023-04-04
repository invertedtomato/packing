using InvertedTomato.Packing;
using InvertedTomato.Packing.Codecs.Integers;

// Encode some values...
using var stream = new MemoryStream(); // Could be a FileStream or a NetworkStream
using (var writer = new StreamBitWriter(stream))
{
    var fib = new FibonacciIntegerEncoder(writer); // Pick a codec - you can use one or many

    // Encode some values using the Fibonacci codec
    fib.EncodeUInt64(1);
    fib.EncodeUInt64(2);
    fib.EncodeUInt64(3);
}

Console.WriteLine("Compressed data is " + stream.Length + " bytes"); // Output: Now data is 2 bytes

// Decode the values...
stream.Position = 0;
using (var reader = new StreamBitReader(stream))
{
    var fib = new FibonacciIntegerDecoder(reader);

    // Decode the Fibonacci values
    Console.WriteLine(fib.DecodeUInt64()); // Output: 1
    Console.WriteLine(fib.DecodeUInt64()); // Output: 2
    Console.WriteLine(fib.DecodeUInt64()); // Output: 3
}


Console.WriteLine("Done.");
Console.ReadKey(true);