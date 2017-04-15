using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers.Sample {
    class Program {
        static void Main(string[] args) {
            
            // ===== COMPRESS =====

            // Instantiate the codec ready to compress
            var codec = new FibonacciCodec();

            // Compress data - 3x8 bytes = 24bytes uncompressed
            var input = new Buffer<ulong>(new ulong[] { 1, 2, 3 });
            var compressed = new Buffer<byte>(10); // 10 bytes of buffer, although we won't use it
            codec.Compress(input, compressed);

            // Check its size
            Console.WriteLine("Compressed data is " + compressed.Used + " bytes"); // Output: Compressed data is 2 bytes


            // ===== DECOMPRESS =====

            // Give it a set of data
            var decompressed = new Buffer<ulong>(3);
            codec.Decompress(compressed, decompressed);
            
            // Check the result
            Console.WriteLine(decompressed.Dequeue()); // Output: 1
            Console.WriteLine(decompressed.Dequeue()); // Output: 2                
            Console.WriteLine(decompressed.Dequeue()); // Output: 3

            Console.ReadKey(true);
        }
    }
}
