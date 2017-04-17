using System;

namespace InvertedTomato.Compression.Integers.Sample {
    class Program {
        static void Main(string[] args) {
            // Instantiate the codec ready to compress
            var codec = new FibonacciCodec();

            // Compress data - 3x8 bytes = 24bytes uncompressed
            var compressed = codec.Compress(new long[] { 1, 2, 3 });
            Console.WriteLine("Compressed data is " + compressed.Length + " bytes"); // Output: Compressed data is 2 bytes

            // Decompress data
            var decompressed = codec.Decompress(compressed);
            Console.WriteLine(string.Join(",", decompressed)); // Output: 1,2,3

            Console.ReadKey(true);
        }
    }
}
