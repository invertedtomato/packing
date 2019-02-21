using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Compression.Integers.Wave2;

namespace InvertedTomato.Compression.Integer.LoadTest {
	internal class Program {
		private static void Main(String[] args) {
			//#1
			// Write: 7865ms 9.7MB/s
			// Read: 17392ms 4.39MB / s
			// ASync Read: 2935ms 25.99MB / s

			// #2
			// Write: 14,115ms 5.41MB/s
			// Read: 6,226ms 12.25MB/s

			// #3
			// Compress: 11801ms 6.47MB / s
			// Decompress: 6967ms 10.95MB / s

			// #4
			// Compress: 10291ms 7.41MB/s
			// Decompress: 5819ms 13.11MB / s

			// #5
			// FIBONACCI
			// Compress: 5986ms 12.75MB / s Total 38MB
			// Decompress: 3455ms 22.08MB / s
			// VLQ
			// Compress: 386ms 197.65MB / s Total 36MB
			// Decompress: 874ms 87.29MB / s

			var min = 100000;
			var count = 10000000;

			// Seed
			var input = new List<UInt64>(count);
			for (var v = min; v < min + count; v++) {
				input.Add((UInt64) v);
			}

			//////////////////////////////////////////
			Console.WriteLine("FIBONACCI");
			Codec wave2Codec = new FibonacciCodec();
			var compressed = new MemoryStream(count * 5);

			// Compress
			var stopWatch = Stopwatch.StartNew();
			wave2Codec.CompressUnsigned(compressed, input.ToArray());
			stopWatch.Stop();
			Console.WriteLine("Compress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((Double) count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s Total " + compressed.Length / 1024 / 1024 + "MB");

			// Rewind
			compressed.Position = 0;

			// Decompress
			stopWatch = Stopwatch.StartNew();
			var output = wave2Codec.DecompressUnsigned(compressed, input.Count).ToList();
			stopWatch.Stop();
			Console.WriteLine("Decompress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((Double) count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");

			// Validate
			var pos = 0;
			for (var v = min; v < min + count; v++) {
				if ((Int32) output[pos++] != v) {
					throw new Exception("Incorrect result. Expected " + v + " got " + output[v] + ".");
				}
			}


			//////////////////////////////////////////
			Console.WriteLine("VLQ");
			wave2Codec = new VLQCodec();
			compressed = new MemoryStream(count * 5);

			// Compress
			stopWatch = Stopwatch.StartNew();
			wave2Codec.CompressUnsigned(compressed, input.ToArray());
			stopWatch.Stop();
			Console.WriteLine("Compress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((Double) count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s Total " + compressed.Length / 1024 / 1024 + "MB");

			// Rewind
			compressed.Position = 0;

			// Decompress
			stopWatch = Stopwatch.StartNew();
			output = wave2Codec.DecompressUnsigned(compressed, input.Count).ToList();
			stopWatch.Stop();
			Console.WriteLine("Decompress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((Double) count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");

			// Validate
			pos = 0;
			for (var v = min; v < min + count; v++) {
				if ((Int32) output[pos++] != v) {
					throw new Exception("Incorrect result. Expected " + v + " got " + output[v] + ".");
				}
			}


			Console.WriteLine("\nDone.");
			Console.ReadKey(true);
		}
	}
}