using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers.Sample {
    class Program {
        static void Main(string[] args) {
            
            // ===== COMPRESS =====

            // Instantiate the codec ready to compress
            var compressor = new FibonacciCodec();

            // Give it a set of data - 3x8 bytes = 24bytes uncompressed 
            compressor.DecompressedSet = new Buffer<ulong>(new ulong[] { 1, 2, 3 });

            // Compress it
            compressor.Compress();

            // Get compressed data
            var compressed = compressor.CompressedSet.ToArray();

            // Check its size
            Console.WriteLine("Compressed data is " + compressed.Length + " bytes");
            // Output: Compressed data is 2 bytes


            // ===== DECOMPRESS =====

            // Instantiate the codec ready to decompress
            var decompressor = new FibonacciCodec();

            // Give it a set of data
            decompressor.CompressedSet = new Buffer<byte>(compressed);

            // Decompress
            decompressor.Decompress();

            // Get decompressed data
            var decompressed = decompressor.DecompressedSet.ToArray();

            // Check the result
            Console.WriteLine(decompressed[0]);
            // Output: 1
            Console.WriteLine(decompressed[1]);
            // Output: 2                
            Console.WriteLine(decompressed[2]);
            // Output: 3

            Console.ReadKey(true);
        }
    }
}
