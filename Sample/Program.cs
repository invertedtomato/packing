using System;
using InvertedTomato.Compression.Integers.Wave2;

namespace InvertedTomato.Compression.Integers.Sample {
	internal class Program {
		private static void Main(String[] args) {
			// Instantiate the codec ready to compress
			Codec wave2Codec = new FibonacciCodec();

			// Compress data - 3x8 bytes = 24bytes uncompressed
			var compressed = wave2Codec.CompressSigned(1, 2, 3);
			Console.WriteLine("Compressed data is " + compressed.Length + " bytes"); // Output: Compressed data is 2 bytes

			// Decompress data
			var decompressed = wave2Codec.DecompressSigned(compressed, 3);
			Console.WriteLine(String.Join(",", decompressed)); // Output: 1,2,3

			Console.WriteLine("Done.");
			Console.ReadKey(true);
		}
	}
}