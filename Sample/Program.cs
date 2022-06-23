using InvertedTomato.Compression.Integers;

// Instantiate the codec ready to compress
Codec codec = new FibonacciCodec(); // Using "InvertedTomato.Compression.Integers.Wave3"

// Compress data - 3x8 bytes = 24bytes uncompressed
using var stream = new MemoryStream();
codec.EncodeMany(stream, new ulong[] {1, 2, 3,});
Console.WriteLine("Compressed data is " + stream.Length + " bytes"); // Output: Compressed data is 2 bytes

// Decompress data
stream.Seek(0, SeekOrigin.Begin);
var decompressed = new ulong[3];
codec.DecodeMany(stream, decompressed);
Console.WriteLine(string.Join(",", decompressed)); // Output: 1,2,3

Console.WriteLine("Done.");
Console.ReadKey(true);