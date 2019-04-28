using System;
using System.IO;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Compression.Integers.Sample {
	internal class Program {
		private static void Main(String[] args) {
			// Instantiate the codec ready to compress
			Codec codec = new FibonacciCodec(); // Using "InvertedTomato.Compression.Integers.Wave3"

			using (var stream = new MemoryStream()) {
				// Compress data - 3x8 bytes = 24bytes uncompressed
				codec.EncodeMany(stream, new UInt64[] {1, 2, 3});
				Console.WriteLine("Compressed data is " + stream.Length + " bytes"); // Output: Compressed data is 2 bytes

				// Decompress data
				stream.Seek(0, SeekOrigin.Begin);
				var decompressed = new UInt64[3];
				codec.DecodeMany(stream, decompressed);
				Console.WriteLine(String.Join(",", decompressed)); // Output: 1,2,3

				Console.WriteLine("Done.");
				Console.ReadKey(true);
			}
		}
	}
}