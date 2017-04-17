using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers.Sample {
    class Program {
        static void Main(string[] args) {

            // ===== COMPRESS =====

            // Instantiate the codec ready to compress
            var codec = new FibonacciCodec();

            // Compress data - 3x8 bytes = 24bytes uncompressed
            var compressed = codec.Compress(new long[] { 1, 2, 3 });

            // Check its size
            Console.WriteLine("Compressed data is " + compressed.Length + " bytes"); // Output: Compressed data is 2 bytes


            // ===== DECOMPRESS =====

            // Give it a set of data
            var decompressed = codec.Decompress(compressed);

            // Check the result
            Console.WriteLine(decompressed[0]); // Output: 1
            Console.WriteLine(decompressed[1]); // Output: 2                
            Console.WriteLine(decompressed[2]); // Output: 3

            Console.ReadKey(true);
        }
    }
}
