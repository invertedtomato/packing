using System;

namespace InvertedTomato.Compression.Integers.Sample {
	internal class Program {
		private static void Main(String[] args) {
			// Instantiate the codec ready to compress
			Codec codec = new FibonacciCodec();

			// Compress data - 3x8 bytes = 24bytes uncompressed
			var compressed = codec.CompressSigned(1, 2, 3);
			Console.WriteLine("Compressed data is " + compressed.Length + " bytes"); // Output: Compressed data is 2 bytes

			// Decompress data
			var decompressed = codec.DecompressSigned(compressed, 3);
			Console.WriteLine(String.Join(",", decompressed)); // Output: 1,2,3

			Console.WriteLine("Done.");
			Console.ReadKey(true);
		}
	}
}